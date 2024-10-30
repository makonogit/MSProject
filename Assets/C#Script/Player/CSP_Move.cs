using System.Diagnostics;
using UnityEngine;

public class CSP_Move : ActionBase
{
    // 硬直
    [System.Serializable]
    public class StringNumberPair
    {
        public string name;
        public float time;
    }
    [SerializeField, Header("状態/硬直時間")]
    public StringNumberPair[] pairs;
    //[SerializeField, Header("状態/移動速度倍率")]
    //public StringNumberPair[] MoveSpeed;

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
    [SerializeField, Header("落下硬直の時間")]
    private float stunnedTime;

    // カウントダウン用クラス
    private CS_Countdown countdown;
    private CS_Countdown countdownDamage;

    // 衝突方向
    private Vector3 collisionNormal;
    [SerializeField, Header("ダメージ時のノックバック")]
    private float forceMagnitude = 0.5f;

    protected override void Start()
    {
        base.Start();

        // 移動速度の初期値を保存
        initSpeed = speed;

        // Countdownオブジェクトを生成
        countdown = gameObject.AddComponent<CS_Countdown>();
        countdownDamage = gameObject.AddComponent<CS_Countdown>();

    }

    void FixedUpdate()
    {
        // 硬直処理
        if (GetPlayerManager().GetStunned())
        {
            // カウントダウンをセット
            countdown.Initialize(stunnedTime);
            GetPlayerManager().SetStunned(false);
            StopMove();
        }


        foreach (var pair in pairs)
        {
            if (GetAnimator().GetBool(pair.name))
            {
                countdown.Initialize(pair.time);
                StopMove();
                break;
            }
        }

        // 移動処理
        if (countdown.IsCountdownFinished())
        {
            HandleMovement();
        }
        // 敵と衝突している場合、逆方向に力を加える
        if (GetAnimator().GetBool("Damage"))
        {
            Vector3 reverseForce = collisionNormal * forceMagnitude;
            reverseForce.y = 0f;
            if ((reverseForce.x == 0f) && (reverseForce.z == 0f))
            {
                reverseForce.z = -forceMagnitude;
            }

            //Vector3 offset = new Vector3(0, 0.25f, 0);
            GetRigidbody().AddForce(reverseForce, ForceMode.Impulse);

            if (countdownDamage.IsCountdownFinished())
                GetAnimator().SetBool("Damage", false);
        }
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

    void StopMove()
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
                StopMove();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            //GetSoundEffect().PlaySoundEffect(3, 7);

            GetAnimator().SetBool("Damage", true);

            // 衝突方向を保存
           collisionNormal = collision.contacts[0].normal;

            countdownDamage.Initialize(1);
        }
    }
}
