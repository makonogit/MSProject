using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using UnityEngine;

using UnityEngine;

public abstract class ActionBase : MonoBehaviour
{
    //**
    //* 変数
    //**
    CS_PlayerManager playerManager;
    public CS_PlayerManager GetPlayerManager() => playerManager;
    public CS_InputSystem GetInputSystem() => playerManager.GetInputSystem();
    public Rigidbody GetRigidbody() => playerManager.GetRigidbody();
    public Animator GetAnimator() => playerManager.GetAnimator();

    protected virtual void Start()
    {
        // プレイヤーオブジェクトを取得
        playerManager = GameObject.Find("Player")?.GetComponent<CS_PlayerManager>();
        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("Playerオブジェクトが見つかりません。");
        }
    }

    [SerializeField, Header("サウンドエフェクト")]
    private CS_SoundEffect soundEffect;
    public CS_SoundEffect GetSoundEffect() => soundEffect;
}

