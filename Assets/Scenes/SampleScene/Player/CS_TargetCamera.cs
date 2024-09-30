using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// ロックオン用カメラ
public class CS_TargetCamera : MonoBehaviour
{
    [Header("設定")]
    public Transform center;            // 当たり判定の中心
    public float detectionRadius = 20f; // 当たり判定の半径
    public string targetTag = "Enemy";  // 対象のタグ
    public LayerMask targetLayer;       // 対象オブジェクトのレイヤー

    // 自身のコンポーネント
    private CinemachineTargetGroup cinemachineTargetGroup;

    private void Start()
    {
        // CinemachineTargetGroupコンポーネントを取得
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    void Update()
    {
        // 範囲内で条件に合う最も近いオブジェクトを検索
        GameObject closest = FindClosest();

        if (closest != null)
        {
            // ターゲットが見つかった場合、追加
            AddTarget(closest.transform);
        }
        else
        {
            // ターゲットが見つからなかった場合、既存のターゲットを削除
            RemoveTarget();
        }
    }

    //**
    //* ターゲットを追加する
    //* 
    //* in: 追加するターゲット
    //* out: なし
    //**

    void AddTarget(Transform target)
    {
        // 既存のターゲットを取得
        var existingTargets = cinemachineTargetGroup.m_Targets;

        // 新しいターゲットを追加
        existingTargets[1] = new CinemachineTargetGroup.Target
        {
            target = target, // ターゲットを設定
            weight = 1f,     // ウェイトを設定
            radius = 2f      // 半径を設定
        };
    }

    //**
    //* ターゲットを消去する
    //* 
    //* in: なし
    //* out: なし
    //**

    void RemoveTarget()
    {
        // ターゲットを空に設定
        var existingTargets = cinemachineTargetGroup.m_Targets;
        existingTargets[1].target = null;
    }

    //**
    //* 範囲内から条件に合うオブジェクトを取得する
    //* 
    //* in: なし
    //* out: 最も近いオブジェクト
    //**

    GameObject FindClosest()
    {
        // 指定した範囲内のコライダーを取得
        Collider[] hitColliders = Physics.OverlapSphere(center.position, detectionRadius, targetLayer);
        GameObject closest = null; // 最も近いオブジェクトを初期化
        float closestDistance = Mathf.Infinity; // 距離を無限大で初期化

        foreach (Collider collider in hitColliders)
        {
            // タグが一致するか確認
            if (collider.CompareTag(targetTag))
            {
                // 中心からの距離を計算
                float distance = Vector3.Distance(center.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance; // 最短距離を更新
                    closest = collider.gameObject; // 最も近いオブジェクトを保存
                }
            }
        }

        return closest; // 最も近いオブジェクトを返す
    }

    // デバッグ用に範囲を可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Gizmoの色を赤に設定
        Gizmos.DrawWireSphere(center.position, detectionRadius); // 範囲を描画
    }
}

