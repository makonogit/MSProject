using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.C_Script.GameEvent;

public class CS_DropGameOver : MonoBehaviour
{
    [SerializeField, Header("カメラマネージャー")]
    private CS_CameraManager cameraManager;     // カメラマネージャー
    [SerializeField, Header("TPSカメラ")]
    private GameObject tpsCamera;               // TPSカメラ
    [SerializeField, Header("落下時カメラ")]
    private GameObject fallingCamera;           // 落下時カメラ
    [SerializeField, Header("遷移までの待機時間")]
    private float delay = 0.5f;           // 遷移までの待機時間


    private CS_Countdown countdown; // カウントダウンオブジェクト

    private bool isGameOver = false; //ゲームオーバーフラグ

    // Start is called before the first frame update
    void Start()
    {
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    void FixedUpdate()
    {
        if (isGameOver && countdown.IsCountdownFinished())
        {
            CSGE_GameOver.GameOver();
        }
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

            // ゲームオーバー状態に変更
            isGameOver = true;

            // 待機してゲームオーバーに遷移
            countdown.Initialize(delay);
        }
    }
}
