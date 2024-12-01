//-------------------------------
// クラス名 :CS_GameEvent 
// 内容     :ゲームイベントの基底クラス
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.C_Script.GameEvent
{
    internal class CSGE_MultiDialog : CS_GameEvent
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private List<Sprite> sprites = new List<Sprite>();
        [SerializeField]
        private bool display = false;
        [SerializeField]
        private bool isOnce = false;
        private bool end = false;
        [SerializeField]
        private Animator animator = null;
        [SerializeField]
        private CS_InputSystem inputSystem = null;
        private int index = 0;
        private int Index 
        {
            set 
            {
                index = Mathf.Max(Mathf.Min(value, sprites.Count - 1),0);
            }
            get { return index; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (sprites.Count > 0) image.sprite = sprites[0];
        }

        protected override void EventUpdate()
        {
            base.EventUpdate();
            animator.SetBool("Display", true);
            if (display) ChangeSprite();
        }

        protected override void Init()
        {
            base.Init();
            if (end && isOnce) isFinish = true;
            animator.SetBool("Close", false);
        }

        protected override void Uninit()
        {
            base.Uninit();
            animator.SetBool("Close", true);
            animator.SetBool("Display", false);
            end = true;
        }
        private void ChangeSprite()
        {
            if (inputSystem.GetDpadLeftTriggered()) 
            { 
                Index--; 
                image.sprite = sprites[index];
            }
            if (inputSystem.GetDpadRightTriggered()) 
            {
                Index++;
                image.sprite = sprites[index];
            }
            if (index>= sprites.Count-1 && inputSystem.GetButtonBTriggered()) isFinish = true;
        }
    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================