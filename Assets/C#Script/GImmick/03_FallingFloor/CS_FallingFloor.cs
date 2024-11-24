//-------------------------------
// クラス名 :CS_FallingFloor
// 内容     :落ちる床
// 担当者   :中川 直登
//-------------------------------
using UnityEditor.Rendering;
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_FallingFloor :CS_MoveObject
    {
        private const string playerTag = "Player";
        private const float waitTime = 0.0f;
        private Rigidbody rigidbody;
        private bool wait = false;
        [SerializeField]
        private float startTime;
        private bool started = false;
        [SerializeField]
        private float fallTime = 1.0f;
        [SerializeField]
        private AudioSource audioSource;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag != playerTag) return;
                
            collision.transform.SetParent(transform, true);
            if (!Power&&rigidbody.isKinematic) started = true;
        }
        private void OnCollisionExit(Collision collision)
        {
            collision.transform.SetParent(null, true);
            if (collision.transform.tag != playerTag) return;
            if (!Power)
            { 
                started = false; 
                nowTime = startTime;
            }
        }


        protected override void Start()
        {
            base.Start();
            Stop = false;
            nowTime = startTime;
            Power =false;
            if (!TryGetComponent(out rigidbody)) Debug.LogError("null rigidbody component");
            
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (started)
            {
                nowTime -= Time.deltaTime;
                Power = nowTime <= 0.0f;
            }
        }
        protected override void PowerOn()
        {
            base.PowerOn();
            started = false;
            audioSource.Play();
        }

        protected override void Execute()
        {
            wait = !Movement(GetPosition());
            fallTime -= Time.deltaTime;
            if (fallTime <= 0) 
            {
                Power = false;
                rigidbody.isKinematic = false;
            }
        }

        

    }
}
//===============================
// date : 2024/11/15
// programmed by Nakagawa Naoto
//===============================