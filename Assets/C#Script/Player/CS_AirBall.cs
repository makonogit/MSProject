using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CS_AirBall : MonoBehaviour
{

    [SerializeField, Header("攻撃力")]
    private float AttackPower = 1.0f;
    [SerializeField, Header("速さ")]
    private float AttackSpeed = 1.0f;


    [SerializeField, Header("衝突Effect")]
    private GameObject HitEffect;

    private Vector3 TargetPos;

    private float TimeMesure = 0.0f;

    public Vector3 TargetPosition
    {
        set
        {
            TargetPos = value;
        }
    }

    /// <summary>
    /// Power
    /// </summary>
    public float Power
    {
        get
        {
            return AttackPower;
        }
    }

    private void FixedUpdate()
    {

        TimeMesure += Time.deltaTime;
        transform.position += transform.forward * AttackSpeed * Time.deltaTime;

        //生成位置から前方向に発射

        //transform.Translate(transform.forward * AttackSpeed * Time.deltaTime);

        //if (TargetPos != Vector3.zero)
        //{
        //    transform.position = Vector3.Lerp(transform.position, TargetPos, AttackSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    //生成位置から前方向に発射
        //    transform.Translate(transform.forward * AttackSpeed * Time.fixedDeltaTime);
        //    //transform.position += transform.forward * AttackSpeed * Time.deltaTime;
        //}

        if (TimeMesure > 30.0f)
        {
            Destroy(this.gameObject);   //衝突したら自信を破棄
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    bool GimmickHit = collision.gameObject.tag == "Burst";

    //    if (GimmickHit)
    //    {

    //        CS_Burst_of_object burst = collision.transform.GetComponent<CS_Burst_of_object>();
    //        if (burst == null) { Debug.LogWarning("null component"); return; }
    //        burst.HitDamage(Power);

    //        //---------------------------------------------------------------
    //        // 衝突したオブジェクトに攻撃力を加算する？オブジェクト側でやる？
    //        //GetComponent<衝突したオブジェクトのコンポーネント>.耐久値;
    //        //耐久値 - AttackPower;
    //        //やるならこんな感じ？

    //        ContactPoint contact = collision.contacts[0]; // 最初の接触点を取得
    //        Vector3 collisionPoint = contact.point; // 衝突した位置

    //        Instantiate(HitEffect,collisionPoint,Quaternion.identity);     //衝突した位置にエフェクト

    //        Destroy(this.gameObject);   //衝突したら自信を破棄
    //    }

    //    if (TimeMesure > 0.2f)
    //    {
    //        ContactPoint contact = collision.contacts[0]; // 最初の接触点を取得
    //        Vector3 collisionPoint = contact.point; // 衝突した位置
    //        Instantiate(HitEffect,collisionPoint, Quaternion.identity);     //衝突した位置にエフェクト
    //        Destroy(this.gameObject);   //衝突したら自信を破棄
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        bool GimmickHit = other.gameObject.CompareTag("Burst");

        if (GimmickHit)
        {
            // 衝突したオブジェクトから CS_Burst_of_object コンポーネントを取得
            CS_Burst_of_object burst = other.transform.GetComponent<CS_Burst_of_object>();
            if (burst == null)
            {
                return;
            }

            // ダメージを与える
            burst.HitDamage(Power);

            //---------------------------------------------------------------
            // もし耐久値を減らすなどの処理を追加したい場合は、コメントアウト部分のように書けます:
            // GetComponent<衝突したオブジェクトのコンポーネント>.耐久値 -= AttackPower;

            // 衝突した位置にエフェクトをインスタンス化
            Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position); // 衝突点の近い位置を取得
            Instantiate(HitEffect, collisionPoint, Quaternion.identity);

            // オブジェクトを破壊
            Destroy(gameObject); // このオブジェクトを破棄
        }

        // TimeMesure が 0.2f より大きい場合の処理
        if (TimeMesure > 0.2f)
        {
            // 衝突点にエフェクトをインスタンス化
            Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position); // 衝突点の近い位置を取得
            Instantiate(HitEffect, collisionPoint, Quaternion.identity);

            // オブジェクトを破壊
            Destroy(gameObject);
        }
    }


}
