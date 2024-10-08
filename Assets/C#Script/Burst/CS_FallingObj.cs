using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落下オブジェクト
/// </summary>
public class CS_FallingObj : MonoBehaviour
{
    /// <summary>
    /// 床と接触したら消滅する
    /// </summary>
    /// <param 衝突物="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //何かと接触したら消す
        Destroy(this.gameObject);

        //プレイヤーと接触したか
        bool HitFloor = collision.gameObject.tag == "Player";
        if (HitFloor)
        {
            //Destroy(this.gameObject);
        }
    }
}
