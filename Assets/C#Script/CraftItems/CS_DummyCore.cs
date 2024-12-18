using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DummyCore : CraftItemBase
{
    [SerializeField, Header("有効時間")]
    private float validityTime = 3f;
    [SerializeField, Header("発射速度")]
    private float speed = 1f;

    // 時間計測
    private CS_Countdown countdown;

    // 設置状態
    private bool isMove = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        // 時間計測用オブジェクトを作成
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 発射して設置
        if (isMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        // 使用後、自らを破棄
        if (countdown.IsCountdownFinished()
            && !isMove)
        {
            Destroy(gameObject);
        }
    }

    // 衝突処理
    private void OnTriggerEnter(Collider other)
    {
        isMove = false;

        isSetUp = true;

        countdown.Initialize(validityTime);
    }
}
