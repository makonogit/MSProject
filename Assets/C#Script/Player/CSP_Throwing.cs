using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//**
//* 投擲・背負う処理
//*
//* 担当：藤原昂祐
//**

public class CSP_Throwing : ActionBase
{
    [Header("コア取得時の振動設定")]
    [SerializeField, Header("振動の長さ")]
    private float duration = 0.5f;         // 振動の長さ
    [SerializeField, Header("振動の強さ")]
    private int powerType = 1;          // 振動の強さ（4段階）
    [SerializeField, Header("振動の周波数")]
    private AnimationCurve curveType;          // 振動の周波数
    [SerializeField, Header("繰り返し回数")]
    private int repetition = 1;         // 繰り返し回数

    [Header("表示用コア")]
    public GameObject core;

    //[Header("コアの位置/回転")]
    //[SerializeField]
    private float distance = 0.75f;
    private Vector3 fixedRotation;

    //[Header("")]
    //[SerializeField]
    //private string targetTag;

    //[Header("ターゲット関連")]
    private GameObject targetObject;
    private CS_Core targetCore;
    public GameObject GetEnergyCore() => targetObject;
    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private List<Vector3> positions = new List<Vector3>();
    private Collider collider;

    //[Header("力の設定")]
    private float forceMagnitude = 10f;          // 力の大きさ
    private float angle = 45f;                   // 投げる角度
    private int steps = 30;                     // 描画の精度
    private float timeStep = 0.1f;              // 時間のステップ

    private float oldLeftTrigger = 0f;// 1フレーム前の入力を保存

    // 時間計測クラス
    private CS_Countdown countdown;

    protected override void Start()
    {
        base.Start();
        // ラインレンダーを取得して初期化
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;

        // Countdownオブジェクトを生成
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    void FixedUpdate()
    {
        //// エネルギーコアの状態を設定
        //if (targetObject != null)
        //{
        //    if (GetAnimator().GetBool("Mount"))
        //    {
        //        targetCore.STATE = CS_Core.CORE_STATE.HAVEPLAYER;
        //    }
        //    else if (targetCore.STATE != CS_Core.CORE_STATE.HAVEENEMY)
        //    {
        //        targetCore.STATE = CS_Core.CORE_STATE.DROP;
        //    }
        //}

        // 硬直処理

        // bool
        foreach (var pair in GetAnimatorBoolParameterList())
        {
            if (GetAnimator().GetBool(pair.name))
            {
                countdown.Initialize(pair.time);
                break;
            }
        }
        // float
        foreach (var pair in GetAnimatorFloatParameterList())
        {
            if (GetAnimator().GetFloat(pair.name) >= 1)
            {
                countdown.Initialize(pair.time);
                break;
            }
        }

        core.SetActive(targetObject != null);

        if (countdown.IsCountdownFinished())
        {
            // 予想線を非表示
            lineRenderer.enabled = false;

            if (GetAnimator().GetBool("Mount"))
            {
                // 投げる処理
                //if ((GetInputSystem().GetLeftTrigger() == 0) && (oldLeftTrigger > 0) && (!GetInputSystem().GetButtonBPressed()))
                //{
                //    if (targetObject != null)
                //    {
                //        GetAnimator().SetBool("Throwing", false);

                //        collider.enabled = true;

                //        rb.useGravity = true;

                //        GetSoundEffect().PlaySoundEffect(2, 8);

                //        // 余計な移動成分を取り除く
                //        Vector3 currentVelocity = rb.velocity;
                //        rb.velocity = Vector3.zero;
                //        rb.angularVelocity = Vector3.zero;

                //        // カメラの正面方向の位置を取得
                //        Vector3 cameraForward = Camera.main.transform.forward;

                //        // 指定した角度で力を加える
                //        Vector3 force = GetForceVector(angle, forceMagnitude, cameraForward);
                //        rb.AddForce(force, ForceMode.Impulse);

                //        // ターゲットをリセット
                //        targetObject = null;
                //        rb = null;
                //        collider = null;
                //    }
                //}

                //// ラインレンダーの初期値位置を更新
                //if (targetObject != null)
                //{
                //    positions.Add(targetObject.transform.position);
                //    lineRenderer.positionCount = positions.Count;
                //    lineRenderer.SetPositions(positions.ToArray());
                //}
                //else
                //{
                //    GetAnimator().SetBool("Mount", false);
                //}

                //// 投げる時のコアの位置を更新
                //// 投げる軌道を予測して描画
                //if ((GetInputSystem().GetLeftTrigger() > 0) && (targetObject != null) && (!GetInputSystem().GetButtonBPressed()))
                //{
                //    GetAnimator().SetBool("Throwing", true);

                //    lineRenderer.enabled = true;

                //    Vector3 offset = new Vector3(0, 2.5f, 0);
                //    targetObject.transform.position = transform.position + offset;

                //    DrawTrajectory();
                //}
                //// 背負う時のコアの位置を更新
                //else if (targetObject != null)
                //{
                //    Vector3 offset = new Vector3(0, 1.0f, 0);
                //    Vector3 backPosition = transform.position - transform.forward * distance;
                //    targetObject.transform.position = backPosition + offset;
                //    targetObject.transform.rotation = Quaternion.identity;
                //    rb.useGravity = false;

                //    targetObject.transform.rotation = Quaternion.Euler(fixedRotation);
                //}

                //oldLeftTrigger = GetInputSystem().GetLeftTrigger();

                // 背負う時のコアの位置を更新
                if (targetObject != null)
                {
                    Vector3 offset = new Vector3(0, 1.0f, 0);
                    Vector3 backPosition = transform.position - transform.forward * distance;
                    targetObject.transform.position = backPosition + offset;
                    targetObject.transform.rotation = Quaternion.identity;
                    rb.useGravity = false;

                    targetObject.transform.rotation = Quaternion.Euler(fixedRotation);
                }
            }
        }

        if (!GetAnimator().GetBool("Mount"))
        {
            if (targetObject != null)
            {
                // ターゲットをリセット
                targetObject.SetActive(true);
                collider.enabled = true;
                rb.useGravity = true;
                targetObject = null;
                rb = null;
                collider = null;
                targetCore = null;
            }
        }
    }

    // 力の方向を計算
    private Vector3 GetForceVector(float angle, float magnitude, Vector3 direction)
    {
        // 方向ベクトルを正規化
        Vector3 normalizedDirection = direction.normalized;

        // 方向に基づいて角度を計算
        float radian = angle * Mathf.Deg2Rad;

        // 回転行列を使用して新しい方向を計算
        Vector3 forceDirection = new Vector3(
            normalizedDirection.x * Mathf.Cos(radian) + normalizedDirection.x * Mathf.Sin(radian),
            normalizedDirection.y * Mathf.Sin(radian) + normalizedDirection.y * Mathf.Cos(radian),
            normalizedDirection.z * Mathf.Cos(radian) + normalizedDirection.z * Mathf.Sin(radian)
        );

        // 大きさを掛ける
        return forceDirection * magnitude;
    }




    // 投げる軌道を描画
    private void DrawTrajectory()
    {
        // カメラの正面方向の位置を取得
        Vector3 cameraForward = Camera.main.transform.forward;
        // 開始位置
        Vector3 startPos = targetObject.transform.position;
        // 力の方向
        Vector3 initialVelocity = GetForceVector(angle, forceMagnitude / targetObject.GetComponent<Rigidbody>().mass, cameraForward);
        // 重力
        float gravity = Physics.gravity.y;

        // 軌道を描画
        lineRenderer.positionCount = steps + 1;
        for (int i = 0; i <= steps; i++)
        {
            float t = i * timeStep;
            Vector3 position = startPos + initialVelocity * t + 0.5f * new Vector3(0, gravity, 0) * t * t;
            lineRenderer.SetPosition(i, position);
        }
    }

//**
//* 衝突処理
//**
    private void OnCollisionStay(Collision collision)
    {
        // エネルギーコアを拾う

        if (countdown.IsCountdownFinished())
        {
            if (collision.gameObject.tag == "EnergyCore"
                && targetObject == null)
            {
                if (GetInputSystem().GetButtonYPressed())
                {
                    targetObject = collision.gameObject;
                    targetObject.SetActive(false);

                    targetCore = targetObject.GetComponent<CS_Core>();

                    // リジットボディを取得
                    rb = targetObject.GetComponent<Rigidbody>();

                    // コライダーを取得
                    collider = targetObject.GetComponent<Collider>();
                    collider.enabled = false;

                    // ラインレンダーの初期位置を設定
                    positions.Add(targetObject.transform.position);

                    GetAnimator().SetBool("Mount", true);

                    // コントローラーを振動
                    CS_ControllerVibration.StartVibrationWithCurve(duration, powerType, curveType, repetition);
                }
            }
        }

        // 敵と衝突した場合、エネルギーコアを落とす
        if (collision.gameObject.tag == "Enemy")
        {
            if(targetObject != null)
            {
                collider.enabled = true;

                rb.useGravity = true;
                rb.AddForce(Vector3.up * 30.0f, ForceMode.Impulse);

                // ターゲットをリセット
                targetObject.SetActive(true);
                targetObject = null;
                rb = null;
                collider = null;
                targetCore = null;
            }
        }
    }
}
