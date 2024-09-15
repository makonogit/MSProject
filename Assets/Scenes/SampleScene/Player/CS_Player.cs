using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

// プレイヤー操作
public class CS_Player : MonoBehaviour
{
    //**
    //* 変数
    //**

    // 外部オブジェクト
    public Transform cameraTransform;// メインカメラ

    // ジャンプ
    public float jumpForce = 5f;                // ジャンプ力
    public float groundCheckDistance = 0.1f;    // 地面との距離
    public string targetTag = "Ground";         // 地面タグ

    // 移動
    public float speed = 0.01f;// 移動速度
    private Vector3 moveVec;

    // 自身のコンポーネント
    private Rigidbody rb;
    private Animator animator;

    //**
    //* 初期化
    //**
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    //**
    //* 更新
    //**
    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    //**
    //* 移動処理
    //*
    //* in：無し
    //* out：無し
    //**
    void HandleMovement()
    {
        if (IsStickActive(-0.01f, 0.01f))
        {
            // スティックの入力を取得
            Vector3 moveVec = GetMovementVector();
            // 位置を更新
            MoveCharacter(moveVec);
            // 前方方向をスムーズに調整
            AdjustForwardDirection(moveVec);
            animator.SetBool("Move", true);
        }
        else
        {
            animator.SetBool("Move", false);
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
        if (IsGrounded() && Input.GetButtonDown("ButtonA"))
        {
            // ジャンプ力を加える
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("Jump", true);
        }
        else
        {
            animator.SetBool("Jump", false);
        }
    }

    //**
    //* 左スティックの入力をチェックする
    //*
    //* in：デッドゾーン
    //* out：入力判定
    //**
    bool IsStickActive(float min, float max)
    {
        float stickV = Input.GetAxis("LStick X");
        float stickH = Input.GetAxis("LStick Y");
        return !(((stickH < max) && (stickH > min)) && ((stickV < max) && (stickV > min)));
    }

    //**
    //* スティック入力から移動ベクトルを取得する
    //*
    //* in：無し
    //* out：移動ベクトル
    //**
    Vector3 GetMovementVector()
    {
        float stickH = Input.GetAxis("LStick X");
        float stickV = Input.GetAxis("LStick Y");
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        Vector3 moveVec = forward * stickV + right * stickH;
        moveVec.y = 0f; // y 軸の移動は不要
        return moveVec.normalized * speed;
    }

    //**
    //* キャラクターの位置を更新する
    //*
    //* in：移動ベクトル
    //* out：無し
    //**
    void MoveCharacter(Vector3 moveVec)
    {
        transform.position += moveVec;
    }

    //**
    //* キャラクターを進行方向に向ける
    //*
    //* in：移動ベクトル
    //* out：無し
    //**
    void AdjustForwardDirection(Vector3 moveVec)
    {
        if (moveVec.sqrMagnitude > 0)
        {
            Vector3 targetForward = moveVec.normalized;
            transform.forward = Vector3.Lerp(transform.forward, targetForward, 0.1f);
        }
    }

    //**
    //* 地面に接地しているかを判断する
    //*
    //* in：無し
    //* out：接地判定
    //**
    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            // ヒットしたオブジェクトのタグを確認する
            if (hit.collider.CompareTag(targetTag))
            {
                return true;
            }
        }

        return false;
    }

}