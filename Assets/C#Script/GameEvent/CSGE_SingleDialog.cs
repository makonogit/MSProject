//-------------------------------
// クラス名 :CS_GameEvent 
// 内容     :ゲームイベントの基底クラス
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Assets.C_Script.GameEvent
{
    internal class CSGE_SingleDialog :CS_GameEvent
    {
        [SerializeField]
        private bool display = false;
        [SerializeField]
        private bool isOnce = false;
        private bool end = false;
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private CS_InputSystem inputSystem = null;
        [SerializeField]
        private GameObject b_button = null;

        protected override void Awake()
        {
            base.Awake();
            if (inputSystem == null) Debug.LogError("InputSystemが設定されていません。 " + gameObject.name);
        }

        protected override void EventUpdate()
        {
            base.EventUpdate();
            animator.SetBool("Display", true);
            if (display && inputSystem.GetButtonBTriggered()) isFinish = true;
            b_button.SetActive(true);
            Time.timeScale = 0f;
        }

        protected override void Init()
        {
            base.Init();
            if(end&&isOnce)isFinish = true;
            animator.SetBool("Close", false);
            b_button.SetActive(false);
        }

        protected override void Uninit()
        {
            base.Uninit();
            animator.SetBool("Close", true);
            animator.SetBool("Display", false);
            Time.timeScale = 1f;
            end = true;
            b_button.SetActive(false);
        }

        
    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================