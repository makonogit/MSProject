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
        [SerializeField]
        private int groupNumber = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") 
            { 
                CSGE_GameOver.SetFloorNumber(floorNumber); 
                CSGE_GameOver.SetGroupNumber(groupNumber);
                Transform trans = this.transform.GetChild(0);
                CSGE_GameOver.SetPlayerPosition(trans.position);
                CSGE_GameOver.SetPlayerRotation(trans.rotation);
                trans = this.transform.GetChild(1);
                CSGE_GameOver.SetCorePosition(trans.position);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
#endif // UNITY_EDITOR
    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================