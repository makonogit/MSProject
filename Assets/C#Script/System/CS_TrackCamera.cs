using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * トラック上を移動するカメラ
 * 
 * 担当：藤原昂祐
 */

public class CS_TrackCamera : MonoBehaviour
{
    private CinemachineVirtualCamera currentVirtualCamera;  // VirtualCamera
    private CinemachineDollyCart dollyCart;                 // Dollyカート

    private float reachThreshold = 0.05f; // 到達判定の閾値

    [Header("オブジェクト設定")]
    [SerializeField, Header("カメラマネージャー")]
    private CS_CameraManager cameraManager;
    [SerializeField, Header("カメラの移動トラック")]
    private CinemachineSmoothPath smoothPath;

    [Header("パラメーター設定")]
    [SerializeField, Header("移動速度")]
    private float speed = 1f;
    [SerializeField, Header("終了後に変更するカメラの番号")]
    private int nextCameraIndex = 0;
    [SerializeField, Header("終了後にActiveにするオブジェクト")]
    private GameObject[] activeObj;

    void Start()
    {
        // 自分のコンポーネントを取得
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
                    obj.SetActive(true); // オブジェクトをアクティブにする
                }
            }
        }
    }

    private bool IsAtEndOfPath(float pathLength)
    {
        // m_Positionがパスの終わりに近いかどうかをチェック
        float position = dollyCart.m_Position;
        return Mathf.Abs(position - pathLength) < reachThreshold;
    }
}
