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
        public UnityEngine.UI.Image image_active;   // スキルが使用可能な時のUI
        public UnityEngine.UI.Image image_skill;   // スキル使用中のUI
        public float interval;                      // クールダウン時間
        public int mp;                              // スキル使用時に必要な素材数
        public CraftItemBase craftItem;
        public bool isCraft;
    }

    [SerializeField, Header("1 トラバサミ")]
    private GameObject skillItem1;
    [SerializeField, Header("2 ダミーコア")]
    private GameObject skillItem2;
    [SerializeField, Header("3 ドームシールド")]
    private GameObject skillItem3;
    [SerializeField, Header("4 オートエイム")]
    private CSP_Shot skill4;
    [SerializeField, Header("オートエイムの効果時間")]
    private float skill4_time = 10f;

    [SerializeField, Header("スキルUI")]
    private SkillUI[] SkillUIList;

    private int stockMP;

    // カウントダウン用クラス
    private CS_Countdown countdown;
    private CS_Countdown skill4_countdown;// オートエイムの時間計測

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        countdown = gameObject.AddComponent<CS_Countdown>();
        skill4_countdown = gameObject.AddComponent<CS_Countdown>();
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

        //if (GetInputSystem().GetLeftTrigger() <= 0 && isCraft)
        //{
        //    isCraft = false;
        //}

        stockMP = GetPlayerManager().GetMP();
        foreach (SkillUI ui in SkillUIList)
        {
            ui.image_active.enabled = (ui.image_inierval.fillAmount == 1);
            ui.image_skill.enabled = (ui.isCraft);

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
        if (GetInputSystem().GetButtonLPressed()
            && (GetInputSystem().GetButtonAPressed())
            && (!SkillUIList[0].isCraft)
            && (SkillUIList[0].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[0].mp))
        {
            Vector3 forwardVec = Camera.main.transform.forward * 3f ;

            GameObject obj = Instantiate(skillItem1);

            Vector3 pos = Camera.main.transform.position;
            pos += forwardVec;

            obj.transform.position = pos;
            obj.transform.forward = forwardVec;

            SkillUIList[0].isCraft = true;

            GetPlayerManager().SetMP(stockMP - SkillUIList[0].mp);

            SkillUIList[0].craftItem = obj.GetComponent<CraftItemBase>();
        }

        if (SkillUIList[0].isCraft)
        {
            // 設置が完了してからインターバル開始
            if (SkillUIList[0].craftItem.GetSetUp())
            {
                SkillUIList[0].isCraft = false;

                SkillUIList[0].image_inierval.fillAmount = Mathf.Clamp01(0);
                StartCoroutine(FillImageOverTime(SkillUIList[0].image_inierval, SkillUIList[0].interval));
            }
        }
    }

    void HandlSkill2()
    {
        if (GetInputSystem().GetButtonLPressed()
            && (GetInputSystem().GetButtonBPressed())
            && (!SkillUIList[1].isCraft)
            && (SkillUIList[1].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[1].mp))
        {
            Vector3 forwardVec = Camera.main.transform.forward;

            GameObject obj = Instantiate(skillItem2);

            Vector3 pos = Camera.main.transform.position;
            pos += forwardVec;

            obj.transform.position = pos;
            obj.transform.forward = forwardVec;

            SkillUIList[1].isCraft = true;

            GetPlayerManager().SetMP(stockMP - SkillUIList[1].mp);

            SkillUIList[1].craftItem = obj.GetComponent<CraftItemBase>();
        }

        if (SkillUIList[1].isCraft)
        {
            // 設置が完了してからインターバル開始
            if (SkillUIList[1].craftItem.GetSetUp())
            {
                SkillUIList[1].isCraft = false;

                SkillUIList[1].image_inierval.fillAmount = Mathf.Clamp01(0);
                StartCoroutine(FillImageOverTime(SkillUIList[1].image_inierval, SkillUIList[1].interval));
            }
        }
    }

    void HandlSkill3()
    {
        if (GetInputSystem().GetButtonLPressed()
            && (GetInputSystem().GetButtonYPressed())
            && (!SkillUIList[2].isCraft)
            && (SkillUIList[2].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[2].mp))
        {
            GameObject obj = Instantiate(skillItem3);

            Vector3 pos = Camera.main.transform.position;
            obj.transform.position = pos;

            SkillUIList[2].isCraft = true;

            GetPlayerManager().SetMP(stockMP - SkillUIList[2].mp);

            SkillUIList[2].craftItem = obj.GetComponent<CraftItemBase>();

        }

        if (SkillUIList[2].isCraft)
        {
            // 設置が完了してからインターバル開始
            if (SkillUIList[2].craftItem.GetSetUp())
            {
                SkillUIList[2].isCraft = false;

                SkillUIList[2].image_inierval.fillAmount = Mathf.Clamp01(0);
                StartCoroutine(FillImageOverTime(SkillUIList[2].image_inierval, SkillUIList[2].interval));
            }
        }
    }

    void HandlSkill4()
    {
        if (GetInputSystem().GetButtonLPressed()
            && (GetInputSystem().GetButtonXPressed())
            && (!SkillUIList[3].isCraft)
            && (SkillUIList[3].image_inierval.fillAmount == 1)
            && (stockMP >= SkillUIList[3].mp))
        {
            SkillUIList[3].isCraft = true;

            skill4.SetAuto(true);

            GetPlayerManager().SetMP(stockMP - SkillUIList[3].mp);

            skill4_countdown.Initialize(skill4_time);
        }

        if (SkillUIList[3].isCraft)
        {
            if (skill4_countdown.IsCountdownFinished())
            {
                SkillUIList[3].isCraft = false;

                SkillUIList[3].image_inierval.fillAmount = Mathf.Clamp01(0);
                StartCoroutine(FillImageOverTime(SkillUIList[3].image_inierval, SkillUIList[3].interval));
            }
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
