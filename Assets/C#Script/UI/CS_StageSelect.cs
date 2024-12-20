using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.C_Script.GameEvent;

/// <summary>
/// 担当：菅　ステージセレクトUIシステム
/// </summary>
public class CS_StageSelectUI : MonoBehaviour
{
    [SerializeField, Header("Animtor")]
    private Animator Selectanimator;

    [SerializeField, Header("入力")]
    private CS_InputSystem csinput;

    [SerializeField, Header("遷移シーン")]
    private List<string> SceneName;

    private int currentstage = 0;
    private int maxstage = 1;   //配列で0始まり(解放分)

    //====確認画面関係======
    [SerializeField, Header("確認ウィンドウ")]
    private GameObject ConfirmationWindow;

    [SerializeField, Header("選択Image")]
    private List<Image> SelectImages;
    private int SelectNum = 0;      //選択番号
    private bool Selected = false;  //選択中か

    [SerializeField, Header("AudioSource")]
    private AudioSource audio;

    [Header("SE")]
    [SerializeField]
    private AudioClip CursorSE;
    [SerializeField]
    private AudioClip SelectSE;
    [SerializeField]
    private AudioClip CancelSE;


    // Start is called before the first frame update
    void Start()
    {
        SelectImages[SelectNum].color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Selected)
        {
            bool DownButton = csinput.GetDpadDownTriggered();
            bool UpButton = csinput.GetDpadUpTriggered();

            int oldstage = currentstage;

            if (DownButton) { currentstage++; audio.PlayOneShot(CursorSE); }
            if (UpButton) { currentstage--; audio.PlayOneShot(CursorSE); }

            //上下限
            if (currentstage < 0) { currentstage = maxstage; }
            if (currentstage > maxstage) { currentstage = 0; }

            //変更したらAnimator状態変更
            bool Change = oldstage != currentstage;
            if (Change)
            {
                Selectanimator.SetInteger("StageNum", currentstage);
            }

            //選択したか
            bool SelectButton = csinput.GetButtonATriggered();
            if (SelectButton)
            {
                audio.PlayOneShot(SelectSE);
                Selected = true;
                ConfirmationWindow.SetActive(true); //確認画面の表示
            }
        }
        else
        {
            bool LeftButton = csinput.GetDpadLeftTriggered();
            bool RightButton = csinput.GetDpadRightTriggered();

            int oldselect = SelectNum;

            if (RightButton) { SelectNum++; audio.PlayOneShot(CursorSE); }
            if (LeftButton) { SelectNum--; audio.PlayOneShot(CursorSE); }

            if (SelectNum < 0) { SelectNum = 1; }
            if(SelectNum > 1) { SelectNum = 0; }

            bool Change = oldselect != SelectNum;

            if(Change)
            {
                SelectImages[oldselect].color = Color.clear;
                SelectImages[SelectNum].color = Color.white;
            }

            //選択したか
            bool SelectButton = csinput.GetButtonATriggered();

            if (!SelectButton) { return; }

            if (SelectNum == 0) { Selected = false; ConfirmationWindow.SetActive(false); audio.PlayOneShot(CancelSE); }    //キャンセル
            if(SelectNum == 1) { audio.PlayOneShot(SelectSE); SceneManager.LoadScene(SceneName[currentstage]); CSGE_GameOver.InitRespawn(); }   //シーン遷移
        }
    }
}
