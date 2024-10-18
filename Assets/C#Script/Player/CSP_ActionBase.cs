using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using UnityEngine;

using UnityEngine;

public abstract class ActionBase : MonoBehaviour
{
    //**
    //* �ϐ�
    //**
    CS_PlayerManager playerManager;
    public CS_PlayerManager GetPlayerManager() => playerManager;
    public CS_InputSystem GetInputSystem() => playerManager.GetInputSystem();
    public Rigidbody GetRigidbody() => playerManager.GetRigidbody();
    public Animator GetAnimator() => playerManager.GetAnimator();

    protected virtual void Start()
    {
        // �v���C���[�I�u�W�F�N�g���擾
        playerManager = GameObject.Find("Player")?.GetComponent<CS_PlayerManager>();
        if (playerManager == null)
        {
            UnityEngine.Debug.LogError("Player�I�u�W�F�N�g��������܂���B");
        }
    }

    [SerializeField, Header("�T�E���h�G�t�F�N�g")]
    private CS_SoundEffect soundEffect;
    public CS_SoundEffect GetSoundEffect() => soundEffect;
}

