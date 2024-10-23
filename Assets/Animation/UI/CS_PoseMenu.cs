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

    [Header("ポーズ用パネル類")]
    [SerializeField, Tooltip("ポーズパネル")]
    private GameObject PosePanel;
    [SerializeField,Tooltip("ポーズ項目パネル")]
    private List<GameObject> PosePanels;
  

    [SerializeField, Header("Option")]
    private CS_Option OptionManager;

    //オプション用カーソル
    [SerializeField, Header("OptionCursor")]
    private GameObject OptionCursor;
    [SerializeField]
    private RectTransform OptionRectTrans;

    [SerializeField,Header("TitleBack項目")]
    private List<RectTransform> TitleSelect;
    private int TitleSelectNum = 0;     //タイトル項目設定
    [SerializeField, Tooltip("タイトルシーン名")]
    string TitleSceneName;

    [SerializeField, Header("InputSystem")]
    private CS_InputSystem InputSystem;



    private int MaxMenu = 0;        //項目数

    private const float CursorSize = 50; //カーソルのサイズ

    //メニュー選択状態
    private enum SelectState
    {
        Resume = 0,
        Option = 1,
        Manual = 2,
        TitleBack = 3
    };

    private SelectState CurrentSelect;

    //オプション状態
    private enum OptionState
    {
        Volume = 0,     //音量
        Camera = 1,     //カメラ
        Quality = 2,    //画質
        Story = 3,      //ストーリー
    }

    private OptionState CurrentOption;

    //選択中か
    private bool Selected = false;
    private void Start()
    {
        MaxMenu = MenuAnimators.Count - 1;
        
        //外部から呼び出してください
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
            //入力がなければ終了
            //if(!DownButton && !UpButton && !SelectButton) { return; }

            //メニュー番号を更新
            int oldMenu = (int)CurrentSelect;
            if (DownButton) { CurrentSelect++; }
            if (UpButton) { CurrentSelect--; }

            if (oldMenu != (int)CurrentSelect)
            {
                //カーソル移動
                CursorMove(oldMenu);
            }


            //ポーズ選択項目を表示
            if (SelectButton)
            {
                Selected = true;
                MenuInfo();
            }
        }

    }

    /// <summary>
    /// カーソルの移動
    /// </summary>
    /// <param 前回のカーソル位置="old"></param>
    private void CursorMove(int old)
    {
        //上下限
        if ((int)CurrentSelect > MaxMenu) { CurrentSelect = 0; }
        if ((int)CurrentSelect < 0) { CurrentSelect = (SelectState)MaxMenu; }

        //今回と前回のAnimatorの状態を保存
        MenuAnimators[(int)CurrentSelect].SetBool("OnCursor", true);
        MenuAnimators[old].SetBool("OnCursor", false);

        //カーソルの位置を設定する
        Vector3 pos = MenuRectTrans[(int)CurrentSelect].anchoredPosition;
        pos.y -= CursorSize;
        CursorTrans.anchoredPosition = pos;
    }
    private void MenuSelect()
    {
        //MenuAnimators[(int)CurrentSelect].SetBool("IsSelect", true);

        //キャンセルしたか
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
                    //ポーズパネルがあれば非表示
                    bool Panal = PosePanels[(int)CurrentSelect];
                    if (Panal) { PosePanels[(int)CurrentSelect].SetActive(false); }
                    Selected = false;
                }
                break;
            case SelectState.Manual:
                if (CanselButton)
                {
                    //ポーズパネルがあれば非表示
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
                    //ポーズパネルがあれば非表示
                    bool Panal = PosePanels[(int)CurrentSelect];
                    if (Panal) { PosePanels[(int)CurrentSelect].SetActive(false); }
                    TitleSelectNum = 0;
                    Selected = false;
                }
                break;
        }
    }

    /// <summary>
    /// ポーズ状態設定
    /// </summary>
    /// <param ポーズ状態="State"></param>
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
    /// ポーズ項目の表示
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
    /// ゲームに戻る
    /// </summary>
    private void ManualAction()
    {
        //説明を表示(キャンセルされていないなら表示)
       // ManualPanel.SetActive(true);
    }

    private void OptionAction()
    {

        ////オプションパネルを表示
        //OptionPanel.SetActive(true);

        ////キャンセルしたか
        //bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

        ////キャンセルされたら終了
        //if(CanselButton)
        //{
        //    OptionPanel.SetActive(false);
        //    return;
        //}

        ////決定ボタン
        //bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);

        //bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
        //bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

        ////メニュー番号を更新
        //int oldoption = (int)CurrentOption;
        //if (DownButton) { CurrentOption++; }
        //if (UpButton) { CurrentOption--; }

        ////現在のパネルの表示
       
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

        //上下限
        if (TitleSelectNum > TitleSelect.Count - 1) { TitleSelectNum = 0; }
        if(TitleSelectNum < 0) { TitleSelectNum = TitleSelect.Count - 1; }

        //カーソル位置調整
        OptionCursor.transform.SetParent(TitleSelect[TitleSelectNum].transform);
        OptionRectTrans.anchorMin = TitleSelect[TitleSelectNum].anchorMin;
        OptionRectTrans.anchorMax = TitleSelect[TitleSelectNum].anchorMax;
        OptionRectTrans.anchoredPosition = Vector2.zero;
      

        bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
        
        bool Yes = SelectButton && TitleSelectNum == 0;
        bool No = SelectButton && TitleSelectNum != 0;

        //タイトルに戻る
        if (Yes){ SceneManager.LoadScene(TitleSceneName); }
        
        return No;

    }
}
