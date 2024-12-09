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
    [SerializeField, Header("�J�n���̃J����")]
    private int initIndex = 0;
    [Header("�g�p����J������ݒ�")]
    public CinemachineVirtualCamera[] virtualCameras;// �J�����̃��X�g

    // ���g�̃R���|�[�l���g
    private CinemachineImpulseSource impulseSource;// �U��

    private int nowIndex; // ���݂̃J�����C���f�b�N�X

    //**
    //* ������
    //**

    void Start()
    {
        // ���g�̃R���|�[�l���g���擾
        impulseSource = GetComponent<CinemachineImpulseSource>();

        // �J�n���̃J������ݒ�
        SwitchingCamera(initIndex);
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
    //* in:�؂�ւ���
    //* out:����
    //**

    public void SwitchingCamera(int index)
    {
        nowIndex = index;

        // �S�ẴJ������Priority��������
        foreach (var virtualCamera in virtualCameras)
        {
            virtualCamera.Priority = -1;
        }
        // �Ώۂ̃J������Priority���グ��
        virtualCameras[index].Priority = 10;
    }

    /*
     * ���݂̃J�������擾����
     */

    public int NowCameraNumber()
    {
        return nowIndex;
    }
}
