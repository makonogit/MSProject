using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using UnityEngine;

// パーティクル制御　担当：藤原昂祐
public class CS_ParticleStarter : MonoBehaviour
{
    [SerializeField, Header("パーティクルのリスト")]
    private ParticleSystem[] particleList;
    private static ParticleSystem[] particleSystems;

    // スタートさせたいパーティクルシステムのインデックス
    public static int startIndex = 0;

    // 開始するメソッド
    public static void StartParticleSystemAtIndex(int index)
    {
        // 配列の範囲内か確認
        if (index >= 0 && index < particleSystems.Length)
        {
            // 指定したインデックスのパーティクルシステムを停止してクリア
            particleSystems[index].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            // 再生を開始
            particleSystems[index].Play();
        }
    }

    private void Start()
    {
        if (particleSystems == null)
        {
            // 振動カーブをコピー
            particleSystems = new ParticleSystem[particleList.Length];
            particleList.CopyTo(particleSystems, 0);
        }

        foreach (var ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void Update()
    {
        // すべてのパーティクルシステムをチェック
        foreach (var ps in particleSystems)
        {
            // パーティクルシステムが停止している場合リセットする
            if (!ps.isPlaying && ps.time <= 0)
            {
                ps.Clear();
            }
        }
    }
}
