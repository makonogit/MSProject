using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class CS_Option : MonoBehaviour
{

    //[SerializeField, Header("OptionAnimators")]
    //private List<Animator> OptionAnimators;

    [SerializeField, Header("OptionRectTrans")]
    private List<RectTransform> OptionRectTrans;

    [SerializeField, Header("InputSystem")]
    private CS_InputSystem InputSystem;

    [SerializeField, Header("�J�[�\��RectTrans")]
    private RectTransform CursorRectTrans;

    [Header("�I�v�V�����p�J�[�\�����")]
    [SerializeField]
    private GameObject OptionCursor;
    [SerializeField]
    private RectTransform OptionCursorTrans;

    [Header("�I�v�V�����p�I��������")]
    [SerializeField,Tooltip("����")]
    private List<RectTransform> VolumeRectTrans;
    [SerializeField, Tooltip("�J����")]
    private List<RectTransform> CameraRectTrans;
    [SerializeField, Tooltip("�掿")]
    private RectTransform QualityRectTrans;


    [SerializeField, Tooltip("���ʃX���C�_�[")]
    private List<Slider> VolumeSlider;
    [SerializeField, Tooltip("�J�����X���C�_�[")]
    private List<Slider> CameraSlider;
    [SerializeField, Tooltip("�掿����")]
    private List<RectTransform> QualityLevelRectTrans;
    [SerializeField, Tooltip("�掿�I�𒆃J�[�\��")]
    private RectTransform QualitySelectCursor;

    [SerializeField, Header("�X���C�_�[�����X�s�[�h")]
    private float SliderSpeed = 1000.0f;
    [SerializeField, Header("�X���C�_�[�����x")]
    private float Slideracceleration = 0.01f;

    private float SliderAccelerationSpeed = 0.0f;

    private int QualityLevel = 0;

    //private Slider QualitySlider;

    [SerializeField, Header("����p�ݒ�")]
    private Cinemachine.CinemachineVirtualCamera CameraFov;
    [SerializeField, Header("�J�������x�ݒ�p")]
    private CS_TpsCamera CameraMove;

    [SerializeField, Header("���ʐݒ�")]
    private AudioMixer Audio;
    //[SerializeField, Header("BGM�ݒ�")]
    //private AudioSource BGMSource;
    //[SerializeField, Header("SE�ݒ�")]
    //private AudioSource SESource;

    //�I�v�V�����I��ԍ�
    private int OptionSelectNum = 0;

    //�I�v�V�������
    private enum OptionState
    {
        Volume = 0,     //����
        Camera = 1,     //�J����
        Quality = 2,    //�掿
        Story = 3,      //�X�g�[���[
    }

    private OptionState CurrentOption;

    [SerializeField, Header("OptionPanels")]
    private List<GameObject> OptionPanels;

    private int MaxOption = 0;

    private bool OptionSelect = false;  //�I�v�V�����I�𒆂�

    private bool SliderSelect = false;  //�����o�[��I�����Ă��邩
    

    // Start is called before the first frame update
    void Start()
    {
        //�掿�̃X���C�_�[��ݒ�
        //QualitySlider.maxValue = QualitySettings.names.Length - 1;

        QualityLevel = QualitySettings.GetQualityLevel() - 1;

        QualitySelectCursor.transform.SetParent(QualityLevelRectTrans[QualityLevel]);
        CopyRectTransform(QualitySelectCursor, QualityLevelRectTrans[QualityLevel]);

        //�X���C�_�[���X�V���ꂽ�Ƃ���CallBack
        //QualitySlider.onValueChanged.AddListener(ChangeQuality);

        //���ʂ̃X���C�_�[��ݒ�
        float BGMVolume = 0.75f;
        float SEVolume = 0.75f;
        //Audio.GetFloat("BGM", out BGMVolume);
        //Audio.GetFloat("SE", out SEVolume);

        //VolumeSlider[0].value = Mathf.Pow(10, BGMVolume / 20);
        //VolumeSlider[1].value = Mathf.Pow(10, SEVolume / 20);

        //���ʂ��ύX���ꂽ�Ƃ���CallBack
        //VolumeSlider[0].onValueChanged.AddListener(SetBGMVolume);
        //VolumeSlider[1].onValueChanged.AddListener(SetSEVolume);

        //�Œ�l�ƍō��l��ݒ肵�Ď���p��ݒ�
        float minfov = 60f;
        float maxfov = 200f;
        CameraSlider[0].minValue = minfov;
        CameraSlider[0].maxValue = maxfov;
        CameraSlider[0].value = minfov;
        //CameraSlider[0].onValueChanged.AddListener(SetFov);

        //�J�������x�ݒ�
        //CameraSlider[1].value = CameraMove.moveSpeed;
        //CameraSlider[1].onValueChanged.AddListener(SetMoveSpeed);
        

        //�I�v�V�������ڐ�
        MaxOption = OptionRectTrans.Count - 1;
    }

    /// <summary>
    /// �I�v�V�����S�̃A�N�V����
    /// </summary>
    /// <returns></returns>
    public bool OptionAction()
    {
        //�e�{�^����������������
        bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
        bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);
        bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);

        //�I�v�V�����I��
        bool EndOption = true;

        //�I�v�V�����̐ݒ�
        if (OptionSelect) { SelectOption(); }
        else
        {
            //���͂��Ȃ���ΏI��
            //if (!DownButton && !UpButton && !SelectButton) { return; }

            //���j���[�ԍ����X�V
            int oldOption = (int)CurrentOption;
            if (DownButton) { CurrentOption++; }
            if (UpButton) { CurrentOption--; }

            //�J�[�\���ړ�(�I�v�V�����I�𒆂���Ȃ���΃J�[�\��������)
            bool ChangeOption = oldOption != (int)CurrentOption;
            if (ChangeOption) { CursorMove(oldOption); }

            //�L�����Z��������
            bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

            if (CanselButton)
            {
                CurrentOption = OptionState.Volume;
                //�J�[�\���̈ړ�
                OptionCursor.transform.SetParent(OptionRectTrans[(int)CurrentOption]);
                CopyRectTransform(OptionCursorTrans, OptionRectTrans[(int)CurrentOption]);
                OptionSelectNum = 0;
                EndOption = false; 
            }
        }


        //�I�v�V�����I����Ԃ�
        if (SelectButton)
        {
            OptionInfo();   //�Ώۂ̃I�v�V������\��
            OptionSelect = true;
        }


        return EndOption;

    }

    /// <summary>
    /// �J�[�\���ړ�
    /// </summary>
    /// <param name="old"></param>
    private void CursorMove(int old)
    {
        //�㉺��
        if ((int)CurrentOption > MaxOption) { CurrentOption = 0; }
        if ((int)CurrentOption < 0) { CurrentOption = (OptionState)MaxOption; }

        //����ƑO���Animator�̏�Ԃ�ۑ�
        //OptionAnimators[(int)CurrentOption].SetBool("OnCursor", true);
        //OptionAnimators[old].SetBool("OnCursor", false);

        //�J�[�\���̈ʒu��ݒ肷��
        OptionCursor.transform.SetParent(OptionRectTrans[(int)CurrentOption]);    //�q�I�u�W�F�N�g�ɐݒ�
        CopyRectTransform(OptionCursorTrans, OptionRectTrans[(int)CurrentOption]);
        //OptionCursorTrans = OptionRectTrans[(int)CurrentOption];
        //Vector3 pos = OptionRectTrans[(int)CurrentOption].anchoredPosition;
        //pos.y -= CursorSize;
        //CursorRectTrans.anchoredPosition = pos;
    }

    /// <summary>
    /// �Ώۂ̃I�v�V������\��
    /// </summary>
    private void OptionInfo()
    {
        OptionPanels[(int)CurrentOption].SetActive(true);
        //�J�[�\���̈ʒu�����킹��
        OptionCursor.transform.SetParent(VolumeRectTrans[OptionSelectNum]);
        CopyRectTransform(OptionCursorTrans, VolumeRectTrans[OptionSelectNum]);
    }

    /// <summary>
    /// �I�v�V�����ڍבI��
    /// </summary>
    private void SelectOption()
    {
        //OptionAnimators[(int)CurrentOption].SetBool("IsSelect", true);

        //�L�����Z��������
        bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

       
        switch (CurrentOption)
        {
            case OptionState.Volume:
                VolumeAction();
                break;
            case OptionState.Camera:
                CameraAction();
                break;
            case OptionState.Quality:
                QualityAction();
                break;
            case OptionState.Story:
                StoryAction();
                break;
        }

        //�I�v�V�����I������
        if (CanselButton)
        {
           
            
        }

    }

    /// <summary>
    /// �I�v�V�������ڂ̏�����
    /// </summary>

    private void OptionInitialize()
    {
        //�L�����Z��������
        bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

        if (!CanselButton) { return; }

        //�I�v�V�������ڂ�������
        OptionCursor.transform.SetParent(OptionRectTrans[(int)CurrentOption]);
        CopyRectTransform(OptionCursorTrans, OptionRectTrans[(int)CurrentOption]);
        OptionSelectNum = 0;

        OptionPanels[(int)CurrentOption].SetActive(false);
        OptionSelect = false;
    }

    
    /// <summary>
    /// ���ʐݒ�
    /// </summary>
    private void VolumeAction()
    {
        if (!SliderSelect)
        {
            //�J�[�\���̈ʒu�����킹��
            OptionCursor.transform.SetParent(VolumeRectTrans[OptionSelectNum]);
            CopyRectTransform(OptionCursorTrans, VolumeRectTrans[OptionSelectNum]);
            //OptionCursorTrans = VolumeRectTrans[OptionSelectNum];
            //Vector3 pos = VolumeRectTrans[OptionSelectNum].anchoredPosition;
            //OptionCursorTrans.anchoredPosition = pos;

           //�㉺�{�^�����擾
            bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
            bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

            //�I�v�V�������ڔԍ����X�V
            if (DownButton) { OptionSelectNum++; }
            if (UpButton) { OptionSelectNum--; }

            //�㉺��
            if (OptionSelectNum > VolumeRectTrans.Count - 1) { OptionSelectNum = 0; }
            if (OptionSelectNum < 0) { OptionSelectNum = VolumeRectTrans.Count - 1; }

            bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
            if (SelectButton) 
            {
                SliderAccelerationSpeed = (VolumeSlider[OptionSelectNum].maxValue - VolumeSlider[OptionSelectNum].minValue) / SliderSpeed;
                SliderSelect = true; 
            }
            
            //�I�v�V�����̏�����
            OptionInitialize();

        }
        else
        {
            //���X�e�B�b�N��x�������擾
            float SideStick = InputSystem.GetLeftStick().x;

            //�X���C�_�[�ɔ��f
            if (SideStick > 0.5f) { VolumeSlider[OptionSelectNum].value += SliderAccelerationSpeed; }
            else if (SideStick < -0.5f) { VolumeSlider[OptionSelectNum].value -= SliderAccelerationSpeed; }
            else {SliderAccelerationSpeed = (VolumeSlider[OptionSelectNum].maxValue - VolumeSlider[OptionSelectNum].minValue) / SliderSpeed; }

            float acceleration = (VolumeSlider[OptionSelectNum].maxValue - VolumeSlider[OptionSelectNum].minValue) / Slideracceleration;
            SliderAccelerationSpeed += acceleration;

            //�L�����Z��������
            bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);
            if (CanselButton) { SliderSelect = false; }

        }
        //float SliderValue = (SideStick + 1) / 2;    //0�`1�̐��l�ɕϊ�
        //VolumeSlider[OptionSelectNum].value = SliderValue;

    }

    /// <summary>
    /// BGM�ύX�p
    /// </summary>
    /// <param name="value"></param>
    private void SetBGMVolume(float value)
    {
        // �X���C�_�[�̒l��-80dB����0dB�ɕϊ�����AudioMixer�ɐݒ�
        float volume = Mathf.Log10(value) * 20;
        Audio.SetFloat("BGM", volume);
    }

    /// <summary>
    /// SE�ύX�p
    /// </summary>
    /// <param name="value"></param>
    private void SetSEVolume(float value)
    {
        // �X���C�_�[�̒l��-80dB����0dB�ɕϊ�����AudioMixer�ɐݒ�
        float volume = Mathf.Log10(value) * 20;
        Audio.SetFloat("SE", volume);
    }

    /// <summary>
    /// �J�����ݒ�
    /// </summary>
    private void CameraAction()
    {
        //�J�[�\���̈ʒu�����킹��
        OptionCursor.transform.SetParent(CameraRectTrans[OptionSelectNum]);
        CopyRectTransform(OptionCursorTrans, CameraRectTrans[OptionSelectNum]);
        //OptionCursorTrans = CameraRectTrans[OptionSelectNum];
        // Vector3 pos = CameraRectTrans[OptionSelectNum].anchoredPosition;
        // OptionCursorTrans.anchoredPosition = pos;

        if (!SliderSelect)
        {
            //�㉺�{�^�����擾
            bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
            bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

            //�I�v�V�������ڔԍ����X�V
            if (DownButton) { OptionSelectNum++; }
            if (UpButton) { OptionSelectNum--; }

            //�㉺��
            if (OptionSelectNum > CameraRectTrans.Count - 1) { OptionSelectNum = 0; }
            if (OptionSelectNum < 0) { OptionSelectNum = CameraRectTrans.Count - 1; }


            bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
            if (SelectButton) 
            {
                SliderAccelerationSpeed = (CameraSlider[OptionSelectNum].maxValue - CameraSlider[OptionSelectNum].minValue) / SliderSpeed;
                SliderSelect = true;
            }

            //�I�v�V�����̏�����
            OptionInitialize();

        }
        else
        {
            //���X�e�B�b�N��x�������擾
            float SideStick = InputSystem.GetLeftStick().x;

            //�X���C�_�[�ɔ��f
            if (SideStick > 0.5f) { CameraSlider[OptionSelectNum].value += SliderAccelerationSpeed; }
            else if (SideStick < -0.5f) { CameraSlider[OptionSelectNum].value -= SliderAccelerationSpeed; }
            else { SliderAccelerationSpeed = (CameraSlider[OptionSelectNum].maxValue - CameraSlider[OptionSelectNum].minValue) / SliderSpeed; }

            float acceleration = (CameraSlider[OptionSelectNum].maxValue - CameraSlider[OptionSelectNum].minValue) / Slideracceleration;
            SliderAccelerationSpeed += acceleration;

            //�L�����Z��������
            bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

            if (CanselButton) { SliderSelect = false; }

        }

        //float SliderValue = (SideStick + 1) / 2;    //0�`1�̐��l�ɕϊ�
        //CameraSlider[OptionSelectNum].value = QualitySlider.value;
    }


    /// <summary>
    /// Fov�ݒ�
    /// </summary>
    /// <param name="value"></param>
    private void SetFov(float value)
    {
        CameraFov.m_Lens.FieldOfView = value;
    }

    /// <summary>
    /// �掿�ݒ�
    /// </summary>
    private void QualityAction()
    {
        //�J�[�\���̈ʒu�����킹��
        //OptionCursor.transform.SetParent(QualityRectTrans);
        //CopyRectTransform(OptionCursorTrans, QualityRectTrans);
        //OptionCursorTrans = QualityRectTrans;
        //Vector3 pos = QualityRectTrans.anchoredPosition;
        //OptionCursorTrans.anchoredPosition = pos;

        //�㉺�{�^�����擾
        bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
        bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

        if (DownButton) { QualityLevel--; }
        if (UpButton) { QualityLevel++; }

        //�㉺��
        int MaxLevel = QualityLevelRectTrans.Count - 1;
        if (QualityLevel > MaxLevel) { QualityLevel = 0; }
        if(QualityLevel < 0) { QualityLevel = MaxLevel; }

        //�J�[�\���̈ړ�
        OptionCursor.transform.SetParent(QualityLevelRectTrans[QualityLevel]);
        CopyRectTransform(OptionCursorTrans, QualityLevelRectTrans[QualityLevel]);

        bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);

        if(SelectButton)
        {
            QualitySelectCursor.transform.SetParent(QualityLevelRectTrans[QualityLevel]);
            CopyRectTransform(QualitySelectCursor, QualityLevelRectTrans[QualityLevel]);
            //�掿�ύX
            QualitySettings.SetQualityLevel(QualityLevel + 1);
        }

        OptionInitialize();
        //���X�e�B�b�N��x�������擾
        //float SideStick = InputSystem.GetLeftStick().x;

        //�X���C�_�[�ɔ��f
        //if(SideStick > 0) { QualitySlider.value += 0.01f; }
        //if(SideStick < 0) { QualitySlider.value -= 0.01f; }
        //QualitySlider.value = (SideStick + 1) / 2;

        //QualitySettings.SetQualityLevel((int)QualitySlider.value);
        //Debug.Log(QualitySettings.GetQualityLevel());
    }


    private void ChangeQuality(int value)
    {
        QualitySettings.SetQualityLevel((int)value);
    }

    /// <summary>
    /// �X�g�[���[�̕\��
    /// </summary>
    private void StoryAction()
    {
        OptionPanels[(int)CurrentOption].SetActive(true);

        OptionInitialize();
    }

    
    /// <summary>
    /// RectTransFotm���R�s�[
    /// </summary?>
    /// <returns></returns>
    private void CopyRectTransform(RectTransform a,RectTransform b)
    {
        a.localPosition = Vector3.zero;
        a.localRotation = Quaternion.identity;
        a.localScale = Vector3.one;
        a.pivot = Vector2.zero;//b.pivot;
        a.anchorMin = Vector2.zero;//a.anchorMin;//new Vector2(0.5f,0.5f);
        a.anchorMax = Vector2.zero;//new Vector2(0.5f, 0.5f);
        a.anchoredPosition = Vector2.zero;
       
    }

}
