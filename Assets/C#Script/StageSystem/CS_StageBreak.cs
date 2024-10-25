using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// 担当;菅　ステージ崩壊システム
/// </summary>
public class CS_StageBreak : MonoBehaviour
{
    [SerializeField, Header("ステージ管理親オブジェクト")]
    private GameObject StageObj;

    [Header("崩壊検知関連")]
    [SerializeField,Tooltip("判定サイズ")]
    private Vector3 BoxSize;
    [SerializeField, Tooltip("判定するlayer")]
    private LayerMask layer;
    [SerializeField, Tooltip("判定後のLayer番号")]
    private int Breakedlayer;
    
    private int BreakObjCount = 0;     //壊れたオブジェクトの数
    private int MaxBreakObjCount = 0; //壊れるオブジェクトの最大数
    private float BreakRate = 0;  //崩壊度

    [Header("移動関係")]
    [SerializeField, Tooltip("崩壊ルート")]
    private SplineAnimate splineanim;
    [SerializeField, Tooltip("移動速度")]
    private float MoveSpeed = 1f;
    

    // Start is called before the first frame update
    void Start()
    {
        //壊れるオブジェクトの数を保存(全て子オブジェクト)
        MaxBreakObjCount = StageObj.transform.childCount;

        //移動速度を設定
        splineanim.Duration = MoveSpeed;

        Debug.Log(MaxBreakObjCount);

    }

    // Update is called once per frame
    void Update()
    {
        BreakJudgment();
    }

    /// <summary>
    /// 崩壊検知(Ray)
    /// </summary>
    private void BreakJudgment()
    {
        RaycastHit hit;

        //あたり判定
        bool hitflg = Physics.BoxCast(transform.position, BoxSize, Vector3.one, out hit,Quaternion.identity, 5f, layer);

        if (!hitflg) { return; }

        //layerを変更
        hit.transform.gameObject.layer = Breakedlayer;
        //衝突したオブジェクトを崩壊状態にする
        hit.collider.GetComponent<Rigidbody>().useGravity = true;   
        BreakObjCount++;    //壊れたオブジェクトの数をカウント
    }


    /// <summary>
    /// Rayの表示
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + Vector3.forward, BoxSize);
    }

    /// <summary>
    /// 崩壊度を返す
    /// </summary>
    /// <returns></returns>
    private int GetBreakRate()
    {
       return (int)(MaxBreakObjCount / BreakObjCount);
    }

}
