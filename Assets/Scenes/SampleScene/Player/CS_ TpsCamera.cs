using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// TPSカメラ
public class CS_TpsCamera : MonoBehaviour
{
    //**
    //* 変数
    //**

    // 外部オブジェクト
    public Transform target; // 追尾対象

    // 移動・回転
    public Vector3 offsetPos = new Vector3 (0, 8, 0);// 位置
    public Vector3 offsetFocus = new Vector3(0, 3, 0);// 焦点
    public float moveSpeed = 50.0f;             // 移動スピード
    public float rotationSpeed = 50.0f;         // 回転スピード
    public float verticalRotationLimit = 80.0f; // 縦回転の制限
    private float cameraRotX = 0.0f;            // 横回転の移動量
    private float cameraRotY = 0.0f;            // 縦回転の移動量

    // 自身のコンポーネント
    private Camera camera; 

    //**
    //* 初期化
    //**
    void Start()
    {
        // カメラコンポーネントを取得
        camera = GetComponent<Camera>();

        // カメラを初期位置にセット
        MoveCamera(Vector3.up);
    }

    //**
    //* 更新
    //**
    void Update()
    {
        // 入力に応じてカメラを回転させる

        // 右スティックの入力を取得
        float rsh = Input.GetAxis("RStick X");
        float rsv = Input.GetAxis("RStick Y");

        // カメラを入力に応じて移動
        Vector3 rotVec = new Vector3(-rsv, rsh, 0);
        rotVec = rotVec.normalized;
        MoveCamera(rotVec);
    }

    //**
    //* カメラをターゲットを中心に移動させる
    //*
    //* in：移動させる方向
    //* out：無し
    //**
    void MoveCamera(Vector3 direction)
    {
        // カメラの位置を更新
        UpdateCameraPosition(direction);

        // カメラの角度を更新
        UpdateCameraRotation();
    }

    //**
    //* カメラの位置を更新する
    //*
    //* in：移動させる方向
    //* out：無し
    //**
    void UpdateCameraPosition(Vector3 direction)
    {
        // 移動速度
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // 移動方向
        Vector3 normalizedDirection = direction.normalized;

        // 水平方向の回転を更新
        if (Mathf.Abs(Vector3.Dot(normalizedDirection, Vector3.right)) > 0.1f)
        {
            cameraRotX += rotationAmount * Mathf.Sign(Vector3.Dot(normalizedDirection, Vector3.right));
            cameraRotX = Mathf.Clamp(cameraRotX, -verticalRotationLimit, verticalRotationLimit);
        }

        // 垂直方向の回転を更新
        if (Mathf.Abs(Vector3.Dot(normalizedDirection, Vector3.up)) > 0.1f)
        {
            cameraRotY += rotationAmount * Mathf.Sign(Vector3.Dot(normalizedDirection, Vector3.up));
        }

        // カメラの位置を滑らかに更新
        Quaternion rotation = Quaternion.Euler(cameraRotX, cameraRotY, 0);
        Vector3 offset = rotation * offsetPos;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 10.0f * Time.deltaTime);
    }

    //**
    //* カメラの回転を更新する
    //* in：無し
    //* out：無し
    //**
    void UpdateCameraRotation()
    {
        // ターゲットの方向を向くようにカメラを回転させる
        Vector3 directionToTarget = (target.position + offsetFocus) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}


