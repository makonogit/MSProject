using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * �g���b�N����ړ�����J����
 * 
 * �S���F�����V�S
 */

public class CS_TrackCamera : MonoBehaviour
{
    private CinemachineVirtualCamera currentVirtualCamera;  // VirtualCamera
    private CinemachineDollyCart dollyCart;                 // Dolly�J�[�g

    private float reachThreshold = 0.05f; // ���B�����臒l

    [Header("�I�u�W�F�N�g�ݒ�")]
    [SerializeField, Header("�J�����}�l�[�W���[")]
    private CS_CameraManager cameraManager;
    [SerializeField, Header("�J�����̈ړ��g���b�N")]
    private CinemachineSmoothPath smoothPath;

    [Header("�p�����[�^�[�ݒ�")]
    [SerializeField, Header("�ړ����x")]
    private float speed = 1f;
    [SerializeField, Header("�I����ɕύX����J�����̔ԍ�")]
    private int nextCameraIndex = 0;
    [SerializeField, Header("�I�����Active�ɂ���I�u�W�F�N�g")]
    private GameObject[] activeObj;

    void Start()
    {
        // �����̃R���|�[�l���g���擾
        currentVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        dollyCart = GetComponent<CinemachineDollyCart>();

        foreach(GameObject obj in activeObj)
        {
            if (obj.activeSelf) { obj.SetActive(false); }
        }
    }

    void FixedUpdate()
    {
        if (currentVirtualCamera.Priority == 10)
        {
            dollyCart.m_Speed = speed;

            if (IsAtEndOfPath(smoothPath.PathLength))
            {
                cameraManager.SwitchingCamera(nextCameraIndex);

                dollyCart.m_Speed = 0;

                foreach (GameObject obj in activeObj)
                {
                    obj.SetActive(true); // �I�u�W�F�N�g���A�N�e�B�u�ɂ���
                }
            }
        }
    }

    private bool IsAtEndOfPath(float pathLength)
    {
        // m_Position���p�X�̏I���ɋ߂����ǂ������`�F�b�N
        float position = dollyCart.m_Position;
        return Mathf.Abs(position - pathLength) < reachThreshold;
    }
}
