using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Burst_of_object : MonoBehaviour
{
    //-変数-
    private Collider collider;

    [SerializeField, Tooltip("耐久力")]
    private float Health;

    [Header("圧力")]
    
    [SerializeField, Tooltip("圧力番号:\n番号から下記の圧力配列の値を参照する為の変数です。")]
    private int PressureNumber;

    [SerializeField, Tooltip("圧力配列:\n数値は、1 = 100%として扱われます。")]
    private List<float> PressureValList = new List<float>();

    [Header("破片")]
    [SerializeField, Tooltip("破片プレハブ:\n")]
    private GameObject DebrisObj;
    
    [SerializeField, Tooltip("破片の数:\n")]
    private int DebrisNum;
    
    [SerializeField, Tooltip("破片の飛ぶ速度:\n")]
    private float BurstSpeed;
    
    [SerializeField, Tooltip("破片の飛ぶ距離:\n")]
    private float BurstDistance;

    [SerializeField, Tooltip("破片の飛ぶ方角:\n")]
    private List<Vector3> BurstVecList= new List<Vector3>();

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
        collider = GetComponent<Collider>();
        if (collider == null) Debug.LogError("null component");
        
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
    private void Update(){}
 
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
    public void AddPressure(int pressureValue)
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
        collider.enabled = false;

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
        for (int i = 0; i < BurstVecList.Count; i++)CreateDebris(speed,i);
    }

    /// <summary>
    /// 破片の生成
    /// </summary>
    /// <param name="Power"></param>
    /// <param name="num"></param>
    private void CreateDebris(float Power,int num)
    {
        GameObject obj = Instantiate(DebrisObj);
        obj.transform.position = this.transform.position + BurstVecList[num];
        Vector3 vector = BurstVecList[num] * Power;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        rb.AddForce(vector, ForceMode.Force);
    }

    /// <summary>
    /// 衝撃波を起こす
    /// </summary>
    /// <returns></returns>
    private void ShockWave(float Power) 
    {
        float range = WaveSize * Power;
        GameObject obj = Instantiate(ShockWaveObj);
        obj.transform.position = this.transform.position;
    }

    /// <summary>
    /// 破裂音を再生
    /// </summary>
    private void MakeBurstSounds(float Power) 
    {
        float volume = Power;
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
        if(!DestroyFlag) return;
        DestroyTime -= Time.deltaTime;
        // 破棄する
        bool shouldDestroy = DestroyTime <= 0;        
        if (shouldDestroy) Destroy(this.gameObject);
    }
}