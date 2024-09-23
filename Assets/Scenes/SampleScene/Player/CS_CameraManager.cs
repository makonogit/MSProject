using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// �����̃J�����𐧌䂷��
public class CS_CameraManager : MonoBehaviour
{
    //**
    //* �ϐ�
    //**

    // �O���I�u�W�F�N�g
    public CinemachineVirtualCamera[] virtualCameras;// �J�����̃��X�g

    // �p�����[�^�[
    private int cameraIndex = 0;// �g�p����J�����̃C���f�b�N�X
    private float elapsedTime = 0f; // �t���[�������J�E���g����ϐ�
    private bool switchingFlg = false;// �؂�ւ����
    private float targetTime = 0f;// �؂�ւ�����

    // ���g�̃R���|�[�l���g
    private CinemachineImpulseSource impulseSource;// �U��


    //**
    //* ������
    //**

    void Start()
    {
        // ���g�̃R���|�[�l���g���擾
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    //**
    //* �X�V
    //**

    void Update()
    {
        // �؂�ւ��X�V
        SwitchingUpdate();
    }

    //**
    //* �J������U��������
    //*
    //* in:����
    //* out:����
    //**

    public void ShakeCamera(float amplitude, float frequency)
    {
        // Impulse Source�̐ݒ�
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitude;// �U���̑傫��
        impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequency;// �U�������鎞��

        // �U�����J�n
        impulseSource.GenerateImpulse();
    }

    //**
    //* �J������؂�ւ���
    //*
    //* in:�؂�ւ���A�؂�ւ��Ă�������
    //* out:����
    //**

    public void SwitchingCamera(int index, int time)
    {
        cameraIndex = index;
        targetTime = time;
        switchingFlg = true;
    }

    // �J�����؂�ւ��̍X�V����
    void SwitchingUpdate()
    {
        // �J�����؂�ւ�
        if (switchingFlg)
        {
            if (elapsedTime == 0f)
            {
                virtualCameras[cameraIndex].gameObject.SetActive(true);
            }

            // ���s���Ԃ��J�E���g
            elapsedTime += Time.deltaTime;

            // �w�肵���t���[�����ɒB�����珈�������s
            if (elapsedTime >= targetTime)
            {
                virtualCameras[cameraIndex].gameObject.SetActive(false);
                elapsedTime = 0f;// ���s���Ԃ����Z�b�g
                targetTime = 0f;
                switchingFlg = false;
            }
        }
    }
}
