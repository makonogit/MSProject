using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CS_Title : MonoBehaviour
{
    [SerializeField,Tooltip("ゲーム開始時のシーンの名前")]
    private string GameSceneName;

    [Header("タイトル項目")]
    [SerializeField, Tooltip("選択項目RectTrans")]
    private List<RectTransform> TitleSelectTrans;

    [SerializeField, Tooltip("選択用カーソル")]
    private RectTransform TitleCursorTrans;
    [SerializeField, Tooltip("選択用カーソルImageコンポーネント")]
    private Image TitleCursorImage;
    [SerializeField, Tooltip("通常時カーソルスプライト")]
    private Sprite TitleCursorNormalSprite;
    [SerializeField, Tooltip("選択中カーソルスプライト")]
    private Sprite TitleCursorSelectSprite;

    [SerializeField, Header("選択中のUIサイズ拡大比率")]
    private float Enlargement = 1.0f;

    //拡大後と通常時のサイズ※今は全部同じサイズと断定して1つ、変わるならList化する
    private Vector2 EnlargementSize;
    private Vector2 NormalSize;

    [Header("Start関係")]
    [SerializeField, Tooltip("ゲームを始めるか選択UIマスク")]
    private RectMask2D StartQuestionMask;
    private float StartMaskMaxSize;       //UIのマスク最大サイズ
    [SerializeField, Tooltip("選択UI表示スピード")]
    private float StartQuestionViewSpeed = 1.0f;
    [SerializeField, Tooltip("選択UITextコンポーネント")]
    private List<Text> StartQuestionTexts;
    private int StartQuestionNum = 0;   //0 はい　1 いいえ

    [Header("つづきから")]
    [SerializeField, Tooltip("カーソル拡大スピード")]
    private float CursorZoomSize = 2.0f;
    [SerializeField, Tooltip("フェードアウト用Image")]
    private Image FadeOutImage;
    [SerializeField, Tooltip("フェードアウトスピード")]
    private float FadeOutSpeed = 2.0f;

    [Header("Option関係")]
    [SerializeField,Tooltip("Option管理用スクリプト")]
    private CS_Option OptionControl;
    [SerializeField, Tooltip("Opiton用パネル")]
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

    [SerializeField, Header("入力システムスクリプト")]
    private CS_InputSystem CSInput;

    private enum TitleState
    {
        START = 0,
        CONTINUE = 1,
        OPTION = 2
    };

    private TitleState state;   //タイトルの選択状態

    private bool TitleSelect = false;       //選択しているか

    private void Start()
    {
        //項目選択時と通常時のサイズを設定
        EnlargementSize = TitleSelectTrans[0].sizeDelta * Enlargement;
        NormalSize = TitleSelectTrans[0].sizeDelta;

        //ゲームを始めるかのマスク最大サイズを保存しておく
        StartMaskMaxSize = StartQuestionMask.padding.x;
        //選択中テキストの設定
        StartQuestionTexts[StartQuestionNum].color = Color.red;
    }

    private void Update()
    {
        if (!TitleSelect)
        {
            int oldstate = (int)state;

            //入力状態取得
            bool DownButton = CSInput.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
            bool UpButton = CSInput.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

            //選択状態更新
            if (DownButton) { state++; PlaySE(CursorSE); }
            if (UpButton) { state--; PlaySE(CursorSE); }

            //上下限
            if ((int)state > (int)TitleState.OPTION) { state = TitleState.START; }
            if ((int)state < (int)TitleState.START) { state = TitleState.OPTION; }

            TitleSelectTrans[(int)state].sizeDelta = EnlargementSize;

            //前回と今回の状態が違ったら変更された判定
            bool Change = (int)state != oldstate;
            if (Change) { CursorMoveAction(oldstate); }

            bool Select = CSInput.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
            if (Select) { SelectTriggerAction(); PlaySE(SelectSE); }

            //ESCでゲーム終了
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
    /// カーソル移動時のアクション
    /// </summary>
    private void CursorMoveAction(int old)
    {
        TitleSelectTrans[old].sizeDelta = NormalSize;

        //カーソルの位置を設定
        Vector3 pos = TitleCursorTrans.anchoredPosition;
        TitleCursorTrans.SetParent(TitleSelectTrans[(int)state]);
        TitleCursorTrans.anchoredPosition = pos;

    }

    /// <summary>
    /// 選択時のアクション
    /// </summary>
    private void SelectTriggerAction()
    {
        TitleSelect = true;
        TitleCursorImage.sprite = TitleCursorSelectSprite;  //スプライトを選択中に

        //オプション時のみ
        if (state == TitleState.OPTION)
        {
            OptionPanel.SetActive(true);
        }

    }

    /// <summary>
    /// キャンセル時のアクション
    /// </summary>
    private void CanselTiggerAction()
    {
        TitleSelect = false;
        TitleCursorImage.sprite = TitleCursorNormalSprite;  //スプライトを通常に


        //オプション時のみ
        if (state == TitleState.OPTION)
        {
            OptionPanel.SetActive(false);
        }
        
    }

    /// <summary>
    /// 項目選択中のアクション
    /// </summary>

    void SelectAction()
    {
        switch(state)
        {
            case TitleState.START:
                StartGame();    //ゲームスタート
                break;
            case TitleState.CONTINUE:
                //続きから遊べるようにセーブシステム
                ContinueAction();
                break;
            case TitleState.OPTION:
                OptionAction();
                break;
        }
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    private void StartGame()
    {

        bool StartView = StartQuestionMask.padding.x > 0;

        //マスク画像表示
        if (StartView)
        {
            Vector4 size = StartQuestionMask.padding;
            size.x -= StartQuestionViewSpeed * Time.deltaTime;
            StartQuestionMask.padding = size;
        }


        bool leftButton = CSInput.GetDpadLeftTriggered() || Input.GetKeyDown(KeyCode.LeftArrow);
        bool RightButton = CSInput.GetDpadRightTriggered() || Input.GetKeyDown(KeyCode.RightArrow);

        int oldquestionnum = StartQuestionNum;

        //はいかいいえの選択肢
        if (leftButton) { StartQuestionNum++; PlaySE(CursorSE); }
        if (RightButton) { StartQuestionNum--; PlaySE(CursorSE); }
        if(StartQuestionNum > StartQuestionTexts.Count - 1) { StartQuestionNum = 0; }
        if (StartQuestionNum < 0) { StartQuestionNum = StartQuestionTexts.Count - 1; }

        //変更されたら色を設定
        bool Change = oldquestionnum != StartQuestionNum;
        if (Change) 
        {
            StartQuestionTexts[oldquestionnum].color = Color.white; //白
            StartQuestionTexts[StartQuestionNum].color = Color.red; //赤 
        }


        //終了
        bool Cansel = CSInput.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

        //選択
        bool SelectButton = CSInput.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
        if(SelectButton && StartQuestionNum == 0)
        {
            //ゲーム開始
            SceneManager.LoadScene(GameSceneName);
            PlaySE(SelectSE);
        }
        else if(SelectButton && StartQuestionNum == 1)
        {
            //終了
            Cansel = true;
            PlaySE(CancelSE);
        }

        if (Cansel)
        {
            StartQuestionMask.padding = new Vector4(StartMaskMaxSize, 0, 0, 0);   //画像単純に消す
            CanselTiggerAction();
            PlaySE(CancelSE);
        }

    }

    /// <summary>
    /// オプションアクション
    /// </summary>
    private void OptionAction()
    {
        //bool Cansel = CSInput.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);
        bool OptionCansel = OptionControl.OptionAction();   //オプションを開く

        if (!OptionCansel) { CanselTiggerAction(); }
    }


    /// <summary>
    /// つづきから
    /// </summary>
    private void ContinueAction()
    {
        //カーソル拡大
        Vector2 size = TitleCursorTrans.sizeDelta;
        size.x += CursorZoomSize * Time.deltaTime;
        size.y += CursorZoomSize * Time.deltaTime;
        TitleCursorTrans.sizeDelta = size;

        //フェードアウトする
        Color Fadecolor = FadeOutImage.color;
        bool fade = Fadecolor.a < 1f;
        if (fade)
        {
            Fadecolor.a += FadeOutSpeed * Time.deltaTime;
            FadeOutImage.color = Fadecolor;
        }
        else
        {
            //一旦普通に開始
            //ゲーム開始
            SceneManager.LoadScene(GameSceneName);
        }
    }

    /// <summary>
    /// ゲーム終了
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
