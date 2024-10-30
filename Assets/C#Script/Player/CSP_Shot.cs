using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class CSP_Shot : ActionBase
{
    [Header("�ˌ��ݒ�")]
    [SerializeField, Header("��C�C�̒e�I�u�W�F�N�g")]
    private GameObject AirBall;// �e
    [SerializeField, Header("���U���̏����l")]
    private int initMagazine;// ���U���̏����l 
    [SerializeField, Header("���݂̑��U��")]
    private int magazine;// ���U��
    [SerializeField, Header("�A�ː�")]
    private int burstfire;// �A�ː�
    [SerializeField, Header("�c�e���̏����l")]
    private int initBulletStock;
    [SerializeField, Header("���݂̎c�e��")]
    private int bulletStock;// �c�e��
    private bool isShot = false;// �ˌ���
    [SerializeField, Header("���炷HP")]
    private float shotHp;
    [SerializeField, Header("�˒�����")]
    private float range = 100f;

    [Header("���e�B�N���ݒ�")]
    [SerializeField, Header("���e�B�N���C���[�W")]
    private Image detectionImage;
    [SerializeField, Header("�A�V�X�g�͈�")]
    float radius = 1.0f;
    [SerializeField, Header("���o���̐F")]
    private Color detectedColor = Color.green; // ���o���̐F
    [SerializeField, Header("�񌟏o���̐F")]
    private Color notDetectedColor = Color.red; // �񌟏o���̐F
    [SerializeField, Header("�j��\�I�u�W�F�N�g�̃^�O")]
    private List<string> targetTag;
    private GameObject targetObject;// ���e�B�N�����̔j��\�I�u�W�F�N�g

    // �J�E���g�_�E���p�N���X
    private CS_Countdown countdown;

    protected override void Start()
    {
        base.Start();

        // �c�e����������
        bulletStock = initBulletStock;

        // ���U����������
        magazine = initMagazine;

        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    void FixedUpdate()
    {
        // ���e�B�N������
        HandlReticle();

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

        // �ˌ�����
        if (countdown.IsCountdownFinished())
        {
            HandlShot();
        }

        // �����[�h����
        //if (GetInputSystem().GetButtonXPressed())
        //{
        //    StartCoroutine(ReloadCoroutine());
        //}

        // �v���C���[�̌����𒲐�
        float offsetAngle = 45f; // �I�t�Z�b�g�l

        // �J�����̑O���x�N�g�����擾
        Vector3 cameraForward = Camera.main.transform.forward;

        // �v���C���[�̐��ʃx�N�g�����擾
        Vector3 playerForward = transform.forward;

        // �J�����̐��ʂƃv���C���[�̐��ʂ̊p�x���v�Z
        float angle = Vector3.Angle(cameraForward, playerForward);

        // �I�t�Z�b�g�l���l�����Ċm�F
        if (angle > offsetAngle)
        {
            // �v���C���[���J�����̕����Ɍ�����
            Vector3 targetDirection = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // 5f�͉�]���x�𒲐�
        }

        // �G�C���A�V�X�g
        bool isAssist = targetObject != null;
        GetPlayerManager().GetTpsCamera().SetAssist(isAssist);
    }

    //**
    //* ���e�B�N��
    //*
    //* in:����
    //* out:����
    //**
    void HandlReticle()
    {
        // ���e�B�N���ƃ^�[�Q�b�g��������
        detectionImage.color = notDetectedColor;
        targetObject = null;

        // �J�������ʂ��烌�C���쐬
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, range);

        // ���e�B�N�����ɔj��\�I�u�W�F�N�g�����邩����
        foreach (RaycastHit hit in hits)
        {
            // �q�b�g�����ꍇ�̏���
            if (targetTag.Contains(hit.collider.tag))
            {
                // �I�u�W�F�N�g���擾���A���e�B�N���̐F��ύX
                detectionImage.color = detectedColor;
                targetObject = hit.collider.gameObject;
                break;
            }
        }
    }


    //**
    //* �ˌ�����
    //*
    //* in:����
    //* out:����
    //**
    void HandlShot()
    {
        // ���U������
        bool isMagazine = magazine > 0;

        // �R���g���[���[����/���˒�/���U���𔻒�
        if (GetInputSystem().GetRightTrigger() > 0 && !isShot)
        {
            CreateBullet(burstfire);
            isShot = true;

            GetAnimator().SetBool("Shot", true);

            //// ���U����0�Ȃ烊���[�h
            //if (isMagazine)
            //{
            //    CreateBullet(burstfire);
            //    isShot = true;

            //    GetAnimator().SetBool("Shot", true);
            //}
            //else if (!GetAnimator().GetBool("Reload"))
            //{
            //    isShot = true;
            //    //StartCoroutine(ReloadCoroutine());

            //    // �A�j���[�V�����̏I���܂őҋ@
            //    countdown.Initialize(1f);
            //    // �����[�h����
            //    ReloadMagazine(initMagazine);
            //    // �A�j���[�V�����Đ�
            //}
        }
        else
        {
            GetAnimator().SetBool("Shot", false);
        }

        if (GetInputSystem().GetRightTrigger() <= 0 && isShot)
        {
            isShot = false;
        }
    }

    //**
    //* �����[�h����
    //*
    //* in:�����[�h��
    //* out:����
    //**
    void ReloadMagazine(int reload)
    {
        magazine = reload;

        //if (bulletStock < reload)
        //{
        //    magazine = bulletStock;
        //    bulletStock = 0;
        //}
        //else
        //{
        //    bulletStock -= (reload - magazine);
        //    magazine = reload;
        //}
    }
    private IEnumerator ReloadCoroutine()
    {
        // �����[�h�A�j���[�V�������J�n
        GetAnimator().SetBool("Reload", true);

        GetAnimator().SetTrigger("Reload");

        // �A�j���[�V�����̒������擾
        AnimationClip clip = GetAnimator().runtimeAnimatorController.animationClips[0];
        float animationLength = clip.length;

        // �A�j���[�V�������Đ������ǂ������m�F
        float elapsedTime = 0f;
        while (elapsedTime < animationLength)
        {
            elapsedTime += Time.deltaTime;
            yield return null; // ���̃t���[���܂őҋ@
        }

        // �����[�h�������s��
        ReloadMagazine(initMagazine);
        GetAnimator().SetBool("Reload", false);
    }

    //**
    //* �e�𐶐����鏈��
    //*
    //* in:������
    //* out:����
    //**
    void CreateBullet(int burst)
    {
        GetSoundEffect().PlaySoundEffect(1, 3);
      
        Vector3 forwardVec = GetPlayerManager().GetCameraTransform().forward;

        // ���e�B�N�����ɔj��\�I�u�W�F�N�g�����݂���Ȃ�A���̕����ɔ��˂���
        if (targetObject != null)
        {
            forwardVec = targetObject.transform.position - transform.position;
            forwardVec = forwardVec.normalized;
        }

        float offsetDistance = 1.5f; // �e�e�̊Ԋu

        if (burst > magazine)
        {
            burst = magazine;
        }

        for (int i = 0; i < burst; i++)
        {
            // �e�𐶐�
            GameObject ballobj = Instantiate(AirBall);

            Vector3 pos = this.transform.position;
            Vector3 offset = new Vector3(0, 1, 0);

            pos += offset;
            pos += forwardVec * (offsetDistance * (i + (burst - 1) / 2f) + 1f);

            ballobj.transform.position = pos;
            ballobj.transform.forward = forwardVec;

            // ���U�������炷
            //magazine--;
            bulletStock--;
            float hp = GetPlayerManager().GetHP();
            GetPlayerManager().SetHP(hp - -shotHp);
        }
    }

    // IKPass
    private void OnAnimatorIK(int layerIndex)
    {
        // �Ր�Ԑ��A�j���[�V�����i�e���\����j
        if ((!GetAnimator().GetBool("Reload")) && (!GetAnimator().GetBool("Throwing")
            && !GetAnimator().GetBool("Recovery") && !GetAnimator().GetBool("Use Item")
            && !GetAnimator().GetBool("Use EnergyCore")))
        {
            // �J�����̐��ʕ����̈ʒu���v�Z
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 targetPosition = Camera.main.transform.position + cameraForward * 100;

            // ���Ƙr���^�[�Q�b�g�̕����Ɍ�����
            if (targetPosition != null)
            {
                // �����^�[�Q�b�g�Ɍ�����
                GetAnimator().SetLookAtWeight(1f, 0.3f, 1f, 0f, 0.5f);
                GetAnimator().SetLookAtPosition(targetPosition);

                // �E�r���^�[�Q�b�g�Ɍ�����
                GetAnimator().SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                GetAnimator().SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                GetAnimator().SetIKPosition(AvatarIKGoal.RightHand, targetPosition);
                GetAnimator().SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(cameraForward));

            }
        }

    }

}
