using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//�L�[�ۑ��p
[System.Serializable]
public class StringSpritePair
{
    public string key;
    public Image KeyImage;
    public Sprite NormalSprite;
    public Sprite PushSprite;
}

//�L�[�ۑ��p
[System.Serializable]
public class DialogData
{
    public string DialogText;
    public Sprite DiaLogImage;
    //public bool
}


/// <summary>
/// �S��:���@�|�[�Y
/// </summary>
public class CS_Pose : MonoBehaviour
{

    [SerializeField,Tooltip("�|�[�Y�p�J�[�\��")]
    private RectTransform PoseCursor;
    [SerializeField,Tooltip("�J�[�\���ړ��ʒuList")]
    private List<RectTransform> PoseTransList;
    [SerializeField, Tooltip("�|�[�Y�p�I�u�W�F�N�g")]
    private GameObject PosePanel;

    //�|�[�Y�̑I�����
    private enum PoseState
    {
        NONE = 0,      //�I�����Ă��Ȃ�
        OPTION = 1,    //�I�v�V����
        RETIRE = 2,    //���^�C�A
        HELP = 3,      //�w���v(�_�C�A���O�H)
        OPERATING = 4, //�������
        RESUME = 5,    //�Q�[���ɖ߂�
    }

    //���݂̏��
    [SerializeField,Header("���̏��")]
    private PoseState CurrentState = PoseState.NONE;

    //���ڑI�𒆂�
    private bool Select = false;

    //����ƃL�����Z����ێ�
    private bool Decision = false;
    private bool Cancel = false;

    private bool PoseStartFlg = false;

    [Header("=============�V�X�e��===============")]
    [SerializeField, Tooltip("����")]
    private CS_InputSystem InputSystem;

    [Header("=============�I�v�V����===============")]
    [SerializeField, Tooltip("�I�v�V�����I�u�W�F�N�g")]
    private GameObject OptionObj;
    [SerializeField, Tooltip("�I�v�V�����}�X�N")]
    private RectMask2D OptionMask;
    [SerializeField, Tooltip("�I�v�V�����e�L�X�g")]
    private Text OptionText;
    [SerializeField, Tooltip("�I�v�V�����V�X�e��")]
    private CS_OptionSystem OptionSystem;
    [SerializeField, Tooltip("�߂�{�^����������")]
    private float BbuttonPushTime = 3f;
    private Coroutine CurrentCoroutine;
    private bool LongPushFlg = false;   //�������t���O

    [Header("=============���^�C�A===============")]
    [SerializeField, Tooltip("���^�C�A�I�u�W�F�N�g")]
    private GameObject RetireObj;
    [SerializeField, Tooltip("�J�ڃV�[����")]
    private string NextSceneName;
    [SerializeField, Tooltip("���^�C�A�J�[�\��")]
    private RectTransform RetireCursor;
    [SerializeField, Tooltip("�I�����ڍ��W")]
    private List<Vector2> RetireSelsctPos; 
    private int Yes = 1;    //�I�� 0:�͂� 1:������

    [Header("============�w���v===============")]
    [SerializeField, Tooltip("�w���v�I�u�W�F�N�g")]
    private GameObject HelpObj;
    [SerializeField, Tooltip("�w���v�e�L�X�g")]
    private Text HelpText;
    [SerializeField, Tooltip("�_�C�A���O���X�g")]
    private List<DialogData> Dialogs;
    [SerializeField, Tooltip("�_�C�A���O����")]
    private Image DialogUpArrow;
    [SerializeField, Tooltip("�_�C�A���O�����")]
    private Image DialogDownArrow;
    [SerializeField, Tooltip("�e�L�X�g����")]
    private Image DialogTextUpArrow;
    [SerializeField, Tooltip("�e�L�X�g�����")]
    private Image DialogTextDownArrow;
    [SerializeField, Tooltip("�_�C�A���O�e�L�X�g�\���p�R���|�[�l���g")]
    private List<Text> DialogTexts;
    [SerializeField, Tooltip("�_�C�A���O�\���p�R���|�[�l���g")]
    private Image DialogImage;
    [SerializeField, Tooltip("�{�^�����������F")]
    private Color ButtonPushArrowColor;
   
    //[SerializeField, Tooltip("���݉�����Ă��郍�O")]
    //private List<Image> ;
    //�\�����Ă���_�C�A���O�i���o�[
    private int CurrentDialogNum = 0;



    [Header("=============�������===============")]
    [SerializeField, Tooltip("��������I�u�W�F�N�g")]
    private GameObject OperatingObj;
    [SerializeField, Tooltip("�L�[�ۑ��p")]
    private List<StringSpritePair> KeyData;
    //�R���g���[���[�X�e�B�b�N�ړ��p
    private Vector2 LSPos;
    private Vector2 RSPos;



    [Header("=============�R���g���[���[�U��===============")]
    [Header("�y�J�[�\���ړ����̐U���ݒ�z")]
    [SerializeField, Tooltip("�U���̒���")]
    private float Cursorduration = 0.5f;         // �U���̒���
    [SerializeField, Tooltip("�U���̋���")]
    private int CursorpowerType = 1;          // �U���̋����i4�i�K�j
    [SerializeField, Tooltip("�U���̎��g��")]
    private AnimationCurve CursorcurveType;          // �U���̎��g��
    [SerializeField, Tooltip("�J��Ԃ���")]
    private int Cursorrepetition = 1;         // �J��Ԃ���

    [Header("�y���莞�̐U���ݒ�z")]
    [SerializeField, Tooltip("�U���̒���")]
    private float Decisionduration = 0.5f;         // �U���̒���
    [SerializeField, Tooltip("�U���̋���")]
    private int DecisionpowerType = 1;          // �U���̋����i4�i�K�j
    [SerializeField, Tooltip("�U���̎��g��")]
    private AnimationCurve DecisioncurveType;          // �U���̎��g��
    [SerializeField, Tooltip("�J��Ԃ���")]
    private int Decisionrepetition = 1;         // �J��Ԃ���

    [SerializeField, Header("�ݒ�p�p�l��")]
    private List<GameObject> SettingPanels;    //�\���p�p�l��


    // Start is called before the first frame update
    void Start()
    {
        //���^�C�A�̃J�[�\���ړ�
        RetireCursor.anchoredPosition = RetireSelsctPos[Yes];

        //��������̃X�e�B�b�N���W�ۑ�
        LSPos = KeyData[9].KeyImage.rectTransform.anchoredPosition;
        RSPos = KeyData[10].KeyImage.rectTransform.anchoredPosition;

    }

    private void Option()
    {
        //�I�v�V�����̕\��
        if (!OptionObj.activeSelf) { OptionObj.SetActive(true); }
        if (!OptionMask.enabled) { OptionMask.enabled = true; }
        if (OptionText.enabled) { OptionText.enabled = false; }

        //�I���������擾
        bool End = OptionSystem.GetOptionBack();

        Debug.Log(OptionSystem.GetOptionBack() == true);

        if (End)
        {
            OptionObj.SetActive(false);
            OptionMask.enabled = false;
            OptionText.enabled = true;
            Select = false;
        }

    }

    private void Retire()
    {
        //���^�C�A�_�C�A���O�̕\��
        if (!RetireObj.activeSelf)
        {
            RetireObj.SetActive(true);
            return;
        }

        //���E�����������ǂ���
        bool LeftButton = InputSystem.GetDpadLeftTriggered();
        bool RightButton = InputSystem.GetDpadRightTriggered();
        if (LeftButton || RightButton) { Yes = Yes == 0 ? 1 : 0; }

        //�J�[�\���ړ�
        RetireCursor.anchoredPosition = RetireSelsctPos[Yes];

        //�߂�
        if (Cancel)
        {
            //���������Ĕ�\��
            Yes = 1;
            RetireObj.SetActive(false);
            //�I����Ԃ̉���
            Select = false;
        }

        //����{�^�����������珈��
        if (!Decision) { return; }

        //�V�[���J��
        if (Yes == 0) { SceneManager.LoadScene(NextSceneName); }
        else 
        {
            //���������Ĕ�\��
            Yes = 1;
            RetireObj.SetActive(false);
            //�I����Ԃ̉���
            Select = false;
        }
    }

    private void Help()
    {
        if (!HelpObj.activeSelf)
        {
            //���̏����\����
            Transform parent = HelpObj.transform.parent;
            parent.transform.SetParent(PosePanel.transform);
            OptionText.enabled = false;
            HelpText.enabled = false;

            OptionMask.enabled = true;

            HelpObj.SetActive(true); 
        }

        //����(�㉺)
        bool DownButton = InputSystem.GetDpadDownTriggered();
        bool UpButton = InputSystem.GetDpadUpTriggered();

        //�O��̃_�C�A���O�ԍ���ۑ�
        int olddialognum = CurrentDialogNum;

        if (DownButton) 
        {
            CS_ControllerVibration.StartVibrationWithCurve(Cursorduration, CursorpowerType, CursorcurveType, Cursorrepetition);
            CurrentDialogNum++; 
        } 
        if (UpButton) 
        {
            CS_ControllerVibration.StartVibrationWithCurve(Cursorduration, CursorpowerType, CursorcurveType, Cursorrepetition);
            CurrentDialogNum--; 
        }
       
        //���̐F�ύX
        bool DownPressed = InputSystem.GetDpadDownPressed();
        bool UpPressed = InputSystem.GetDpadUpPressed();
        if(DownPressed)
        {
            DialogDownArrow.color = ButtonPushArrowColor;
            DialogTextDownArrow.color = ButtonPushArrowColor;
        }
        else
        {
            DialogDownArrow.color = Color.white;
            DialogTextDownArrow.color = Color.white;
        }
        if(UpPressed)
        {
            DialogUpArrow.color = ButtonPushArrowColor;
            DialogTextUpArrow.color = ButtonPushArrowColor;
        }
        else
        {
            DialogUpArrow.color = Color.white;
            DialogTextUpArrow.color = Color.white;
        }

        //���͏㉺���̐ݒ�
        if (CurrentDialogNum > Dialogs.Count - 1) { CurrentDialogNum = 0; }
        if (CurrentDialogNum < 0) { CurrentDialogNum = Dialogs.Count - 1; }

        //�_�C�A���O�摜�ؑ�
        DialogImage.sprite = Dialogs[CurrentDialogNum].DiaLogImage;
        
        //���я��ɍX�V
        int count = Dialogs.Count;
        int previous2Index = (CurrentDialogNum - 2 + count) % count;
        int previousIndex = (CurrentDialogNum - 1 + count) % count;
        int NextIndex = (CurrentDialogNum + 1 + count) % count;
        int Next2Index = (CurrentDialogNum + 2 + count) % count;

        DialogTexts[3].text = Dialogs[previous2Index].DialogText;
        DialogTexts[4].text = Dialogs[previousIndex].DialogText;
        DialogTexts[0].text = Dialogs[CurrentDialogNum].DialogText;
        DialogTexts[1].text = Dialogs[NextIndex].DialogText;
        DialogTexts[2].text = Dialogs[Next2Index].DialogText;

        if(Cancel)
        {
            Select = false;
            //���̏����\����
            Transform parent = HelpObj.transform.parent;
            parent.transform.SetParent(OptionMask.transform);
            parent.transform.SetSiblingIndex(1);
            OptionText.enabled = true;
            HelpText.enabled = true;

            OptionMask.enabled = false;

            HelpObj.SetActive(false);
        }


    }

    private void Operating()
    {
        if (!OperatingObj.activeSelf) { OperatingObj.SetActive(true); }

        //---------------�{�^�����ƂɕύX------------------
        bool a = InputSystem.GetButtonAPressed();
        bool b = InputSystem.GetButtonBPressed();
        bool x = InputSystem.GetButtonXPressed();
        bool y = InputSystem.GetButtonYPressed();
        bool Start = InputSystem.GetButtonStartPressed();

        bool left = InputSystem.GetDpadLeftPressed();
        bool right = InputSystem.GetDpadRightPressed();
        bool up = InputSystem.GetDpadUpPressed();
        bool down = InputSystem.GetDpadDownPressed();

        bool lb = InputSystem.GetButtonLPressed();
        bool lt = InputSystem.GetLeftTrigger() > 0.1f;
        bool rb = InputSystem.GetButtonRPressed();
        bool rt = InputSystem.GetRightTrigger() > 0.1f;

        bool share = InputSystem.GetButtonSharePressed();
        bool view = InputSystem.GetButtonViewPressed();

        Vector2 ls = InputSystem.GetLeftStick();
        Vector2 rs = InputSystem.GetRightStick();

        

        if (a) { KeyData[0].KeyImage.sprite = KeyData[0].PushSprite; }
        else { KeyData[0].KeyImage.sprite = KeyData[0].NormalSprite; }
        //�߂鏈��
        if (b)
        {
            if(CurrentCoroutine == null) 
            {
                CurrentCoroutine =
                StartCoroutine(LongPushButton(BbuttonPushTime)); 
            }
            KeyData[1].KeyImage.sprite = KeyData[1].PushSprite; 
        }
        else
        {
            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
            KeyData[1].KeyImage.sprite = KeyData[1].NormalSprite; 
        }
        if (x) { KeyData[2].KeyImage.sprite = KeyData[2].PushSprite; }
        else { KeyData[2].KeyImage.sprite = KeyData[2].NormalSprite; }
        if (y) { KeyData[3].KeyImage.sprite = KeyData[3].PushSprite; }
        else { KeyData[3].KeyImage.sprite = KeyData[3].NormalSprite; }


        if (up) { KeyData[4].KeyImage.sprite = KeyData[4].PushSprite; }
        else { KeyData[4].KeyImage.sprite = KeyData[4].NormalSprite; }
        if (down) { KeyData[5].KeyImage.sprite = KeyData[5].PushSprite; }
        else { KeyData[5].KeyImage.sprite = KeyData[5].NormalSprite; }
        if (left) { KeyData[6].KeyImage.sprite = KeyData[6].PushSprite; }
        else { KeyData[6].KeyImage.sprite = KeyData[6].NormalSprite; }
        if (right) { KeyData[7].KeyImage.sprite = KeyData[7].PushSprite; }
        else { KeyData[7].KeyImage.sprite = KeyData[7].NormalSprite; }

        if (Start) { KeyData[8].KeyImage.sprite = KeyData[8].PushSprite; }
        else { KeyData[8].KeyImage.sprite = KeyData[8].NormalSprite; }


        if (lb) { KeyData[11].KeyImage.sprite = KeyData[11].PushSprite; }
        else { KeyData[11].KeyImage.sprite = KeyData[11].NormalSprite; }
        if (lt) { KeyData[12].KeyImage.sprite = KeyData[12].PushSprite; }
        else { KeyData[12].KeyImage.sprite = KeyData[12].NormalSprite; }
        if (rb) { KeyData[13].KeyImage.sprite = KeyData[13].PushSprite; }
        else { KeyData[13].KeyImage.sprite = KeyData[13].NormalSprite; }
        if (rt) { KeyData[14].KeyImage.sprite = KeyData[14].PushSprite; }
        else { KeyData[14].KeyImage.sprite = KeyData[14].NormalSprite; }
        if (share) { KeyData[15].KeyImage.sprite = KeyData[15].PushSprite; }
        else { KeyData[15].KeyImage.sprite = KeyData[15].NormalSprite; }
        if (view) { KeyData[16].KeyImage.sprite = KeyData[16].PushSprite; }
        else { KeyData[16].KeyImage.sprite = KeyData[16].NormalSprite; }



        //0.3��臒l
        if (ls.x > 0.3f || ls.x < -0.3f || ls.y > 0.3f || ls.y < -0.3f) { KeyData[9].KeyImage.sprite = KeyData[9].PushSprite; }
        else { KeyData[9].KeyImage.sprite = KeyData[9].NormalSprite; }
        if (rs.x > 0.3f || rs.x < -0.3f || rs.y > 0.3f || rs.y < -0.3f) { KeyData[10].KeyImage.sprite = KeyData[10].PushSprite; }
        else { KeyData[10].KeyImage.sprite = KeyData[10].NormalSprite; }

        //�X�e�B�b�N�𓮂���
        KeyData[9].KeyImage.rectTransform.anchoredPosition = LSPos + (ls * 10f);
        KeyData[10].KeyImage.rectTransform.anchoredPosition = RSPos + (rs * 10f);


        //�I��
        if (LongPushFlg)
        {
            LongPushFlg = false;
            OperatingObj.SetActive(false);
            Select = false;
        }

    }


    /// <summary>
    /// �{�^�������������m����
    /// </summary>
    /// <returns></returns>
    private IEnumerator LongPushButton(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        LongPushFlg = true;
    }

    private void Resume()
    {
        //�|�[�Y�I��
        EndPose();
        //�Ȃɂ����Ă��Ȃ���Ԃ�
        //CurrentState = PoseState.NONE;
    }

    private void SelectAction()
    {
        switch (CurrentState)
        {
            case PoseState.NONE:     �@//�Ȃɂ����Ă��Ȃ�
                //EndPose();
                break;
            case PoseState.OPTION:     //�I�v�V����
                Option();
                break;
            case PoseState.RETIRE:     //���^�C�A
                Retire();
                break;
            case PoseState.HELP:       //�w���v(�_�C�A���O)
                Help();
                break;
            case PoseState.OPERATING:  //�������
                Operating();
                break;
            case PoseState.RESUME:     //�Q�[���ɖ߂�
                Resume();
                break;
        }


    }

    /// <summary>
    /// �|�[�Y�I��
    /// </summary>
    private void EndPose()
    {
        Time.timeScale = 1f;            //���Ԃ�߂�
        PosePanel.SetActive(false);    //���g���\����
        Select = false;
        CurrentState = PoseState.NONE;
    }

    /// <summary>
    /// �|�[�Y�J�n
    /// </summary>
    private void PoseStart()
    {
        Time.timeScale = 0f;
        PosePanel.SetActive(true);
        CurrentState = PoseState.OPTION;
    }


    // Update is called once per frame
    void Update()
    {
        bool startbutton = InputSystem.GetButtonStartTriggered();
        if (startbutton && !Select) { PoseStartFlg = true; }
        if (PoseStartFlg) 
        {
            if(CurrentState != PoseState.NONE) { EndPose(); }
            else if(CurrentState == PoseState.NONE) { PoseStart(); }
            PoseStartFlg = false;
        }

        if(CurrentState == PoseState.NONE) { return; }

        Cancel = InputSystem.GetButtonBTriggered();�@//�߂�
        Decision = InputSystem.GetButtonATriggered(); //����

        //���ڑI�𒆂��̏��
        if (Decision)
        {
            CS_ControllerVibration.StartVibrationWithCurve(Decisionduration, DecisionpowerType, DecisioncurveType, Decisionrepetition);
            Select = true; 
        }

        if (Select) { SelectAction(); } //�I���s��
        else { CursorMove(); }          //�J�[�\���ړ�        
    
    }

    /// <summary>
    /// �J�[�\���ړ�
    /// </summary>
    private void CursorMove()
    {
        //����(�㉺)
        bool DownButton = InputSystem.GetDpadDownTriggered();
        bool UpButton = InputSystem.GetDpadUpTriggered();

        if (DownButton)
        {
            CS_ControllerVibration.StartVibrationWithCurve(Cursorduration, CursorpowerType, CursorcurveType, Cursorrepetition);
            CurrentState++; 
        }
        if (UpButton)
        {
            CS_ControllerVibration.StartVibrationWithCurve(Cursorduration, CursorpowerType, CursorcurveType, Cursorrepetition);
            CurrentState--; 
        }

        //���͏㉺���̐ݒ�
        if (CurrentState < PoseState.OPTION) { CurrentState = PoseState.RESUME; }
        if (CurrentState > PoseState.RESUME) { CurrentState = PoseState.OPTION; }

        PoseCursor.anchoredPosition = PoseTransList[(int)CurrentState - 1].anchoredPosition;
    }


}
