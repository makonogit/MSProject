//-------------------------------
// クラス名 :CS_VoltShock
// 内容     :電気床
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_VoltShock : CS_Mechanical
    {
        [SerializeField,Tooltip("クールダウンタイム")]
        private float cooldownTime = 1.0f;
        private float nowTime = 0.0f;
        private bool isHit = false;
        [Header("効果音")]
        [SerializeField,Tooltip("常に流れる効果音")]
        private AudioSource permanentlySound;
        [SerializeField,Tooltip("当たった時の効果音")]
        private AudioSource hitSound;


        private static string playerTag = "Player";
        private static string offTag = "Gimmick";
        private static string stunTag = "StunObject";

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == playerTag) 
            {
                isHit = true; 
                hitSound.Play();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == playerTag) isHit = false;
        }


        protected override void Start()
        {
            base.Start();
            if (Power)PowerOn();
            else PowerOff();
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!permanentlySound.isPlaying) permanentlySound.Play();
        }

        protected override void PowerOn()
        {
            base.PowerOn();
            this.tag = stunTag;
            nowTime = 1.0f;
        }
        protected override void Execute()
        {
            base.Execute();
            if (nowTime <= 0.0f) Power = false;
            if (isHit) nowTime -= Time.deltaTime;
        }
        protected override void PowerOff()
        {
            base.PowerOff();
            this.tag = offTag;
            nowTime = cooldownTime;
        }
        protected override void Stopped()
        {
            base.Stopped();
            nowTime -= Time.deltaTime;
            if (nowTime < 0.0f) Power = true;
        }
    }
}
//===============================
// date : 2024/11/15
// programmed by Nakagawa Naoto
//===============================