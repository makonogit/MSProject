using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.C_Script.Electric.Mechanical;
using Assets.C_Script.Electric.Other;

//**
//* �G�l���M�[�R�A���g�p����
//*
//* �S���F�����V�S
//**

public class CSP_UseEnergyCore : ActionBase
{
    [SerializeField, Header("�M�~�b�N�̃^�O")]
    private string targetTag;// �����Ώۂ̃^�O

    [Header("�^�[�Q�b�g�֘A")]
    [SerializeField]
    private GameObject targetObject;    // �M�~�b�N�I�u�W�F�N�g
    [SerializeField]
    private CS_CoreUnit coreUnit;

    // ���g�̃R���|�[�l���g
    private CSP_Throwing csp_throwing;

    // ���Ԍv���p�N���X
    //private CS_Countdown countdown;
    private CS_Countdown countdownFreeze;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        csp_throwing = GetComponent<CSP_Throwing>();

        //countdown = gameObject.AddComponent<CS_Countdown>();
        countdownFreeze = gameObject.AddComponent<CS_Countdown>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //// �d������

        //// bool
        //foreach (var pair in GetAnimatorBoolParameterList())
        //{
        //    if (GetAnimator().GetBool(pair.name))
        //    {
        //        countdownFreeze.Initialize(pair.time);
        //        break;
        //    }
        //}
        //// float
        //foreach (var pair in GetAnimatorFloatParameterList())
        //{
        //    if (GetAnimator().GetFloat(pair.name) >= 1)
        //    {
        //        countdownFreeze.Initialize(pair.time);
        //        break;
        //    }
        //}

        //// �M�~�b�N�N������
        //if (countdownFreeze.IsCountdownFinished())
        //{
        //    HandleUseEnergyCore();
        //}

        HandleUseEnergyCore();
    }

    void HandleUseEnergyCore()
    {
        if (GetAnimator().GetBool("UseEnergyCore"))
        {
            GetAnimator().SetBool("UseEnergyCore", false);
            GetAnimator().SetBool("Mount", false);
            // �R�A���Z�b�g����
            coreUnit.SetCore(csp_throwing.GetEnergyCore());
            //csp_throwing.GetEnergyCore().transform.position = targetObject.transform.position;
            targetObject = null;
            coreUnit = null;
        }
        else if (GetAnimator().GetBool("Mount"))
        {
            if (targetObject != null)
            {
                if (GetInputSystem().GetButtonYPressed() &&
                    !GetAnimator().GetBool("UseEnergyCore"))
                {
                    GetSoundEffect().PlaySoundEffect(2, 6);
                    GetAnimator().SetBool("UseEnergyCore", true);
                    foreach (var pair in GetAnimatorBoolParameterList())
                    {
                        if (pair.name == "UseEnergyCore")
                        {
                            //countdown.Initialize(pair.time);
                            break;
                        }
                    }
                }

            }
        }
    }

    //**
    //* �Փˏ���
    //**
    private void OnCollisionEnter(Collision collision)
    {
        // �N���\�ȃM�~�b�N���擾
        if (collision.gameObject.tag == targetTag)
        {
            coreUnit = collision.gameObject.GetComponent<CS_CoreUnit>();

            if(coreUnit != null)
            {
                targetObject = collision.gameObject;
            }

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        // �N���\�ȃM�~�b�N���擾
        if (collision.gameObject.tag == targetTag)
        {
            coreUnit = null;
            targetObject = null;

        }
    }
}
