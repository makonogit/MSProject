using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_PoseMenu : MonoBehaviour
{

    [SerializeField, Header("MenuAnimator")]
    private List<Animator> MenuAnimators;
    [SerializeField, Header("MenuRectTrans")]
    private List<RectTransform> MenuRectTrans;

    [SerializeField, Header("Cursor")]
    private RectTransform CursorTrans;

    [Header("�|�[�Y�p�p�l����")]
    [SerializeField, Tooltip("�|�[�Y�p�l��")]
    private GameObject PosePanel;
    [SerializeField,Tooltip("�|�[�Y���ڃp�l��")]
    private List<GameObject> PosePanels;
  

    [SerializeField, Header("Option")]
    private CS_Option OptionManager;

    //�I�v�V�����p�J�[�\��
    [SerializeField, Header("OptionCursor")]
    private GameObject OptionCursor;
    [SerializeField]
    private RectTransform OptionRectTrans;

    [SerializeField,Header("TitleBack����")]
    private List<RectTransform> TitleSelect;
    private int TitleSelectNum = 0;     //�^�C�g�����ڐݒ�
    [SerializeField, Tooltip("�^�C�g���V�[����")]
    string TitleSceneName;

    [SerializeField, Header("InputSystem")]
    private CS_InputSystem InputSystem;



    private int MaxMenu = 0;        //���ڐ�

    private const float CursorSize = 50; //�J�[�\���̃T�C�Y

    //���j���[�I�����
    private enum SelectState
    {
        Resume = 0,
        Option = 1,
        Manual = 2,
        TitleBack = 3
    };

    private SelectState CurrentSelect;

    //�I�v�V�������
    private enum OptionState
    {
        Volume = 0,     //����
        Camera = 1,     //�J����
        Quality = 2,    //�掿
        Story = 3,      //�X�g�[���[
    }

    private OptionState CurrentOption;

    //�I�𒆂�
    private bool Selected = false;
    private void Start()
    {
        MaxMenu = MenuAnimators.Count - 1;
        
        //�O������Ăяo���Ă�������
        SetPoseState(true);
    }

    // Update is called once per frame
    void Update()
    {
        bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
        bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

        if (Selected)
        {
            MenuSelect();
        }
        else
        {
            bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
            //���͂��Ȃ���ΏI��
            //if(!DownButton && !UpButton && !SelectButton) { return; }

            //���j���[�ԍ����X�V
            int oldMenu = (int)CurrentSelect;
            if (DownButton) { CurrentSelect++; }
            if (UpButton) { CurrentSelect--; }

            if (oldMenu != (int)CurrentSelect)
            {
                //�J�[�\���ړ�
                CursorMove(oldMenu);
            }


            //�|�[�Y�I�����ڂ�\��
            if (SelectButton)
            {
                Selected = true;
                MenuInfo();
            }
        }

    }

    /// <summary>
    /// �J�[�\���̈ړ�
    /// </summary>
    /// <param �O��̃J�[�\���ʒu="old"></param>
    private void CursorMove(int old)
    {
        //�㉺��
        if ((int)CurrentSelect > MaxMenu) { CurrentSelect = 0; }
        if ((int)CurrentSelect < 0) { CurrentSelect = (SelectState)MaxMenu; }

        //����ƑO���Animator�̏�Ԃ�ۑ�
        MenuAnimators[(int)CurrentSelect].SetBool("OnCursor", true);
        MenuAnimators[old].SetBool("OnCursor", false);

        //�J�[�\���̈ʒu��ݒ肷��
        Vector3 pos = MenuRectTrans[(int)CurrentSelect].anchoredPosition;
        pos.y -= CursorSize;
        CursorTrans.anchoredPosition = pos;
    }
    private void MenuSelect()
    {
        //MenuAnimators[(int)CurrentSelect].SetBool("IsSelect", true);

        //�L�����Z��������
        bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

        switch (CurrentSelect)
        {
            case SelectState.Resume:
             //   MenuAnimators[(int)CurrentSelect].SetBool("IsSelect", false);
                Selected = false;
                SetPoseState(false);
                break;
            case SelectState.Option:
                bool Optionselect = OptionManager.OptionAction();
                if (CanselButton && !Optionselect)
                {
                    //�|�[�Y�p�l��������Δ�\��
                    bool Panal = PosePanels[(int)CurrentSelect];
                    if (Panal) { PosePanels[(int)CurrentSelect].SetActive(false); }
                    Selected = false;
                }
                break;
            case SelectState.Manual:
                if (CanselButton)
                {
                    //�|�[�Y�p�l��������Δ�\��
                    bool Panal = PosePanels[(int)CurrentSelect];
                    if (Panal) { PosePanels[(int)CurrentSelect].SetActive(false); }
                    Selected = false;
                }
                //ManualAction();
                break;
            case SelectState.TitleBack:
                bool TitleBackNo = TitleBackAction();
                if (CanselButton || TitleBackNo)
                {
                    Debug.Log(PosePanels[(int)CurrentSelect]);
                    //�|�[�Y�p�l��������Δ�\��
                    bool Panal = PosePanels[(int)CurrentSelect];
                    if (Panal) { PosePanels[(int)CurrentSelect].SetActive(false); }
                    TitleSelectNum = 0;
                    Selected = false;
                }
                break;
        }
    }

    /// <summary>
    /// �|�[�Y��Ԑݒ�
    /// </summary>
    /// <param �|�[�Y���="State"></param>
    public void SetPoseState(bool State)
    {
        if (State)
        {
            Time.timeScale = 0.0f;
            PosePanel.SetActive(true);
        }
        if (!State)
        { 
            Time.timeScale = 1.0f;
            PosePanel.SetActive(false);
        }
    }


    /// <summary>
    /// �|�[�Y���ڂ̕\��
    /// </summary>
    private void MenuInfo()
    {
        bool PanalInfo = PosePanels[(int)CurrentSelect];
        if(PanalInfo)
        {
            PosePanels[(int)CurrentSelect].SetActive(true);
        }
    }

    /// <summary>
    /// �Q�[���ɖ߂�
    /// </summary>
    private void ManualAction()
    {
        //������\��(�L�����Z������Ă��Ȃ��Ȃ�\��)
       // ManualPanel.SetActive(true);
    }

    private void OptionAction()
    {

        ////�I�v�V�����p�l����\��
        //OptionPanel.SetActive(true);

        ////�L�����Z��������
        //bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

        ////�L�����Z�����ꂽ��I��
        //if(CanselButton)
        //{
        //    OptionPanel.SetActive(false);
        //    return;
        //}

        ////����{�^��
        //bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);

        //bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
        //bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

        ////���j���[�ԍ����X�V
        //int oldoption = (int)CurrentOption;
        //if (DownButton) { CurrentOption++; }
        //if (UpButton) { CurrentOption--; }

        ////���݂̃p�l���̕\��
       
        //switch (CurrentOption)
        //{
        //    case OptionState.Volume:
        //        break;
        //    case OptionState.Camera:
        //        break;
        //    case OptionState.Quality:
        //        break;
        //    case OptionState.Story:
        //        break;
        //}

    }
    private bool TitleBackAction()
    {
        bool LeftButton = InputSystem.GetDpadLeftTriggered() || Input.GetKeyDown(KeyCode.LeftArrow);
        bool RightButton = InputSystem.GetDpadRightTriggered() || Input.GetKeyDown(KeyCode.RightArrow);

        if (LeftButton) { TitleSelectNum--; }
        if (RightButton) { TitleSelectNum++; }

        //�㉺��
        if (TitleSelectNum > TitleSelect.Count - 1) { TitleSelectNum = 0; }
        if(TitleSelectNum < 0) { TitleSelectNum = TitleSelect.Count - 1; }

        //�J�[�\���ʒu����
        OptionCursor.transform.SetParent(TitleSelect[TitleSelectNum].transform);
        OptionRectTrans.anchorMin = TitleSelect[TitleSelectNum].anchorMin;
        OptionRectTrans.anchorMax = TitleSelect[TitleSelectNum].anchorMax;
        OptionRectTrans.anchoredPosition = Vector2.zero;
      

        bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
        
        bool Yes = SelectButton && TitleSelectNum == 0;
        bool No = SelectButton && TitleSelectNum != 0;

        //�^�C�g���ɖ߂�
        if (Yes){ SceneManager.LoadScene(TitleSceneName); }
        
        return No;

    }
}
