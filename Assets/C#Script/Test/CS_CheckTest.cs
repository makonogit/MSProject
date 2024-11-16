//-------------------------------
// クラス名 :CS_CheckTest
// 内容     :クラスのチェック用
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.notElectric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.C_Script.Test
{
    internal class CS_CheckTest : MonoBehaviour
    {

        [SerializeField]
        private CS_NumberChanger numberChanger;
        [SerializeField, Range(0, 11)]
        private int num = 0;
        [SerializeField]
        private bool flag = false;
        private void Start() 
        {
        }
        private void Update()
        {
            if (flag)
            {
                numberChanger.SetNumber(num);
                flag = false;
            }
        }
        

    }
}
