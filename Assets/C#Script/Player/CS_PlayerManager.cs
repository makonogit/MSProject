using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//**
//* �v���C���[�}�l�[�W���[
//*
//* �S���F����
//**
public class CS_PlayerManager : MonoBehaviour
{
    //**
    //* �ϐ�
    //**

    [Header("�p�����[�^�[�ݒ�")]
    [SerializeField, Header("�̗͒l")]
    private float initHP = 100;
    [SerializeField]
    private float nowHP;
    public float GetHP() => nowHP;
    public void SetHP(float setHP) { nowHP = setHP; }

    [Header("�ڒn����")]
    [SerializeField]
    private float groundCheckDistance = 0.01f;
    [SerializeField]
    private LayerMask groundLayer;

    // �O���I�u�W�F�N�g�i���O�����Ŏ擾�j
    private Transform cameraTransform;
    public Transform GetCameraTransform() => cameraTransform;
    private CS_InputSystem inputSystem;     // �C���v�b�g�V�X�e��
    public CS_InputSystem GetInputSystem()=> inputSystem;
    private CS_CameraManager cameraManager; // �J�����}�l�[�W���[
    public CS_CameraManager GetCameraManager() => cameraManager;
    private CS_TpsCamera tpsCamera;         // TPS�J����
    public CS_TpsCamera GetTpsCamera() => tpsCamera;

    // ���g�̃R���|�[�l���g
    private Rigidbody rb;       // ���W�b�g�{�f�B
    public Rigidbody GetRigidbody() => rb;
    private Animator animator;  // �A�j���[�^�[
    public Animator GetAnimator() => animator;

    //**
    //* ������
    //**
    void Start()
    {
        // HP��ݒ�
        nowHP = initHP;

        // ���g�̃R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // �C���X�y�N�^�[�r���[�������̃I�u�W�F�N�g���擾
        cameraTransform = GameObject.Find("Main Camera")?.transform;
        if (cameraTransform == null)
        {
            UnityEngine.Debug.LogError("Main Camera��Hierarchy�ɒǉ����Ă��������B");
        }

        inputSystem = GameObject.Find("InputSystem")?.GetComponent<CS_InputSystem>();
        if (inputSystem == null)
        {
            UnityEngine.Debug.LogError("InputSystem��Hierarchy�ɒǉ����Ă��������B");
        }

        cameraManager = GameObject.Find("CameraManager")?.GetComponent<CS_CameraManager>();
        if (cameraManager == null)
        {
            UnityEngine.Debug.LogError("CameraManager��Hierarchy�ɒǉ����Ă��������B");
        }

        tpsCamera = GameObject.Find("TpsCamera")?.GetComponent<CS_TpsCamera>();
        if (tpsCamera == null)
        {
            UnityEngine.Debug.LogError("TpsCamera��Hierarchy�ɒǉ����Ă��������B");
        }

    }

    void Update()
    {
        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isWall", IsWall());
    }

    //**
    //* �Փˏ���
    //**
    private void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g�̃^�O���`�F�b�N
        if (collision.gameObject.tag == "Item")
        {
            CS_Item item = collision.gameObject.GetComponent<CS_Item>();

            nowHP += 1;
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            nowHP -= 1;
        }
        else if (collision.gameObject.tag == "Goal")
        {
        }
    }

    //**
    //* �n�ʂɐڒn���Ă��邩�𔻒f����
    //*
    //* in�F����
    //* out�F�ڒn����
    //**
    public bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);
    }

    //**
    //* �ǂɐڂ��Ă��邩�𔻒f����
    //*
    //* in�F����
    //* out�F�ڒn����
    //**
    public bool IsWall()
    {
        RaycastHit hit;
        Vector3 offset = new Vector3(0f,0f,0f);
        return Physics.Raycast(transform.position + offset, transform.forward, out hit, groundCheckDistance, groundLayer);
    }
}
