using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CS_Title : MonoBehaviour
{
    [SerializeField,Tooltip("�Q�[���J�n���̃V�[���̖��O")]
    private string GameSceneName;

    [SerializeField, Header("�Z�[�u�f�[�^")]
    private CS_DataSave Save;

    [Header("�^�C�g������")]
    [SerializeField, Tooltip("�I������Text")]
    private List<Text> TitleSelectText;

    [SerializeField, Tooltip("�I��p�J�[�\��")]
    private RectTransform TitleCursorTrans;
    [SerializeField, Tooltip("�I��p�J�[�\��Image�R���|�[�l���g")]
    private Image TitleCursorImage;
    [SerializeField, Tooltip("�ʏ펞�J�[�\���X�v���C�g")]
    private Sprite TitleCursorNormalSprite;
    [SerializeField, Tooltip("�I�𒆃J�[�\���X�v���C�g")]
    private Sprite TitleCursorSelectSprite;

    [SerializeField, Header("�I�𒆂�UI�T�C�Y�g��䗦")]
    private float Enlargement = 1.0f;

    //�g���ƒʏ펞�̃T�C�Y�����͑S�������T�C�Y�ƒf�肵��1�A�ς��Ȃ�List������
    private float EnlargementSize;
    private float NormalSize;

    [Header("Start�֌W")]
    [SerializeField, Tooltip("�Q�[�����n�߂邩�̉摜")]
    private GameObject StartQuestioImage;
    [SerializeField, Tooltip("�I��UITrans")]
    private List<RectTransform> StartQuestionTrans;
    [SerializeField, Tooltip("�J�[�\��UITrans")]
    private RectTransform StartCursorTrans;
    private int StartQuestionNum = 1;   //0 �͂��@1 ������

    [Header("�Â�����")]
    [SerializeField, Tooltip("�J�[�\���g��X�s�[�h")]
    private float CursorZoomSize = 2.0f;
    [SerializeField, Tooltip("�t�F�[�h�A�E�g�pImage")]
    private Image FadeOutImage;
    [SerializeField, Tooltip("�t�F�[�h�A�E�g�X�s�[�h")]
    private float FadeOutSpeed = 2.0f;

    [Header("Option�֌W")]
    [SerializeField,Tooltip("Option�Ǘ��p�X�N���v�g")]
    private CS_OptionSystem OptionControl;
    [SerializeField, Tooltip("Opiton�p�p�l��")]
    private GameObject OptionPanel;

    [SerializeField, Header("AudioSource")]
    private AudioSource audio;

    [Header("SE")]
    [SerializeField]
    private AudioClip CursorSE;
    [SerializeField]
    private AudioClip SelectSE;
    [SerializeField]
    private AudioClip CancelSE;

    [SerializeField, Header("���̓V�X�e���X�N���v�g")]
    private CS_InputSystem CSInput;

    private enum TitleState
    {
        START = 0,
        CONTINUE = 1,
        OPTION = 2
    };

    private TitleState state;   //�^�C�g���̑I�����

    private bool TitleSelect = false;       //�I�����Ă��邩

    private void Start()
    {
        //���ڑI�����ƒʏ펞�̃T�C�Y��ݒ�
        EnlargementSize = TitleSelectText[0].fontSize * Enlargement;
        NormalSize = TitleSelectText[0].fontSize;

        ////�I�𒆃e�L�X�g�̐ݒ�
        //StartQuestionTexts[StartQuestionNum].color = Color.red;
    }

    private void Update()
    {
        if (!TitleSelect)
        {
            int oldstate = (int)state;

            //���͏�Ԏ擾
            bool DownButton = CSInput.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
            bool UpButton = CSInput.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

            //�I����ԍX�V
            if (DownButton) { state++; PlaySE(CursorSE); }
            if (UpButton) { state--; PlaySE(CursorSE); }

            //�㉺��
            if ((int)state > (int)TitleState.OPTION) { state = TitleState.START; }
            if ((int)state < (int)TitleState.START) { state = TitleState.OPTION; }

            TitleSelectText[(int)state].fontSize = (int)EnlargementSize;

            //�O��ƍ���̏�Ԃ��������ύX���ꂽ����
            bool Change = (int)state != oldstate;
            if (Change) { CursorMoveAction(oldstate); }

            bool Select = CSInput.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
            if (Select) { SelectTriggerAction(); PlaySE(SelectSE); }

            //ESC�ŃQ�[���I��
            //if(Input.GetKeyDown(KeyCode.Escape))
            //{
            //    OptionAction();
            //}

        }
        else
        {
            SelectAction();
        }
    }

    /// <summary>
    /// �J�[�\���ړ����̃A�N�V����
    /// </summary>
    private void CursorMoveAction(int old)
    {
        TitleSelectText[old].fontSize = (int)NormalSize;

        //�J�[�\���̈ʒu��ݒ�
        Vector3 pos = TitleCursorTrans.anchoredPosition;
        TitleCursorTrans.SetParent(TitleSelectText[(int)state].transform.parent);
        TitleCursorTrans.anchoredPosition = pos;

    }

    /// <summary>
    /// �I�����̃A�N�V����
    /// </summary>
    private void SelectTriggerAction()
    {
        TitleSelect = true;
        //TitleCursorImage.sprite = TitleCursorSelectSprite;  //�X�v���C�g��I�𒆂�

        //�I�v�V�������̂�
        if (state == TitleState.OPTION)
        {
            if (!OptionPanel.activeSelf) { OptionPanel.SetActive(true); }
            if (OptionPanel.GetComponent<CS_OptionSystem>().GetOptionBack())
            {
                OptionPanel.SetActive(false);
                state = TitleState.START;
            }
        }

    }

    /// <summary>
    /// �L�����Z�����̃A�N�V����
    /// </summary>
    private void CanselTiggerAction()
    {
        TitleSelect = false;
        //TitleCursorImage.sprite = TitleCursorNormalSprite;  //�X�v���C�g��ʏ��


        //�I�v�V�������̂�
        if (state == TitleState.OPTION)
        {
            OptionPanel.SetActive(false);
        }
        
    }

    /// <summary>
    /// ���ڑI�𒆂̃A�N�V����
    /// </summary>

    void SelectAction()
    {
        switch(state)
        {
            case TitleState.START:
                StartGame();    //�Q�[���X�^�[�g
                break;
            case TitleState.CONTINUE:
                //��������V�ׂ�悤�ɃZ�[�u�V�X�e��
                ContinueAction();
                break;
            case TitleState.OPTION:
                OptionAction();
                break;
        }
    }

    /// <summary>
    /// �Q�[���X�^�[�g
    /// </summary>
    private void StartGame()
    {
        //�}�X�N�摜�\��
        if (!StartQuestioImage.activeSelf)
        {
            StartQuestioImage.SetActive(true);
        }


        bool leftButton = CSInput.GetDpadLeftTriggered() || Input.GetKeyDown(KeyCode.LeftArrow);
        bool RightButton = CSInput.GetDpadRightTriggered() || Input.GetKeyDown(KeyCode.RightArrow);

        int oldquestionnum = StartQuestionNum;

        //�͂����������̑I����
        if (leftButton) { StartQuestionNum++; PlaySE(CursorSE); }
        if (RightButton) { StartQuestionNum--; PlaySE(CursorSE); }
        if(StartQuestionNum > StartQuestionTrans.Count - 1) { StartQuestionNum = 0; }
        if (StartQuestionNum < 0) { StartQuestionNum = StartQuestionTrans.Count - 1; }

        //�ύX���ꂽ��J�[�\�����ړ�
        bool Change = oldquestionnum != StartQuestionNum;
        if (Change) 
        {
            StartCursorTrans.SetParent(StartQuestionTrans[StartQuestionNum]);
            StartCursorTrans.anchoredPosition = Vector2.zero;
            //StartQuestionTexts[oldquestionnum].color = Color.white; //��
            //StartQuestionTexts[StartQuestionNum].color = Color.red; //�� 
        }


        //�I��
        bool Cansel = CSInput.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

        //�I��
        bool SelectButton = CSInput.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);

        bool Start = CSInput.GetButtonStartTriggered();

        if (SelectButton && StartQuestionNum == 0)
        {
            //�Z�[�u�f�[�^�����Z�b�g
            Save.ResetSaveData();
            //�Q�[���J�n
            SceneManager.LoadScene(GameSceneName);
            PlaySE(SelectSE);
        }
        else if(SelectButton && StartQuestionNum == 1)
        {
            //�I��
            Cansel = true;
            PlaySE(CancelSE);
        }
        
        if(Start && StartQuestionNum == 0)
        {
            //�ő����R�}���h
            Save.MaxClearCommand();
            //�Q�[���J�n
            SceneManager.LoadScene(GameSceneName);
        }

        if (Cansel)
        {
            StartQuestioImage.SetActive(false);
            CanselTiggerAction();
            PlaySE(CancelSE);
        }

    }

    /// <summary>
    /// �I�v�V�����A�N�V����
    /// </summary>
    private void OptionAction()
    {
        //bool Cansel = CSInput.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);
        bool OptionCansel = OptionControl.GetOptionBack();   //�I�v�V�������J��
        if (OptionCansel) { CanselTiggerAction(); }
    }


    /// <summary>
    /// �Â�����
    /// </summary>
    private void ContinueAction()
    {
        //�J�[�\���g��
        //Vector2 size = TitleCursorTrans.sizeDelta;
        //size.x += CursorZoomSize * Time.deltaTime;
        //size.y += CursorZoomSize * Time.deltaTime;
        //TitleCursorTrans.sizeDelta = size;

        //�t�F�[�h�A�E�g����
        Color Fadecolor = FadeOutImage.color;
        bool fade = Fadecolor.a < 1f;
        if (fade)
        {
            Fadecolor.a += FadeOutSpeed * Time.deltaTime;
            FadeOutImage.color = Fadecolor;
        }
        else
        {
            //��U���ʂɊJ�n
            //�Q�[���J�n
            SceneManager.LoadScene(GameSceneName);
        }
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void OPTIONGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 

    }


    private void PlaySE(AudioClip clip)
    {
        audio.PlayOneShot(clip);
    }


}
