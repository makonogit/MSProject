//-------------------------------
// クラス名 :CS_GameEvent 
// 内容     :ゲームイベントの基底クラス
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
namespace Assets.C_Script.GameEvent
{
    public class CSGE_Continue :CS_GameEvent
    {
        protected override void Awake()
        {
            base.Awake();
        }
        protected override void EventUpdate()
        {
            base.EventUpdate();

        }

        protected override void Init()
        {
            base.Init();
        }

        protected override void Uninit()
        {
            Time.timeScale = 1;
            base.Uninit();
        }
    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================