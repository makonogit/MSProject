using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Core : MonoBehaviour
{
    public enum CORE_STATE
    {
        HAVEPLAYER, //プレイヤーが持っている
        DROP,       //落ちている
        HAVEENEMY,  //敵が持っている
    }

    [SerializeField, Header("コアの残量")]
    private float CoreEnergy = 1f;

    [SerializeField, Header("コアの状態")]
    private CORE_STATE state;

    public CORE_STATE STATE
    {
        set
        {
            state = value;
        }
        get
        {
            return state;
        }
    }

}
