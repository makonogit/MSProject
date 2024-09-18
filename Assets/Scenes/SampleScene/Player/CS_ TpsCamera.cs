using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// TPS�J����
public class CS_TpsCamera : MonoBehaviour
{
    //**
    //* �ϐ�
    //**

    // �O���I�u�W�F�N�g
    public Transform target; // �ǔ��Ώ�

    // �ړ��E��]
    public Vector3 offsetPos = new Vector3 (0, 8, 0);// �ʒu
    public Vector3 offsetFocus = new Vector3(0, 3, 0);// �œ_
    public float moveSpeed = 50.0f;             // �ړ��X�s�[�h
    public float rotationSpeed = 50.0f;         // ��]�X�s�[�h
    public float verticalRotationLimit = 80.0f; // �c��]�̐���
    private float cameraRotX = 0.0f;            // ����]�̈ړ���
    private float cameraRotY = 0.0f;            // �c��]�̈ړ���

    // ���g�̃R���|�[�l���g
    private Camera camera; 

    //**
    //* ������
    //**
    void Start()
    {
        // �J�����R���|�[�l���g���擾
        camera = GetComponent<Camera>();

        // �J�����������ʒu�ɃZ�b�g
        MoveCamera(Vector3.up);
    }

    //**
    //* �X�V
    //**
    void Update()
    {
        // ���͂ɉ����ăJ��������]������

        // �E�X�e�B�b�N�̓��͂��擾
        float rsh = Input.GetAxis("RStick X");
        float rsv = Input.GetAxis("RStick Y");

        // �J��������͂ɉ����Ĉړ�
        Vector3 rotVec = new Vector3(-rsv, rsh, 0);
        rotVec = rotVec.normalized;
        MoveCamera(rotVec);
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
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // �ړ�����
        Vector3 normalizedDirection = direction.normalized;

        // ���������̉�]���X�V
        if (Mathf.Abs(Vector3.Dot(normalizedDirection, Vector3.right)) > 0.1f)
        {
            cameraRotX += rotationAmount * Mathf.Sign(Vector3.Dot(normalizedDirection, Vector3.right));
            cameraRotX = Mathf.Clamp(cameraRotX, -verticalRotationLimit, verticalRotationLimit);
        }

        // ���������̉�]���X�V
        if (Mathf.Abs(Vector3.Dot(normalizedDirection, Vector3.up)) > 0.1f)
        {
            cameraRotY += rotationAmount * Mathf.Sign(Vector3.Dot(normalizedDirection, Vector3.up));
        }

        // �J�����̈ʒu�����炩�ɍX�V
        Quaternion rotation = Quaternion.Euler(cameraRotX, cameraRotY, 0);
        Vector3 offset = rotation * offsetPos;
        Vector3 desiredPosition = target.position + offset;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}


