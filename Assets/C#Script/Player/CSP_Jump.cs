using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class CSP_Jump : ActionBase
{
    //// �d��
    //[System.Serializable]
    //public class StringNumberPair
    //{
    //    public string name;
    //    public float time;
    //}
    //[SerializeField, Header("���/�d������")]
    //public StringNumberPair[] pairs;

    // �W�����v
    [Header("�W�����v�ݒ�")]
    [SerializeField, Header("�W�����v��")]
    private float jumpForce = 5f;
    [SerializeField, Header("�}�E���g���̌����{��")]
    private float decelerationMount;
    [SerializeField, Header("�Q�쎞�̌����{��")]
    private float decelerationHunger;
    [SerializeField, Header("���i�W�����v�񐔂̏����l")]
    private int initJumpStock = 1;
    private int jumpStock;      // �c��̃W�����v��
    private bool isJump = false;// �W�����v��
    [Header("�ǔ���")]
    [SerializeField, Header("�ǂƂ̋���")]
    private float climbCheckDistance = 1f;
    [SerializeField, Header("�o���ǂ̍���")]
    private float climbMax = 2f;
    [SerializeField]
    private float climbMin = 0.25f;
    [SerializeField, Header("�o���ǂ̃��C���[")]
    private LayerMask climbLayer;
    [SerializeField, Header("�o�鑬���̔{��")]
    private float climbSpeed = 1f;

    // ���Ԍv���N���X
    private CS_Countdown countdown;

    protected override void Start()
    {
        base.Start();

        jumpStock = initJumpStock;

        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

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

        // �ړ�����
        if (countdown.IsCountdownFinished())
        {
            HandleJump();
        }

        //if (GetAnimator().GetBool("Mount"))
        //{
        //    HandleClimb();
        //}
    }

    //**
    //* �W�����v����
    //*
    //* in�F����
    //* out�F����
    //**
    void HandleJump()
    {
        bool isJumpStock = jumpStock > 0;

        // �ڒn����ƏՓˏ�ԂƃW�����v�{�^���̓��͂��`�F�b�N
        if (GetInputSystem().GetButtonAPressed() && !isJump && isJumpStock)
        {
            // ���ʉ����Đ�
            GetSoundEffect().PlaySoundEffect(1, 1);

            // �W�����v�͂�������
            float forceVal = jumpForce;

            if (GetAnimator().GetBool("Mount"))
            {
                forceVal *= decelerationMount;
            }
            if (GetAnimator().GetFloat("Hunger") == 1)
            {
                forceVal *= decelerationHunger;
            }

            GetRigidbody().AddForce(Vector3.up * forceVal, ForceMode.Impulse);

            // �A�j���[�^�[�̒l��ύX
            GetAnimator().SetBool("Jump", true);
            GetAnimator().SetFloat("UsuallyMove_Y", 0.5f);

            isJump = true;
        }

        if (!GetInputSystem().GetButtonAPressed() && isJump)
        {
            GetAnimator().SetBool("Jump", false);
            GetAnimator().SetFloat("UsuallyMove_Y", 0);

            isJump = false;
            jumpStock--;
        }

        if (GetPlayerManager().IsGrounded())
        {
            jumpStock = initJumpStock;
        }
    }

    //**
    //* �o�鏈��
    //*
    //* in�F����
    //* out�F����
    //**
    void HandleClimb()
    {
        if (IsClimb() && GetAnimator().GetBool("Dash"))
        {
            GetAnimator().SetBool("Climb", true);

            // ������ɗ͂�������
             float liftForce = 0.3f * climbSpeed;

            GetRigidbody().AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        }
        else
        {
            GetAnimator().SetBool("Climb", false);
        }

    }

    //**
    //* ���ʂɓo���ǂ����邩����
    //*
    //* in:����
    //* out:���茋��
    //**

    bool IsClimb()
    {
        bool isMax = false;
        bool isMin = false;

        RaycastHit hit;
        Vector3 offsetMax = new Vector3(0f, climbMax, 0f);

        // ���Raycast
        isMax = Physics.Raycast(transform.position + offsetMax, transform.forward, out hit, climbCheckDistance, climbLayer);

        Vector3 offsetMin = new Vector3(0f, climbMin, 0f);

        // ����Raycast
        isMin = Physics.Raycast(transform.position + offsetMin, transform.forward, out hit, climbCheckDistance, climbLayer);

        // �o��������Ԃ�
        return !isMax && isMin;
    }

}
