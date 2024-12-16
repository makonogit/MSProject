using Assets.C_Script.UI.Result;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �S���F���@�f�J�ʋl�̃V�X�e��
/// </summary>
public class CS_BigCan : MonoBehaviour
{
    [SerializeField, Header("����V�X�e��")]
    private CS_Break BreakSystem;
    [SerializeField, Header("�X�e�[�^�X�\���X�N���v�g")]
    private CS_StageInfo StatusInfo;

    [Header("�e�p�����[�^")]
    [SerializeField, Tooltip("HP")]
    private float HP = 5f;
    private float NowHP;
    [SerializeField, Tooltip("����Œ���~����")]
    private float StopMaxTime = 5f;
    [SerializeField, Tooltip("�����~�ő勗��(�G���A����?)")]
    private int StopDistance = 5;

    [Header("�G��Ȃ��Ł[�[�[�I")]
    [SerializeField, Header("�o���A�R���C�_�[")]
    private SphereCollider BarrierCollider;
    [SerializeField, Header("�f�J�ʋl�R���C�_�[")]
    private SphereCollider BigCanCollider;
    
    [SerializeField, Header("HP�Q�[�W�L�����o�X")]
    private GameObject HPCanvas;
    [SerializeField, Header("HP�Q�[�W")]
    private Image HPGage;
    private bool BreakFlg = false;   //��ꂽ���ǂ���

    //--------�^�C�}�[�֘A---------
    private Coroutine currentCoroutine; //���݌v�����Ă���R���[�`��

    private void Start()
    {
        //HP�Q�[�W�̏���
        NowHP = HP;
        HPGage.fillAmount = NowHP / HP;
    }

    private IEnumerator EndStopBreak(float time)
    {
        yield return new WaitForSeconds(time);

        //�X�e�[�^�X�̕\��
        StatusInfo.SetStatus(CS_StageInfo.StageStatus.BreakStart);
        //�ĂэĊJ
        BreakSystem.ArartStop(false);

    }

    /// <summary>
    /// HP�Q�[�W�̕\��
    /// </summary>
    public void ViewHPGage(Transform PlayerTrans)
    {
        if (HPCanvas == null) { return; }
        HPCanvas.transform.LookAt(PlayerTrans); //HP�Q�[�W����Ƀv���C���[�̕���
        HPCanvas.SetActive(true);
        if(currentCoroutine != null) { return; }
        currentCoroutine = StartCoroutine(EndViewHP());//HP���\������Ă��������
    }


    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        currentCoroutine = null;
        //�Ăє�\����
        if(HPCanvas)HPCanvas.SetActive(false);

    }

    //�����蔻��

    private void OnTriggerEnter(Collider other)
    {
        //�o���A�����Ă��Ȃ��ꍇ�̏���
        if (BreakFlg) { return; }
       
        //-----�Փ˂����e���G�̒e����Ȃ����HP�����炷------
        other.transform.TryGetComponent<CS_AirBall>(out CS_AirBall ball);
        bool Attack = other.gameObject.tag == "Attack" && ball != null && !ball.GetEnemyType();
        if (!Attack) { return; }

        NowHP -= ball.Power;

        //HP�Q�[�W������
        HPGage.fillAmount = NowHP / HP;

        //�o���A�̓����蔻��OFF�A�f�J�ʋl�̓����蔻���ON
        if (NowHP <= 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(HPCanvas);
            BreakFlg = true;
            BarrierCollider.enabled = false;
            BigCanCollider.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if(BreakFlg)
        {
            //�v���C���[�ƏՓ˂�����擾�A��~����
            bool PlayerHit = collision.transform.tag == "Player";
            if (!PlayerHit) { return; }

            //��\���ɂ���
            TryGetComponent<MeshRenderer>(out MeshRenderer renderer);
            renderer.enabled = false;
            BigCanCollider.enabled = false;
    
            //�j��G���A�Ƃ̍����擾
            int BreakDistance = BreakSystem.GetBreakAreaDistance();
            //�ő勗����藣��Ă������~���Ȃ�
            if(BreakDistance > StopDistance)
            {
                Destroy(this.gameObject);
                return; 
            }

            //�X�e�[�^�X�̕\��
            StatusInfo.SetStatus(CS_StageInfo.StageStatus.BreakStop);
            BreakSystem.ArartStop(true);    //����V�X�e���̒�~

            //���������~���Ԃ����߂�
            float StopTime = StopMaxTime * (1 - ((float)BreakDistance / (float)StopDistance));

            //��莞�Ԓ�~����
            StartCoroutine(EndStopBreak(StopTime));

            // �������𑝉�����             // * �ǉ��F����
            CSGE_Result.GettingBigCan();    // * �ǉ��F����
        }
    }

    
}
