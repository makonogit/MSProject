//-------------------------------
// クラス名 :CS_StopArmAnimation
// 内容     :クレーンアームのアニメーションをストップする
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Electric.Mechanical
{
    public class CS_StopArmAnimation :MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Core") animator.SetFloat("speed", 0);
        }
    }
}
//===============================
// date : 2024/10/29
// programmed by Nakagawa Naoto
//===============================