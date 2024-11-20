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
        public UnityEngine.UI.Image image_inierval;
        public UnityEngine.UI.Image image_active;
        public float interval;
        public int cost;
    }

    [SerializeField, Header("1 �g���o�T�~")]
    private GameObject skillItem1;
    [SerializeField, Header("2 �_�~�[�R�A")]
    private GameObject skillItem2;
    [SerializeField, Header("3 �h�[���V�[���h")]
    private GameObject skillItem3;
    [SerializeField, Header("4 �I�[�g�G�C��")]
    private CSP_Shot skill4;

    [SerializeField, Header("�X�L��UI")]
    private SkillUI[] SkillUIList;

    private int stock;

    // �N���t�g��
    private bool isCraft;

    // �J�E���g�_�E���p�N���X
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
        // �d������

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

        // �X�L���g�p����
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

        stock = GetPlayerManager().GetIngredientsStock();
        foreach (SkillUI ui in SkillUIList)
        {
            ui.image_active.enabled = (ui.image_inierval.fillAmount == 1);

            if (stock < ui.cost)
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
            && (stock >= SkillUIList[0].cost))
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

            GetPlayerManager().SetIngredientsStock(stock - SkillUIList[0].cost);
        }
    }

    void HandlSkill2()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonBPressed())
            && (!isCraft)
            && (SkillUIList[1].image_inierval.fillAmount == 1)
            && (stock >= SkillUIList[1].cost))
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

            GetPlayerManager().SetIngredientsStock(stock - SkillUIList[1].cost);

        }
    }

    void HandlSkill3()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonYPressed())
            && (!isCraft)
            && (SkillUIList[2].image_inierval.fillAmount == 1)
            && (stock >= SkillUIList[2].cost))
        {
            GameObject obj = Instantiate(skillItem3);

            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            obj.transform.position = pos;

            isCraft = true;

            // �C���^�[�o���J�n
            SkillUIList[2].image_inierval.fillAmount = Mathf.Clamp01(0);
            StartCoroutine(FillImageOverTime(SkillUIList[2].image_inierval, SkillUIList[2].interval));

            GetPlayerManager().SetIngredientsStock(stock - SkillUIList[2].cost);

        }
    }

    void HandlSkill4()
    {
        if ((GetInputSystem().GetLeftTrigger() > 0)
            && (GetInputSystem().GetButtonXPressed())
            && (!isCraft)
            && (SkillUIList[2].image_inierval.fillAmount == 1)
            && (stock >= SkillUIList[2].cost))
        {
            isCraft = true;

            skill4.SetAuto(true);

            // �C���^�[�o���J�n
            SkillUIList[3].image_inierval.fillAmount = Mathf.Clamp01(0);
            StartCoroutine(FillImageOverTime(SkillUIList[3].image_inierval, SkillUIList[2].interval));

            GetPlayerManager().SetIngredientsStock(stock - SkillUIList[3].cost);

        }
    }

    // �摜��FillAmount��0����1�Ɏw�肵�����Ԃŉ��Z����R���[�`��
    private IEnumerator FillImageOverTime(UnityEngine.UI.Image fillImage, float duration)
    {
        float startFillAmount = 0f;  // �����l�i0�j
        float endFillAmount = 1f;    // �ڕW�l�i1�j
        float elapsedTime = 0f;      // �o�ߎ��Ԃ�ǐ�

        // �ŏ���FillAmount��0�̏��
        fillImage.fillAmount = startFillAmount;

        // �o�ߎ��Ԃ��w���duration�ɒB����܂Ń��[�v
        while (elapsedTime < duration)
        {
            // �o�ߎ��Ԃ�i�s
            elapsedTime += Time.deltaTime;

            // FillAmount�̐i�����v�Z�i0����1�Ɍ������ĉ��Z�j
            fillImage.fillAmount = Mathf.Lerp(startFillAmount, endFillAmount, elapsedTime / duration);

            // �X���[�Y�ɐi�s�����邽�߂ɏ����҂�
            yield return null;
        }

        fillImage.fillAmount = endFillAmount;
    }
}
