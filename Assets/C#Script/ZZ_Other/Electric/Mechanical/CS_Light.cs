//-------------------------------
// クラス名 :CS_Light
// 内容     :光らせるやつ
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Gimmick;
using UnityEngine;

namespace Assets.C_Script.Electric.Mechanical
{
    public class CS_Light :CS_Mechanical
    {
        [SerializeField]
        private Light light;
        [SerializeField,Range(0, 40000.00f)]
        private float MaxIntensity = 0.0f;
        [SerializeField]
        private float time = 1.0f;
        private float speed = 0.0f;

        private void Start()
        {
            speed = MaxIntensity / time;
        }

        protected override void Execute()
        {
            base.Execute();
            if (light.intensity >= MaxIntensity) return;
            light.intensity += speed * Time.deltaTime;
        }

        protected override void Stopped()
        {
            base.Stopped();
            light.intensity -= speed * Time.deltaTime;
        }

    }
}
//===============================
// date : 2024/10/19
// programmed by Nakagawa Naoto
//===============================