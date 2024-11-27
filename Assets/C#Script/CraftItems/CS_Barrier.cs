using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using UnityEngine;

// クラフトアイテム　バリア
// 藤原昂祐
public class CS_Barrier : CraftItemBase
{
    // 設定項目
    [SerializeField, Header("有効時間")]
    private float validityTime = 3f;
    [SerializeField, Header("移動速度")]
    private float speed = 3f;


    // 設置位置確認用オブジェクト
    private GameObject setup;
    MeshRenderer[] meshRenderer;

    // バリアのモデル
    private GameObject domeshield;

    // オーディオ
    private CS_SoundEffect soundEffect;

    // 時間計測
    private CS_Countdown countdown;

    // コリジョン
    Collider collider;

    // 設置状態
    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        // コリジョンを取得
        collider = GetComponent<Collider>();
        collider.enabled = false;

        // 時間計測用オブジェクトを作成
        countdown = gameObject.AddComponent<CS_Countdown>();

        // オーディオを取得
        soundEffect = transform.GetChild(1).gameObject.GetComponent<CS_SoundEffect>();

        // 子オブジェクトを取得
        domeshield = transform.GetChild(0).gameObject;
        domeshield.SetActive(false);

        // 設置位置を表示

        // "Player"オブジェクトのTransformを取得
        GameObject player = GameObject.Find("Player");
        Transform playerTransform = player.transform;

        // 子オブジェクト"setup"を検索して取得
        setup = playerTransform.Find("setup").gameObject;
        meshRenderer = setup.GetComponentsInChildren<MeshRenderer>(true); ;
        foreach (MeshRenderer renderer in meshRenderer)
        {
            renderer.enabled = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isActive)
        {
            Vector3 moveVec = GetMovementVector();
            setup.transform.position = new Vector3(
                setup.transform.position.x + moveVec.x * speed * Time.deltaTime,
                setup.transform.position.y,
                setup.transform.position.z + moveVec.z * speed * Time.deltaTime
            );
        }


        // Rトリガーで設置
        if (GetInputSystem().GetRightTrigger() > 0
            && !isActive)
        {
            transform.position = setup.transform.position;
            countdown.Initialize(validityTime);
            domeshield.SetActive(true);
            setup.SetActive(false);
            isActive = true;
            soundEffect.PlaySoundEffect(0, 0);
            foreach (MeshRenderer renderer in meshRenderer)
            {
                renderer.enabled = false;
            }

            isSetUp = true;
        }

        // 使用後、自らを破棄
        if (countdown.IsCountdownFinished()
            && isActive)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // オブジェクトにアタッチされている全てのコライダーを取得
        Collider[] colliders = GetComponents<Collider>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Attack"))
            {
                Destroy(other.gameObject);
            }
        }
    }

    //**
    //* スティック入力からカメラから見た移動ベクトルを取得する
    //*
    //* in：無し
    //* out：移動ベクトル
    //**
    Vector3 GetMovementVector()
    {
        Vector2 stick = GetInputSystem().GetRightStick();
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        Vector3 moveVec = forward * stick.y + right * stick.x;
        moveVec.y = 0f; // y 軸の移動は不要
        return moveVec.normalized;
    }
}
