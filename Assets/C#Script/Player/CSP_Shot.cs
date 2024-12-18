using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;

public class CSP_Shot : ActionBase
{
    //[Header("表示項目")]
    //[SerializeField, Header("待機中の武器")]
    //private GameObject weaponIdle;
    //[SerializeField, Header("射撃中の武器")]
    //private GameObject weaponShot;

    [Header("射撃設定")]
    [SerializeField, Header("空気砲の弾オブジェクト")]
    private GameObject AirBall;// 弾
    [SerializeField, Header("装填数の初期値")]
    private int initMagazine;// 装填数の初期値 
    [SerializeField, Header("現在の装填数")]
    private int magazine;// 装填数
    //[SerializeField, Header("1トリガーの連射数")]
    private int burstfire = 1;// 連射数
    //[SerializeField, Header("残弾数の初期値")]
    private int initBulletStock;
    //[SerializeField, Header("現在の残弾数")]
    private int bulletStock;// 残弾数
    private bool isShot = false;// 射撃中
    //[SerializeField, Header("減らすHP")]
    private float shotHp;
    [SerializeField, Header("射程距離")]
    private float range = 100f;
    [SerializeField, Header("散弾範囲")]
    private float scatter = 0.1f;
    [SerializeField, Header("オートエイム有効")]
    private bool isAuto;
    public void SetAuto(bool flg) { isAuto = flg; }
    public bool GetAuto() => isAuto;
    [SerializeField, Header("フルオートの発射間隔")]
    private float interval = 0.5f;
    [SerializeField, Header("リロードにかかる時間")]
    private float intervalReload = 3f;

    [Header("レティクル設定")]
    [SerializeField, Header("表示位置")]
    private UnityEngine.UI.Image detectionImage;
    private Vector2 defaultSize;
    //[SerializeField, Header("レティクル変更の検出範囲")]
    //float radius = 1.0f;
    //[SerializeField, Header("オートエイムレティクル")]
    private UnityEngine.UI.Image autoaim;
    //[SerializeField, Header("検出時のレティクル設定")]
    private Sprite detectedSprite;
    //[SerializeField]
    //private Color detectedColor = Color.green; // 検出時の色
    [SerializeField, Header("通常時のレティクル")]
    private Sprite reticle;
    [SerializeField, Header("冷却中のレティクル")]
    private Sprite ColdREticle;
    //[SerializeField, Header("非検出時のレティクル設定")]
    private Sprite notDetectedSprite; 
    //[SerializeField]
    //private Color notDetectedColor = Color.red; // 非検出時の色
    [SerializeField, Header("オートエイム対象のタグ")]
    private List<string> targetTag;
    private GameObject targetObject;// レティクル内の破壊可能オブジェクト
    [SerializeField, Header("オーバーヒートゲージ")]
    private UnityEngine.UI.Image overheat;
    [SerializeField, Header("オーバーヒート中ゲージ")]
    private UnityEngine.UI.Image overheatOut;
    [SerializeField, Header("オーバーヒート中テキスト")]
    private TextMeshProUGUI overheatText;
    //[SerializeField, Header("ロックオンレティクル")]
    private UnityEngine.UI.Image lockon;
    private RectTransform rectTransform;

    [Header("オートエイム設定")]
    [SerializeField, Header("オートエイム効果時間")]
    private float autoTime = 10f;
    [SerializeField, Header("判定間隔")]
    private float autoaimCheckInterval = 0.5f;
    private float nextAutoaimCheckTime = 0;
    [SerializeField, Header("カメラターゲット")]
    private GameObject cameraTarget; 
    [SerializeField, Header("エイムターゲット")]
    private GameObject aimTarget;
    [SerializeField, Header("検知範囲")]
    float radiusAuto = 10f;

    [Header("攻撃時の振動設定")]
    [SerializeField, Header("振動の長さ")]
    private float duration = 0.5f;         // 振動の長さ
    [SerializeField, Header("振動の強さ")]
    private int powerType = 1;          // 振動の強さ（4段階）
    [SerializeField, Header("振動の周波数")]
    private AnimationCurve curveType;          // 振動の周波数
    [SerializeField, Header("繰り返し回数")]
    private int repetition = 1;         // 繰り返し回数

    private bool Cold = false;  //冷却中かどうか
    private float OverHeatAlpha = 0f;    //オーバーヒート中の画像の透明度(徐々に赤くする)

    // カウントダウン用クラス
    private CS_Countdown countdown;
    private CS_Countdown intervalCountdown;
    private CS_Countdown autoaimCountdown;

    protected override void Start()
    {
        base.Start();

        // オーバーヒートテキストを非表示
        overheatText.enabled = false;
        // オーバーヒートゲージを非表示
        overheat.enabled = false;
        overheatOut.enabled = false;

        // 残弾数を初期化
        bulletStock = initBulletStock;

        // 装填数を初期化
        magazine = initMagazine;

        // Countdownオブジェクトを生成
        countdown = gameObject.AddComponent<CS_Countdown>();
        intervalCountdown = gameObject.AddComponent<CS_Countdown>();
        autoaimCountdown = gameObject.AddComponent<CS_Countdown>();

        // 初期レティクルサイズを保存
        //defaultSize = autoaim.transform.localScale;

        // ロックオンカーソルを非表示
        //lockon.enabled = false;
    }

    void FixedUpdate()
    {
        // オートエイム処理
        HandlAutoaim();

        // レティクル処理
        HandlReticle();

        // 照準処理            
        //if (GetInputSystem().GetLeftTrigger() > 0.1f 
        //    && GetPlayerManager().GetCameraManager().NowCameraNumber() != 1)
        //{
        //    GetPlayerManager().GetCameraManager().SwitchingCamera(2);
        //}
        //if (GetInputSystem().GetLeftTrigger() < 0.1f
        //    && GetPlayerManager().GetCameraManager().NowCameraNumber() != 1)
        //{
        //    GetPlayerManager().GetCameraManager().SwitchingCamera(0);
        //}

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

        // 射撃処理
        if (countdown.IsCountdownFinished())
        {
            if (!GetAnimator().GetBool("Push"))
            {
                HandlShot();
            }
        }
        if (intervalCountdown.IsCountdownFinished() && isShot)
        {
           isShot = false;
        }

        // ヒートゲージ処理
        HandlOverheat();

        // プレイヤーの向きを調整
        HandlPlayerAngle();
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
        //detectionImage.color = notDetectedColor;
        //detectionImage.sprite = notDetectedSprite;
        targetObject = null;


        //// カメラ正面からレイを作成
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //RaycastHit[] hits = Physics.SphereCastAll(ray, radius, range);

        //// レティクルの変更処理
        //// レティクル内に破壊可能オブジェクトがあるか判定
        //foreach (RaycastHit hit in hits)
        //{
        //    // ヒットした場合の処理
        //    if (targetTag.Contains(hit.collider.tag))
        //    {
                
        //        break;
        //    }
        //}

        //// レティクルをオートエイム用に切り替え
        //if (isAuto)
        //{
        //    // 通常時のレティクルを非表示にする
        //    detectionImage.enabled = false;

        //    // ロックオンカーソルを表示
        //    lockon.enabled = true;

        //    // カーソルを拡大（ここはオートエイム用のレティクルを表示するように変更）
        //    autoaim.transform.localScale = new Vector3(2, 2, 2);

        //    // ロックオンカーソルを最初に検知した敵の位置の上に移動させる
        //    Vector2 screenPos = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        //    lockon.rectTransform.position = screenPos;
        //}
        //// 通常時
        //else
        //{
        //    // 通常レティクルを表示
        //    detectionImage.enabled = true;

        //    // ロックオンカーソルを非表示
        //    lockon.enabled = false;

        //    // カーソルを元に戻す（ここはオートエイム用のレティクルを非表示にするよう変更）
        //    autoaim.transform.localScale = defaultSize;
        //}
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
            // 射撃状態に変更
            GetAnimator().SetBool("Shot", true);

            // オーバーヒートゲージを表示
            overheat.enabled = true;
            overheatOut.enabled = true;

            // 装填数が0ならリロード
            if (isMagazine)
            {
                // コントローラーを振動
                CS_ControllerVibration.StartVibrationWithCurve(duration, powerType, curveType, repetition);

                CreateBullet(burstfire);
                isShot = true;
                //weaponIdle.SetActive(false);
                //weaponShot.SetActive(true);

                GetAnimator().SetBool("Shot", true);

                intervalCountdown.Initialize(interval);
            }
            else if (!GetAnimator().GetBool("Reload"))
            {
                isShot = true;
                StartCoroutine(ReloadCoroutine());

                // アニメーションの終了まで待機
                countdown.Initialize(intervalReload);
                // リロード処理
                ReloadMagazine(initMagazine);
            }
        }
        else
        {
            GetAnimator().SetBool("Shot", false);

            //ReloadMagazine(initMagazine);

            //weaponIdle.SetActive(true);
            //weaponShot.SetActive(false);
        }

        if (GetInputSystem().GetRightTrigger() <= 0 && isShot)
        {
            ReloadMagazine(initMagazine);
            Cold = true;
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
        //ReloadMagazine(initMagazine);
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
        if ((targetObject != null)&&(isAuto))
        {            
            forwardVec = targetObject.transform.position - GetPlayerManager().GetCameraTransform().position;
            forwardVec = forwardVec.normalized;
        }

        float offsetDistance = 0f;//1.5f; // 各弾の間隔

        if (burst > magazine)
        {
            burst = magazine;
        }

        for (int i = 0; i < burst; i++)
        {
            // 弾を生成
            GameObject ballobj = Instantiate(AirBall);

            //Vector3 pos = this.transform.position;
            Vector3 pos = GetPlayerManager().GetCameraTransform().position;
            Vector3 offset = new Vector3(0, 0, 0);

            pos += offset;
            pos += forwardVec * (offsetDistance * (i + (burst - 1) / 2f) + 1f);

            // 散弾処理
            if (!(GetInputSystem().GetLeftTrigger() > 0.1f) && (!isAuto))
            {
                float randomRangeX = UnityEngine.Random.Range(-scatter, scatter);
                float randomRangeY = UnityEngine.Random.Range(-scatter, scatter);
                float randomRangeZ = UnityEngine.Random.Range(-scatter, scatter);
                forwardVec.x += randomRangeX;
                forwardVec.y += randomRangeY;
                forwardVec.z += randomRangeZ;
            }

            ballobj.transform.position = pos;
            ballobj.transform.forward = forwardVec;

            // 装填数を減らす
            magazine--;
            //bulletStock--;
            //float hp = GetPlayerManager().GetHP();
            //GetPlayerManager().SetHP(hp - -shotHp);
        }
    }

    // IKPass
    private void OnAnimatorIK(int layerIndex)
    {
        // 臨戦態勢アニメーション（銃を構える）
        if ((!GetAnimator().GetBool("Reload")) && (!GetAnimator().GetBool("Throwing")
            && !GetAnimator().GetBool("Recovery") && !GetAnimator().GetBool("Use Item")
            && !GetAnimator().GetBool("Use EnergyCore") && !GetAnimator().GetBool("Push")))
        {
            // カメラの正面方向の位置を計算
            Vector3 cameraForward = Camera.main.transform.forward * 100;
            Vector3 targetPosition = Camera.main.transform.position + cameraForward;

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

    /*
     * オートエイム処理
     */
    void HandlAutoaim()
    {
        // オートエイム状態の処理
        if (isAuto && Time.time >= nextAutoaimCheckTime)
        {
            // オートエイムの効果時間のカウントを開始
            autoaimCountdown.Initialize(autoTime);

            // レイの判定間隔を更新
            nextAutoaimCheckTime = Time.time + autoaimCheckInterval;

            // カメラ正面からレイを作成
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            Vector3 boxSize = new Vector3(radiusAuto, radiusAuto, radiusAuto);

            if (Physics.BoxCast(ray.origin, boxSize / 2, ray.direction, out hit, Quaternion.identity, range))
            {
                if (targetTag.Contains(hit.collider.tag))
                {
                    targetObject = hit.collider.gameObject;
                }
            }
        }

        // 効果時間の終了処理
        if (autoaimCountdown.IsCountdownFinished())
        {
            isAuto = false;
        }
    }

    /*
     * オーバーヒートゲージ処理
     */
    void HandlOverheat()
    {
        //冷却中じゃなければ
        if (!Cold)
        {
            // オーバーヒート処理
            overheat.fillAmount = magazine * 0.1f;
            overheatOut.fillAmount = overheat.fillAmount;

        }
        else
        {
            //レティクルを冷却中画像にする
            detectionImage.sprite = ColdREticle;

            //冷却処理
            overheat.fillAmount += interval * Time.deltaTime;
            overheatOut.fillAmount = overheat.fillAmount;
            //リロードアニメーションが終了していたら冷却完了
            if (overheat.fillAmount >= 1f)
            {
                // レティクルを元に戻す
                detectionImage.sprite = reticle;

                // オーバーヒートゲージを非表示
                overheat.enabled = false;
                overheatOut.enabled = false;
                Cold = false;
                overheatText.enabled = false;
            }
            else
            {
                overheat.enabled = true;
                overheatOut.enabled = true;
            }
        }

        //一定数残弾数が減ったらゲージを赤くする
        if (overheat.fillAmount < 0.5f)
        {
            //float AlphaSpeed = 2f;
            //OverHeatAlpha -= AlphaSpeed * Time.deltaTime;
            //overheat.color = new Color(1.0f, 1.0f, 1.0f, OverHeatAlpha);

            overheat.enabled = false;
            overheatText.enabled = true;
        }
        else
        {
            //float AlphaSpeed = 2f;
            //OverHeatAlpha += AlphaSpeed * Time.deltaTime;
            //overheat.color = new Color(1.0f, 1.0f, 1.0f, OverHeatAlpha);

            //overheat.enabled = true;
            overheatText.enabled = false;
        }

        //残弾数が0になったら冷却
        if (magazine <= 0)
        {
            Cold = true;
        }
    }

    /*
     * カメラの角度からプレイヤーの向きを調整する関数
     */
    void HandlPlayerAngle()
    {
        // 回転軸を計算
        Vector3 axis = Vector3.Cross(transform.forward, cameraTarget.transform.forward);

        // 角度を計算
        float angle = Vector3.Angle(transform.forward, cameraTarget.transform.forward) * (axis.y < 0 ? -1 : 1);

        // 角度が閾値より大きい場合のみ回転を行う
        if (Mathf.Abs(angle) > 25f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraTarget.transform.rotation, 2f * Time.deltaTime);
            cameraTarget.transform.rotation = Quaternion.Slerp(cameraTarget.transform.rotation, transform.rotation, 2f * Time.deltaTime);
        }
        else
        {
            GetAnimator().SetFloat("Turn", 0f);
        }

        // Aimのずらす方向を調整
        Vector2 stick = GetInputSystem().GetRightStick();
        if (stick.x > 0.3f)
        {
            aimTarget.transform.localPosition = Vector3.Lerp(
            aimTarget.transform.localPosition,
            new Vector3(1, 1, -1),
            1f * Time.deltaTime
            );

            if (Mathf.Abs(angle) > 25f)
                GetAnimator().SetFloat("Turn", 1f);
        }
        else if (stick.x < -0.3f)
        {
            aimTarget.transform.localPosition = Vector3.Lerp(
            aimTarget.transform.localPosition,
            new Vector3(1, 1, 1),
            1f * Time.deltaTime
            );

            if (Mathf.Abs(angle) > 25f)
                GetAnimator().SetFloat("Turn", -1f);
        }
    }

    // デバック用に拡散範囲の円錐を表示
    //private void OnDrawGizmos()
    //{
    //    float radius = range * scatter;
    //    float height = range;
    //    int segments = 30;

    //    // 底面の円を描く
    //    Vector3 center = Camera.main.transform.position + Camera.main.transform.forward * height;
    //    for (int i = 0; i < segments; i++)
    //    {
    //        float angle0 = i * Mathf.PI * 2f / segments;
    //        float angle1 = (i + 1) * Mathf.PI * 2f / segments;

    //        Vector3 point0 = center + new Vector3(Mathf.Cos(angle0) * radius, Mathf.Sin(angle0) * radius,0);
    //        Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius,0);

    //        Gizmos.DrawLine(point0, point1); // 底面の円を描く
    //        Gizmos.DrawLine(center, point0); // 頂点に向かって線を描く
    //    }

    //    // 底面と頂点を結ぶ線を描く
    //    Vector3 top = Camera.main.transform.position;
    //    for (int i = 0; i < segments; i++)
    //    {
    //        float angle = i * Mathf.PI * 2f / segments;
    //        Vector3 point = center + new Vector3(Mathf.Cos(angle) * radius,  Mathf.Sin(angle) * radius,0);
    //        Gizmos.DrawLine(top, point);
    //    }
    //}
}
