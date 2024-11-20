//-------------------------------
// クラス名 :CS_CrabTrolley
// 内容     :レールの走行するシステム
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.Splines;
using Assets.C_Script.Gimmick;

namespace Assets.C_Script.Gimmick
{
    public class CS_CrabTrolley :CS_Mechanical
    {
        private SplineAnimate splineAnimate;
        private float duration;
        private bool Return = false;
        private AudioSource audioSource;

        protected override void Start()
        {
            splineAnimate = GetComponent<SplineAnimate>();
            if (splineAnimate == null) Debug.LogError("null component");
            splineAnimate.ElapsedTime = 0.01f;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) Debug.LogError("null component");
        }

        override protected void FixedUpdate()
        {
            base.FixedUpdate();
        }

        // 起動した瞬間
        protected override void PowerOn()
        {
            base.PowerOn();
            splineAnimate.Play();
        }
        // 止めた瞬間
        protected override void PowerOff()
        {
            base.PowerOff();
            splineAnimate.Pause();
        }
        // 起動中
        protected override void Execute()
        {
            base.Execute();
            
            if (ShouldStop) 
            {
                Return = !Return;
                Power = false;
            }
        }

        /// <summary>
        /// 止めるとき
        /// </summary>
        private bool ShouldStop 
        {
            get 
            {
                bool endPoint = splineAnimate.Duration <= splineAnimate.ElapsedTime && !Return;
                bool startPoint = splineAnimate.Duration * 2 <= splineAnimate.ElapsedTime && Return;
                if (endPoint) return true;
                if (startPoint) 
                {
                    splineAnimate.ElapsedTime = 0;
                    return true; 
                }
                return false;
            }
        }
    }
}
//===============================
// date : 2024/10/19
// programmed by Nakagawa Naoto
//===============================