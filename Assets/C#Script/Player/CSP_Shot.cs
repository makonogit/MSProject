using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class CSP_Shot : ActionBase
{
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
    [SerializeField, Header("減らすHP")]
    private float shotHp;
    [SerializeField, Header("射程距離")]
    private float range = 100f;

    [Header("レティクル設定")]
    [SerializeField, Header("レティクルイメージ")]
    private Image detectionImage;
    [SerializeField, Header("アシスト範囲")]
    float radius = 1.0f;
    [SerializeField, Header("検出時の色")]
    private Color detectedColor = Color.green; // 検出時の色
    [SerializeField, Header("非検出時の色")]
    private Color notDetectedColor = Color.red; // 非検出時の色
    [SerializeField, Header("破壊可能オブジェクトのタグ")]
    private List<string> targetTag;
    private GameObject targetObject;// レティクル内の破壊可能オブジェクト

    // カウントダウン用クラス
    private CS_Countdown countdown;

    protected override void Start()
    {
        base.Start();

        // 残弾数を初期化
        bulletStock = initBulletStock;

        // 装填数を初期化
        magazine = initMagazine;

        // Countdownオブジェクトを生成
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    void FixedUpdate()
    {
        // レティクル処理
        HandlReticle();

        // 射撃処理
        if (countdown.IsCountdownFinished())
        {
            HandlShot();
        }

        // リロード処理
        //if (GetInputSystem().GetButtonXPressed())
        //{
        //    StartCoroutine(ReloadCoroutine());
        //}

        // プレイヤーの向きを調整
        float offsetAngle = 45f; // オフセット値

        // カメラの前方ベクトルを取得
        Vector3 cameraForward = Camera.main.transform.forward;

        // プレイヤーの正面ベクトルを取得
        Vector3 playerForward = transform.forward;

        // カメラの正面とプレイヤーの正面の角度を計算
        float angle = Vector3.Angle(cameraForward, playerForward);

        // オフセット値を考慮して確認
        if (angle > offsetAngle)
        {
            // プレイヤーをカメラの方向に向ける
            Vector3 targetDirection = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // 5fは回転速度を調整
        }

        // エイムアシスト
        bool isAssist = targetObject != null;
        GetPlayerManager().GetTpsCamera().SetAssist(isAssist);
    }

    //**
    //* レティクル
    //*
    //* in:無し
    //* out:無し
    //**
    void HandlReticle()
    {
        // レティクルとターゲットを初期化
        detectionImage.color = notDetectedColor;
        targetObject = null;

        // カメラ正面からレイを作成
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, range);

        // レティクル内に破壊可能オブジェクトがあるか判定
        foreach (RaycastHit hit in hits)
        {
            // ヒットした場合の処理
            if (targetTag.Contains(hit.collider.tag))
            {
                // オブジェクトを取得し、レティクルの色を変更
                detectionImage.color = detectedColor;
                targetObject = hit.collider.gameObject;
                break;
            }
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
        // 装填数判定
        bool isMagazine = magazine > 0;

        // コントローラー入力/発射中/装填数を判定
        if (GetInputSystem().GetRightTrigger() > 0 && !isShot)
        {
            CreateBullet(burstfire);
            isShot = true;

            GetAnimator().SetBool("Shot", true);

            //// 装填数が0ならリロード
            //if (isMagazine)
            //{
            //    CreateBullet(burstfire);
            //    isShot = true;

            //    GetAnimator().SetBool("Shot", true);
            //}
            //else if (!GetAnimator().GetBool("Reload"))
            //{
            //    isShot = true;
            //    //StartCoroutine(ReloadCoroutine());

            //    // アニメーションの終了まで待機
            //    countdown.Initialize(1f);
            //    // リロード処理
            //    ReloadMagazine(initMagazine);
            //    // アニメーション再生
            //}
        }
        else
        {
            GetAnimator().SetBool("Shot", false);
        }

        if (GetInputSystem().GetRightTrigger() <= 0 && isShot)
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
        magazine = reload;

        //if (bulletStock < reload)
        //{
        //    magazine = bulletStock;
        //    bulletStock = 0;
        //}
        //else
        //{
        //    bulletStock -= (reload - magazine);
        //    magazine = reload;
        //}
    }
    private IEnumerator ReloadCoroutine()
    {
        // リロードアニメーションを開始
        GetAnimator().SetBool("Reload", true);

        GetAnimator().SetTrigger("Reload");

        // アニメーションの長さを取得
        AnimationClip clip = GetAnimator().runtimeAnimatorController.animationClips[0];
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
        GetAnimator().SetBool("Reload", false);
    }

    //**
    //* 弾を生成する処理
    //*
    //* in:生成数
    //* out:無し
    //**
    void CreateBullet(int burst)
    {
        GetSoundEffect().PlaySoundEffect(1, 3);
      
        Vector3 forwardVec = GetPlayerManager().GetCameraTransform().forward;

        // レティクル内に破壊可能オブジェクトが存在するなら、その方向に発射する
        if (targetObject != null)
        {
            forwardVec = targetObject.transform.position - transform.position;
            forwardVec = forwardVec.normalized;
        }

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
            pos += forwardVec * (offsetDistance * (i + (burst - 1) / 2f) + 1f);

            ballobj.transform.position = pos;
            ballobj.transform.forward = forwardVec;

            // 装填数を減らす
            //magazine--;
            bulletStock--;
            float hp = GetPlayerManager().GetHP();
            GetPlayerManager().SetHP(hp - -shotHp);
        }
    }

    // IKPass
    private void OnAnimatorIK(int layerIndex)
    {
        // 臨戦態勢アニメーション（銃を構える）
        if ((!GetAnimator().GetBool("Reload")) && (!GetAnimator().GetBool("Throwing")
            && !GetAnimator().GetBool("Recovery") && !GetAnimator().GetBool("Use Item")
            && !GetAnimator().GetBool("Use EnergyCore")))
        {
            // カメラの正面方向の位置を計算
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 targetPosition = Camera.main.transform.position + cameraForward * 100;

            // 頭と腕をターゲットの方向に向ける
            if (targetPosition != null)
            {
                // 頭をターゲットに向ける
                GetAnimator().SetLookAtWeight(1f, 0.3f, 1f, 0f, 0.5f);
                GetAnimator().SetLookAtPosition(targetPosition);

                // 右腕をターゲットに向ける
                GetAnimator().SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                GetAnimator().SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
                GetAnimator().SetIKPosition(AvatarIKGoal.RightHand, targetPosition);
                GetAnimator().SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(cameraForward));

            }
        }
    }

}
