using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_AirBall : MonoBehaviour
{

    [SerializeField, Header("攻撃力")]
    private float AttackPower = 1.0f;
    
    private void FixedUpdate()
    {
        //生成位置から前方向に発射
        transform.position += Vector3.forward * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool GimmickHit = collision.gameObject.tag == "Gimmick";
        
        if (GimmickHit)
        {
            //---------------------------------------------------------------
            // 衝突したオブジェクトに攻撃力を加算する？オブジェクト側でやる？
            //GetComponent<衝突したオブジェクトのコンポーネント>.耐久値;
            //耐久値 - AttackPower;
            //やるならこんな感じ？
            
            Destroy(this.gameObject);   //衝突したら自信を破棄
        }
        
    }

}
