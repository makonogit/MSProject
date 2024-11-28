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
        public UnityEngine.UI.Image image_inierval; // �N�[���_�E�����̃Q�[�W
        public UnityEngine.UI.Image image_active;   // �X�L�����g�p�\�Ȏ���UI
        public UnityEngine.UI.Image image_skill;   // �X�L���g�p����UI
        public float interval;                      // �N�[���_�E������
        public int mp;                              // �X�L���g�p���ɕK�v�ȑf�ސ�
        public CraftItemBase craftItem;
        public bool isCraft;
    }

    [SerializeField, Header("1 �g���o�T�~")]
    private GameObject skillItem1;
    [SerializeField, Header("2 �_�~�[�R�A")]
    private GameObject skillItem2;
    [SerializeField, Header("3 �h�[���V�[���h")]
    private GameObject skillItem3;
    [SerializeField, Header("4 �I�[�g�G�C��")]
    private CSP_Shot skill4;
    [SerializeField, Header("�I�[�g�G�C���̌��ʎ���")]
    private float skill4_time = 10f;

    [SerializeField, Header("�X�L��UI")]
    private SkillUI[] SkillUIList;

    private int stockMP;

    // �J�E���g�_�E���p�N���X
    private CS_Countdown countdown;
    private CS_Countdown skill4_countdown;// �I�[�g�G�C���̎��Ԍv��

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
            // �ݒu���������Ă���C���^�[�o���J�n
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
            // �ݒu���������Ă���C���^�[�o���J�n
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
            // �ݒu���������Ă���C���^�[�o���J�n
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
