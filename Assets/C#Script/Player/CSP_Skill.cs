using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class CSP_Skill : ActionBase
{
    [System.Serializable]
    public class SkillUI
    {
        public UnityEngine.UI.Image image_inierval; // クールダウン中のゲージ
        public UnityEngine.UI.Image image_active;   // スキルが使用可能なUI
        public float interval;                      // クールダウン時間
        public int mp;                              // スキル使用時に必要な素材数
    }

    [SerializeField, Header("1 トラバサミ")]
    private GameObject skillItem1;
    [SerializeField, Header("2 ダミーコア")]
    private GameObject skillItem2;
    [SerializeField, Header("3 ドームシールド")]
    private GameObject skillItem3;
    [SerializeField, Header("4 オートエイム")]
    private CSP_Shot skill4;

    [SerializeField, Header("スキルUI")]
    private SkillUI[] SkillUIList;

    private int stockMP;

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
            HandlSkill4();
        }

        if (GetInputSystem().GetLeftTrigger() <= 0 && isCraft)
        {
            isCraft = false;
        }

        stockMP = GetPlayerManager().GetMP();
        foreach (SkillUI ui in SkillUIList)
        {
            ui.image_active.enabled = (ui.image_inierval.fillAmount == 1);

            if (stockMP < ui.mp)
            {
                ui.image_inierval.fillAmount = 0;
            }
            else if(ui.image_inierval.fillAmount == 0)
            {
                ui.image_inierval.fillAmount = 1;
            }
        }

    }

    void HandlSkill1()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonAPressed())
            && (!isCraft)
            && (SkillUIList[0].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[0].mp))
        {
            Vector3 forwardVec = GetPlayerManager().GetCameraTransform().forward;

            GameObject obj = Instantiate(skillItem1);

            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            pos += forwardVec;

            obj.transform.position = pos;
            obj.transform.forward = forwardVec;

            isCraft = true;

            SkillUIList[0].image_inierval.fillAmount = Mathf.Clamp01(0);
            StartCoroutine(FillImageOverTime(SkillUIList[0].image_inierval, SkillUIList[0].interval));

            GetPlayerManager().SetMP(stockMP - SkillUIList[0].mp);
        }
    }

    void HandlSkill2()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonBPressed())
            && (!isCraft)
            && (SkillUIList[1].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[1].mp))
        {
            Vector3 forwardVec = GetPlayerManager().GetCameraTransform().forward;

            GameObject obj = Instantiate(skillItem2);

            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            pos += forwardVec;

            obj.transform.position = pos;
            obj.transform.forward = forwardVec;

            isCraft = true;

            SkillUIList[1].image_inierval.fillAmount = Mathf.Clamp01(0);
            StartCoroutine(FillImageOverTime(SkillUIList[1].image_inierval, SkillUIList[1].interval));

            GetPlayerManager().SetMP(stockMP - SkillUIList[1].mp);

        }
    }

    void HandlSkill3()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonYPressed())
            && (!isCraft)
            && (SkillUIList[2].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[2].mp))
        {
            GameObject obj = Instantiate(skillItem3);

            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            obj.transform.position = pos;

            isCraft = true;

            // インターバル開始
            SkillUIList[2].image_inierval.fillAmount = Mathf.Clamp01(0);
            StartCoroutine(FillImageOverTime(SkillUIList[2].image_inierval, SkillUIList[2].interval));

            GetPlayerManager().SetMP(stockMP - SkillUIList[2].mp);

        }
    }

    void HandlSkill4()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonXPressed())
            && (!isCraft)
            && (SkillUIList[2].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[2].mp))
        {
            isCraft = true;

            skill4.SetAuto(true);

            // インターバル開始
            SkillUIList[3].image_inierval.fillAmount = Mathf.Clamp01(0);
            StartCoroutine(FillImageOverTime(SkillUIList[3].image_inierval, SkillUIList[2].interval));

            GetPlayerManager().SetMP(stockMP - SkillUIList[3].mp);

        }
    }

    // 画像のFillAmountを0から1に指定した時間で加算するコルーチン
    private IEnumerator FillImageOverTime(UnityEngine.UI.Image fillImage, float duration)
    {
        float startFillAmount = 0f;  // 初期値（0）
        float endFillAmount = 1f;    // 目標値（1）
        float elapsedTime = 0f;      // 経過時間を追跡

        // 最初はFillAmountが0の状態
        fillImage.fillAmount = startFillAmount;

        // 経過時間が指定のdurationに達するまでループ
        while (elapsedTime < duration)
        {
            // 経過時間を進行
            elapsedTime += Time.deltaTime;

            // FillAmountの進捗を計算（0から1に向かって加算）
            fillImage.fillAmount = Mathf.Lerp(startFillAmount, endFillAmount, elapsedTime / duration);

            // スムーズに進行させるために少し待つ
            yield return null;
        }

        fillImage.fillAmount = endFillAmount;
    }
}
