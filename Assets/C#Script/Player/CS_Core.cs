using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Core : MonoBehaviour
{
    public enum CORE_STATE
    {
        HAVEPLAYER, //�v���C���[�������Ă���
        DROP,       //�����Ă���
        HAVEENEMY,  //�G�������Ă���
    }

    [SerializeField, Header("�R�A�̎c��")]
    private float CoreEnergy = 1f;

    [SerializeField, Header("�R�A�̏��")]
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
