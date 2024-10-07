using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class CS_Aim : MonoBehaviour
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
    [Header("移動/回転の速さ")]
    public float wideSpeed = 2.0f;             // 横スピード
    public float hydeSpeed = 0.25f;             // 縦スピード
    public float rotationSpeed = 50.0f;         // 回転スピード
    [Header("移動/回転の制限")]
    public float rotationLimitMax = 80.0f; // X軸回転の制限（最大）
    public float rotationLimitMin = 10.0f; // X軸回転の制限（最小）
    private float cameraRotX = 0.0f;     // X軸回転の移動量
    private float cameraRotY = 0.0f;       // Y軸転の移動量

    // 自身のコンポーネント
    private CinemachineVirtualCamera camera;

    [Header("リコイル設定")]
    public float recoilAmount = 1f; // リコイルの強さ
    public float recoilDuration = 0.1f; // リコイルの持続時間
    public float returnSpeed = 0.1f; // 元に戻る速さ
    private Quaternion originalRotation;
    private float recoilTimer = 0f;

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
        // 入力に応じてカメラを回転させる

        if(camera.Priority == 10)
        {
            // 右スティックの入力を取得
            Vector2 stick = inputSystem.GetRightStick();

            // カメラを入力に応じて回転
            Vector3 rotVec = new Vector3(0, stick.x, 0);
            rotVec = rotVec.normalized;
            Vector3 rot = target.rotation.eulerAngles;
            rot += wideSpeed * rotVec;
            target.rotation = Quaternion.Euler(rot);

            // ターゲットの背面にカメラを配置
            Vector3 targetPosition = target.position - target.forward;
            transform.position = targetPosition + offsetPos;

            UpdateCameraRotation();

            offsetFocus.y += hydeSpeed * stick.y;
        }
        else
        {
            offsetFocus = new Vector3(0, 0, 0);
        }
    }

    public void TriggerRecoil()
    {
        originalRotation = transform.localRotation;
        recoilTimer = recoilDuration; // リコイルをトリガー
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

        if (recoilTimer > 0)
        {
            // リコイルの効果
            Quaternion recoilRotation = Quaternion.Euler(-recoilAmount, 0, 0);
            transform.localRotation *= recoilRotation; // リコイルを適用
            recoilTimer -= Time.deltaTime; // タイマーを減少
        }
        else
        {
            // 元に戻す
            transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, returnSpeed * Time.deltaTime);
        }
    }
}
