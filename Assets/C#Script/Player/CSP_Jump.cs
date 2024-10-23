using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CSP_Jump : ActionBase
{
    // ジャンプ
    [Header("ジャンプ設定")]
    [SerializeField, Header("ジャンプ力")]
    private float jumpForce = 5f;
    [SerializeField, Header("多段ジャンプ回数の初期値")]
    private int initJumpStock = 1;
    private int jumpStock;      // 残りのジャンプ回数
    private bool isJump = false;// ジャンプ中

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
    //* ジャンプ処理
    //*
    //* in：無し
    //* out：無し
    //**
    void HandleJump()
    {
        bool isJumpStock = jumpStock > 0;

        // 接地判定と衝突状態とジャンプボタンの入力をチェック
        if (GetInputSystem().GetButtonAPressed() && !isJump && isJumpStock)
        {
            // 効果音を再生
            GetSoundEffect().PlaySoundEffect(1, 1);

            // ジャンプ力を加える
            GetRigidbody().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // アニメーターの値を変更
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
    //* 登る処理
    //*
    //* in：無し
    //* out：無し
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
