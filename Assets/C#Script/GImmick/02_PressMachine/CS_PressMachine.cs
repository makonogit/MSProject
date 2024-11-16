//-------------------------------
// クラス名 :CS_PressMachine
// 内容     :プレス機ギミック
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Electric.Mechanical
{
    public class CS_PressMachine :CS_MoveObject
    {
        [SerializeField]
        private float startTime;
        private bool started = false;
        [SerializeField]
        private float waitTime;
       
        private bool wait = false;

        protected override void Start()
        {
            base.Start();
            Stop = false;
            nowTime = startTime;
            started = false;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!started) 
            {
                nowTime -= Time.deltaTime;
                started = nowTime <= 0.0f;
                Power = started;
            }
            
        }

        protected override void Execute()
        {
            // base.Execute();
            if (wait) Wait();
            else 
            { 
                wait = !Movement(GetPosition());
                if (wait) nowTime = waitTime;
            }
        }

        private void Wait() 
        {
            nowTime -= Time.deltaTime;
            wait = nowTime > 0.0f;
            if (!wait) nowTime = 0.0f;
            if (moveAudioSource != null && moveAudioSource.isPlaying)
            { 
                moveAudioSource.Stop();
            }
        }
        protected override void PressSound()
        {
            if (pressAudioSource != null && !GoEndPoint) pressAudioSource.Play();
        }

        /// <summary>
        /// 効果音全停止
        /// </summary>
        public void StopSounds() 
        {
            if ( moveAudioSource != null && moveAudioSource.isPlaying) moveAudioSource.Stop();
            if ( pressAudioSource != null && pressAudioSource.isPlaying) pressAudioSource.Stop();
        }
    }
}
//===============================
// date : 2024/10/25
// programmed by Nakagawa Naoto
//===============================