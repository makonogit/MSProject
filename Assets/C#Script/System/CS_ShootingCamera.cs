using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using System.Diagnostics;

public class CS_ShootingCamera : ActionBase
{
    //**
    //* �ϐ�
    //**

    // �O���I�u�W�F�N�g
    [Header("�O���I�u�W�F�N�g")]
    public Transform target; // �ǔ��Ώ�
    public Transform focus;  // �ǔ��Ώ�
    public CS_InputSystem inputSystem;// �C���v�b�g�}�l�[�W���[


    // �ړ��E��]
    [Header("�ʒu")]
    public Vector3 offsetPos = new Vector3(0, 8, 0);// �ʒu
    public Vector3 offsetFocus = new Vector3(0, 3, 0);// �œ_
    [Header("�J�������x �c/��")]
    public float wideSpeed = 90.0f;             // ���X�s�[�h
    public float hydeSpeed = 40.0f;             // �c�X�s�[�h
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
    [SerializeField, Header("���͔͈�")]
    private float maxInputValue = 1f;         // �X�e�B�b�N�̍ő���͒l
    [SerializeField]
    private float minInputValue = 0.3f;         // �X�e�B�b�N�̍ŏ����͒l
    private float currentAcceleration = 0f;  // ���݂̉����x
    [SerializeField, Header("�Ə����̊��x�{��")]
    private float adsWeight = 0.75f;
    private bool isAds = false; // �Ə����

    [Header("�A�V�X�g�ݒ�")]
    [SerializeField, Header("���m�^�O")]
    private List<string> targetTag;
    //[SerializeField, Header("��Q�����C���[")]
    private LayerMask mask;
    [SerializeField, Header("���m����")]
    private float range = 100f;
    [SerializeField, Header("���o�͈�")]
    float radius = 0.5f;
    [SerializeField, Header("�P�̂ւ̃A�V�X�g�{��")]
    private float assistVal = 0.3f;
    //[SerializeField, Header("�����̂ւ̃A�V�X�g�{��")]
    private float manyAssistVal = 0.7f;
    private int enemyCount = 0;
    [SerializeField, Header("����Ԋu")]
    private float checkInterval = 0.5f;
    private float nextCheckTime = 0;
    private bool isOldCheck = false;
    [SerializeField, Header("�A�V�X�g���")]
    private bool isAssist = false;

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
        // ADS����
        if (inputSystem.GetLeftTrigger() > 0.1f)
        {
            cameraDistance = Mathf.Lerp(cameraDistance, 0.75f, 0.1f);
            isAds = true;
        }
        else
        {
            cameraDistance = Mathf.Lerp(cameraDistance, 2.0f, 0.1f);
            isAds = false;
        }

        // �A�V�X�g���X�V
        isAssist = IsAssist();

        // �J�������䏈��
        HandlCamera();
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

    /*
     * �J��������
     */
    void HandlCamera()
    {
        // ���͂ɉ����ăJ��������]������

        // �E�X�e�B�b�N�̓��͂��擾
        Vector2 stick = inputSystem.GetRightStick();

        if (Mathf.Abs(stick.x) < minInputValue)
        {
            stick.x = 0f;
        }
        if (Mathf.Abs(stick.y) < minInputValue)
        {
            stick.y = 0f;
        }

        // ���͂̑傫�����v�Z
        float inputMagnitude = new Vector2(stick.y, stick.x).magnitude;
        float normalizedInput = Mathf.Clamp(inputMagnitude / maxInputValue, 0f, 1f);
        currentAcceleration = accelerationCurve.Evaluate(normalizedInput);

        // �J��������͂ɉ����ĉ�]
        if ((offsetFocus.y > -5) && (offsetFocus.y < 5f))
        {
            float speedX = wideSpeed;
            if (isAds) speedX *= adsWeight;
            if (isAssist) speedX *= assistVal;
            Vector3 rotVec = new Vector3(0, stick.x, 0);
            rotVec = rotVec.normalized;
            Vector3 rot = target.rotation.eulerAngles;
            rot += speedX * rotVec * currentAcceleration * Time.deltaTime;
            target.rotation = Quaternion.Euler(rot);

            float speedY = hydeSpeed;
            if (isAds) speedY *= adsWeight;
            if (isAssist) speedY *= assistVal;
            float movePos = offsetFocus.y + speedY * stick.y * currentAcceleration * Time.deltaTime;
            offsetFocus.y = Mathf.Lerp(offsetFocus.y, movePos, 0.1f);
        }

        // �^�[�Q�b�g�̔w�ʂɃJ������z�u
        Vector3 targetPosition = target.position - (target.forward * cameraDistance);
        transform.position = targetPosition + offsetPos;

        UpdateCameraRotation();

        if (offsetFocus.y < -5)
        {
            offsetFocus.y = -5f;
        }
        if (offsetFocus.y > 5f)
        {
            offsetFocus.y = 5f;
        }

    }

    /*
     * �A�V�X�g��Ԃ��m�F����
     */
    bool IsAssist()
    {
        if (Time.time >= nextCheckTime)
        {
            // �����Ԃ�������
            isOldCheck = false;

            // ���C�̔���Ԋu���X�V
            nextCheckTime = Time.time + checkInterval;

            // �J�������ʂ��烌�C���쐬
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            Vector3 boxSize = new Vector3(radius, radius, radius);

            if (Physics.BoxCast(ray.origin, boxSize / 2, ray.direction, out hit, Quaternion.identity, range))
            {
                if (targetTag.Contains(hit.collider.tag))
                {
                    isOldCheck = true;
                }
            }
        }

        return isOldCheck;
    }
}
