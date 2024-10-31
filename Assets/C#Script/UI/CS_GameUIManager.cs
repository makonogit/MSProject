using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームUIシステム
/// </summary>
public class CS_GameUIManager : MonoBehaviour
{
    [SerializeField, Header("飢餓ゲージマスク")]
    private RectMask2D HungerGageMask;
    private float MaskMAXValue; //マスク最大値
    private float MaxPlayerHP; //プレイヤー最大HP
    //飢餓ゲージのマスクサイズとプレイヤーの飢餓リソースをスケールした値
    private float HungerGageScale;

    //private CS_Player player;   //プレイヤーの情報(リソースを取得したい)

    [Header("缶詰Text")]
    [SerializeField, Tooltip("取得缶詰")]
    private Text CanText;
    [SerializeField, Tooltip("取得空き缶")]
    private Text EnptyCanText;


    [SerializeField, Header("ショートカットパネル")]
    private GameObject CraftPanel;

    [SerializeField, Header("レティクル")]
    private GameObject Reticle;

    [SerializeField, Header("TPSカメラ")]
    private CS_TpsCamera tpscamera;
    private float CamSpeed = 0;

    [SerializeField, Header("入力関係")]
    private CS_InputSystem CS_Input;

    [SerializeField, Header("CS_PlayerManager")]
    private CS_PlayerManager playermanager;

    private void Start()
    {
        //マスクの最大サイズ(Left)を取得
        MaskMAXValue = HungerGageMask.padding.w;
        HungerGageMask.padding = Vector4.zero;

        //プレイヤーの情報取得
        MaxPlayerHP = playermanager.GetHP();

        //缶詰の数を取得(一旦99)
        string CanNum = 0.ToString("00");
        //if(value < 10) { //10より小さかったら01とかにする }
        CanText.text = CanNum;
        EnptyCanText.text = CanNum;

        //カメラのスピードを保存
        CamSpeed = tpscamera.CameraSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーの体力を取得して反映させる
        float Hpprogress = playermanager.GetHP() / MaxPlayerHP;
        HungerGageScale = MaskMAXValue - (MaskMAXValue * Hpprogress);
        UpdateHungerGage(HungerGageScale);

        //缶の数を反映
        string CanNum = playermanager.GetItemStock().ToString("00");
        CanText.text = CanNum;
        string EmptyCanNum = playermanager.GetIngredientsStock().ToString("00");
        EnptyCanText.text = EmptyCanNum;

        bool LBTriggered = CS_Input.GetButtonLTriggered();
        bool LBButton = CS_Input.GetButtonLPressed();
        
        //LBでクラフト表示,レティクル非表示
        if (LBButton) 
        {
            //カメラの回転を止める
            tpscamera.CameraSpeed = 0f;
            Reticle.SetActive(false);
            CraftPanel.SetActive(true); 
        }
        else
        {
            tpscamera.CameraSpeed = CamSpeed;
            Reticle.SetActive(true);
            CraftPanel.SetActive(false); 
        }


    }

    /// <summary>
    /// ゲージの値更新する
    /// </summary>
    /// <param 新しいゲージの値="value"></param>
    private void UpdateHungerGage(float value)
    {
        HungerGageMask.padding = new Vector4(0, 0, 0, value);
    }

}
