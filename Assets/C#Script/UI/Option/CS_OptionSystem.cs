using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// �S���F���@�I�v�V�����V�X�e��ver2
/// </summary>
public class CS_OptionSystem : MonoBehaviour
{

    [SerializeField, Tooltip("�I�v�V�����p�J�[�\��")]
    private RectTransform OptionCursor;
    [SerializeField, Tooltip("�J�[�\���ړ��ʒuList")]
    private List<RectTransform> OptionTransList;

    private enum OptionState
    {
        NONE,   //�Ȃɂ����Ă��Ȃ�
        SOUND,  //�T�E���h
        CAMERA, //�J����
        GRAPHIC,//�O���t�B�b�N
    }

    [SerializeField, Header("���݂̏��")]
    private OptionState CurrentState;

    //���ڑI�𒆂�
    private bool Select = false;
    
    //�I�v�V�������ڑI�𒆂�
    private bool OptionSelect = false;

    //����ƃL�����Z����ێ�
    private bool Decision = false;
    private bool Cancel = false;

    [Header("=============�V�X�e��===============")]
    [SerializeField, Tooltip("����")]
    private CS_InputSystem InputSystem;
    [SerializeField, Tooltip("���莞�̐F")]
    private Color DecisionColor;
    [SerializeField, Tooltip("���I�����̐F")]
    private Color NormalColor;
    [SerializeField, Tooltip("�ݒ�X���C�_�[�ړ����x")]
    private float SliderSpeed = 1;
    //�X���C�_�[�����x
    private float SliderAccelerationSpeed = 1;


    [Header("=============�T�E���h===============")]
    [SerializeField, Tooltip("AudioMixer")]
    private AudioMixer Mixer;
    [SerializeField, Tooltip("�ݒ�p�J�[�\��")]
    private RectTransform SoundCursor;
    [SerializeField, Tooltip("�J�[�\���ړ��p���W")]
    private List<RectTransform> SoundCursorPos;
    [SerializeField, Tooltip("�ݒ�p�X���C�_�[")]
    private List<Image> SoundSlider;
    private int SoundSelectNum = 0; //0:SE 1:BGM

    [Header("=============�J����===============")]
    [SerializeField, Tooltip("����p�ݒ�")]
    private Cinemachine.CinemachineVirtualCamera CameraFov;
    [SerializeField, Tooltip("�J�������x�ݒ�p")]
    private CS_TpsCamera CameraMove;
    [SerializeField, Tooltip("�ݒ�p�J�[�\��")]
    private RectTransform CameraCursor;
    [SerializeField, Tooltip("�J�[�\���ړ��p���W")]
    private List<RectTransform> CameraCursorPos;
    [SerializeField, Tooltip("�ݒ�p�X���C�_�[")]
    private List<Image> CameraSlider;
    private float CameraMaxSpeed = 200f;    //Camera�̍ő�ړ����x
    private float CameraMaxFov = 120f;     //Camera�̍ő压��p
    private int CameraSelectNum = 0; //0:���x 1:����p

    [Header("=============�掿�ݒ�===============")]
    [SerializeField, Tooltip("�ݒ�p�J�[�\��")]
    private RectTransform GraphicCursor;
    [SerializeField, Tooltip("�J�[�\���ړ��p���W")]
    private List<RectTransform> GraphicCursorPos;
    [SerializeField, Tooltip("�ݒ荀�ڃe�L�X�g")]
    private List<Text> GraphicSettingTextList;
    private int CurrentGraphicSettingNum = 2; //�掿�ݒ�ԍ� 0~��掿�@4~���掿
    private int GraphicSettingNum;  //���ݐݒ蒆�̉掿


    [SerializeField, Header("�ݒ�p�p�l��")]
    private List<GameObject> SettingPanels;    //�\���p�p�l��

    private bool End = false;

    /// <summary>
    /// �I�v�V�����I���ł����Ԃ�
    /// </summary>
    /// true �I��<returns>  false �p�� </returns>
    public bool GetOptionBack()
    {
        return End;
    }

    // Start is called before the first frame update
    void Start()
    {
        GraphicSettingTextList[CurrentGraphicSettingNum].color = DecisionColor;
        GraphicSettingNum = CurrentGraphicSettingNum;
    }

    
    private void OnEnable()
    {
        End = false;
        CurrentState = OptionState.SOUND;
        SettingPanels[(int)CurrentState - 1].SetActive(true);
        //if(CurrentState != OptionState.NONE) { return; }
        //CurrentState = OptionState.SOUND;
    }


    /// <summary>
    /// �J�����ݒ�
    /// </summary>
    private void Camera()
    {
        
        if (!OptionSelect)
        {
            //�I�v�V�������ڂ̑I�𒆂ɂ���
            if (Decision) 
            {
               CameraCursorPos[CameraSelectNum].GetComponent<Text>().color = DecisionColor;
               OptionSelect = true; 
            }
            //�I�v�V�������L�����Z��
            if (Cancel)
            {
                CameraSelectNum = 0;
                Select = false;
                //CurrentState = OptionState.NONE;

                OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = NormalColor;
            }

            //����(�㉺)
            bool DownButton = InputSystem.GetDpadDownTriggered();
            bool UpButton = InputSystem.GetDpadUpTriggered();
            if (DownButton || UpButton) { CameraSelectNum = CameraSelectNum == 0 ? 1 : 0; }

            //�J�[�\���ړ�
            CameraCursor.anchoredPosition = CameraCursorPos[CameraSelectNum].anchoredPosition;

        }
        else
        {
            //���X�e�B�b�N��x�������擾
            float SideStick = InputSystem.GetLeftStick().x;
            bool Left = SideStick < -0.5f && CameraSlider[CameraSelectNum].fillAmount >= 0;
            bool Right = SideStick > 0.5f && CameraSlider[CameraSelectNum].fillAmount <= 1;
            if (Left) { CameraSlider[CameraSelectNum].fillAmount -= SliderAccelerationSpeed * Time.deltaTime; }
            if (Right) { CameraSlider[CameraSelectNum].fillAmount += SliderAccelerationSpeed * Time.deltaTime; }

            //�X���C�_�[���x�����������Ă�
            if (Left || Right) { SliderAccelerationSpeed += SliderSpeed * Time.deltaTime; }
            else { SliderAccelerationSpeed = SliderSpeed; }

            //���x�ݒ�
            if (CameraSelectNum == 0)
            {
                if(CameraMove) CameraMove.CameraSpeed = CameraSlider[CameraSelectNum].fillAmount / CameraMaxSpeed;
            }
            //����p�ݒ�
            else
            {
                if(CameraFov) CameraFov.m_Lens.FieldOfView = CameraSlider[CameraSelectNum].fillAmount / CameraMaxFov;
            }

            //�I�v�V�������ڐݒ�I��
            if (Cancel) 
            {
               CameraCursorPos[CameraSelectNum].GetComponent<Text>().color = NormalColor;
               OptionSelect = false; 
            }
        }
    }

    /// <summary>
    /// �T�E���h�ݒ�
    /// </summary>
    private void Sound()
    {

        
        if (!OptionSelect)
        {
            //�I�v�V�������ڂ̑I�𒆂ɂ���
            if (Decision) 
            {
                SoundCursorPos[SoundSelectNum].GetComponent<Text>().color = DecisionColor;
                OptionSelect = true; 
            }
            //�I�v�V�������L�����Z��
            if (Cancel)
            {
                SoundSelectNum = 0;
                Select = false;

                //CurrentState = OptionState.NONE;

                OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = NormalColor;
            }

            //����(�㉺)
            bool DownButton = InputSystem.GetDpadDownTriggered();
            bool UpButton = InputSystem.GetDpadUpTriggered();
            if (DownButton || UpButton) { SoundSelectNum = SoundSelectNum == 0 ? 1 : 0; }

            //�J�[�\���ړ�
            SoundCursor.anchoredPosition = SoundCursorPos[SoundSelectNum].anchoredPosition;

        }
        else
        {
            //���X�e�B�b�N��x�������擾
           
            float SideStick = InputSystem.GetLeftStick().x;
            bool Left = SideStick < -0.5f && CameraSlider[CameraSelectNum].fillAmount >= 0;
            bool Right = SideStick > 0.5f && CameraSlider[CameraSelectNum].fillAmount <= 1;
            if (Left) { SoundSlider[SoundSelectNum].fillAmount -= SliderAccelerationSpeed * Time.deltaTime; }
            if (Right) { SoundSlider[SoundSelectNum].fillAmount += SliderAccelerationSpeed * Time.deltaTime; }

            //�X���C�_�[���x�����������Ă�
            if (Left || Right) { SliderAccelerationSpeed += SliderSpeed * Time.deltaTime; }
            else { SliderAccelerationSpeed = SliderSpeed; }

            //SE�ݒ�
            if (SoundSelectNum == 0)
            {
                // �X���C�_�[�̒l��-80dB����0dB�ɕϊ�
                float volume = Mathf.Lerp(-80f, 0f, SoundSlider[SoundSelectNum].fillAmount);
                Mixer.SetFloat("SE", volume);

            }
            //BGM�ݒ�
            else
            {
                // �X���C�_�[�̒l��-80dB����0dB�ɕϊ�
                float volume = Mathf.Lerp(-80f, 0f, SoundSlider[SoundSelectNum].fillAmount);
                Mixer.SetFloat("BGM", volume);
            }

            //�I�v�V�������ڐݒ�I��
            if (Cancel) 
            {
                SoundCursorPos[SoundSelectNum].GetComponent<Text>().color = NormalColor;
                OptionSelect = false; 
            }
        }
    }

    /// <summary>
    /// �掿�ݒ�
    /// </summary>
    private void Graphic()
    {
        
        if (Cancel)
        {
            CurrentGraphicSettingNum = 0;
            Select = false;

            //CurrentState = OptionState.NONE;

            OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = NormalColor;
        }

        //����(�㉺)
        bool DownButton = InputSystem.GetDpadDownTriggered();
        bool UpButton = InputSystem.GetDpadUpTriggered();

        if (DownButton) { CurrentGraphicSettingNum--; }
        if (UpButton) { CurrentGraphicSettingNum++; }

        //�㉺���̐ݒ�
        if (CurrentGraphicSettingNum > GraphicCursorPos.Count - 1) { CurrentGraphicSettingNum = 0; }
        if (CurrentGraphicSettingNum < 0) { CurrentGraphicSettingNum = GraphicCursorPos.Count - 1; }

        //�J�[�\���ړ�
        GraphicCursor.anchoredPosition = GraphicCursorPos[CurrentGraphicSettingNum].anchoredPosition;
      
        if (!Decision) { return; }

        QualitySettings.SetQualityLevel(CurrentGraphicSettingNum + 1);

        //�I�𒆂̉掿���X�V
        GraphicSettingTextList[GraphicSettingNum].color = NormalColor;
        GraphicSettingTextList[CurrentGraphicSettingNum].color = DecisionColor;
        GraphicSettingNum = CurrentGraphicSettingNum;
    }

    private void SelectAction()
    {
        switch (CurrentState)
        {
            case OptionState.NONE:      //�Ȃɂ����Ă��Ȃ�
                break;
            case OptionState.SOUND:     //�T�E���h
                Sound();
                break;
            case OptionState.CAMERA:    //�J����
                Camera();
                break;
            case OptionState.GRAPHIC:   //�掿
                Graphic();
                break;
        }


    }

    // Update is called once per frame
    void Update()
    {                                  
        if (Decision) { Select = true; }//���ڑI�𒆂��̏��
        
        Cancel = InputSystem.GetButtonBTriggered();�@//�߂�
        Decision = InputSystem.GetButtonATriggered(); //����

        if (Select) //�I���s��
        {
            OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = DecisionColor;
            SelectAction(); 
        }
        else  //�J�[�\���ړ�
        {
            CursorMove();
            if (!OptionSelect && Cancel) 
            {
                SettingPanels[(int)CurrentState - 1].SetActive(false);
                End = true; 
            }
        }
    }

    /// <summary>
    /// �J�[�\���ړ�
    /// </summary>
    private void CursorMove()
    {
        //�O��̏��
        OptionState oldstate = CurrentState;

        //����(�㉺)
        bool DownButton = InputSystem.GetDpadDownTriggered();
        bool UpButton = InputSystem.GetDpadUpTriggered();

        if (DownButton) { CurrentState++; }
        if (UpButton) { CurrentState--; }

        //���͏㉺���̐ݒ�
        if (CurrentState < OptionState.SOUND) { CurrentState = OptionState.GRAPHIC; }
        if (CurrentState > OptionState.GRAPHIC) { CurrentState = OptionState.SOUND; }

        //�J�[�\���ړ�
        OptionCursor.anchoredPosition = OptionTransList[(int)CurrentState - 1].anchoredPosition;

        //�p�l���̕\����\��
        if (oldstate == CurrentState) { return; }
        if (!SettingPanels[(int)CurrentState - 1].activeSelf) { SettingPanels[(int)CurrentState -1].SetActive(true); }
        if(oldstate < OptionState.SOUND || oldstate > OptionState.GRAPHIC) { return; }
        if (SettingPanels[(int)oldstate - 1].activeSelf) { SettingPanels[(int)oldstate - 1].SetActive(false); }



    }


}
