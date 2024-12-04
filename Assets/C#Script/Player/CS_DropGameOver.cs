using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.C_Script.GameEvent;

public class CS_DropGameOver : MonoBehaviour
{
    private CS_CameraManager cameraManager;     // カメラマネージャー
    private GameObject tpsCamera;               // TPSカメラ
    private GameObject fallingCamera;           // 落下時カメラ


    // Start is called before the first frame update
    void Start()
    {
        cameraManager = GameObject.Find("CameraManager")?.GetComponent<CS_CameraManager>();
        tpsCamera = GameObject.Find("TpsCamera");
        fallingCamera = GameObject.Find("FallingCamera");
        if (fallingCamera == null)
        {
            UnityEngine.Debug.LogError("FallingCameraをHierarchyに追加してください。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // メインカメラの位置を自身に設定
            fallingCamera.transform.position = tpsCamera.transform.position;
            fallingCamera.transform.rotation = tpsCamera.transform.rotation;

            // カメラの切り替え
            cameraManager.SwitchingCamera(1);

            CSGE_GameOver.GameOver();
        }
    }
}
