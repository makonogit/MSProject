//-------------------------------
// クラス名 :CS_MovingWall
// 内容     :動く壁
// 担当者   :中川 直登
//-------------------------------

using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_MovingWall : CS_MoveObject
    {
        [SerializeField, Tooltip("開始時に待つ時間")]
        private float firstTime;
        [SerializeField,Tooltip("初めの位置で待つ時間")]
        private float startPointWaitTime;
        private bool started = false;
        [SerializeField,Tooltip("終わりの位置で待つ時間")]
        private float endPointWaitTime;

        [Header("効果音")]
        [SerializeField, Tooltip("稼働音")]
        protected AudioSource moveAudioSource;
        
        private bool wait = false;

        protected override void Start()
        {
            base.Start();
            Stop = false;
            nowTime = firstTime;
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
                if (wait) nowTime = (GoEndPoint ? startPointWaitTime : endPointWaitTime);
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
        
        protected override void MoveSound()
        {
            if (moveAudioSource != null && !moveAudioSource.isPlaying)
            {
                moveAudioSource.loop = true;
                moveAudioSource.Play();
            }
        }

    }
}
//===============================
// date : 2024/11/20
// programmed by Nakagawa Naoto
//===============================