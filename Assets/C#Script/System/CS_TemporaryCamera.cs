using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * 一定時間後に切り替えを行うカメラ
 */

public class CS_TemporaryCamera : MonoBehaviour
{
    [Header("オブジェクト設定")]
    [SerializeField, Header("カメラマネージャー")]
    private CS_CameraManager cameraManager;

    [Header("パラメーター設定")]
    [SerializeField, Header("遷移までの待機時間")]
    private float delayTime = 3f;
    [SerializeField, Header("終了後に変更するカメラの番号")]
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
