using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クラフトアイテム　バリア
// 藤原昂祐
public class CS_Barrier : MonoBehaviour
{
    // 設定項目
    [SerializeField, Header("有効時間")]
    private float validityTime = 3f;

    // オーディオ
    private CS_SoundEffect soundEffect;

    // 時間計測
    private CS_Countdown countdown;

    // Start is called before the first frame update
    void Start()
    {
        // 時間計測用オブジェクトを作成
        countdown = gameObject.AddComponent<CS_Countdown>();

        // 子オブジェクトを取得
        soundEffect = transform.GetChild(0).gameObject.GetComponent<CS_SoundEffect>();

        // 効果音を再生
        soundEffect.PlaySoundEffect(0, 0);

        // 有効時間のカウント開始
        countdown.Initialize(validityTime);
    }

    // Update is called once per frame
    void Update()
    {
        // 使用後、自らを破棄
        if (countdown.IsCountdownFinished())
        {
            Destroy(gameObject);
        }
    }
}
