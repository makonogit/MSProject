﻿//-------------------------------
// クラス名 :CS_GameEvent 
// 内容     :ゲームイベントの基底クラス
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
namespace Assets.C_Script.GameEvent
{
    public class CSGE_GameOver : CS_GameEvent
    {
        // 静的メンバー
        private static CSGE_GameOver GameOverEvent;
        private static int floorNumber = 0;
        private static Vector3 position = Vector3.zero;
        private static Quaternion rotation = Quaternion.identity;
        public static void GameOver() 
        {
            if (GameOverEvent == null) return;
            GameOverEvent.enabled = true;
        }
        public static void SetFloorNumber(int num) => floorNumber = num;
        public static void SetPosition(Vector3 pos) => position = pos;
        public static void SetRotation(Quaternion rotate)=> rotation = rotate;

        [SerializeField]
        private float timeSpeed =1.0f;
        [SerializeField]
        private AudioMixer audioMixer = null;
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private CS_InputSystem inputSystem = null;
        [SerializeField]
        private Transform player = null;
        [SerializeField]
        private bool canContinue = false;

        protected override void Awake()
        {
            base.Awake();
            GameOverEvent = this;
            if (player != null) 
            { 
                player.position = position; 
                player.rotation = rotation;
            }
            
        }

        protected override void EventUpdate()
        {
            base.EventUpdate();
            animator.SetBool("GameOver", true);
            SetSlowly(timeSpeed);
            if (canContinue&&inputSystem.GetButtonATriggered()) Restart();
        }

        protected override void Init()
        {
            base.Init();
        }

        protected override void Uninit()
        {
            base.Uninit();
        }

        private void Restart()
        {
            canContinue = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SetSlowly(1.0f);
        }

        /// <summary>
        /// 時間と音の速度設定
        /// </summary>
        /// <param name="value"></param>
        private void SetSlowly(float value) 
        {
            float val = Mathf.Min(Mathf.Max(0f, value), 1f);
            Time.timeScale = val;
            audioMixer.SetFloat("MasterPitch", val);
        }

    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================