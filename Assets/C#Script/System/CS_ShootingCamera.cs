using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CS_ShootingCamera : ActionBase
{
    //**
    //* �ϐ�
    //**

    // �O���I�u�W�F�N�g
    [Header("�O���I�u�W�F�N�g")]
    public Transform target; // �ǔ��Ώ�
    public Transform focus; // �ǔ��Ώ�
    public CS_InputSystem inputSystem;// �C���v�b�g�}�l�[�W���[


    // �ړ��E��]
    [Header("�ʒu")]
    public Vector3 offsetPos = new Vector3(0, 8, 0);// �ʒu
    public Vector3 offsetFocus = new Vector3(0, 3, 0);// �œ_
    [Header("�ړ�/��]�̑���")]
    public float wideSpeed = 2.0f;             // ���X�s�[�h
    public float hydeSpeed = 0.25f;             // �c�X�s�[�h
    public float rotationSpeed = 50.0f;         // ��]�X�s�[�h
    [Header("�ړ�/��]�̐���")]
    public float rotationLimitMax = 80.0f; // X����]�̐����i�ő�j
    public float rotationLimitMin = 10.0f; // X����]�̐����i�ŏ��j
    private float cameraRotX = 0.0f;     // X����]�̈ړ���
    private float cameraRotY = 0.0f;       // Y���]�̈ړ���
    [Header("�J�����̋���")]
    public float cameraDistance = 5.0f;
    [SerializeField, Header("�����Ȑ�")]
    private AnimationCurve accelerationCurve;       // �����Ȑ�
    [SerializeField, Header("���͔͈͂̌��E")]
    private float maxInputValue = 1f;         // �X�e�B�b�N�̍ő���͒l
    private float currentAcceleration = 0f;  // ���݂̉����x

    // ���g�̃R���|�[�l���g
    private CinemachineVirtualCamera camera;

    //**
    //* ������
    //**
    void Start()
    {
        // �J�����R���|�[�l���g���擾
        camera = GetComponent<CinemachineVirtualCamera>();
    }

    //**
    //* �X�V
    //**
    void FixedUpdate()
    {
        // ���͂ɉ����ăJ��������]������

        // �E�X�e�B�b�N�̓��͂��擾
        Vector2 stick = inputSystem.GetRightStick();

        // ���͂̑傫�����v�Z
        float inputMagnitude = new Vector2(stick.y, stick.x).magnitude;
        float normalizedInput = Mathf.Clamp(inputMagnitude / maxInputValue, 0f, 1f);
        currentAcceleration = accelerationCurve.Evaluate(normalizedInput);

        // �J��������͂ɉ����ĉ�]
        if ((offsetFocus.y > -5) && (offsetFocus.y < 5f))
        {
            Vector3 rotVec = new Vector3(0, stick.x, 0);
            rotVec = rotVec.normalized;
            Vector3 rot = target.rotation.eulerAngles;
            rot += wideSpeed * rotVec * currentAcceleration * Time.deltaTime;
            target.rotation = Quaternion.Euler(rot);
        }

        // �^�[�Q�b�g�̔w�ʂɃJ������z�u
        Vector3 targetPosition = target.position - (target.forward * cameraDistance);
        transform.position = targetPosition + offsetPos;

        UpdateCameraRotation();

        float movePos = offsetFocus.y + hydeSpeed * stick.y * currentAcceleration * Time.deltaTime;
        offsetFocus.y = Mathf.Lerp(offsetFocus.y, movePos, 0.1f);

        if (offsetFocus.y < -5) 
        { 
            offsetFocus.y = -5f;
        }
        if (offsetFocus.y > 5f) 
        { 
            offsetFocus.y = 5f; 
        }
    }

    //**
    //* �J�����̉�]���X�V����
    //* in�F����
    //* out�F����
    //**
    void UpdateCameraRotation()
    {
        Vector3 directionToTarget = (focus.position + offsetFocus) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
