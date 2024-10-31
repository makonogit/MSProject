using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// クラフト用のスクリプト
/// </summary>
public class CS_Craft : MonoBehaviour
{
    [SerializeField, Header("クラフトカーソル")]
    private RectTransform CraftCursor;

    [SerializeField, Header("入力関係")]
    private CS_InputSystem csInput;

    [SerializeField, Header("クラフトウィンドウ")]
    private List<GameObject> CraftWindow;

    [SerializeField, Header("クラフト選択ウィンドウ")]
    private List<Image> CraftSelectWindow;

    //[SerializeField, Tooltip("選択中クラフトウィンドウスプライト")]
    //private Sprite SelectionCraftWindowSprite;

    //スティックの倒すしきい値　これより大きいと倒している判定(0.7が最大かな)
    private float StickInputThreshold = 0.5f;

    //クラフト番号0:上1:右2:左
    private int CraftWindowNum = -1;

    private Color oldcolor;

    // Update is called once per frame
    void Update()
    {
        //右スティックの入力を取得
        Vector2 StickValue = csInput.GetRightStick();

        //閾値を超えたらスティック入力判定
        bool StickInput = Mathf.Abs(StickValue.x) > StickInputThreshold || Mathf.Abs(StickValue.y) > StickInputThreshold;

        int oldWindowNum = CraftWindowNum;

        //入力している時だけ回転を反映
        if(StickInput){ CraftWindowNum = StickRotationtoObject(StickValue.x, StickValue.y); }
        else{ CraftWindowNum = -1; }
        
        //前回の値と変わっていたら選択ウィンドウを設定
        bool WindowChange = oldWindowNum != CraftWindowNum;
        if (WindowChange) { SetSelectionCraftWindow(oldWindowNum, CraftWindowNum); }

    }


    /// <summary>
    /// スティックの回転をオブジェクトに反映させる
    /// </summary>
    /// <param X軸入力="StickX"></param>
    /// <param Y軸入力="StickY"></param>
    /// セクション番号0:真上1:右2:左
    private int StickRotationtoObject(float StickX,float StickY)
    {
        // 入力から回転角度を計算
        float angle = Mathf.Atan2(StickY, StickX) * Mathf.Rad2Deg;

        //画像のカーソル位置が90度ずれてるので-90度
        angle -= 90f;

        //オブジェクトに回転を反映
        CraftCursor.localRotation = Quaternion.Euler(0, 0, angle);

        //現在の角度からセクション番号を求める(Y字に3分割)
        int Section = -1;
        if (angle <= 60f && angle > -60f) { Section = 0; } //真上
        else if(angle <= -60f && angle > -180f) { Section = 1; } //右
        else { Section = 2; }  //左

        return Section;

    }


    /// <summary>
    /// 選択している項目に選択中クラフトウィンドウを設定する
    /// </summary>
    /// <param クラフト番号="CraftNum"></param>
    /// クラフト番号は以下に準ずる
    /// 0:上 1:右 2:左
    private void SetSelectionCraftWindow(int OldCraftNum,int CraftNum)
    {
        //番号が範囲内か確認して反映
        bool active = OldCraftNum > -1 && OldCraftNum < CraftWindow.Count;
        if (active) 
        {
            CraftSelectWindow[OldCraftNum].color = oldcolor;
            CraftWindow[OldCraftNum].SetActive(false); 
        }

        active = CraftNum > -1 && CraftNum < CraftWindow.Count;
        if (active)
        {
            oldcolor = CraftSelectWindow[CraftNum].color;
            CraftSelectWindow[CraftNum].color = Color.clear;
            CraftWindow[CraftNum].SetActive(true); 
        }
    }
}
