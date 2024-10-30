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
    [SerializeField, Header("�̗͂̏����l")]
    private float initHP = 100;
    [SerializeField]
    private float nowHP;
    public float GetHP() => nowHP;
    public void SetHP(float setHP) { nowHP = setHP; }
    [SerializeField, Header("�ʋl�߂̎擾��")]
    private int itemStock = 0;
    public int GetItemStock() => itemStock;
    public void SetItemStock(int val) { itemStock = val; }
    [SerializeField, Header("�󂫊ʂ̌�")]
    private int ingredientsStock = 0;
    public int GetIngredientsStock() => ingredientsStock;
    public void SetIngredientsStock(int val) { ingredientsStock = val; }
    [SerializeField, Header("CS_Core�������ɃZ�b�g")]
    private CS_Core core;


    [Header("�ڒn����")]
    [SerializeField]
    private float groundCheckDistance = 0.01f;
    [SerializeField]
    private float fallingThreshold = 5f;
    [SerializeField]
    private LayerMask groundLayer;
    public Vector3 oldAcceleration;// 1�t���[���O�̏d�͉����x

    [Header("���")]
    [SerializeField]
    private bool isStunned = false;
    public bool GetStunned() => isStunned;
    public void SetStunned(bool val) { isStunned = val; }

    [SerializeField, Header("�I�[�f�B�I")]
    private CS_SoundEffect soundEffect;
    public CS_SoundEffect GetSoundEffect() => soundEffect;

    // �O���I�u�W�F�N�g�i���O�����Ŏ擾�j
    private Transform cameraTransform;
    public Transform GetCameraTransform() => cameraTransform;
    private CS_InputSystem inputSystem;     // �C���v�b�g�V�X�e��
    public CS_InputSystem GetInputSystem()=> inputSystem;
    private CS_CameraManager cameraManager; // �J�����}�l�[�W���[
    public CS_CameraManager GetCameraManager() => cameraManager;
    private CS_TpsCamera tpsCamera;         // TPS�J����
    public CS_TpsCamera GetTpsCamera() => tpsCamera;
    private CS_Result result;
    public CS_Result GetResult() => result;

    // ���g�̃R���|�[�l���g
    private Rigidbody rb;       // ���W�b�g�{�f�B
    public Rigidbody GetRigidbody() => rb;
    private Animator animator;  // �A�j���[�^�[
    public Animator GetAnimator() => animator;

    // �J�E���g�_�E���p�N���X
    private CS_Countdown countdown;

    //**
    //* ������
    //**
    void Start()
    {
        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();

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



        GameObject resultCanvas = GameObject.Find("ResultCanvas");

        if (resultCanvas == null)
        {
            UnityEngine.Debug.LogError("ResultCanvas��Hierarchy�ɒǉ����Ă��������B");
        }
        else
        {
            // ResultCanvas�����������ꍇ�ɂ̂݁AResultPanel��T��
            Transform resultPanelTransform = resultCanvas.transform.Find("ResultPanel");
            if (resultPanelTransform != null)
            {
                result = resultPanelTransform.GetComponent<CS_Result>();
                if (result == null)
                {
                    UnityEngine.Debug.LogError("ResultPanel��CS_Result�R���|�[�l���g���A�^�b�`����Ă��邩�m�F���Ă��������B");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("ResultPanel��Hierarchy�ɒǉ����Ă��������B");
            }
        }

    }

    void Update()
    {
        // �G�l���M�[�R�A�̏�Ԃ�ݒ�
        if(core != null)
        {
            if (animator.GetBool("Mount"))
            {
                core.STATE = CS_Core.CORE_STATE.HAVEPLAYER;
            }
            else
            {
                core.STATE = CS_Core.CORE_STATE.DROP;
            }
        }
        else
        {
            UnityEngine.Debug.LogError("CS_Core�I�u�W�F�N�g���ݒ肳��Ă��܂���");
        }



        // �Q�[���I�[�o�[
        if (nowHP <= 0)
        {
            animator.SetBool("GameOver", true);
            isStunned = true;
        }

        animator.SetBool("isGrounded", IsGrounded());

        animator.SetBool("isWall", IsWall());

        if (animator.GetBool("Mount"))
        {
            animator.SetFloat("Mass", 1);
        }
        else
        {
            animator.SetFloat("Mass", 0);
        }

        if(initHP - nowHP >= initHP - initHP / 3)
        {
            animator.SetFloat("Hunger", 1);
        }
        else
        {
            animator.SetFloat("Hunger", 0);
        }

        // �d�͉����x���痎����Ԃ�ݒ肷��
        Vector3 gravity = Physics.gravity;
        Vector3 acceleration = rb.velocity / Time.deltaTime;

        if (countdown.IsCountdownFinished())
        {
            if (IsGrounded() && acceleration.y == 0 && oldAcceleration.y != 0)
            {
                if ((oldAcceleration.y < -fallingThreshold))
                {
                    animator.SetFloat("Falling", 1);
                    isStunned = true;
                    countdown.Initialize(0.5f);
                }
                else
                {
                    animator.SetFloat("Falling", 0);
                }
            }
        }
        oldAcceleration = acceleration;
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

            itemStock++;

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Goal")
        {
            TemporaryStorage.DataSave("ingredientsStock",ingredientsStock);
            animator.SetBool("Goal", true);
            isStunned = true;
            result.StartResult();
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
