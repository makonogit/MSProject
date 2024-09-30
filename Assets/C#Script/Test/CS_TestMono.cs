using Assets.C_Script.Test;//←クラスを呼ぶと自動的につく
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TestMono : MonoBehaviour
{
    CS_TestClass testFunc;
    
    private void Start(){
        testFunc = new CS_TestClass(10,"name");
    }

    private void Update() {
    }
}



