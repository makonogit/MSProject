using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSP_Skill : ActionBase
{
    [SerializeField, Header("スキルアイテム1")]
    private GameObject skillItems1;
    [SerializeField, Header("スキルアイテム2")]
    private GameObject skillItems2;
    [SerializeField, Header("スキルアイテム3")]
    private GameObject skillItems3;

    // クラフト中
    private bool isCraft;

    // カウントダウン用クラス
    private CS_Countdown countdown;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        countdown = gameObject.AddComponent<CS_Countdown>();
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
                countdown.Initialize(pair.time);
                break;
            }
        }
        // float
        foreach (var pair in GetAnimatorFloatParameterList())
        {
            if (GetAnimator().GetFloat(pair.name) >= 1)
            {
                countdown.Initialize(pair.time);
                break;
            }
        }

        // スキル使用処理
        if (countdown.IsCountdownFinished())
        {
            HandlSkill1();
            HandlSkill2();
            HandlSkill3();
        }

        if (GetInputSystem().GetLeftTrigger() <= 0 && isCraft)
        {
            isCraft = false;
        }
    }

    void HandlSkill1()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonAPressed())
            && (!isCraft))
        {
            Vector3 forwardVec = GetPlayerManager().GetCameraTransform().forward;

            GameObject obj = Instantiate(skillItems1);

            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            pos += forwardVec;

            obj.transform.position = pos;
            obj.transform.forward = forwardVec;

            isCraft = true;
        }
    }

    void HandlSkill2()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonBPressed())
            && (!isCraft))
        {
            Vector3 forwardVec = GetPlayerManager().GetCameraTransform().forward;

            GameObject obj = Instantiate(skillItems2);

            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            pos += forwardVec;

            obj.transform.position = pos;
            obj.transform.forward = forwardVec;

            isCraft = true;
        }
    }

    void HandlSkill3()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonYPressed())
            && (!isCraft))
        {
            GameObject obj = Instantiate(skillItems3);

            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            obj.transform.position = pos;

            isCraft = true;
        }
    }
}
