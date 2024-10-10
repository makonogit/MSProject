using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_AirBall : MonoBehaviour
{

    [SerializeField, Header("攻撃力")]
    private float AttackPower = 1.0f;
    [SerializeField, Header("速さ")]
    private float AttackSpeed = 1.0f;

    private Vector3 TargetPos;

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

    }

    private void OnCollisionEnter(Collision collision)
    {
        bool GimmickHit = collision.gameObject.tag == "Burst";
        
        if (GimmickHit)
        {

            CS_Burst_of_object burst = collision.transform.GetComponent<CS_Burst_of_object>();
            if (burst == null) { Debug.LogWarning("null component"); return; }
            burst.HitDamage(Power);

            //---------------------------------------------------------------
            // 衝突したオブジェクトに攻撃力を加算する？オブジェクト側でやる？
            //GetComponent<衝突したオブジェクトのコンポーネント>.耐久値;
            //耐久値 - AttackPower;
            //やるならこんな感じ？
            
        }


        Destroy(this.gameObject);   //衝突したら自信を破棄
    }

}
