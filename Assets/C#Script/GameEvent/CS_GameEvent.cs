//-------------------------------
// クラス名 :CS_GameEvent 
// 内容     :ゲームイベントの基底クラス
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.GameEvent
{
    public class CS_GameEvent:MonoBehaviour
    {
        [SerializeField,Tooltip("次のイベント")]
        private CS_GameEvent nextEvent;
        [SerializeField]
        protected bool Wait = false;
        [SerializeField]
        protected bool isFinish = false;

        // アクティブ化
        private void OnEnable() =>Init();
        // スタート
        private void Start() => enabled = false;
        
        // 仮想関数
        virtual protected void Awake() {}
        private void Update() 
        {
            if (Wait) return;
            if (isFinish)
            {
                Uninit();
                this.enabled = false;
                ProceedNextEvent();
            }
            else EventUpdate();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        virtual protected void Init() => isFinish = false;
        /// <summary>
        /// 終了処理
        /// </summary>
        virtual protected void Uninit() {}

        virtual protected void EventUpdate() {}

        /// <summary>
        /// 次のイベントに進む
        /// </summary>
        private void ProceedNextEvent() 
        {
            if (nextEvent != null) nextEvent.enabled = true;
        }

    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================