using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CS_ShootingCamera : ActionBase
{
    //**
    //* 変数
    //**

    // 外部オブジェクト
    [Header("外部オブジェクト")]
    public Transform target; // 追尾対象
    public Transform focus; // 追尾対象
    public CS_InputSystem inputSystem;// インプットマネージャー


    // 移動・回転
    [Header("位置")]
    public Vector3 offsetPos = new Vector3(0, 8, 0);// 位置
    public Vector3 offsetFocus = new Vector3(0, 3, 0);// 焦点
    [Header("カメラ感度 縦/横")]
    public float wideSpeed = 90.0f;             // 横スピード
    public float hydeSpeed = 40.0f;             // 縦スピード
    public float rotationSpeed = 50.0f;         // 回転スピード
    [Header("移動/回転の制限")]
    public float rotationLimitMax = 80.0f; // X軸回転の制限（最大）
    public float rotationLimitMin = 10.0f; // X軸回転の制限（最小）
    private float cameraRotX = 0.0f;     // X軸回転の移動量
    private float cameraRotY = 0.0f;       // Y軸転の移動量
    [Header("カメラの距離")]
    public float cameraDistance = 5.0f;
    [SerializeField, Header("反応曲線")]
    private AnimationCurve accelerationCurve;       // 反応曲線
    [SerializeField, Header("入力範囲")]
    private float maxInputValue = 1f;         // スティックの最大入力値
    [SerializeField]
    private float minInputValue = 0.3f;         // スティックの最小入力値

    private float currentAcceleration = 0f;  // 現在の加速度

    [Header("アシスト設定")]
    [SerializeField, Header("検知タグ")]
    private List<string> targetTag;
    [SerializeField, Header("障害物レイヤー")]
    private LayerMask mask;
    [SerializeField, Header("検知距離")]
    private float range = 100f;
    [SerializeField, Header("検出範囲")]
    float radius = 3.0f;
    [SerializeField, Header("単体へのアシスト倍率")]
    private float assistVal = 0.3f;
    [SerializeField, Header("複数体へのアシスト倍率")]
    private float manyAssistVal = 0.7f;
    private int enemyCount = 0;


    // 自身のコンポーネント
    private CinemachineVirtualCamera camera;

    //**
    //* 初期化
    //**
    void Start()
    {
        // カメラコンポーネントを取得
        camera = GetComponent<CinemachineVirtualCamera>();
    }

    //**
    //* 更新
    //**
    void FixedUpdate()
    {
        if (IsAssist())
        {
            if(enemyCount == 0)
            {
                HandlAssistCamera();
            }
            else
            {
                HandlManyAssistCamera();
            }

        }
        else
        {
            HandlCamera();
        }
    }

    //**
    //* カメラの回転を更新する
    //* in：無し
    //* out：無し
    //**
    void UpdateCameraRotation()
    {
        Vector3 directionToTarget = (focus.position + offsetFocus) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    /*
     * カメラ処理
     */
    void HandlCamera()
    {
        // 入力に応じてカメラを回転させる

        // 右スティックの入力を取得
        Vector2 stick = inputSystem.GetRightStick();

        if (Mathf.Abs(stick.x) < minInputValue)
        {
            stick.x = 0f;
        }
        if (Mathf.Abs(stick.y) < minInputValue)
        {
            stick.y = 0f;
        }

        // 入力の大きさを計算
        float inputMagnitude = new Vector2(stick.y, stick.x).magnitude;
        float normalizedInput = Mathf.Clamp(inputMagnitude / maxInputValue, 0f, 1f);
        currentAcceleration = accelerationCurve.Evaluate(normalizedInput);

        // カメラを入力に応じて回転
        if ((offsetFocus.y > -5) && (offsetFocus.y < 5f))
        {
            Vector3 rotVec = new Vector3(0, stick.x, 0);
            rotVec = rotVec.normalized;
            Vector3 rot = target.rotation.eulerAngles;
            rot += wideSpeed * rotVec * currentAcceleration * Time.deltaTime;
            target.rotation = Quaternion.Euler(rot);

            float movePos = offsetFocus.y + hydeSpeed * stick.y * currentAcceleration * Time.deltaTime;
            offsetFocus.y = Mathf.Lerp(offsetFocus.y, movePos, 0.1f);
        }

        // ターゲットの背面にカメラを配置
        Vector3 targetPosition = target.position - (target.forward * cameraDistance);
        transform.position = targetPosition + offsetPos;

        UpdateCameraRotation();

        if (offsetFocus.y < -5)
        {
            offsetFocus.y = -5f;
        }
        if (offsetFocus.y > 5f)
        {
            offsetFocus.y = 5f;
        }

    }

    /*
     * アシストカメラ処理（単体）
     */
    void HandlAssistCamera()
    {
        // 入力に応じてカメラを回転させる

        // 右スティックの入力を取得
        Vector2 stick = inputSystem.GetRightStick();

        if (Mathf.Abs(stick.x) < minInputValue)
        {
            stick.x = 0f;
        }
        if (Mathf.Abs(stick.y) < minInputValue)
        {
            stick.y = 0f;
        }

        // 入力の大きさを計算
        float inputMagnitude = new Vector2(stick.y, stick.x).magnitude;
        float normalizedInput = Mathf.Clamp(inputMagnitude / maxInputValue, 0f, 1f);
        currentAcceleration = accelerationCurve.Evaluate(normalizedInput);

        // カメラを入力に応じて回転
        if ((offsetFocus.y > -5) && (offsetFocus.y < 5f))
        {
            Vector3 rotVec = new Vector3(0, stick.x, 0);
            rotVec = rotVec.normalized;
            Vector3 rot = target.rotation.eulerAngles;
            rot += wideSpeed * assistVal * rotVec * currentAcceleration * Time.deltaTime;
            target.rotation = Quaternion.Euler(rot);

            float movePos = offsetFocus.y + hydeSpeed * assistVal * stick.y * currentAcceleration * Time.deltaTime;
            offsetFocus.y = Mathf.Lerp(offsetFocus.y, movePos, 0.1f);
        }

        // ターゲットの背面にカメラを配置
        Vector3 targetPosition = target.position - (target.forward * cameraDistance);
        transform.position = targetPosition + offsetPos;

        UpdateCameraRotation();

        if (offsetFocus.y < -5)
        {
            offsetFocus.y = -5f;
        }
        if (offsetFocus.y > 5f)
        {
            offsetFocus.y = 5f;
        }
    }

    /*
     * アシストカメラ処理（複数）
     */
    void HandlManyAssistCamera()
    {
        // 入力に応じてカメラを回転させる

        // 右スティックの入力を取得
        Vector2 stick = inputSystem.GetRightStick();

        if (Mathf.Abs(stick.x) < minInputValue)
        {
            stick.x = 0f;
        }
        if (Mathf.Abs(stick.y) < minInputValue)
        {
            stick.y = 0f;
        }

        // 入力の大きさを計算
        float inputMagnitude = new Vector2(stick.y, stick.x).magnitude;
        float normalizedInput = Mathf.Clamp(inputMagnitude / maxInputValue, 0f, 1f);
        currentAcceleration = accelerationCurve.Evaluate(normalizedInput);

        // カメラを入力に応じて回転
        if ((offsetFocus.y > -5) && (offsetFocus.y < 5f))
        {
            Vector3 rotVec = new Vector3(0, stick.x, 0);
            rotVec = rotVec.normalized;
            Vector3 rot = target.rotation.eulerAngles;
            rot += wideSpeed * manyAssistVal * rotVec * currentAcceleration * Time.deltaTime;
            target.rotation = Quaternion.Euler(rot);

            float movePos = offsetFocus.y + hydeSpeed * manyAssistVal * stick.y * currentAcceleration * Time.deltaTime;
            offsetFocus.y = Mathf.Lerp(offsetFocus.y, movePos, 0.1f);
        }

        // ターゲットの背面にカメラを配置
        Vector3 targetPosition = target.position - (target.forward * cameraDistance);
        transform.position = targetPosition + offsetPos;

        UpdateCameraRotation();

        if (offsetFocus.y < -5)
        {
            offsetFocus.y = -5f;
        }
        if (offsetFocus.y > 5f)
        {
            offsetFocus.y = 5f;
        }
    }

    /*
     * アシスト状態か確認する
     */
    bool IsAssist()
    {
        enemyCount = 0;

        // カメラ正面からレイを作成
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        // 壁に当たるかをチェック
        RaycastHit wallhit;
        if (Physics.Raycast(ray, out wallhit, range, mask))
        {
            // 壁に衝突した場合は、ターゲット検出を行わない
            return false;
        }

        // 壁を避けて、ターゲットオブジェクトを探す
        RaycastHit[] hits = Physics.SphereCastAll(ray, radius, range);

        // レティクル内に破壊可能オブジェクトがあるか判定
        foreach (RaycastHit hit in hits)
        {
            if (targetTag.Contains(hit.collider.tag))
            {
                enemyCount++;
            }
        }

        return enemyCount > 0;
    }

}
