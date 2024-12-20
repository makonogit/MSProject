//-------------------------------
// クラス名 :CS_CrabTrolley
// 内容     :レールの走行するシステム
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.Splines;
using Assets.C_Script.Gimmick;
using System.Net;

namespace Assets.C_Script.Gimmick
{
    public class CS_CrabTrolley :CS_Mechanical
    {
        private SplineAnimate splineAnimate;
        private bool Return = false;
        private AudioSource audioSource;
        [SerializeField]
        private Transform startUnit;
        [SerializeField]
        private Transform endUnit;
        [SerializeField]
        private Transform arm;

        protected override void Start()
        {
            splineAnimate = GetComponent<SplineAnimate>();
            if (splineAnimate == null) Debug.LogError("null component");
            splineAnimate.ElapsedTime = 0.01f;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) Debug.LogError("null component");
            SetUnitPosition();
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

        public bool endPoint 
        {
            get
            {
                bool value = splineAnimate.Duration <= splineAnimate.ElapsedTime;
                return value; 
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
                if (endPoint) 
                { 
                    splineAnimate.ElapsedTime = splineAnimate.Duration;
                    return true; 
                }
                if (startPoint) 
                {
                    splineAnimate.ElapsedTime = 0;
                    return true; 
                }
                return false;
            }
        }

        private void SetUnitPosition() 
        {
            if (splineAnimate == null) return;
            if (startUnit == null) return;
            if (endUnit == null) return;
            if (arm == null) return;
            Vector3 position= Vector3.zero;
            // スタートポイントの設定
            float time = splineAnimate.ElapsedTime;
            // エンドポイントの設定
            {
                // エンド位置に設定
                splineAnimate.ElapsedTime = splineAnimate.Duration;
                // 位置の取得
                position = arm.transform.position;
                position.y = endUnit.position.y;
                endUnit.position = position;
            }

            {
                // スタート位置に設定
                splineAnimate.ElapsedTime = 0;
                // 位置の取得
                position = arm.transform.position;
                position.y = startUnit.position.y;
                startUnit.position = position;
            }
            // 元に戻す
            splineAnimate.ElapsedTime = time;

        }
    }
}
//===============================
// date : 2024/10/19
// programmed by Nakagawa Naoto
//===============================