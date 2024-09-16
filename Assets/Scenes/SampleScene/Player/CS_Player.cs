using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
    public Transform cameraTransform;// ���C���J����

    // �W�����v
    public float jumpForce = 5f;                // �W�����v��
    public float groundCheckDistance = 0.1f;    // �n�ʂƂ̋���
    public string targetTag = "Ground";         // �n�ʃ^�O

    // �ړ�
    public float speed = 1f;     // �ړ����x
    public float targetSpeed = 10f; // �ڕW���x
    public float smoothTime = 0.5f; // �ō����x�ɓ��B����܂ł̎���
    private float velocity = 0f;    // ���݂̑��x
    private Vector3 moveVec;        // ���݂̈ړ�����

    // ���g�̃R���|�[�l���g
    private Rigidbody rb;
    private Animator animator;
    public AudioSource[] audioSource;

    // �Đ����鉹���t�@�C���̃��X�g
    public AudioClip[] audioClips;

    //**
    //* ������
    //**
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        //audioSource = GetComponent<AudioSource>();
    }

    //**
    //* �X�V
    //**
    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    //**
    //* �ړ�����
    //*
    //* in�F����
    //* out�F����
    //**
    void HandleMovement()
    {
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
            StopPlayingSound(0);

            speed = 1f;

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
        if (IsGrounded() && Input.GetButtonDown("ButtonA"))
        {
            PlaySoundEffect(1,1);

            // �W�����v�͂�������
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("Jump", true);
        }
        else
        {
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
            PlaySoundEffect(0, 2);
            speed = Mathf.SmoothDamp(speed, targetSpeed, ref velocity, smoothTime);

            animator.SetBool("Dash", true);
        }
        else
        {
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
    //* �����t�@�C�����Đ�����
    //*
    //* in�F�Đ����鉹���t�@�C���̃C���f�b�N�X
    //* out:����
    //**
    void PlaySoundEffect(int indexSource,int indexClip)
    {
        // �T�E���h�G�t�F�N�g���Đ�
        if (audioSource[indexSource] != null && audioClips != null && indexClip >= 0 && indexClip < audioClips.Length)
        {
            if (!audioSource[indexSource].isPlaying || audioSource[indexSource].clip != audioClips[indexClip])
            {
                audioSource[indexSource].clip = audioClips[indexClip];

                audioSource[indexSource].Play();
            }
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

}