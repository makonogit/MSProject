//-------------------------------
// クラス名 :CS_GameEvent 
// 内容     :ゲームイベントの基底クラス
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace Assets.C_Script.UI.Gage
{
    public class CS_Slider :MonoBehaviour
    {
        [SerializeField]
        private RectMask2D RectMask;
        private float value = 0.0f;

        public bool SetValue(float val) 
        {
            if (RectMask == null) return false;
            Vector4 padding = RectMask.padding;
            float Max = RectMask.rectTransform.rect.width;
            float rate = Max * (1 - val);
            padding.z = rate;
            RectMask.padding = padding;
            value = val;
            return true;
        }
        public float GetValue() { return value; }
#if UNITY_EDITOR
        [SerializeField,Range(0,1)]
        private float val;

        private void OnValidate()
        {
            if (RectMask != null) SetValue(val);
        }
#endif // UNITY_EDITOR
    }
}
