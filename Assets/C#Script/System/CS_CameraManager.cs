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
    [SerializeField, Header("開始時のカメラ")]
    private int initIndex = 0;
    [Header("使用するカメラを設定")]
    public CinemachineVirtualCamera[] virtualCameras;// カメラのリスト

    // 自身のコンポーネント
    private CinemachineImpulseSource impulseSource;// 振動

    private int nowIndex; // 現在のカメラインデックス

    //**
    //* 初期化
    //**

    void Start()
    {
        // 自身のコンポーネントを取得
        impulseSource = GetComponent<CinemachineImpulseSource>();

        // 開始時のカメラを設定
        SwitchingCamera(initIndex);
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
    //* in:切り替え先
    //* out:無し
    //**

    public void SwitchingCamera(int index)
    {
        nowIndex = index;

        // 全てのカメラのPriorityを下げる
        foreach (var virtualCamera in virtualCameras)
        {
            virtualCamera.Priority = -1;
        }
        // 対象のカメラのPriorityを上げる
        virtualCameras[index].Priority = 10;
    }

    /*
     * 現在のカメラを取得する
     */

    public int NowCameraNumber()
    {
        return nowIndex;
    }
}
