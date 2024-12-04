//-------------------------------
// クラス名 :CSGE_ImageDisplay
// 内容     :イメージ表示
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.UI;
namespace Assets.C_Script.GameEvent
{
    public class CSGE_ImageDisplay : CS_GameEvent
    {
        [SerializeField]
        private Image image;
        private Color color;

        private void OnDisable() =>Uninit();
        protected override void Awake()
        {
            base.Awake();
            if (image == null && !TryGetComponent(out image)) Debug.LogError("null image");
            color = image.color;
            color.a = 0f;
        }

        protected override void Init()
        {
            base.Init();
            
        }

        protected override void EventUpdate()
        {
            base.EventUpdate();
            color.a = 1f;
            image.color = color;
        }

        protected override void Uninit()
        {
            base.Uninit();
            color.a = 0f;
            image.color = color;
        }
    }
}
//===============================
// date : 2024/12/04
// programmed by Nakagawa Naoto
//===============================