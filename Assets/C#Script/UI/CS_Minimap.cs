using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 担当：菅　Minimapシステム
/// </summary>
public class CS_Minimap : MonoBehaviour
{
    //Gizmosで表示する
    [SerializeField, Header("スタート地点")]
    private Vector3 StartPosition;

    [SerializeField, Header("ゴール地点")]
    private Vector3 EndPosition;

    [SerializeField, Header("プレイヤー位置")]
    private Transform PlayerTrans;

    [SerializeField, Header("Minimapプレイヤー位置")]
    private RectTransform MiniPlayerPin;

    [SerializeField, Header("Minmap表示範囲")]
    private RectTransform MinimapSize;

    [SerializeField, Header("崩壊システム位置")]
    private Transform BreakObjTrans;

    [SerializeField, Header("崩壊中位置")]
    private RectTransform BreakNowPos;

    [SerializeField, Header("崩壊済位置")]
    private RectTransform BreakedPos;

    private float BreakUImaxsize = 0;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーの現在のY座標を割合に変換
        float progress = Mathf.InverseLerp(StartPosition.y, EndPosition.y, PlayerTrans.position.y);

        // Minimap上の位置を設定
        Vector2 markerPosition = MinimapSize.rect.min + new Vector2(0, MinimapSize.rect.height * progress);
        MiniPlayerPin.anchoredPosition = markerPosition;

        //崩壊UIのサイズを設定
        BreakUImaxsize = BreakNowPos.sizeDelta.y;
        BreakNowPos.sizeDelta = new Vector2(BreakNowPos.sizeDelta.x, 0f);
        BreakedPos.sizeDelta = new Vector2(BreakedPos.sizeDelta.x, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // プレイヤーの現在のY座標を割合に変換
        float progress = Mathf.InverseLerp(StartPosition.y - 10f, EndPosition.y, PlayerTrans.position.y);

        // Minimap上の位置を設定
        Vector2 markerPosition = MinimapSize.rect.min + new Vector2(0, MinimapSize.rect.height * progress);
        MiniPlayerPin.anchoredPosition = markerPosition;

        // プレイヤーの現在のY座標を割合に変換
        float Breakprogress = Mathf.InverseLerp(StartPosition.y, EndPosition.y, PlayerTrans.position.y);
        // UIサイズを進捗に合わせて設定
        float currentSize = BreakUImaxsize * Breakprogress;
        BreakNowPos.sizeDelta = new Vector2(BreakNowPos.sizeDelta.x, currentSize); // UIの高さを変化

    }

    /// <summary>
    /// Rayを表示
    /// </summary>
    private void OnDrawGizmos()
    {
        //開始位置
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(StartPosition, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero,Vector3.one);

        //終了位置
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(EndPosition, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

    }

}
