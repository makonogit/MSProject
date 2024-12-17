using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//キー保存用
[System.Serializable]
public class StringSpritePair
{
    public string key;
    public Image KeyImage;
    public Sprite NormalSprite;
    public Sprite PushSprite;
}

//キー保存用
[System.Serializable]
public class DialogData
{
    public string DialogText;
    public Sprite DiaLogImage;
    //public bool
}


/// <summary>
/// 担当:菅　ポーズ
/// </summary>
public class CS_Pose : MonoBehaviour
{

    [SerializeField,Tooltip("ポーズ用カーソル")]
    private RectTransform PoseCursor;
    [SerializeField,Tooltip("カーソル移動位置List")]
    private List<RectTransform> PoseTransList;
    [SerializeField, Tooltip("ポーズ用オブジェクト")]
    private GameObject PosePanel;

    //ポーズの選択状態
    private enum PoseState
    {
        NONE = 0,      //選択していない
        OPTION = 1,    //オプション
        RETIRE = 2,    //リタイア
        HELP = 3,      //ヘルプ(ダイアログ？)
        OPERATING = 4, //操作説明
        RESUME = 5,    //ゲームに戻る
    }

    //現在の状態
    [SerializeField,Header("今の状態")]
    private PoseState CurrentState = PoseState.NONE;

    //項目選択中か
    private bool Select = false;

    //決定とキャンセルを保持
    private bool Decision = false;
    private bool Cancel = false;

    private bool PoseStartFlg = false;

    [Header("=============システム===============")]
    [SerializeField, Tooltip("入力")]
    private CS_InputSystem InputSystem;

    [Header("=============オプション===============")]
    [SerializeField, Tooltip("オプションオブジェクト")]
    private GameObject OptionObj;
    [SerializeField, Tooltip("オプションマスク")]
    private RectMask2D OptionMask;
    [SerializeField, Tooltip("オプションテキスト")]
    private Text OptionText;
    [SerializeField, Tooltip("オプションシステム")]
    private CS_OptionSystem OptionSystem;
    [SerializeField, Tooltip("戻るボタン押下時間")]
    private float BbuttonPushTime = 3f;
    private Coroutine CurrentCoroutine;
    private bool LongPushFlg = false;   //長押しフラグ

    [Header("=============リタイア===============")]
    [SerializeField, Tooltip("リタイアオブジェクト")]
    private GameObject RetireObj;
    [SerializeField, Tooltip("遷移シーン名")]
    private string NextSceneName;
    [SerializeField, Tooltip("リタイアカーソル")]
    private RectTransform RetireCursor;
    [SerializeField, Tooltip("選択項目座標")]
    private List<Vector2> RetireSelsctPos; 
    private int Yes = 1;    //選択 0:はい 1:いいえ

    [Header("============ヘルプ===============")]
    [SerializeField, Tooltip("ヘルプオブジェクト")]
    private GameObject HelpObj;
    [SerializeField, Tooltip("ヘルプテキスト")]
    private Text HelpText;
    [SerializeField, Tooltip("ダイアログリスト")]
    private List<DialogData> Dialogs;
    [SerializeField, Tooltip("ダイアログ上矢印")]
    private Image DialogUpArrow;
    [SerializeField, Tooltip("ダイアログ下矢印")]
    private Image DialogDownArrow;
    [SerializeField, Tooltip("テキスト上矢印")]
    private Image DialogTextUpArrow;
    [SerializeField, Tooltip("テキスト下矢印")]
    private Image DialogTextDownArrow;
    [SerializeField, Tooltip("ダイアログテキスト表示用コンポーネント")]
    private List<Text> DialogTexts;
    [SerializeField, Tooltip("ダイアログ表示用コンポーネント")]
    private Image DialogImage;
    [SerializeField, Tooltip("ボタン押下時矢印色")]
    private Color ButtonPushArrowColor;
   
    //[SerializeField, Tooltip("現在解放しているログ")]
    //private List<Image> ;
    //表示しているダイアログナンバー
    private int CurrentDialogNum = 0;



    [Header("=============操作説明===============")]
    [SerializeField, Tooltip("操作説明オブジェクト")]
    private GameObject OperatingObj;
    [SerializeField, Tooltip("キー保存用")]
    private List<StringSpritePair> KeyData;
    //コントローラースティック移動用
    private Vector2 LSPos;
    private Vector2 RSPos;



    [Header("=============コントローラー振動===============")]
    [Header("【カーソル移動時の振動設定】")]
    [SerializeField, Tooltip("振動の長さ")]
    private float Cursorduration = 0.5f;         // 振動の長さ
    [SerializeField, Tooltip("振動の強さ")]
    private int CursorpowerType = 1;          // 振動の強さ（4段階）
    [SerializeField, Tooltip("振動の周波数")]
    private AnimationCurve CursorcurveType;          // 振動の周波数
    [SerializeField, Tooltip("繰り返し回数")]
    private int Cursorrepetition = 1;         // 繰り返し回数

    [Header("【決定時の振動設定】")]
    [SerializeField, Tooltip("振動の長さ")]
    private float Decisionduration = 0.5f;         // 振動の長さ
    [SerializeField, Tooltip("振動の強さ")]
    private int DecisionpowerType = 1;          // 振動の強さ（4段階）
    [SerializeField, Tooltip("振動の周波数")]
    private AnimationCurve DecisioncurveType;          // 振動の周波数
    [SerializeField, Tooltip("繰り返し回数")]
    private int Decisionrepetition = 1;         // 繰り返し回数

    [SerializeField, Header("設定用パネル")]
    private List<GameObject> SettingPanels;    //表示用パネル


    // Start is called before the first frame update
    void Start()
    {
        //リタイアのカーソル移動
        RetireCursor.anchoredPosition = RetireSelsctPos[Yes];

        //操作説明のスティック座標保存
        LSPos = KeyData[9].KeyImage.rectTransform.anchoredPosition;
        RSPos = KeyData[10].KeyImage.rectTransform.anchoredPosition;

    }

    private void Option()
    {
        //オプションの表示
        if (!OptionObj.activeSelf) { OptionObj.SetActive(true); }
        if (!OptionMask.enabled) { OptionMask.enabled = true; }
        if (OptionText.enabled) { OptionText.enabled = false; }

        //終了したか取得
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
        //リタイアダイアログの表示
        if (!RetireObj.activeSelf)
        {
            RetireObj.SetActive(true);
            return;
        }

        //左右を押したかどうか
        bool LeftButton = InputSystem.GetDpadLeftTriggered();
        bool RightButton = InputSystem.GetDpadRightTriggered();
        if (LeftButton || RightButton) { Yes = Yes == 0 ? 1 : 0; }

        //カーソル移動
        RetireCursor.anchoredPosition = RetireSelsctPos[Yes];

        //戻る
        if (Cancel)
        {
            //初期化して非表示
            Yes = 1;
            RetireObj.SetActive(false);
            //選択状態の解除
            Select = false;
        }

        //決定ボタンを押したら処理
        if (!Decision) { return; }

        //シーン遷移
        if (Yes == 0) { SceneManager.LoadScene(NextSceneName); }
        else 
        {
            //初期化して非表示
            Yes = 1;
            RetireObj.SetActive(false);
            //選択状態の解除
            Select = false;
        }
    }

    private void Help()
    {
        if (!HelpObj.activeSelf)
        {
            //後ろの情報を非表示に
            Transform parent = HelpObj.transform.parent;
            parent.transform.SetParent(PosePanel.transform);
            OptionText.enabled = false;
            HelpText.enabled = false;

            OptionMask.enabled = true;

            HelpObj.SetActive(true); 
        }

        //入力(上下)
        bool DownButton = InputSystem.GetDpadDownTriggered();
        bool UpButton = InputSystem.GetDpadUpTriggered();

        //前回のダイアログ番号を保存
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
       
        //矢印の色変更
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

        //入力上下限の設定
        if (CurrentDialogNum > Dialogs.Count - 1) { CurrentDialogNum = 0; }
        if (CurrentDialogNum < 0) { CurrentDialogNum = Dialogs.Count - 1; }

        //ダイアログ画像切替
        DialogImage.sprite = Dialogs[CurrentDialogNum].DiaLogImage;
        
        //並び順に更新
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
            //後ろの情報を非表示に
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

        //---------------ボタンごとに変更------------------
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
        //戻る処理
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



        //0.3は閾値
        if (ls.x > 0.3f || ls.x < -0.3f || ls.y > 0.3f || ls.y < -0.3f) { KeyData[9].KeyImage.sprite = KeyData[9].PushSprite; }
        else { KeyData[9].KeyImage.sprite = KeyData[9].NormalSprite; }
        if (rs.x > 0.3f || rs.x < -0.3f || rs.y > 0.3f || rs.y < -0.3f) { KeyData[10].KeyImage.sprite = KeyData[10].PushSprite; }
        else { KeyData[10].KeyImage.sprite = KeyData[10].NormalSprite; }

        //スティックを動かす
        KeyData[9].KeyImage.rectTransform.anchoredPosition = LSPos + (ls * 10f);
        KeyData[10].KeyImage.rectTransform.anchoredPosition = RSPos + (rs * 10f);


        //終了
        if (LongPushFlg)
        {
            LongPushFlg = false;
            OperatingObj.SetActive(false);
            Select = false;
        }

    }


    /// <summary>
    /// ボタン長押しを検知する
    /// </summary>
    /// <returns></returns>
    private IEnumerator LongPushButton(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        LongPushFlg = true;
    }

    private void Resume()
    {
        //ポーズ終了
        EndPose();
        //なにもしていない状態に
        //CurrentState = PoseState.NONE;
    }

    private void SelectAction()
    {
        switch (CurrentState)
        {
            case PoseState.NONE:     　//なにもしていない
                //EndPose();
                break;
            case PoseState.OPTION:     //オプション
                Option();
                break;
            case PoseState.RETIRE:     //リタイア
                Retire();
                break;
            case PoseState.HELP:       //ヘルプ(ダイアログ)
                Help();
                break;
            case PoseState.OPERATING:  //操作説明
                Operating();
                break;
            case PoseState.RESUME:     //ゲームに戻る
                Resume();
                break;
        }


    }

    /// <summary>
    /// ポーズ終了
    /// </summary>
    private void EndPose()
    {
        Time.timeScale = 1f;            //時間を戻す
        PosePanel.SetActive(false);    //自身を非表示に
        Select = false;
        CurrentState = PoseState.NONE;
    }

    /// <summary>
    /// ポーズ開始
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

        Cancel = InputSystem.GetButtonBTriggered();　//戻る
        Decision = InputSystem.GetButtonATriggered(); //決定

        //項目選択中かの状態
        if (Decision)
        {
            CS_ControllerVibration.StartVibrationWithCurve(Decisionduration, DecisionpowerType, DecisioncurveType, Decisionrepetition);
            Select = true; 
        }

        if (Select) { SelectAction(); } //選択行動
        else { CursorMove(); }          //カーソル移動        
    
    }

    /// <summary>
    /// カーソル移動
    /// </summary>
    private void CursorMove()
    {
        //入力(上下)
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

        //入力上下限の設定
        if (CurrentState < PoseState.OPTION) { CurrentState = PoseState.RESUME; }
        if (CurrentState > PoseState.RESUME) { CurrentState = PoseState.OPTION; }

        PoseCursor.anchoredPosition = PoseTransList[(int)CurrentState - 1].anchoredPosition;
    }


}
