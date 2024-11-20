//-------------------------------
// クラス名 :CS_AND
// 内容     :and
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Mechanical;
using Assets.C_Script.Gimmick;
using UnityEngine;

namespace Assets.C_Script.Electric.Basic
{
    public class CS_AND:MonoBehaviour
    {
        [SerializeField,Tooltip("受信_A")]
        private CS_Mechanical receive_A;
        [SerializeField,Tooltip("受信_B")]
        private CS_Mechanical receive_B;
        
        [SerializeField,Tooltip("送信先")]
        private CS_Transmitter transmitter;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            if (receive_A == null) Debug.LogError("receive_Aが設定されていません。");
            if (receive_B == null) Debug.LogError("receive_Aが設定されていません。");
            if (transmitter == null) Debug.LogError("transmitterが設定されていません。");
        }

        /// <summary>
        /// FixedUpdate
        /// </summary>
        private void FixedUpdate()
        {
            transmitter.Transmission(AND);
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update() {}

        /// <summary>
        /// AND
        /// 受信機の二つの結果がTrue呑みTrueを返す
        /// </summary>
        private bool AND 
        {
            get 
            {
                if(!receive_A.Power)return false;
                if(!receive_B.Power)return false;
                return true;
            }
        }
    }
}

//===============================
// date : 2024/10/19
// programmed by Nakagawa Naoto
//===============================