using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Cinemachine;

//using System.Numerics;

//using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

// プレイヤー操作
public class CS_PrPhase2 : MonoBehaviour
{
    //**
    //* 変数
    //**

    // 外部オブジェクト
    [Header("外部オブジェクト")]
    [SerializeField, Header("メインカメラ")]
    private Transform cameraTransform;
    [SerializeField, Header("インプットマネージャー")]
    private CS_InputSystem inputSystem;// インプットマネージャー
    [SerializeField, Header("カメラマネージャー")]
    private CS_CameraManager cameraManager;// カメラマネージャー
    [SerializeField, Header("ロックオンカメラ")]
    private CS_TargetCamera targetCamera;// ロックオンカメラ
    [SerializeField, Header("エイムカメラ")]
    private CS_Aim aimCamera;// エイムカメラ
    [SerializeField, Header("追尾カメラ")]
    private CS_TpsCamera tpsCamera;// tpsカメラ

    [Header("プレイヤー設定")]
    [SerializeField, Header("体力値")]
    private int hitPoints = 100;

    // ジャンプ
    [Header("ジャンプ設定")]
    [SerializeField, Header("ジャンプ力")]
    private float jumpForce = 5f;                // ジャンプ力
    [SerializeField, Header("地面との距離")]
    private float groundCheckDistance = 0.1f;    // 地面との距離
    [SerializeField, Header("ジャンプ可能なレイヤー")]
    private LayerMask targetLayer;               // ジャンプ可能なレイヤー
    [SerializeField, Header("多段ジャンプ回数の初期値")]
    private int initJumpStock = 1;// 多段ジャンプ回数
    private int jumpStock;
    private bool isJump = false;// ジャンプ中

    // 移動
    [Header("移動設定")]
    [SerializeField, Header("移動速度")]
    private float speed = 1f;        // 移動速度
    [SerializeField, Header("目標速度")]
    private float targetSpeed = 10f; // 目標速度
    [SerializeField, Header("最高速度に到達するまでの時間")]
    private float smoothTime = 0.5f; // 最高速度に到達するまでの時間
    private float velocity = 0f;    // 現在の速度
    private Vector3 moveVec;        // 現在の移動方向
    private float initSpeed;        // スピードの初期値を保存しておく変数

    // 再生する音声ファイルのリスト
    [Header("効果音設定")]
    [SerializeField, Header("オーディオソース")]
    private AudioSource[] audioSource;
    [SerializeField, Header("音声ファイル")]
    private AudioClip[] audioClips;

    // 自身のコンポーネント
    private Rigidbody rb;
    private Animator animator;

    [Header("射撃設定")]
    [SerializeField, Header("空気砲の弾オブジェクト")]
    private GameObject AirBall;// 弾
    [SerializeField, Header("装填数の初期値")]
    private int initMagazine;// 装填数の初期値
    [SerializeField, Header("現在の装填数")]
    private int magazine;// 装填数
    [SerializeField, Header("連射数")]
    private int burstfire;// 連射数
    [SerializeField, Header("残弾数の初期値")]
    private int initBulletStock;
    [SerializeField, Header("現在の残弾数")]
    private int bulletStock;// 残弾数
    private bool isShot = false;// 射撃中


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
        jumpStock = initJumpStock;

        // 残弾数を初期化
        bulletStock = initBulletStock;

        // 装填数を初期化
        magazine = initMagazine;

        // 移動速度の初期値を保存
        initSpeed = speed;

        // 自身のコンポーネントを取得
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    //**
    //* 更新
    //**

    void FixedUpdate()
    {
        // カメラを初期化
        cameraManager.SwitchingCamera(0);

        // 移動処理
        HandleMovement();
        // ジャンプ処理
        HandleJump();
        // エイム処理
        HandleAim();
        // 射撃処理
        HandlShot();
        // スライディング
        HandlSliding();

        AirInjection();
        //AirGun();
    }

    //**
    //* ゲッター
    //**
    public int GetHitPoints()
    {
        return hitPoints;
    }

    //**
    //* スライディング処理
    //*
    //* in:無し
    //* out:無し
    //**
    void HandlSliding()
    {
        if (inputSystem.GetLeftStickPush())
        {
            animator.SetBool("Sliding", true);
        }
        else
        {
            animator.SetBool("Sliding", false);
        }
    }

    //**
    //* 射撃処理
    //*
    //* in:無し
    //* out:無し
    //**
    void HandlShot()
    {
        // 近接判定
        bool isAirInjection = !HitBurstObjFlag || !csButstofObj;
        // 装填数判定
        bool isMagazine = magazine > 0;

        // コントローラー入力/発射中/近接/装填数を判定
        if (inputSystem.GetButtonXPressed() && !isShot && isAirInjection && animator.GetBool("Aim"))
        {
            // 装填数が0ならリロード
            if (isMagazine)
            {
                CreateBullet(burstfire);
                isShot = true;

                animator.SetBool("Shot", true);
            }
            else if (!animator.GetBool("Reload"))
            {
                isShot = true;
                StartCoroutine(ReloadCoroutine());
            }
        }
        else
        {
            animator.SetBool("Shot", false);
        }

        if (!inputSystem.GetButtonXPressed() && isShot)
        {
            isShot = false;
        }
    }

    //**
    //* リロード処理
    //*
    //* in:リロード数
    //* out:無し
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
            magazine = reload;
            bulletStock -= reload;
        }
    }
    private IEnumerator ReloadCoroutine()
    {
        // リロードアニメーションを開始
        animator.SetBool("Reload", true);

        animator.SetTrigger("Reload");

        // アニメーションの長さを取得
        AnimationClip clip = animator.runtimeAnimatorController.animationClips[0];
        float animationLength = clip.length;

        // アニメーションが再生中かどうかを確認
        float elapsedTime = 0f;
        while (elapsedTime < animationLength)
        {
            elapsedTime += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }

        // リロード処理を行う
        ReloadMagazine(initMagazine);
        animator.SetBool("Reload", false);
    }

    //**
    //* 弾を生成する処理
    //*
    //* in:生成数
    //* out:無し
    //**
    void CreateBullet(int burst)
    {
        PlaySoundEffect(1, 3);

        Vector3 forwardVec = aimCamera.transform.forward;

        float offsetDistance = 1.5f; // 各弾の間隔

        if (burst > magazine)
        {
            burst = magazine;
        }

        for (int i = 0; i < burst; i++)
        {
            // 弾を生成
            GameObject ballobj = Instantiate(AirBall);

            Vector3 pos = this.transform.position;
            Vector3 offset = new Vector3(0, 1, 0);

            pos += offset;
            pos += forwardVec * (offsetDistance * (i + (burst - 1) / 2f));

            ballobj.transform.position = pos;
            ballobj.transform.forward = forwardVec;

            // 装填数を減らす
            magazine--;

            aimCamera.TriggerRecoil();
        }
    }



    //**
    //* 照準処理
    //*
    //* in:無し
    //* out:無し
    //**
    void HandleAim()
    {
        // Lトリガーの入力がある場合、ターゲットカメラに切り替える
        if (inputSystem.GetLeftTrigger() > 0.1f)
        {
            if (animator.GetBool("Aim") == false)
            {
                // 追尾カメラの方向に向ける
                Vector3 normalizedDirection = cameraTransform.forward;
                float yRotation = Mathf.Atan2(normalizedDirection.x, normalizedDirection.z) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, yRotation, 0);
                //transform.rotation = rotation;

                // 回転にオフセットを適用
                float offsetAngle = 45f;
                Quaternion offsetRotation = Quaternion.Euler(0, offsetAngle, 0);
                transform.rotation = rotation * offsetRotation;
            }

            cameraManager.SwitchingCamera(2);
            animator.SetBool("Aim", true);

            // 移動速度を初期化
            speed = initSpeed;

            // アニメーターの値を変更
            animator.SetBool("Dash", false);
        }
        else
        {
            if (animator.GetBool("Aim") == true)
            {
                tpsCamera.CameraReset();
            }

            animator.SetBool("Aim", false);
        }
    }

    //**
    //* ロックオン処理
    //*
    //* in:無し
    //* out:無し
    //**
    void HandleTarget()
    {
        // ターゲット中のオブジェクトを取得
        GameObject closest = targetCamera.GetClosest();

        // Lトリガーの入力がある場合、ターゲットカメラに切り替える
        if ((inputSystem.GetLeftTrigger() > 0.1f) && (closest != null))
        {
            cameraManager.SwitchingCamera(1);
            animator.SetBool("LockOn", true);
        }
        else
        {
            animator.SetBool("LockOn", false);

        }
    }
    // IK Pass
    private void OnAnimatorIK(int layerIndex)
    {
        if (animator.GetBool("Aim"))
        {
            // 頭をターゲットに向ける
            animator.SetLookAtWeight(1f, 0.3f, 1f, 0f, 0.5f);
            animator.SetLookAtPosition(aimCamera.transform.forward * 100f);
        }

        if (!animator.GetBool("LockOn")) return;

        // ターゲット中のオブジェクトを取得
        GameObject closest = targetCamera.GetClosest();

        // 頭と腕をターゲットの方向に向ける
        if (closest != null)
        {
            // 頭をターゲットに向ける
            animator.SetLookAtWeight(1f, 0.3f, 1f, 0f, 0.5f);
            animator.SetLookAtPosition(closest.transform.position);

            // 右腕をターゲットに向ける
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, closest.transform.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, closest.transform.rotation);
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
        if (inputSystem.GetLeftStickActive())
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
        bool isJumpStock = jumpStock > 0;

        // 接地判定と衝突状態とジャンプボタンの入力をチェック
        //if (IsGrounded() && inputSystem.GetButtonAPressed() && !animator.GetBool("Jump") && !isJump)
        if (inputSystem.GetButtonAPressed() && !isJump && isJumpStock)
        {
            // 効果音を再生
            PlaySoundEffect(1, 1);

            // ジャンプ力を加える
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // アニメーターの値を変更
            animator.SetBool("Jump", true);

            isJump = true;
        }
        else
        {
            // アニメーターの値を変更
            animator.SetBool("Jump", false);
        }

        if (!inputSystem.GetButtonAPressed() && isJump)
        {
            isJump = false;
            jumpStock--;
        }

        if (IsGrounded())
        {
            jumpStock = initJumpStock;
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
        // Aim時
        if (animator.GetBool("Aim"))
        {
            forward = aimCamera.transform.forward;// Aimカメラを参照
            stick = RoundDirection(stick);        // スティック入力を平行移動に変換
        }
        Vector3 right = cameraTransform.right;
        Vector3 moveVec = forward * stick.y + right * stick.x;
        moveVec.y = 0f; // y 軸の移動は不要
        return moveVec.normalized;
    }

    Vector2 RoundDirection(Vector2 input)
    {
        // x軸とy軸の絶対値が大きい方を優先して方向を決定
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return input.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return input.y > 0 ? Vector2.up : Vector2.down;
        }
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
        if ((tri > 0) && (animator.GetBool("Aim") == false))
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
        if (animator.GetBool("Aim")) return;

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
        return Physics.Raycast(transform.position, Vector3.down, out hit, 0.01f, targetLayer);
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
    void PlaySoundEffect(int indexSource, int indexClip)
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
        animator.SetBool("Shot", false);

        //発射可能か(キーが押された瞬間&オブジェクトに近づいていない)
        bool StartShooting = inputSystem.GetButtonXTriggered() && (!HitBurstObjFlag || !csButstofObj);

        if (!StartShooting) { return; }
        if (!animator.GetBool("Aim")) { return; }

        //SEが再生されていたら止める
        if (IsPlayingSound(1)) { StopPlayingSound(1); }

        PlaySoundEffect(1, 3);

        Vector3 forwardVec = aimCamera.transform.forward;//cameraTransform.forward;

        //入力があれば弾を生成
        //ポインタの位置から　Instantiate(AirBall,transform.pointa);
        GameObject ballobj = Instantiate(AirBall);

        Vector3 pos = Vector3.zero;
        float scaler = 0.5f;
        Vector3 offset = new Vector3(0, 1, 0);

        pos = this.transform.position;
        pos += offset;
        pos += forwardVec * (scaler * 2f);
        ballobj.transform.position = pos;
        ballobj.transform.forward = forwardVec;

        aimCamera.TriggerRecoil();
        animator.SetBool("Shot", true);
    }

    //----------------------------
    // 直刺し(空気注入)関数
    // 引数:入力キー,近づいているか,近づいているオブジェクトの圧力,近づいているオブジェクトの耐久値
    // 戻り値：なし
    //----------------------------
    void AirInjection()
    {
        //注入可能か(キーが入力されていてオブジェクトに近づいている)
        bool Injection = inputSystem.GetButtonBPressed() && HitBurstObjFlag && csButstofObj;

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
        bool MaxPressure = !inputSystem.GetButtonBPressed() || csButstofObj.AddPressure(InjectionPower);

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
        //**
        //* めり込み防止
        //**

        // 垂直な壁と衝突した場合に移動状態を停止し、加えた移動量を0にする
        if ((collision.contactCount > 0) && (animator != null))
        {
            Vector3 collisionNormal = collision.contacts[0].normal;

            if (Mathf.Abs(collisionNormal.y) < 0.1f)
            {
                // 0番インデックスの効果音を停止
                StopPlayingSound(0);

                // 移動速度を初期化
                speed = initSpeed;

                // アニメーターの値を変更
                animator.SetBool("Move", false);
                animator.SetBool("Dash", false);

                // 平行な移動成分を取り除く
                Vector3 currentVelocity = rb.velocity;
                rb.velocity = new Vector3(0f, currentVelocity.y, 0f);

            }
        }
    }

}