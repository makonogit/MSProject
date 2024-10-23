using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//**
//* プレイヤーマネージャー
//*
//* 担当：藤原
//**
public class CS_PlayerManager : MonoBehaviour
{
    //**
    //* 変数
    //**

    [Header("パラメーター設定")]
    [SerializeField, Header("体力値")]
    private float initHP = 100;
    [SerializeField]
    private float nowHP;
    public float GetHP() => nowHP;
    public void SetHP(float setHP) { nowHP = setHP; }

    [Header("接地判定")]
    [SerializeField]
    private float groundCheckDistance = 0.01f;
    [SerializeField]
    private LayerMask groundLayer;

    // 外部オブジェクト（名前検索で取得）
    private Transform cameraTransform;
    public Transform GetCameraTransform() => cameraTransform;
    private CS_InputSystem inputSystem;     // インプットシステム
    public CS_InputSystem GetInputSystem()=> inputSystem;
    private CS_CameraManager cameraManager; // カメラマネージャー
    public CS_CameraManager GetCameraManager() => cameraManager;
    private CS_TpsCamera tpsCamera;         // TPSカメラ
    public CS_TpsCamera GetTpsCamera() => tpsCamera;

    // 自身のコンポーネント
    private Rigidbody rb;       // リジットボディ
    public Rigidbody GetRigidbody() => rb;
    private Animator animator;  // アニメーター
    public Animator GetAnimator() => animator;

    //**
    //* 初期化
    //**
    void Start()
    {
        // HPを設定
        nowHP = initHP;

        // 自身のコンポーネントを取得
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // インスペクタービューから特定のオブジェクトを取得
        cameraTransform = GameObject.Find("Main Camera")?.transform;
        if (cameraTransform == null)
        {
            UnityEngine.Debug.LogError("Main CameraをHierarchyに追加してください。");
        }

        inputSystem = GameObject.Find("InputSystem")?.GetComponent<CS_InputSystem>();
        if (inputSystem == null)
        {
            UnityEngine.Debug.LogError("InputSystemをHierarchyに追加してください。");
        }

        cameraManager = GameObject.Find("CameraManager")?.GetComponent<CS_CameraManager>();
        if (cameraManager == null)
        {
            UnityEngine.Debug.LogError("CameraManagerをHierarchyに追加してください。");
        }

        tpsCamera = GameObject.Find("TpsCamera")?.GetComponent<CS_TpsCamera>();
        if (tpsCamera == null)
        {
            UnityEngine.Debug.LogError("TpsCameraをHierarchyに追加してください。");
        }

    }

    void Update()
    {
        animator.SetBool("isGrounded", IsGrounded());
        animator.SetBool("isWall", IsWall());
    }

    //**
    //* 衝突処理
    //**
    private void OnCollisionEnter(Collision collision)
    {
        // 衝突したオブジェクトのタグをチェック
        if (collision.gameObject.tag == "Item")
        {
            CS_Item item = collision.gameObject.GetComponent<CS_Item>();

            nowHP += 1;
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            nowHP -= 1;
        }
        else if (collision.gameObject.tag == "Goal")
        {
        }
    }

    //**
    //* 地面に接地しているかを判断する
    //*
    //* in：無し
    //* out：接地判定
    //**
    public bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);
    }

    //**
    //* 壁に接しているかを判断する
    //*
    //* in：無し
    //* out：接地判定
    //**
    public bool IsWall()
    {
        RaycastHit hit;
        Vector3 offset = new Vector3(0f,0f,0f);
        return Physics.Raycast(transform.position + offset, transform.forward, out hit, groundCheckDistance, groundLayer);
    }
}
