using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class CSP_Jump : ActionBase
{
    //// 硬直
    //[System.Serializable]
    //public class StringNumberPair
    //{
    //    public string name;
    //    public float time;
    //}
    //[SerializeField, Header("状態/硬直時間")]
    //public StringNumberPair[] pairs;

    // ジャンプ
    [Header("ジャンプ設定")]
    [SerializeField, Header("ジャンプ力")]
    private float jumpForce = 5f;
    [SerializeField, Header("マウント時の減速倍率")]
    private float decelerationMount;
    [SerializeField, Header("飢餓時の減速倍率")]
    private float decelerationHunger;
    [SerializeField, Header("多段ジャンプ回数の初期値")]
    private int initJumpStock = 1;
    private int jumpStock;      // 残りのジャンプ回数
    private bool isJump = false;// ジャンプ中
    [Header("壁判定")]
    [SerializeField, Header("壁との距離")]
    private float climbCheckDistance = 1f;
    [SerializeField, Header("登れる壁の高さ")]
    private float climbMax = 2f;
    [SerializeField]
    private float climbMin = 0.25f;
    [SerializeField, Header("登れる壁のレイヤー")]
    private LayerMask climbLayer;
    [SerializeField, Header("登る速さの倍率")]
    private float climbSpeed = 1f;

    // 時間計測クラス
    private CS_Countdown countdown;

    protected override void Start()
    {
        base.Start();

        jumpStock = initJumpStock;

        // Countdownオブジェクトを生成
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

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

        // 移動処理
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

            // アニメーターの値を変更
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
    //* 登る処理
    //*
    //* in：無し
    //* out：無し
    //**
    void HandleClimb()
    {
        if (IsClimb() && GetAnimator().GetBool("Dash"))
        {
            GetAnimator().SetBool("Climb", true);

            // 上方向に力を加える
             float liftForce = 0.3f * climbSpeed;

            GetRigidbody().AddForce(Vector3.up * liftForce, ForceMode.Impulse);
        }
        else
        {
            GetAnimator().SetBool("Climb", false);
        }

    }

    //**
    //* 正面に登れる壁があるか判定
    //*
    //* in:無し
    //* out:判定結果
    //**

    bool IsClimb()
    {
        bool isMax = false;
        bool isMin = false;

        RaycastHit hit;
        Vector3 offsetMax = new Vector3(0f, climbMax, 0f);

        // 上のRaycast
        isMax = Physics.Raycast(transform.position + offsetMax, transform.forward, out hit, climbCheckDistance, climbLayer);

        Vector3 offsetMin = new Vector3(0f, climbMin, 0f);

        // 下のRaycast
        isMin = Physics.Raycast(transform.position + offsetMin, transform.forward, out hit, climbCheckDistance, climbLayer);

        // 登れる条件を返す
        return !isMax && isMin;
    }

}
