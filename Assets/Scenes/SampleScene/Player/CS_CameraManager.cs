using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// 複数のカメラを制御する
public class CS_CameraManager : MonoBehaviour
{
    //**
    //* 変数
    //**

    // 外部オブジェクト
    public CinemachineVirtualCamera[] virtualCameras;// カメラのリスト

    // パラメーター
    private int cameraIndex = 0;// 使用するカメラのインデックス
    private float elapsedTime = 0f; // フレーム数をカウントする変数
    private bool switchingFlg = false;// 切り替え状態
    private float targetTime = 0f;// 切り替え時間

    // 自身のコンポーネント
    private CinemachineImpulseSource impulseSource;// 振動


    //**
    //* 初期化
    //**

    void Start()
    {
        // 自身のコンポーネントを取得
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    //**
    //* 更新
    //**

    void Update()
    {
        // 切り替え更新
        SwitchingUpdate();
    }

    //**
    //* カメラを振動させる
    //*
    //* in:無し
    //* out:無し
    //**

    public void ShakeCamera(float amplitude, float frequency)
    {
        // Impulse Sourceの設定
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitude;// 振動の大きさ
        impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequency;// 振動させる時間

        // 振動を開始
        impulseSource.GenerateImpulse();
    }

    //**
    //* カメラを切り替える
    //*
    //* in:切り替え先、切り替えておく時間
    //* out:無し
    //**

    public void SwitchingCamera(int index, int time)
    {
        cameraIndex = index;
        targetTime = time;
        switchingFlg = true;
    }

    // カメラ切り替えの更新処理
    void SwitchingUpdate()
    {
        // カメラ切り替え
        if (switchingFlg)
        {
            if (elapsedTime == 0f)
            {
                virtualCameras[cameraIndex].gameObject.SetActive(true);
            }

            // 実行時間をカウント
            elapsedTime += Time.deltaTime;

            // 指定したフレーム数に達したら処理を実行
            if (elapsedTime >= targetTime)
            {
                virtualCameras[cameraIndex].gameObject.SetActive(false);
                elapsedTime = 0f;// 実行時間をリセット
                targetTime = 0f;
                switchingFlg = false;
            }
        }
    }
}
