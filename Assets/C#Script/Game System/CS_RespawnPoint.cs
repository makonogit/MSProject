//-------------------------------
// クラス名 :CS_RespawnPoint
// 内容     :リスポーンポイント
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.GameEvent;
using UnityEngine;

namespace Assets.C_Script.Game_System
{
    internal class CS_RespawnPoint : MonoBehaviour
    {
        [SerializeField]
        private int floorNumber = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") 
            { 
                CSGE_GameOver.SetFloorNumber(floorNumber); 
                Transform trans = this.transform.GetChild(0);
                CSGE_GameOver.SetPosition(trans.position);
                CSGE_GameOver.SetRotation(trans.rotation);
            }
        }
    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================