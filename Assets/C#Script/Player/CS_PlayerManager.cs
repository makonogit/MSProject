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
    private Vector3 oldAcceleration;// 1�t���[���O�̏d�͉����x

    // �d��
    [System.Serializable]
    public class StringNumberPair
    {
        public string name;
        public float time;
    }
    [SerializeField, Header("���(bool)/�d������")]
    private StringNumberPair[] animatorBoolParameterList;
    public StringNumberPair[] GetAnimatorBoolParameterList() => animatorBoolParameterList;
    [SerializeField, Header("���(float)/�d������")]
    private StringNumberPair[] animatorFloatParameterList;
    public StringNumberPair[] GetAnimatorFloatParameterList() => animatorFloatParameterList;

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
    private CS_Countdown countdownGoal;
    private CS_Countdown countdownStun;


    //**
    //* ������
    //**
    void Start()
    {
        // Countdown�I�u�W�F�N�g�𐶐�
        countdown = gameObject.AddComponent<CS_Countdown>();
        countdownGoal = gameObject.AddComponent<CS_Countdown>();
        countdownStun = gameObject.AddComponent<CS_Countdown>();

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
        if (core != null)
        {
            if (animator.GetBool("Mount"))
            {
                core.STATE = CS_Core.CORE_STATE.HAVEPLAYER;
            }
            else if(core.STATE != CS_Core.CORE_STATE.HAVEENEMY)
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
                    countdown.Initialize(0.5f);
                }
                else
                {
                    animator.SetFloat("Falling", 0);
                }
            }
        }
        oldAcceleration = acceleration;


        // �S�[�����[�V�������I�����Ă��烊�U���g��\��
        if (countdownGoal.IsCountdownFinished() && animator.GetBool("Goal"))
        {
            result.StartResult();
            animator.SetBool("Goal",false);
        }

        // �C����
        if (countdownStun.IsCountdownFinished() && animator.GetBool("Stun"))
        {
            animator.SetBool("Stun", false);
        }
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
        else if (collision.gameObject.tag == "StunObject")
        {
            animator.SetBool("Stun", true);

            Vector3 reverseForce = collision.contacts[0].normal * 10.0f;
            reverseForce.y = 0f;
            GetRigidbody().AddForce(reverseForce, ForceMode.Impulse);

            countdownStun.Initialize(3);
        }
        else if (collision.gameObject.tag == "Goal")
        {
            TemporaryStorage.DataSave("ingredientsStock",ingredientsStock);
            animator.SetBool("Goal", true);

            countdownGoal.Initialize(3);
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
        //RaycastHit hit;
        //return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);

        //float radius = 1.0f;
        //float range = 0.1f;

        //Ray ray = new Ray(transform.position, Vector3.down);
        //RaycastHit[] hits = Physics.SphereCastAll(ray, radius, range);

        //foreach (RaycastHit hit in hits)
        //{
        //    return true;
        //}

        //return false;

        float radius = 0.125f;
        float groundCheckDistance = 0.01f;
        return Physics.CheckSphere(transform.position - Vector3.up * groundCheckDistance, radius, groundLayer);
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

    // �ڒn����̉���
    private void OnDrawGizmos()
    {
        float radius = 0.125f;               // Sphere�̔��a
        float groundCheckDistance = 0.01f;  // �`�F�b�N�ʒu�̃I�t�Z�b�g

        // �ڒn��Ԃ�F�ŉ���
        if (Physics.CheckSphere(transform.position, radius, groundLayer))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        // Sphere��`��
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
