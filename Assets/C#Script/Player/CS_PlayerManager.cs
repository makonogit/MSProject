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
    [SerializeField, Header("体力の初期値")]
    private float initHP = 100;
    [SerializeField]
    private float nowHP;
    public float GetHP() => nowHP;
    public void SetHP(float setHP) { nowHP = setHP; }
    [SerializeField, Header("缶詰めの取得数")]
    private int itemStock = 0;
    public int GetItemStock() => itemStock;
    public void SetItemStock(int val) { itemStock = val; }
    [SerializeField, Header("空き缶の個数")]
    private int ingredientsStock = 0;
    public int GetIngredientsStock() => ingredientsStock;
    public void SetIngredientsStock(int val) { ingredientsStock = val; }
    [SerializeField, Header("CS_Coreをここにセット")]
    private CS_Core core;

    [Header("接地判定")]
    [SerializeField]
    private float groundCheckDistance = 0.01f;
    [SerializeField]
    private float fallingThreshold = 5f;
    [SerializeField]
    private LayerMask groundLayer;
    private Vector3 oldAcceleration;// 1フレーム前の重力加速度

    // 硬直
    [System.Serializable]
    public class StringNumberPair
    {
        public string name;
        public float time;
    }
    [SerializeField, Header("状態(bool)/硬直時間")]
    private StringNumberPair[] animatorBoolParameterList;
    public StringNumberPair[] GetAnimatorBoolParameterList() => animatorBoolParameterList;
    [SerializeField, Header("状態(float)/硬直時間")]
    private StringNumberPair[] animatorFloatParameterList;
    public StringNumberPair[] GetAnimatorFloatParameterList() => animatorFloatParameterList;

    [SerializeField, Header("オーディオ")]
    private CS_SoundEffect soundEffect;
    public CS_SoundEffect GetSoundEffect() => soundEffect;

    // 外部オブジェクト（名前検索で取得）
    private Transform cameraTransform;
    public Transform GetCameraTransform() => cameraTransform;
    private CS_InputSystem inputSystem;     // インプットシステム
    public CS_InputSystem GetInputSystem()=> inputSystem;
    private CS_CameraManager cameraManager; // カメラマネージャー
    public CS_CameraManager GetCameraManager() => cameraManager;
    private CS_TpsCamera tpsCamera;         // TPSカメラ
    public CS_TpsCamera GetTpsCamera() => tpsCamera;
    private CS_Result result;
    public CS_Result GetResult() => result;

    // 自身のコンポーネント
    private Rigidbody rb;       // リジットボディ
    public Rigidbody GetRigidbody() => rb;
    private Animator animator;  // アニメーター
    public Animator GetAnimator() => animator;

    // カウントダウン用クラス
    private CS_Countdown countdown;
    private CS_Countdown countdownGoal;
    private CS_Countdown countdownStun;


    //**
    //* 初期化
    //**
    void Start()
    {
        // Countdownオブジェクトを生成
        countdown = gameObject.AddComponent<CS_Countdown>();
        countdownGoal = gameObject.AddComponent<CS_Countdown>();
        countdownStun = gameObject.AddComponent<CS_Countdown>();

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



        GameObject resultCanvas = GameObject.Find("ResultCanvas");

        if (resultCanvas == null)
        {
            UnityEngine.Debug.LogError("ResultCanvasをHierarchyに追加してください。");
        }
        else
        {
            Transform resultPanelTransform = resultCanvas.transform.Find("ResultPanel");
            if (resultPanelTransform != null)
            {
                result = resultPanelTransform.GetComponent<CS_Result>();
                if (result == null)
                {
                    UnityEngine.Debug.LogError("ResultPanelにCS_Resultコンポーネントがアタッチされているか確認してください。");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("ResultPanelをHierarchyに追加してください。");
            }
        }

    }

    void Update()
    {
        // エネルギーコアの状態を設定
        if (core != null)
        {
            if (animator.GetBool("Mount"))
            {
                core.STATE = CS_Core.CORE_STATE.HAVEPLAYER;
            }
            else if(core.STATE != CS_Core.CORE_STATE.HAVEENEMY)
            {
                core.STATE = CS_Core.CORE_STATE.DROP;
            }
        }
        else
        {
            UnityEngine.Debug.LogError("CS_Coreオブジェクトが設定されていません");
        }



        // ゲームオーバー
        if (nowHP <= 0)
        {
            animator.SetBool("GameOver", true);
        }

        animator.SetBool("isGrounded", IsGrounded());

        animator.SetBool("isWall", IsWall());

        if (animator.GetBool("Mount"))
        {
            animator.SetFloat("Mass", 1);
        }
        else
        {
            animator.SetFloat("Mass", 0);
        }

        if(initHP - nowHP >= initHP - initHP / 3)
        {
            animator.SetFloat("Hunger", 1);
        }
        else
        {
            animator.SetFloat("Hunger", 0);
        }

        // 重力加速度から落下状態を設定する
        Vector3 gravity = Physics.gravity;
        Vector3 acceleration = rb.velocity / Time.deltaTime;

        if (countdown.IsCountdownFinished())
        {
            if (IsGrounded() && acceleration.y == 0 && oldAcceleration.y != 0)
            {
                if ((oldAcceleration.y < -fallingThreshold))
                {
                    animator.SetFloat("Falling", 1);
                    countdown.Initialize(0.5f);
                }
                else
                {
                    animator.SetFloat("Falling", 0);
                }
            }
        }
        oldAcceleration = acceleration;


        // ゴールモーションが終了してからリザルトを表示
        if (countdownGoal.IsCountdownFinished() && animator.GetBool("Goal"))
        {
            result.StartResult();
            animator.SetBool("Goal",false);
        }

        // 気絶状態
        if (countdownStun.IsCountdownFinished() && animator.GetBool("Stun"))
        {
            animator.SetBool("Stun", false);
        }
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

            itemStock++;

            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "StunObject")
        {
            animator.SetBool("Stun", true);

            Vector3 reverseForce = collision.contacts[0].normal * 10.0f;
            reverseForce.y = 0f;
            GetRigidbody().AddForce(reverseForce, ForceMode.Impulse);

            countdownStun.Initialize(3);
        }
        else if (collision.gameObject.tag == "Goal")
        {
            TemporaryStorage.DataSave("ingredientsStock",ingredientsStock);
            animator.SetBool("Goal", true);

            countdownGoal.Initialize(3);
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
        //RaycastHit hit;
        //return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer);

        //float radius = 1.0f;
        //float range = 0.1f;

        //Ray ray = new Ray(transform.position, Vector3.down);
        //RaycastHit[] hits = Physics.SphereCastAll(ray, radius, range);

        //foreach (RaycastHit hit in hits)
        //{
        //    return true;
        //}

        //return false;

        float radius = 0.125f;
        float groundCheckDistance = 0.01f;
        return Physics.CheckSphere(transform.position - Vector3.up * groundCheckDistance, radius, groundLayer);
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

    // 接地判定の可視化
    private void OnDrawGizmos()
    {
        float radius = 0.125f;               // Sphereの半径
        float groundCheckDistance = 0.01f;  // チェック位置のオフセット

        // 接地状態を色で可視化
        if (Physics.CheckSphere(transform.position, radius, groundLayer))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        // Sphereを描画
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
