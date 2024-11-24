//-------------------------------
// クラス名 :CS_VoltShock
// 内容     :落ちる床
// 担当者   :中川 直登
//-------------------------------
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_Generator : CS_Mechanical
    {
        [SerializeField, Tooltip("明るさ"),Range(0,40000)]
        private float intensity;
        [SerializeField ,Tooltip("持続時間")]
        private float runTime = 1.0f;
        [SerializeField, Tooltip("点灯時間")]
        private float  lightUpTime = 1.0f;
        [SerializeField, Tooltip("消灯時間")]
        private float lightOutTime = 1.0f;
        private float lightTime =0.0f;
        private float nowTime = 0.0f;
        [SerializeField,Tooltip("点灯するまでの明るさのカーブ")]
        private AnimationCurve brightCurve;
        [SerializeField, Tooltip("消灯するまでの明るさのカーブ")]
        private AnimationCurve darkCurve;
        [SerializeField]
        private List<Light> lights = new List<Light>();
        [SerializeField]
        private List<string>tags = new List<string>();
        [Header("効果音")]
        [SerializeField,Tooltip("アイドリングサウンドソース")]
        private AudioSource IdlingSound;
        [SerializeField,Tooltip("起動時サウンドソ－ス")]
        private AudioSource bootSound;

        private void OnCollisionEnter(Collision collision)
        {
            foreach (string tag in tags)
            {
                if (tag == collision.transform.tag)
                { 
                    Power = true; 
                    nowTime = runTime;
                }
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            foreach (string tag in tags)
            {
                if (tag == other.tag)
                {
                    Power = true;
                    nowTime = runTime;
                }
            }

        }
        /// <summary>
        /// スタート
        /// </summary>
        protected override void Start()
        {
            base.Start();
            IdlingSound.loop = true;
            IdlingSound.Play();
            foreach (Light light in lights) light.intensity = 0.0f;
        }
        /// <summary>
        /// 起動時
        /// </summary>
        protected override void PowerOn()
        {
            base.PowerOn();
            lightTime = lightUpTime;
            bootSound.Play();
        }
        /// <summary>
        /// 実行時
        /// </summary>
        protected override void Execute()
        {
            base.Execute();

            BrighteningLight(lightUpTime,brightCurve);
            if (nowTime <= 0)Power = false;
            nowTime -= Time.deltaTime;
        }
        /// <summary>
        /// 終了時
        /// </summary>
        protected override void PowerOff()
        {
            base.PowerOff();
            lightTime = lightOutTime;
        }
        /// <summary>
        /// 終了後
        /// </summary>
        protected override void Stopped()
        {
            base.Stopped();
            BrighteningLight(lightOutTime,darkCurve);
        }

        /// <summary>
        /// 明るくなる処理
        /// </summary>
        private void BrighteningLight(float maxTime,AnimationCurve animCurve) 
        {
            float time = 1.0f;
            if (lightTime > 0)
            {
                time = maxTime - lightTime;
                time /= maxTime;
            }
            // 明るさ設定
            foreach (Light light in lights) light.intensity = animCurve.Evaluate(time) * intensity;
            lightTime -= Time.deltaTime;
        }
        

    }
}
//===============================
// date : 2024/11/21
// programmed by Nakagawa Naoto
//===============================