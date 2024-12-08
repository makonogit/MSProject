using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * ��莞�Ԍ�ɐ؂�ւ����s���J����
 */

public class CS_TemporaryCamera : MonoBehaviour
{
    [Header("�I�u�W�F�N�g�ݒ�")]
    [SerializeField, Header("�J�����}�l�[�W���[")]
    private CS_CameraManager cameraManager;

    [Header("�p�����[�^�[�ݒ�")]
    [SerializeField, Header("�J�ڂ܂ł̑ҋ@����")]
    private float delayTime = 3f;
    [SerializeField, Header("�I����ɕύX����J�����̔ԍ�")]
    private int nextCameraIndex = 0;

    private CinemachineVirtualCamera currentVirtualCamera;  // VirtualCamera

    private CS_Countdown countdown;

    private int oldPriority;

    // Start is called before the first frame update
    void Start()
    {
        countdown = gameObject.AddComponent<CS_Countdown>();

        currentVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        oldPriority = currentVirtualCamera.Priority;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentVirtualCamera.Priority == 10)
        {
            if (oldPriority == -1)
            {
                countdown.Initialize(delayTime);
            }

            if (countdown.IsCountdownFinished())
            {
                cameraManager.SwitchingCamera(nextCameraIndex);
            }
        }

        oldPriority = currentVirtualCamera.Priority;
    }
}
