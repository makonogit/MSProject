using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSP_Move : ActionBase
{
    // 移動
    [Header("移動設定")]
    [SerializeField, Header("移動速度")]
    private float speed = 1f;        // 移動速度
    [SerializeField, Header("目標速度")]
    private float targetSpeed = 10f; // 目標速度
    [SerializeField, Header("最高速度に到達するまでの時間")]
    private float smoothTime = 0.5f; // 最高速度に到達するまでの時間
    private float velocity = 0f;     // 現在の速度
    private Vector3 moveVec;         // 現在の移動方向
    private float initSpeed;         // スピードの初期値を保存しておく変数
    [SerializeField, Header("ダッシュ入力の閾値")]
    private float dashInputValue = 0.75f;
    [SerializeField, Header("マウント時の減速倍率")]
    private float decelerationMount;
    [SerializeField, Header("飢餓時の減速倍率")]
    private float decelerationHunger;

    protected override void Start()
    {
        base.Start();

        // 移動速度の初期値を保存
        initSpeed = speed;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    //**
    //* 移動処理
    //*
    //* in：無し
    //* out：無し
    //**
    void HandleMovement()
    {
        // Lステックの入力をチェック
        if (GetInputSystem().GetLeftStickActive())
        {
            // スティックの入力を取得
            Vector3 moveVec = GetMovementVector();

            // 位置を更新
            MoveCharacter(moveVec);

            // 前方方向をスムーズに調整
            AdjustForwardDirection(moveVec);
            GetAnimator().SetBool("Move", true);
        }
        else
        {
            // 0番インデックスの効果音を停止
            GetSoundEffect().StopPlayingSound(0);

            // 移動速度を初期化
            speed = initSpeed;
           
            // アニメーターの値を変更
            GetAnimator().SetBool("Move", false);
            GetAnimator().SetBool("Dash", false);
        }
    }

    //**
    //* スティック入力からカメラから見た移動ベクトルを取得する
    //*
    //* in：無し
    //* out：移動ベクトル
    //**
    Vector3 GetMovementVector()
    {
        Vector2 stick = GetInputSystem().GetLeftStick();
        Vector3 forward = GetPlayerManager().GetCameraTransform().forward;
        Vector3 right = GetPlayerManager().GetCameraTransform().right;
        Vector3 moveVec = forward * stick.y + right * stick.x;
        moveVec.y = 0f; // y 軸の移動は不要
        return moveVec.normalized;
    }

    //**
    //* キャラクターの位置を更新する
    //*
    //* in：移動ベクトル
    //* out：無し
    //**
    void MoveCharacter(Vector3 moveVec)
    {
        // スティックが大きく倒されていれば、加速する
        Vector2 stick = GetInputSystem().GetLeftStick();
        if ((stick.y > dashInputValue) || (stick.x > dashInputValue) 
            || (stick.y < -dashInputValue) || (stick.x < -dashInputValue))
        {
            speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocity, smoothTime);
            GetAnimator().SetBool("Dash", true);

        }

        // 効果音を再生する
        if (GetAnimator().GetBool("Dash"))
        {
            // ダッシュ
            GetSoundEffect().PlaySoundEffect(0, 2);
        }
        else if (GetAnimator().GetBool("Move"))
        {
            // 移動
            GetSoundEffect().PlaySoundEffect(0, 0);
        }

        // プレイヤーの位置を更新
        Vector3 direction = moveVec * speed * Time.deltaTime;

        if (GetAnimator().GetBool("Mount"))
        {
            direction *= decelerationMount;
        }
        if (GetAnimator().GetFloat("Hunger") == 1)
        {
            direction *= decelerationHunger;
        }

        GetRigidbody().MovePosition(GetRigidbody().position + direction);

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

    private void OnCollisionStay(Collision collision)
    {
        //**
        //* めり込み防止
        //**

        // 垂直な壁と衝突した場合に移動状態を停止し、加えた移動量を0にする
        if (collision.contactCount > 0)
        {
            Vector3 collisionNormal = collision.contacts[0].normal;

            if (Mathf.Abs(collisionNormal.y) < 0.1f)
            {
                // 0番インデックスの効果音を停止
                GetSoundEffect().StopPlayingSound(0);

                // 移動速度を初期化
                speed = initSpeed;

                // アニメーターの値を変更
                GetAnimator().SetBool("Move", false);
                GetAnimator().SetBool("Dash", false);

                // 平行な移動成分を取り除く
                Vector3 currentVelocity = GetRigidbody().velocity;
                GetRigidbody().velocity = new Vector3(0f, currentVelocity.y, 0f);

            }
        }
    }
}
