//-------------------------------
// クラス名 :CS_Switch
// 内容     :送信スイッチ
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Basic;
using UnityEngine;

namespace Assets.C_Script.Electric.Sensor
{
    public class CS_Switch : MonoBehaviour
    {
        [SerializeField]
        private bool signal = false;
        [SerializeField, Tooltip("送信")]
        protected CS_Transmitter transmitter;
        private bool oldSignal;
        [SerializeField]
        protected bool Signal {  get { return signal; }set { signal = value; } }

        private enum SendMethod 
        {
            always,         // 常に
            change_on,      // 変わる瞬間 on
            change_off,     // 変わる瞬間 off
            change_on_off,  // 変わる瞬間 on-off
        }
        private enum OutputSignal
        {
            raw_signal,     // シグナルをそのまま送る
            anti_signal,    // シグナルを反転して送る
            true_only,      // Trueのみ送る
            false_only,     // Falseのみ送る
        }
        [SerializeField]
        private SendMethod sendType;
        [SerializeField]
        private OutputSignal outputType;


        virtual protected void FixedUpdate()
        {
            if (ShouldTransmission) 
            { 
                oldSignal = Signal;
                transmitter.Transmission(SignalOutput); 
            }
        }
        private void Update(){}
        
        private bool ShouldTransmission
        {
            get 
            {
                if (sendType == SendMethod.always) return true;
                if (sendType == SendMethod.change_on) return signal && !oldSignal;
                if (sendType == SendMethod.change_off) return !signal && oldSignal;
                if (sendType == SendMethod.change_on_off) return signal == !oldSignal;
                return false; 
            }
        }
        
        private bool SignalOutput 
        {
            get             
            {
                if (outputType == OutputSignal.raw_signal) return signal;
                if (outputType == OutputSignal.anti_signal) return !signal;
                if (outputType == OutputSignal.true_only) return true;
                if (outputType == OutputSignal.false_only) return false;
                return false;
            }
        }
    }
}

//===============================
// date : 2024/10/20
// programmed by Nakagawa Naoto
//===============================