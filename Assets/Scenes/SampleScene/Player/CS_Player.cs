using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public Transform cameraTransform;// メインカメラ

    // ジャンプ
    public float jumpForce = 5f;                // ジャンプ力
    public float groundCheckDistance = 0.1f;    // 地面との距離
    public string targetTag = "Ground";         // 地面タグ

    // 移動
    public float speed = 1f;     // 移動速度
    public float targetSpeed = 10f; // 目標速度
    public float smoothTime = 0.5f; // 最高速度に到達するまでの時間
    private float velocity = 0f;    // 現在の速度
    private Vector3 moveVec;        // 現在の移動方向

    // 自身のコンポーネント
    private Rigidbody rb;
    private Animator animator;
    public AudioSource[] audioSource;

    // 再生する音声ファイルのリスト
    public AudioClip[] audioClips;

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



    //弾けるオブジェクトのスクリプト
    CS_Burst_of_object csButstofObj;

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

        
        AirInjection("ButtonB",HitBurstObjFlag);
        AirGun("ButtonX",HitBurstObjFlag);
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
            StopPlayingSound(0);

            speed = 1f;

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
        if (IsGrounded() && Input.GetButtonDown("ButtonA"))
        {
            PlaySoundEffect(1,1);

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
        float tri = Input.GetAxis("LRTrigger");
        if (tri > 0)
        {
            PlaySoundEffect(0, 2);
            speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocity, smoothTime);

            animator.SetBool("Dash", true);
        }
        else
        {
            PlaySoundEffect(0, 0);
        }

        // プレイヤーの位置を更新
        transform.position += moveVec * speed * Time.deltaTime;
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

    //**
    //* 音声ファイルを再生する
    //*
    //* in：再生する音声ファイルのインデックス
    //* out:無し
    //**
    void PlaySoundEffect(int indexSource,int indexClip)
    {
        // サウンドエフェクトを再生
        if (audioSource[indexSource] != null && audioClips != null && indexClip >= 0 && indexClip < audioClips.Length)
        {
            if (!audioSource[indexSource].isPlaying || audioSource[indexSource].clip != audioClips[indexClip])
            {
                audioSource[indexSource].clip = audioClips[indexClip];

                audioSource[indexSource].Play();
            }
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

    //----------------------------
    // 空気砲関数
    // 引数:入力キー,オブジェクトに近づいているか
    // 戻り値：なし
    //----------------------------
    void AirGun(string button, bool ObjDistance)
    {
        //発射可能か(キーが押された瞬間&オブジェクトに近づいていない)
        bool StartShooting = Input.GetButtonDown(button) && !ObjDistance;

        if (!StartShooting) { return; }

        Vector3 forwardVec = transform.forward;

        //入力があれば弾を生成
        //ポインタの位置から　Instantiate(AirBall,transform.pointa);
        GameObject ballobj = Instantiate(AirBall);

        Vector3 pos = Vector3.zero;
        float scaler =2.0f;
        Vector3 offset = new Vector3(0,1,0);

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
    void AirInjection(string button, bool ObjDistance)
    {
        //注入可能か(キーが入力されていてオブジェクトに近づいている)
        bool Injection = Input.GetButton(button) && ObjDistance;

        if (!Injection) { return; }
        
        //時間計測
        Injection_IntarvalTime += Time.deltaTime;
        bool TimeProgress = Injection_IntarvalTime > Injection_Interval;   //注入間隔分時間経過しているか
        if (!TimeProgress) { return; }

        Injection_IntarvalTime = 0.0f;  //時間をリセット
                                        // bool StartInjection = ObjPressure > MaxPressure;                   //攻撃開始か(圧力が最大か)



        if (!csButstofObj) 
        { 
            Debug.LogWarning("null");
            return; 
        }
        
        csButstofObj.AddPressure(InjectionPower);

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

}