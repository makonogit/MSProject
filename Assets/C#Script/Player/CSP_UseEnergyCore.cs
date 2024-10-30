using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.C_Script.Electric.Mechanical;

public class CSP_UseEnergyCore : ActionBase
{
    [SerializeField, Header("ギミックのタグ")]
    private string targetTag;// 投擲対象のタグ

    [Header("ターゲット関連")]
    [SerializeField]
    private GameObject targetObject;    // ギミックオブジェクト
    [SerializeField]
    private CS_Mechanical mechanical;

    private CS_Countdown countdown;
    private CS_Countdown countdownFreeze;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        countdown = gameObject.AddComponent<CS_Countdown>();
        countdownFreeze = gameObject.AddComponent<CS_Countdown>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 硬直処理

        // bool
        foreach (var pair in GetAnimatorBoolParameterList())
        {
            if (GetAnimator().GetBool(pair.name))
            {
                countdownFreeze.Initialize(pair.time);
                break;
            }
        }
        // float
        foreach (var pair in GetAnimatorFloatParameterList())
        {
            if (GetAnimator().GetFloat(pair.name) >= 1)
            {
                countdownFreeze.Initialize(pair.time);
                break;
            }
        }

        // ギミック起動処理
        if (countdownFreeze.IsCountdownFinished())
        {
            HandleUseEnergyCore();
        }
    }

    void HandleUseEnergyCore()
    {
        if (countdown.IsCountdownFinished() && GetAnimator().GetBool("UseEnergyCore"))
        {
            GetAnimator().SetBool("UseEnergyCore", false);
            mechanical.Power = true;
            targetObject = null;
            mechanical = null;
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
                            countdown.Initialize(pair.time);
                            break;
                        }
                    }
                }

            }
        }
    }

    //**
    //* 衝突処理
    //**
    private void OnCollisionEnter(Collision collision)
    {
        // 起動可能なギミックを取得
        if (collision.gameObject.tag == targetTag)
        {
            targetObject = collision.gameObject;

            mechanical = targetObject.GetComponent<CS_Mechanical>();
        }
    }
}
