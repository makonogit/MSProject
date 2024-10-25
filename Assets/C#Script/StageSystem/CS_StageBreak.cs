using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// 担当;菅　ステージ崩壊システム
/// </summary>
public class CS_StageBreak : MonoBehaviour
{
    [SerializeField, Header("Effect")]
    private ParticleSystem BreakEffect;

    [Header("崩壊検知関連")]
    [SerializeField,Tooltip("判定サイズ")]
    private Vector3 BoxSize;
    [SerializeField, Tooltip("判定するlayer")]
    private LayerMask layer;
    [SerializeField, Tooltip("判定後のLayer")]
    private LayerMask Breakedlayer;

    private int BreakObjCount = 0;  //壊れるオブジェクトの数
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
        BreakObjCount = transform.childCount;

        //移動速度を設定
        splineanim.Duration = MoveSpeed;

    }

    // Update is called once per frame
    void FixedUpdate()
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
        bool hitflg = Physics.BoxCast(transform.position, BoxSize, Vector3.forward, out hit,Quaternion.identity, 0f, layer);

        if (!hitflg) { return; }

        //layerを変更
        hit.transform.gameObject.layer = Breakedlayer;
        Debug.Log(hit);
        //衝突したオブジェクトを崩壊状態にする
        hit.collider.GetComponent<Rigidbody>().useGravity = true;   
        BreakObjCount--;    //全体の破壊オブジェクト数を減らす
    }


    /// <summary>
    /// Rayの表示
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + Vector3.forward, BoxSize);
    }


}
