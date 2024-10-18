//-------------------------------
// クラス名 :CS_Mechanical
// 内容     :機械ギミックの基底クラス
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Basic;
using UnityEngine;

namespace Assets.C_Script.Electric.Sensor
{
    public class CS_Sensor :MonoBehaviour
    {
        [SerializeField, Tooltip("送信")]
        protected CS_Transmitter CS_Transmitter;
    }
}

//===============================
// date : 2024/10/17
// programmed by Nakagawa Naoto
//===============================