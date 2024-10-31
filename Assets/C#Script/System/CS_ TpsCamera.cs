using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Cinemachine;

// TPS�J����
public class CS_TpsCamera : MonoBehaviour
{
    //**
    //* �ϐ�
    //**

   

    // �J�����ݒ�
    [Header("���x�ݒ�")]
    [SerializeField, Header("��{���x")]
    private float cameraSpeed = 50.0f;          // ��{���x
    public float GetCameraSpeed() => cameraSpeed;
    [SerializeField, Header("�����Ȑ�")]
    private AnimationCurve accelerationCurve;       // �����Ȑ�
    [SerializeField]
    private AnimationCurve accelerationCurveAssist; // �G�C���A�V�X�g���̔����Ȑ�
    private bool isAssist = false;                  // �A�V�X�g�t���O
    public bool GetAssist() => isAssist;
    public void SetAssist(bool flg) { isAssist = flg; }
    [SerializeField, Header("���͔͈͂̌��E")]
    private float maxInputValue = 1f;         // �X�e�B�b�N�̍ő���͒l
    private float currentAcceleration = 0f;  // ���݂̉����x
    [Header("�I�t�Z�b�g")]
    [SerializeField, Header("�ʒu")]
    private Vector3 offsetPos = new Vector3(0, 8, 0);// �ʒu
    [SerializeField, Header("�œ_")]
    private Vector3 offsetFocus = new Vector3(0, 3, 0);// �œ_
    [Header("�J�����̈ړ�����")]
    [SerializeField, Header("X����]�̐����i�ő�j")]
    private float rotationLimitMax = 80.0f; // X����]�̐����i�ő�j
    [SerializeField, Header("X����]�̐����i�ŏ��j")]
    private float rotationLimitMin = 10.0f; // X����]�̐����i�ŏ��j
    private float cameraRotX = 45.0f;       // X����]�̈ړ���
    private float cameraRotY = 0.0f;        // Y���]�̈ړ���

    // �O���I�u�W�F�N�g
    [Header("�O���I�u�W�F�N�g")]
    [SerializeField, Header("�^�[�Q�b�g��")]
    string targetName = "Player";       // �^�[�Q�b�g��
    private Transform target;           // �ǔ��Ώ�
    private CS_InputSystem inputSystem; // �C���v�b�g�}�l�[�W���[

    // ���g�̃R���|�[�l���g
    private CinemachineVirtualCamera camera;

    //===========�ǉ��F��
    public float CameraSpeed
    {
        set
        {
            cameraSpeed = value;
        }
        get
        {
            return cameraSpeed;
        }
    }

    //**
    //* ������
    //**
    void Start()
    {
        // �J�����R���|�[�l���g���擾
        camera = GetComponent<CinemachineVirtualCamera>();

        // �^�[�Q�b�g��ݒ�
        target = GameObject.Find(targetName).transform;

        // �C���v�b�g�V�X�e���ݒ�
        inputSystem = GameObject.Find("InputSystem").GetComponent<CS_InputSystem>();

        // �J�����������ʒu�ɃZ�b�g
        CameraReset();
    }

    //**
    //* �X�V
    //**
    void FixedUpdate()
    {
        // ���͂ɉ����ăJ��������]������

        if (camera.Priority == 10)
        {
            // �E�X�e�B�b�N�̓��͂��擾
            Vector2 stick = inputSystem.GetRightStick();

            // ���͂̑傫�����v�Z
            float inputMagnitude = new Vector2(stick.y, stick.x).magnitude;
            float normalizedInput = Mathf.Clamp(inputMagnitude / maxInputValue, 0f, 1f);
            if (isAssist)
            {
                currentAcceleration = accelerationCurveAssist.Evaluate(normalizedInput);
            }
            else
            {
                currentAcceleration = accelerationCurve.Evaluate(normalizedInput);
            }

            // �J��������͂ɉ����Ĉړ�
            Vector3 rotVec = new Vector3(stick.y, stick.x, 0);
            rotVec = rotVec.normalized;
            MoveCamera(rotVec);
        }
    }

    //**
    //* �J�����̈ړ��ʂ����Z�b�g����
    //*
    //* in�F����
    //* out�F����
    //**
    public void CameraReset()
    {
        // �^�[�Q�b�g�̔w�ʂɃJ������z�u
        Vector3 targetPosition = target.position - target.forward;
        transform.position = targetPosition + offsetPos;
    }

    //**
    //* �J�������^�[�Q�b�g�𒆐S�Ɉړ�������
    //*
    //* in�F�ړ����������
    //* out�F����
    //**
    void MoveCamera(Vector3 direction)
    {
        // �J�����̈ʒu���X�V
        UpdateCameraPosition(direction);

        // �J�����̊p�x���X�V
        UpdateCameraRotation();
    }

    //**
    //* �J�����̈ʒu���X�V����
    //*
    //* in�F�ړ����������
    //* out�F����
    //**
    void UpdateCameraPosition(Vector3 direction)
    {
        // �ړ����x
        float rotationAmount = cameraSpeed * currentAcceleration * Time.deltaTime;

        // �ړ�����
        Vector3 normalizedDirection = direction.normalized;

        // X���̉�]���X�V
        if (Mathf.Abs(Vector3.Dot(normalizedDirection, Vector3.right)) > 0.1f)
        {
            cameraRotX += rotationAmount * Mathf.Sign(Vector3.Dot(normalizedDirection, Vector3.right));
            cameraRotX = Mathf.Clamp(cameraRotX, rotationLimitMin, rotationLimitMax);
        }

        // Y���̉�]���X�V
        if (Mathf.Abs(Vector3.Dot(normalizedDirection, Vector3.up)) > 0.1f)
        {
            cameraRotY += rotationAmount * Mathf.Sign(Vector3.Dot(normalizedDirection, Vector3.up));
        }

        // �J�����̈ʒu�����炩�ɍX�V
        Quaternion rotation = Quaternion.Euler(cameraRotX, cameraRotY, 0);
        Vector3 offset = rotation * offsetPos;
        Vector3 desiredPosition = target.position + offset + offsetFocus;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, 10.0f * Time.deltaTime);
    }

    //**
    //* �J�����̉�]���X�V����
    //* in�F����
    //* out�F����
    //**
    void UpdateCameraRotation()
    {
        // �^�[�Q�b�g�̕����������悤�ɃJ��������]������
        Vector3 directionToTarget = (target.position + offsetFocus) - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSpeed * Time.deltaTime);
    }
}
