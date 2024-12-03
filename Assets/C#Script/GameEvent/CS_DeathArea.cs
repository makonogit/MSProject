//-------------------------------
// クラス名 :CS_DeathArea
// 内容     :当たったら死ぬ
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.GameEvent;
using UnityEngine;
public class CS_DeathArea : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    { if(collision.transform.tag == "Player")CSGE_GameOver.GameOver(); }
    private void OnTriggerEnter(Collider other)
    { if (other.tag == "Player") CSGE_GameOver.GameOver(); }
}