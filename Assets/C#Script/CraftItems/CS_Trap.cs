using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クラフトアイテム　トラバサミ
// 藤原昂祐
public class CS_Trap : MonoBehaviour
{
    // 設定項目
    [SerializeField, Header("拘束時間")]
    private float restraintTime = 3f;
    [SerializeField, Header("拘束対象")]
    private string restraintTag = "Enemy";

    // トラップのモデル
    private GameObject openTrap;    // 待機
    private GameObject closeTrap;   // 起動

    // 衝突したオブジェクト
    private GameObject hitObject;

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
        openTrap = transform.GetChild(0).gameObject;
        closeTrap = transform.GetChild(1).gameObject;
        soundEffect = transform.GetChild(2).gameObject.GetComponent<CS_SoundEffect>();

        // 表示状態を初期化
        openTrap.SetActive(true);
        closeTrap.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 拘束中の処理
        if (!countdown.IsCountdownFinished())
        {
            hitObject.transform.position = transform.position;
        }
        else if(hitObject != null)
        {
            hitObject = null;
            Destroy(gameObject);
        }
    }

    // 衝突処理
    private void OnTriggerEnter(Collider other)
    {
        // 敵と衝突した場合、敵を移動不能にする

        if (other.gameObject.CompareTag(restraintTag))
        {
            // 拘束カウント開始
            countdown.Initialize(restraintTime);

            // 衝突したオブジェクトを取得
            hitObject = other.gameObject;

            // モデルの表示状態を更新
            openTrap.SetActive(false);
            closeTrap.SetActive(true);

            // 効果音を再生
            soundEffect.PlaySoundEffect(0,0);
        }
    }
}
