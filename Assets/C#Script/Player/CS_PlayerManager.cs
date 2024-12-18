using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Assets.C_Script.GameEvent;
using Assets.C_Script.UI.Result;
using Assets.C_Script.GameEvent;
using Assets.C_Script.UI.Gage;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField, Header("MP�̍ő�l")]
    private int MaxMP = 100;
    [SerializeField]
    private int nowMP = 0;
    public int GetMP() => nowMP;
    public void SetMP(int MP) { nowMP = MP;}
    [SerializeField, Header("�f�J�ʂ̎擾��")]
    private int nowBigCan = 0;
    public int GetBigCan() => nowBigCan;
    public void SetBigCan(int set) { nowBigCan = set; }


    //[SerializeField, Header("�ʋl�߂̎擾��")]
    private int itemStock = 0;
    public int GetItemStock() => itemStock;
    public void SetItemStock(int val) { itemStock = val; }
    //[SerializeField, Header("�󂫊ʂ̌�")]
    private int ingredientsStock = 0;
    public int GetIngredientsStock() => ingredientsStock;
    public void SetIngredientsStock(int val) { ingredientsStock = val; }
    //[SerializeField, Header("CS_Core�������ɃZ�b�g")]
    //private CS_Core core;

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
    [SerializeField, Header("�C���v�b�g�V�X�e���ݒ�")]
    private CS_InputSystem inputSystem;     // �C���v�b�g�V�X�e��
    public CS_InputSystem GetInputSystem()=> inputSystem;
    [SerializeField, Header("�J�����}�l�[�W���[�ݒ�")]
    private CS_CameraManager cameraManager; // �J�����}�l�[�W���[
    public CS_CameraManager GetCameraManager() => cameraManager;
    //[SerializeField, Header("�J�����ݒ�")]
    //private CS_TpsCamera tpsCamera;
    //public CS_TpsCamera GetTpsCamera() => tpsCamera;
    [SerializeField, Header("���U���g�\���ݒ�")]
    private Assets.C_Script.UI.Result.CSGE_Result result;
    public Assets.C_Script.UI.Result.CSGE_Result GetResult() => result;

    [Header("�󂫊ʎ擾���̐U���ݒ�")]
    [SerializeField, Header("�U���̒���")]
    private float duration = 0.5f;         // �U���̒���
    [SerializeField, Header("�U���̋���")]
    private int powerType = 1;          // �U���̋����i4�i�K�j
    [SerializeField, Header("�U���̎��g��")]
    private AnimationCurve curveType;          // �U���̎��g��
    [SerializeField, Header("�J��Ԃ���")]
    private int repetition = 1;         // �J��Ԃ���

    [Header("�f�J�ʎ擾���̐U���ݒ�")]
    [SerializeField, Header("�U���̒���")]
    private float duration1 = 0.5f;         // �U���̒���
    [SerializeField, Header("�U���̋���")]
    private int powerType1 = 1;          // �U���̋����i4�i�K�j
    [SerializeField, Header("�U���̎��g��")]
    private AnimationCurve curveType1;          // �U���̎��g��
    [SerializeField, Header("�J��Ԃ���")]
    private int repetition1 = 1;         // �J��Ԃ���

    [Header("�X�^�����̐U���ݒ�")]
    [SerializeField, Header("�U���̒���")]
    private float duration2 = 0.5f;         // �U���̒���
    [SerializeField, Header("�U���̋���")]
    private int powerType2 = 1;          // �U���̋����i4�i�K�j
    [SerializeField, Header("�U���̎��g��")]
    private AnimationCurve curveType2;          // �U���̎��g��
    [SerializeField, Header("�J��Ԃ���")]
    private int repetition2 = 1;         // �J��Ԃ���

    // ���g�̃R���|�[�l���g
    private Rigidbody rb;       // ���W�b�g�{�f�B
    public Rigidbody GetRigidbody() => rb;
    private Animator animator;  // �A�j���[�^�[
    public Animator GetAnimator() => animator;

    // �J�E���g�_�E���p�N���X
    private CS_Countdown countdown;
    private CS_Countdown countdownGoal;
    private CS_Countdown countdownStun;

    [SerializeField, Header("�󂫊ʎc�ʃQ�[�W")]
    private UnityEngine.UI.Image CanGage;

    private CS_GoalDoor goal;

    //[SerializeField, Header("���U���g�p�l��")]
    //private CS_Result result;


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

        //�󂫊ʃQ�[�W�̐ݒ�
        CanGage.fillAmount = nowMP / MaxMP;


        // ���g�̃R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // �C���X�y�N�^�[�r���[�������̃I�u�W�F�N�g���擾
        cameraTransform = GameObject.Find("Main Camera")?.transform;
        if (cameraTransform == null)
        {
            UnityEngine.Debug.LogError("Main Camera��Hierarchy�ɒǉ����Ă��������B");
        }

        //inputSystem = GameObject.Find("InputSystem")?.GetComponent<CS_InputSystem>();
        //if (inputSystem == null)
        //{
        //    UnityEngine.Debug.LogError("InputSystem��Hierarchy�ɒǉ����Ă��������B");
        //}

        //cameraManager = GameObject.Find("CameraManager")?.GetComponent<CS_CameraManager>();
        //if (cameraManager == null)
        //{
        //    UnityEngine.Debug.LogError("CameraManager��Hierarchy�ɒǉ����Ă��������B");
        //}

        //tpsCamera = GameObject.Find("TpsCamera")?.GetComponent<CS_TpsCamera>();
        //if (tpsCamera == null)
        //{
        //    //UnityEngine.Debug.LogError("TpsCamera��Hierarchy�ɒǉ����Ă��������B");
        //}



        //GameObject resultCanvas = GameObject.Find("ResultCanvas");

        //if (resultCanvas == null)
        //{
        //    UnityEngine.Debug.LogError("ResultCanvas��Hierarchy�ɒǉ����Ă��������B");
        //}
        //else
        //{
        //    Transform resultPanelTransform = resultCanvas.transform.Find("ResultPanel");
        //    if (resultPanelTransform != null)
        //    {
        //        result = resultPanelTransform.GetComponent<CS_Result>();
        //        if (result == null)
        //        {
        //            UnityEngine.Debug.LogError("ResultPanel��CS_Result�R���|�[�l���g���A�^�b�`����Ă��邩�m�F���Ă��������B");
        //        }
        //    }
        //    else
        //    {
        //        UnityEngine.Debug.LogError("ResultPanel��Hierarchy�ɒǉ����Ă��������B");
        //    }
        //}

    }

    void FixedUpdate()
    {
        //�Q�[�W�̏�Ԃ��X�V
        CanGage.fillAmount = (float)nowMP / (float)MaxMP;

        // �Q�[���I�[�o�[
        if (nowHP <= 0)
        {
            animator.SetBool("GameOver", true);
            CSGE_GameOver.GameOver();
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
            // ���n����
            if (IsGrounded() && Mathf.Approximately(acceleration.y, 0) && oldAcceleration.y != 0)
            {
                if ((oldAcceleration.y < -fallingThreshold))
                {
                    animator.SetFloat("Falling", 1);
                    countdown.Initialize(0.5f);

                    // �G�t�F�N�g�Đ�
                    CS_ParticleStarter.StartParticleSystemAtIndex(1);
                }
                else
                {
                    animator.SetFloat("Falling", 0);
                }
            }
        }
        oldAcceleration = acceleration;


        // �S�[�����[�V�������I�����Ă��烊�U���g��\��
        //if (countdownGoal.IsCountdownFinished() && animator.GetBool("Goal"))
        if(goal != null)
        {
            if (goal.GetEnd())
            {
                //result.StartResult();
                result.enabled = true;
                animator.SetBool("Goal", false);
            }
        }

        // �C����
        if (countdownStun.IsCountdownFinished() && animator.GetBool("Stun"))
        {
            animator.SetBool("Stun", false);
        }
        
        if(animator.GetBool("Stun"))
        {
            CS_ParticleStarter.StartParticleSystemAtIndex(3);
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

            // �R���g���[���[��U��
            CS_ControllerVibration.StartVibrationWithCurve(duration, powerType, curveType, repetition);

            //ingredientsStock++;
            SetMP(nowMP + item.GetMP());
             //ingredientsStock * 2;
            //itemStock++;

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "BigCanItem")
        {
            nowBigCan++;

            // �R���g���[���[��U��
            CS_ControllerVibration.StartVibrationWithCurve(duration1, powerType1, curveType1, repetition1);

        }
        else if (collision.gameObject.tag == "StunObject")
        {
            animator.SetBool("Stun", true);

            Vector3 reverseForce = collision.contacts[0].normal * 10.0f;
            reverseForce.y = 0f;
            GetRigidbody().AddForce(reverseForce, ForceMode.Impulse);

            countdownStun.Initialize(3);

            // �R���g���[���[��U��
            CS_ControllerVibration.StartVibrationWithCurve(duration2, powerType2, curveType2, repetition2);

        }

    }

    void OnTriggerStay(Collider collision)
    {
        if ((collision.gameObject.tag == "Goal")&&(animator.GetBool("Mount")))
        {
            if (GetInputSystem().GetButtonBPressed() && !animator.GetBool("Goal"))
            {
                TemporaryStorage.DataSave("ingredientsStock", ingredientsStock);
                animator.SetBool("Goal", true);

                countdownGoal.Initialize(2);

                goal = collision.gameObject.GetComponent<CS_GoalDoor>();
                goal.Open(2);
            }

            //result.StartResult();
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
        float radius = 0.125f;              // �`�F�b�N���锼�a
        float groundCheckDistance = 0.0f;   // �n�ʂƂ̋���

        // �n�ʔ���
        Collider[] colliders = Physics.OverlapSphere(transform.position - Vector3.up * groundCheckDistance, radius, groundLayer);

        // �g���K�[�ƏՓ˂����ꍇ�� false ��Ԃ�
        bool notTrigger = true;
        foreach (var collider in colliders)
        {
            notTrigger = !collider.isTrigger;
        }

        if(notTrigger == false)return false;

        // �g���K�[�łȂ��ꍇ�A�n�ʂƐڐG���Ă���Ɣ���
        return colliders.Length > 0;
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
        float groundCheckDistance = 0.0f;

        // �ڒn��Ԃ�F�ŉ���
        if (IsGrounded())
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        // Sphere��`��
        Gizmos.DrawWireSphere(transform.position - Vector3.up * groundCheckDistance, radius);
    }
}
