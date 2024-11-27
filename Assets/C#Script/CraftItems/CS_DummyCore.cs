using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DummyCore : CraftItemBase
{
    [SerializeField, Header("発射速度")]
    private float speed = 1f;

    // 設置状態
    private bool isMove = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 発射して設置
        if (isMove)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }

    // 衝突処理
    private void OnTriggerEnter(Collider other)
    {
        isMove = false;

        isSetUp = true;
    }
}
