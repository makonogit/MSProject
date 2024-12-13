//-------------------------------
// クラス名 :CS_Transmitter
// 内容     :信号送信機
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Gimmick;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.Electric.Basic
{
    [System.Serializable]
    public class CS_Transmitter
    {

        [SerializeField,Tooltip("送信先リスト：\n送信する相手")]
        private List<CS_Mechanical> receivers =new List<CS_Mechanical>();
        /// <summary>
        /// 送信
        /// </summary>
        /// <param name="signal"></param>
        public void Transmission(bool signal)
        {
            foreach (CS_Mechanical mechanical in receivers) if (mechanical != null) mechanical.Power = signal;
        }
    }
}

//===============================
// date 2024/10/17
// programmed by Nakagawa Naoto
//===============================