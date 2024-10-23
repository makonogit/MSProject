using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CSP_Jump : ActionBase
{
    // �W�����v
    [Header("�W�����v�ݒ�")]
    [SerializeField, Header("�W�����v��")]
    private float jumpForce = 5f;
    [SerializeField, Header("���i�W�����v�񐔂̏����l")]
    private int initJumpStock = 1;
    private int jumpStock;      // �c��̃W�����v��
    private bool isJump = false;// �W�����v��

    protected override void Start()
    {
        base.Start();

        jumpStock = initJumpStock;
    }

    void FixedUpdate()
    {
        HandleJump();

        if ((GetPlayerManager().IsWall())&&(GetAnimator().GetBool("Mount")))
        {
            jumpStock = initJumpStock;

            GetAnimator().SetBool("Sticky", true);

            if (GetRigidbody().velocity.y < 0)
            {
                GetRigidbody().velocity = new Vector3(GetRigidbody().velocity.x, 0, GetRigidbody().velocity.z);
            }
        }
        else
        {
            GetAnimator().SetBool("Sticky", false);
        }
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
            GetRigidbody().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // �A�j���[�^�[�̒l��ύX
            GetAnimator().SetBool("Jump", true);
            isJump = true;
        }

        if (!GetInputSystem().GetButtonAPressed() && isJump)
        {
            GetAnimator().SetBool("Jump", false);
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
        if (GetPlayerManager().IsWall())
        {
            GetAnimator().SetBool("Sticky", true);
        }
        else
        {
            GetAnimator().SetBool("Sticky", false);
        }
    }
}
