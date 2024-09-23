using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Numerics;

//using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

// �v���C���[����
public class CS_Player : MonoBehaviour
{
    //**
    //* �ϐ�
    //**

    // �O���I�u�W�F�N�g
    [Header("�O���I�u�W�F�N�g")]
    public Transform cameraTransform;// �ǔ��J����

    // �W�����v
    [Header("�W�����v�ݒ�")]
    public float jumpForce = 5f;                // �W�����v��
    public float groundCheckDistance = 0.1f;    // �n�ʂƂ̋���
    public string targetTag = "Ground";         // �n�ʃ^�O

    // �ړ�
    [Header("�ړ��ݒ�")]
    public float speed = 1f;        // �ړ����x
    public float targetSpeed = 10f; // �ڕW���x
    public float smoothTime = 0.5f; // �ō����x�ɓ��B����܂ł̎���
    private float velocity = 0f;    // ���݂̑��x
    private Vector3 moveVec;        // ���݂̈ړ�����
    private float initSpeed;        // �X�s�[�h�̏����l��ۑ����Ă����ϐ�

    // �Đ����鉹���t�@�C���̃��X�g
    [Header("���ʉ��ݒ�")]
    public AudioSource[] audioSource;
    public AudioClip[] audioClips;

    // ���g�̃R���|�[�l���g
    private Rigidbody rb;
    private Animator animator;



    [SerializeField, Header("��C�C�̒e�I�u�W�F�N�g")]
    private GameObject AirBall;

    [SerializeField, Header("���h���̒����Ԋu")]
    [Header("���U����/�����Ԋu")]
    private const float Injection_Interval = 0.5f;

    //�����Ԋu�v�Z�p
    private float Injection_IntarvalTime = 0.0f;

    private bool HitBurstObjFlag = false;
    [SerializeField, Tooltip("���h���ς�[")]
    private int InjectionPower = 1;

    private bool InjectionState = false;

    //�e����I�u�W�F�N�g�̃X�N���v�g
    CS_Burst_of_object csButstofObj;

    //**
    //* ������
    //**
    void Start()
    {
        // �ړ����x�̏����l��ۑ�
        initSpeed = speed;

        // ���g�̃R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    //**
    //* �X�V
    //**
    void Update()
    {
        // �ړ�����
        HandleMovement();
        // �W�����v����
        HandleJump();

        
        AirInjection("ButtonB");
        AirGun("ButtonX");
    }

    //**
    //* �ړ�����
    //*
    //* in�F����
    //* out�F����
    //**
    void HandleMovement()
    {
        // L�X�e�b�N�̓��͂��`�F�b�N
        if (IsStickActive(-0.01f, 0.01f))
        {
            // �X�e�B�b�N�̓��͂��擾
            Vector3 moveVec = GetMovementVector();
            // �ʒu���X�V
            MoveCharacter(moveVec);
            // �O���������X���[�Y�ɒ���
            AdjustForwardDirection(moveVec);
            animator.SetBool("Move", true);
        }
        else
        {
            // 0�ԃC���f�b�N�X�̌��ʉ����~
            StopPlayingSound(0);

            // �ړ����x��������
            speed = initSpeed;

            // �A�j���[�^�[�̒l��ύX
            animator.SetBool("Move", false);
            animator.SetBool("Dash", false);
        }
    }

    //**
    //* �W�����v����
    //*
    //* in�F����
    //* out�F����
    //**
    void HandleJump()
    {
        // �ڒn����ƃW�����v�{�^���̓��͂��`�F�b�N
        if (IsGrounded() && Input.GetButtonDown("ButtonA"))
        {
            // ���ʉ����Đ�
            PlaySoundEffect(1,1);

            // �W�����v�͂�������
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // �A�j���[�^�[�̒l��ύX
            animator.SetBool("Jump", true);
        }
        else
        {
            // �A�j���[�^�[�̒l��ύX
            animator.SetBool("Jump", false);
        }
    }

    //**
    //* ���X�e�B�b�N�̓��͂��`�F�b�N����
    //*
    //* in�F�f�b�h�]�[��
    //* out�F���͔���
    //**
    bool IsStickActive(float min, float max)
    {
        float stickV = Input.GetAxis("LStick X");
        float stickH = Input.GetAxis("LStick Y");
        return !(((stickH < max) && (stickH > min)) && ((stickV < max) && (stickV > min)));
    }

    //**
    //* �X�e�B�b�N���͂���ړ��x�N�g�����擾����
    //*
    //* in�F����
    //* out�F�ړ��x�N�g��
    //**
    Vector3 GetMovementVector()
    {
        float stickH = Input.GetAxis("LStick X");
        float stickV = Input.GetAxis("LStick Y");
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        Vector3 moveVec = forward * stickV + right * stickH;
        moveVec.y = 0f; // y ���̈ړ��͕s�v
        return moveVec.normalized;
    }

    //**
    //* �L�����N�^�[�̈ʒu���X�V����
    //*
    //* in�F�ړ��x�N�g��
    //* out�F����
    //**
    void MoveCharacter(Vector3 moveVec)
    {
        // L�g���K�[�̓��͒��͉�������
        float tri = Input.GetAxis("LRTrigger");
        if (tri > 0)
        {
            speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocity, smoothTime);

            animator.SetBool("Dash", true);
        }

        // ���ʉ����Đ�����
        if (animator.GetBool("Dash"))
        {
            // �_�b�V��
            PlaySoundEffect(0, 2);
        }
        else if (animator.GetBool("Move"))
        {
            // �ړ�
            PlaySoundEffect(0, 0);
        }

        // �v���C���[�̈ʒu���X�V
        transform.position += moveVec * speed * Time.deltaTime;
    }

    //**
    //* �L�����N�^�[��i�s�����Ɍ�����
    //*
    //* in�F�ړ��x�N�g��
    //* out�F����
    //**
    void AdjustForwardDirection(Vector3 moveVec)
    {
        if (moveVec.sqrMagnitude > 0)
        {
            Vector3 targetForward = moveVec.normalized;
            transform.forward = Vector3.Lerp(transform.forward, targetForward, 0.1f);
        }
    }

    //**
    //* �n�ʂɐڒn���Ă��邩�𔻒f����
    //*
    //* in�F����
    //* out�F�ڒn����
    //**
    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            // �q�b�g�����I�u�W�F�N�g�̃^�O���m�F����
            if (hit.collider.CompareTag(targetTag))
            {
                return true;
            }
        }

        return false;
    }

    //**
    //* �����t�@�C�����Đ��\���`�F�b�N����
    //*
    //* in�F�Đ����鉹���t�@�C���̃C���f�b�N�X
    //* out:�Đ��\���`�F�b�N
    //**
    bool CanPlaySound(int indexSource, int indexClip)
    {
        return audioSource[indexSource] != null
               && audioClips != null
               && indexClip >= 0
               && indexClip < audioClips.Length
               && (!audioSource[indexSource].isPlaying || audioSource[indexSource].clip != audioClips[indexClip]);
    }

    //**
    //* �����t�@�C�����Đ�����
    //*
    //* in�F�Đ����鉹���t�@�C���̃C���f�b�N�X
    //* out:����
    //**
    void PlaySoundEffect(int indexSource,int indexClip)
    {
        // �T�E���h�G�t�F�N�g���Đ�
        if (CanPlaySound(indexSource, indexClip))
        {
            audioSource[indexSource].clip = audioClips[indexClip];
            audioSource[indexSource].Play();
        }
    }

    //**
    //* �����t�@�C�����~����
    //*
    //* in�F����
    //* out:����
    //**
    void StopPlayingSound(int indexSource)
    {
        // �T�E���h�G�t�F�N�g���~
        if (audioSource[indexSource].isPlaying)
        {
            audioSource[indexSource].Stop();
        }
    }

    bool IsPlayingSound(int indexSource)
    {
        return audioSource[indexSource].isPlaying;
    }
    //----------------------------
    // ��C�C�֐�
    // ����:���̓L�[,�I�u�W�F�N�g�ɋ߂Â��Ă��邩
    // �߂�l�F�Ȃ�
    //----------------------------
    void AirGun(string button)
    {
        //���ˉ\��(�L�[�������ꂽ�u��&�I�u�W�F�N�g�ɋ߂Â��Ă��Ȃ�)
        bool StartShooting = Input.GetButtonDown(button) && !HitBurstObjFlag;

        if (!StartShooting) { return; }

        //SE���Đ�����Ă�����~�߂�
        if (IsPlayingSound(1)) { StopPlayingSound(1); }

        //PlaySoundEffect(1, 3);

        Vector3 forwardVec  = cameraTransform.forward;

        //���͂�����Βe�𐶐�
        //�|�C���^�̈ʒu����@Instantiate(AirBall,transform.pointa);
        GameObject ballobj = Instantiate(AirBall);

        Vector3 pos = Vector3.zero;
        float scaler = 2.0f;
        Vector3 offset = new Vector3(0, 1, 0);

        pos = this.transform.position;
        pos += offset;
        pos += forwardVec * scaler;
        ballobj.transform.position = pos;
        ballobj.transform.forward = forwardVec;

    }
    //----------------------------
    // ���h��(��C����)�֐�
    // ����:���̓L�[,�߂Â��Ă��邩,�߂Â��Ă���I�u�W�F�N�g�̈���,�߂Â��Ă���I�u�W�F�N�g�̑ϋv�l
    // �߂�l�F�Ȃ�
    //----------------------------
    void AirInjection(string button)
    {
        //�����\��(�L�[�����͂���Ă��ăI�u�W�F�N�g�ɋ߂Â��Ă���)
        bool Injection = Input.GetButtonDown(button) && HitBurstObjFlag;

        if (!Injection)
        {
            return;
        }
        else
        {
            StopPlayingSound(1);    //�������Ă�����~�߂�
            PlaySoundEffect(1, 4);  //�}��SE
            InjectionState = true;  //�������̃t���O��On
        }

        //����������Ȃ���ΏI��
        if (!InjectionState)
        {
            return;
        }

        PlaySoundEffect(1, 6);  //�}��SE

        //�{�^���������ꂽ or �Ώۂ����ł�����I��
        InjectionState = !(Input.GetButtonUp(button) || !csButstofObj);

        //���Ԍv��
        Injection_IntarvalTime += Time.deltaTime;
        bool TimeProgress = Injection_IntarvalTime > Injection_Interval;   //�����Ԋu�����Ԍo�߂��Ă��邩
        if (!TimeProgress) { return; }

        Injection_IntarvalTime = 0.0f;  //���Ԃ����Z�b�g

        if (!csButstofObj)
        {
            Debug.LogWarning("null");
            return;
        }

        //���͂��ő�ɂȂ����火
        bool MaxPressure = csButstofObj.AddPressure(InjectionPower);

        //�ő�ɂȂ����璍���I��
        if (MaxPressure)
        {
            InjectionState = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        bool isHitBurstObj = collision.gameObject.tag == "Burst";
        if (isHitBurstObj)
        {
            csButstofObj = collision.transform.GetComponent<CS_Burst_of_object>();
            HitBurstObjFlag = true;
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit(Collision collision)
    {
        bool isHitBurstObj = collision.gameObject.tag == "Burst";
        if (isHitBurstObj) 
        {
            csButstofObj = null;
            HitBurstObjFlag = false; 
            return; 
        }

    }

}