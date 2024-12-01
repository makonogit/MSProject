//-------------------------------
// クラス名 :CS_LockControl
// 内容     :プレイヤーの行動制限
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.GameEvent
{
    internal class CS_EventAre : MonoBehaviour
    {
        [SerializeField,Tooltip("イベント")]
        private CS_GameEvent gameEvent;
        [Header("行動制限の解放したいもの")]
        [SerializeField,Tooltip("ジャンプ")]
        private CSP_Jump jump;
        [SerializeField,Tooltip("撃つ")]
        private CSP_Shot shot;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Player") StartEvent();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") StartEvent();
        }

        private void StartEvent() 
        {
            gameEvent.enabled = true;
            if (jump != null) jump.enabled = true;
            if (shot != null) shot.enabled = true;
        }
    }
}
