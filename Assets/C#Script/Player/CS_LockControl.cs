//-------------------------------
// クラス名 :CS_LockControl
// 内容     :プレイヤーの行動制限
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.Player
{
    internal class CS_LockControl : MonoBehaviour
    {
        [SerializeField]
        private CSP_Jump jumpControl;
        [SerializeField]
        private CSP_Shot ShotControl;

        private void Start() 
        {
            ShotControl.enabled = false;
            jumpControl.enabled = false;
        }
    }
}
