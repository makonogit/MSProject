//-------------------------------
// クラス名 :CS_Mechanical
// 内容     :機械ギミックの基底クラス
// 担当者   :中川 直登
//-------------------------------
using UnityEditor;
using UnityEngine;

namespace Assets.C_Script.Gimmick._00_Base
{
    public class CS_Mechanical :MonoBehaviour
    {
        [SerializeField, Tooltip("起動スイッチ")]
        private bool power = false;
        private bool oldPower = false;
        public bool Power { get { return power; } set { power = value;} }

        virtual protected void Start(){}

        virtual protected void FixedUpdate()
        {
            bool powerOn = power && !oldPower;
            bool powerOff = !power && oldPower;
            if (powerOn) PowerOn();
            if (powerOff) PowerOff();
            if (Power)Execute();
            else Stopped();
        }

        private void Update(){}

        

        virtual protected void PowerOn() { oldPower = power; }
        virtual protected void PowerOff(){ oldPower = power; }
        virtual protected void Execute() {}
        virtual protected void Stopped() {}



#if UNITY_EDITOR
        virtual public void DrawGizmos(){}
#endif // UNITY_EDITOR
    }
}

//===============================
// date 2024/10/18
// programmed by Nakagawa Naoto
//===============================