using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

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

    protected override void Start()
    {
        base.Start();

        // �c�e����������
        bulletStock = initBulletStock;

        // ���U����������
        magazine = initMagazine;
    }

    void FixedUpdate()
    {
        // �ˌ�����
        HandlShot();

        // �����[�h����
        if (GetInputSystem().GetButtonXPressed())
        {
            StartCoroutine(ReloadCoroutine());
        }

        // �v���C���[�̌����𒲐�
        float offsetAngle = 45f; // �I�t�Z�b�g�l�i�p�x�j��ݒ�

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
            // ���U����0�Ȃ烊���[�h
            if (isMagazine)
            {
                CreateBullet(burstfire);
                isShot = true;

                GetAnimator().SetBool("Shot", true);
            }
            else if (!GetAnimator().GetBool("Reload"))
            {
                isShot = true;
                StartCoroutine(ReloadCoroutine());
            }
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
        if (bulletStock < reload)
        {
            magazine = bulletStock;
            bulletStock = 0;
        }
        else
        {
            bulletStock -= (reload - magazine);
            magazine = reload;
        }
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
            magazine--;
            float hp = GetPlayerManager().GetHP();
            GetPlayerManager().SetHP(hp - -shotHp);
        }
    }

    // IKPass
    private void OnAnimatorIK(int layerIndex)
    {
        // �Ր�Ԑ��A�j���[�V�����i�e���\����j
        if (!GetAnimator().GetBool("Reload"))
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
