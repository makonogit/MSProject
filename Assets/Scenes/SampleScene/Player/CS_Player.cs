using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

//using System.Numerics;

//using System.Diagnostics;
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
    [Header("外部オブジェクト")]
    public Transform cameraTransform;// 追尾カメラ
    public CS_InputSystem inputSystem;// インプットマネージャー

    // ジャンプ
    [Header("ジャンプ設定")]
    public float jumpForce = 5f;                // ジャンプ力
    public float groundCheckDistance = 0.1f;    // 地面との距離
    public LayerMask targetLayer;               // ジャンプ可能なレイヤー

    // 移動
    [Header("移動設定")]
    public float speed = 1f;        // 移動速度
    public float targetSpeed = 10f; // 目標速度
    public float smoothTime = 0.5f; // 最高速度に到達するまでの時間
    private float velocity = 0f;    // 現在の速度
    private Vector3 moveVec;        // 現在の移動方向
    private float initSpeed;        // スピードの初期値を保存しておく変数

    // 再生する音声ファイルのリスト
    [Header("効果音設定")]
    public AudioSource[] audioSource;
    public AudioClip[] audioClips;

    // 自身のコンポーネント
    private Rigidbody rb;
    private Animator animator;

    [SerializeField, Header("空気砲の弾オブジェクト")]
    private GameObject AirBall;

    [SerializeField, Header("直刺しの注入間隔")]
    [Header("※攻撃力/注入間隔")]
    private const float Injection_Interval = 0.5f;

    //注入間隔計算用
    private float Injection_IntarvalTime = 0.0f;

    private bool HitBurstObjFlag = false;
    [SerializeField, Tooltip("直刺しぱわー")]
    private int InjectionPower = 1;

    private bool InjectionState = false;

    //弾けるオブジェクトのスクリプト
    CS_Burst_of_object csButstofObj;

    //**
    //* 初期化
    //**
    void Start()
    {
        // 移動速度の初期値を保存
        initSpeed = speed;

        // 自身のコンポーネントを取得
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    //**
    //* 更新
    //**
    void Update()
    {
        // ジャンプ処理
        HandleJump();
    }
    void FixedUpdate()
    {
        // 移動処理
        HandleMovement();
        
        AirInjection();
        AirGun();
    }

    //**
    //* 移動処理
    //*
    //* in：無し
    //* out：無し
    //**
    void HandleMovement()
    {
        // Lステックの入力と衝突状態をチェック
        if (inputSystem.IsLeftStickActive(0.1f))
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
            // 0番インデックスの効果音を停止
            StopPlayingSound(0);

            // 移動速度を初期化
            speed = initSpeed;

            // アニメーターの値を変更
            animator.SetBool("Move", false);
            animator.SetBool("Dash", false);
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
        // 接地判定と衝突状態とジャンプボタンの入力をチェック
        if (IsGrounded() && inputSystem.IsButtonATriggered())
        {
            // 効果音を再生
            PlaySoundEffect(1,1);

            // ジャンプ力を加える
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // アニメーターの値を変更
            animator.SetBool("Jump", true);
        }
        else
        {
            // アニメーターの値を変更
            animator.SetBool("Jump", false);
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
        Vector2 stick = inputSystem.GetLeftStick();
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
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
        // Lトリガーの入力中は加速する
        float tri = inputSystem.GetRightTrigger();
        if (tri > 0)
        {
            speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocity, smoothTime);

            animator.SetBool("Dash", true);
        }

        // 効果音を再生する
        if (animator.GetBool("Dash"))
        {
            // ダッシュ
            PlaySoundEffect(0, 2);
        }
        else if (animator.GetBool("Move"))
        {
            // 移動
            PlaySoundEffect(0, 0);
        }

        // プレイヤーの位置を更新
        Vector3 direction = moveVec * speed * Time.deltaTime;
        rb.MovePosition(rb.position + direction);
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
        return Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f, targetLayer);
    }

    //**
    //* 音声ファイルが再生可能かチェックする
    //*
    //* in：再生する音声ファイルのインデックス
    //* out:再生可能かチェック
    //**
    bool CanPlaySound(int indexSource, int indexClip)
    {
        return audioSource[indexSource] != null
               && audioClips != null
               && indexClip >= 0
               && indexClip < audioClips.Length
               && (!audioSource[indexSource].isPlaying || audioSource[indexSource].clip != audioClips[indexClip]);
    }

    //**
    //* 音声ファイルを再生する
    //*
    //* in：再生する音声ファイルのインデックス
    //* out:無し
    //**
    void PlaySoundEffect(int indexSource,int indexClip)
    {
        // サウンドエフェクトを再生
        if (CanPlaySound(indexSource, indexClip))
        {
            audioSource[indexSource].clip = audioClips[indexClip];
            audioSource[indexSource].Play();
        }
    }

    //**
    //* 音声ファイルを停止する
    //*
    //* in：無し
    //* out:無し
    //**
    void StopPlayingSound(int indexSource)
    {
        // サウンドエフェクトを停止
        if (audioSource[indexSource].isPlaying)
        {
            audioSource[indexSource].Stop();
        }
    }

    bool IsPlayingSound(int indexSource)
    {
        return audioSource[indexSource].isPlaying;
    }
    //----------------------------
    // 空気砲関数
    // 引数:入力キー,オブジェクトに近づいているか
    // 戻り値：なし
    //----------------------------
    void AirGun()
    {
        //発射可能か(キーが押された瞬間&オブジェクトに近づいていない)
        bool StartShooting = inputSystem.IsButtonXTriggered() && (!HitBurstObjFlag || !csButstofObj);

        if (!StartShooting) { return; }

        //SEが再生されていたら止める
        if (IsPlayingSound(1)) { StopPlayingSound(1); }

        PlaySoundEffect(1, 3);

        Vector3 forwardVec  = cameraTransform.forward;

        //入力があれば弾を生成
        //ポインタの位置から　Instantiate(AirBall,transform.pointa);
        GameObject ballobj = Instantiate(AirBall);

        Vector3 pos = Vector3.zero;
        float scaler = 2.0f;
        Vector3 offset = new Vector3(0, 1, 0);

        pos = this.transform.position;
        pos += offset;
        pos += forwardVec * scaler;
        ballobj.transform.position = pos;
        ballobj.transform.forward = forwardVec;

    }

    //----------------------------
    // 直刺し(空気注入)関数
    // 引数:入力キー,近づいているか,近づいているオブジェクトの圧力,近づいているオブジェクトの耐久値
    // 戻り値：なし
    //----------------------------
    void AirInjection()
    {
        //注入可能か(キーが入力されていてオブジェクトに近づいている)
        bool Injection = inputSystem.IsButtonBPressed() && HitBurstObjFlag && csButstofObj;

        if (Injection)
        {
            StopPlayingSound(1);    //音が鳴っていたら止める
            PlaySoundEffect(1, 4);  //挿入SE
            InjectionState = true;  //注入中のフラグをOn
        }

        //注入中じゃなければ終了
        if (!InjectionState)
        {
            return;
        }

        //時間計測
        Injection_IntarvalTime += Time.deltaTime;
        bool TimeProgress = Injection_IntarvalTime > Injection_Interval;   //注入間隔分時間経過しているか
        if (!TimeProgress) { return; }

        Injection_IntarvalTime = 0.0f;  //時間をリセット

        if (!csButstofObj)
        {
            //Debug.LogWarning("null");
            return;
        }

        PlaySoundEffect(1, 6);  //挿入SE

        //圧力が最大になったら or ボタンを離したら
        bool MaxPressure = !inputSystem.IsButtonBPressed() || csButstofObj.AddPressure(InjectionPower);

        //最大になったら注入終了
        if (MaxPressure)
        {
            StopPlayingSound(1);
            PlaySoundEffect(1, 5);
            InjectionState = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        bool isHitBurstObj = collision.gameObject.tag == "Burst";
        if (isHitBurstObj)
        {
            csButstofObj = collision.transform.GetComponent<CS_Burst_of_object>();
            HitBurstObjFlag = true;
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        bool isHitBurstObj = collision.gameObject.tag == "Burst";
        if (isHitBurstObj) 
        {
            csButstofObj = null;
            HitBurstObjFlag = false; 
            return; 
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // 接触点が存在するか確認
        if (collision.contactCount > 0)
        {
            Vector3 collisionNormal = collision.contacts[0].normal;

            // 衝突が水平面かどうかチェック
            if (Mathf.Abs(collisionNormal.y) < 0.1f)
            {
                // 0番インデックスの効果音を停止
                StopPlayingSound(0);

                // 移動速度を初期化
                speed = initSpeed;

                // アニメーターの値を変更
                animator.SetBool("Move", false);
                animator.SetBool("Dash", false);
            }
        }
    }

}