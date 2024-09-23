using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class CS_Burst_of_object : MonoBehaviour
{
    //-変数-
    private Collider thisCollider;

    private AudioSource thisAudioSource;

    private Vector3 defaultScale;

    [SerializeField, Tooltip("地雷設定")]
    private bool MineFlag = false;


    [SerializeField, Tooltip("耐久力")]
    private float Health;

    [Header("圧力")]

    [SerializeField, Tooltip("圧力番号:\n番号から下記の圧力配列の値を参照する為の変数です。")]
    private int PressureNumber;

    [SerializeField, Tooltip("圧力配列:\n数値は、1 = 100%として扱われます。")]
    private List<float> PressureValList = new List<float>();

    [SerializeField, Tooltip("圧力配列:\n数値は、1 = 100%として扱われます。")]
    private float ExplosionVolume = 0.9f;

    [Header("破片")]
    [SerializeField, Tooltip("破片プレハブ:\n")]
    private GameObject DebrisObj;

    [SerializeField, Tooltip("破片の数:\n")]
    private int DebrisNum;

    [SerializeField, Tooltip("破片の飛ぶ速度:\n")]
    private float BurstSpeed;

    [Header("衝撃波")]
    [SerializeField, Tooltip("衝撃波プレハブ:\n")]
    private GameObject ShockWaveObj;

    [SerializeField, Tooltip("衝撃波の力:\n他オブジェクトに当たった時の加える力")]
    private float ShockPower;

    [SerializeField, Tooltip("衝撃波範囲:\n")]
    private float WaveSize;

    [Header("消滅")]
    [SerializeField, Tooltip("消滅フラグ:")]
    private bool DestroyFlag;

    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float DestroyTime;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        DestroyFlag = false;
        thisCollider = GetComponent<Collider>();
        if (thisCollider == null) Debug.LogError("null component");

        thisAudioSource = GetComponent<AudioSource>();
        if (thisAudioSource == null) Debug.LogError("null component");
        defaultScale = this.transform.localScale;
    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
    private void FixedUpdate()
    {
        Explosion();
        UntilDestroyMyself();
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update() { }

    /// <summary>
    /// OnCollisionEnter
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        bool isPlayer = collision.transform.tag == "Player";
        if (!MineFlag) return;
        if (!isPlayer) return;
        Health = 0;
    }



    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damageValue">ダメージ値</param>
    public void HitDamage(float damageValue)
    {
        Health -= damageValue;
        bool isMin = Health < 0;
        if (isMin) Health = 0;
    }

    /// <summary>
    /// 加圧
    /// </summary>
    /// <param name="pressureValue">加圧値</param>
    public bool AddPressure(int pressureValue)
    {
        int tmp = pressureValue + PressureNumber;
        bool isMax = tmp >= PressureValList.Count;
        bool isMin = tmp < 0;


        if (isMax)
        {
            PressureNumber = PressureValList.Count - 1;
            HitDamage(pressureValue);
        }
        else if (isMin) PressureNumber = 0;
        else PressureNumber = tmp;
        SetScale();
        return isMax;
    }

    /// <summary>
    /// 加圧によるサイズ変更
    /// </summary>
    private void SetScale()
    {
        float size = 1.0f + PressureValList[PressureNumber];
        this.transform.localScale = defaultScale * size;
    }



    /// <summary>
    /// 自身の破裂
    /// </summary>
    private void Explosion()
    {
        bool isNoHealth = Health <= 0;
        bool canExplosion = isNoHealth && !DestroyFlag;

        // 破裂できないなら終わる
        if (!canExplosion) return;

        float power = 1.0f + PressureValList[PressureNumber];

        // 非アクティブ化
        thisCollider.enabled = false;

        // 破片を飛ばす
        BurstDebris(power);
        // 衝撃波を出す
        ShockWave(power);
        // 破裂音の再生
        MakeBurstSounds(power);
        // モデル変更
        ChangeModel();

        // 消失フラグを立てる
        DestroyFlag = true;
    }

    /// <summary>
    /// 破片が飛ぶ処理
    /// </summary>
    private void BurstDebris(float Power)
    {
        //float distance = BurstDistance * Power;
        float speed = BurstSpeed * Power;
        for (int i = 0; i < DebrisNum; i++) CreateDebris(speed, i);
    }

    /// <summary>
    /// 破片の生成
    /// </summary>
    /// <param name="Power"></param>
    /// <param name="num"></param>
    private void CreateDebris(float Power, int num)
    {
        Vector3 vec = GetFlyVector(1, DebrisNum, num);
        GameObject obj = Instantiate(DebrisObj);
        obj.transform.position = this.transform.position + vec;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("null component!");
            return;
        }
        Vector3 vector = vec * Power;
        rb.AddForce(vector, ForceMode.Force);
    }

    /// <summary>
    /// 飛ぶ方向を取得する
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="maxNum"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private Vector3 GetFlyVector(float radius, int maxNum, int num)
    {
        float phi = Mathf.Acos(1 - 2 * (num + 0.5f) / maxNum);
        float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * num;

        float x = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
        float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
        float z = radius * Mathf.Cos(phi);

        return new Vector3(x, y, z);
    }


    /// <summary>
    /// 衝撃波を起こす
    /// </summary>
    /// <returns></returns>
    private void ShockWave(float Power)
    {
        float range = WaveSize * Power;
        float speed = Power;
        float time = 1.5f;

        GameObject obj = Instantiate(ShockWaveObj);
        obj.transform.position = this.transform.position;

        CS_ShockWave sw = obj.GetComponent<CS_ShockWave>();
        if (sw == null)
        {
            Debug.LogWarning("null component!");
            return;
        }

        sw.DestroyTime = time;
        sw.Speed = speed;
        sw.Power = Power;
    }

    /// <summary>
    /// 破裂音を再生
    /// </summary>
    private void MakeBurstSounds(float Power)
    {
        float volume = ExplosionVolume * Power;

        thisAudioSource.volume = volume;
        thisAudioSource.Play();
    }

    /// <summary>
    /// モデル変更
    /// </summary>
    private void ChangeModel()
    {/**/}

    /// <summary>
    /// 消滅するまでの処理
    /// </summary>
    private void UntilDestroyMyself()
    {
        // 破壊しない
        if (!DestroyFlag) return;
        DestroyTime -= Time.deltaTime;
        // 破棄する
        bool shouldDestroy = DestroyTime <= 0;
        if (shouldDestroy) Destroy(this.gameObject);
    }
}