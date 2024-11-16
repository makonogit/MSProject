using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落下オブジェクト
/// </summary>
public class CS_FallingObj : MonoBehaviour
{
    [SerializeField, Header("生成Effect")]
    private GameObject BreakEffect;
    [SerializeField]
    private GameObject BreakDust;

    [SerializeField,Header("Playerを検知する範囲(半径)")]
    private float DetectionRadius = 10.0f;
    [SerializeField,Header("検知するレイヤー")]
    private LayerMask DetectionLayer;

    [Header("空気抵抗")]
    [SerializeField,Tooltip("大きいほど落ちるの遅いよー")]
    private float FallingDrag = 5.0f;

    [SerializeField, Header("RigidBody")]
    private Rigidbody thisrd;

    private void FixedUpdate()
    {
        DetectObjectsInRange();

    }


    /// <summary>
    /// 範囲内にPlayerが入ったか検知して重力を発動させる関数
    /// </summary>

    void DetectObjectsInRange()
    {
        // Sphereで範囲内のオブジェクトを検知
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRadius, DetectionLayer);

        foreach (var hitCollider in hitColliders)
        {
            // 検知したオブジェクトに対して何か処理を行う
            bool PlayerHit = hitCollider.CompareTag("Player");
            if (PlayerHit)
            {
                thisrd.drag = FallingDrag;
                thisrd.useGravity = true;
                
            }
        }
    }

    // 範囲を可視化するためのデバッグ用
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }

    /// <summary>
    /// 床と接触したら消滅する
    /// </summary>
    /// <param 衝突物="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //何かと接触したら消す
        Destroy(this.gameObject);

        Vector3 pos = transform.position;
        Instantiate(BreakEffect,pos,Quaternion.identity);
        //Instantiate(BreakDust,pos,Quaternion.identity);

        //プレイヤーと接触したか
        bool HitFloor = collision.gameObject.tag == "Player";
        if (HitFloor)
        {
            //Destroy(this.gameObject);
        }
    }
}
