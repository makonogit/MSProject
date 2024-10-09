using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;

public class CS_Burst_of_object : MonoBehaviour
{
    //-変数-
    private Collider thisCollider;

    private AudioSource thisAudioSource;

    private Vector3 defaultScale;

    private MeshRenderer thisMeshRenderer;

    [SerializeField, Tooltip("壊れた後")]
    private GameObject brokenObj;

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

    [SerializeField, Tooltip("破片の方向:\n")]
    private List<Vector3> BurstVecList = new List<Vector3>();


    [SerializeField, Tooltip("破片生成位置調整:\n")]
    protected Vector3 CreateOffsetPosition = Vector3.zero;

    [SerializeField, Tooltip("破片の飛ぶ速度:\n")]
    private float BurstSpeed;

    [Header("衝撃波")]
    [SerializeField, Tooltip("衝撃波プレハブ:\n")]
    private GameObject ShockWaveObj;

    [SerializeField, Tooltip("衝撃波の力:\n他オブジェクトに当たった時の加える力")]
    private float ShockPower;

    [SerializeField, Tooltip("衝撃波範囲:\n")]
    private float WaveSize;

    [SerializeField, Tooltip("エフェクト")]
    private VisualEffect ExplosionEffect;
    

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

        thisMeshRenderer = GetComponent<MeshRenderer>();
        if (thisMeshRenderer == null) Debug.LogError("null component");

        ExplosionEffect.Stop();
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
        // 爆破エフェクトの生成
        CreateExplosionEffect();
        // モデル変更
        ChangeModel();


        // 破裂後のオブジェ差し替えと、その他処理
        {
            GameObject obj = Instantiate(brokenObj);
            obj.transform.position = this.transform.position;
            thisMeshRenderer.enabled = false;
        }

        // 消失フラグを立てる
        DestroyFlag = true;
    }
    /// <summary>
    /// 爆破エフェクト
    /// </summary>
    private void CreateExplosionEffect()
    {
        ExplosionEffect.Play();
    }

    /// <summary>
    /// 破片が飛ぶ処理
    /// </summary>
    private void BurstDebris(float Power)
    {
        float speed = BurstSpeed * Power;
        for (int i = 0; i < BurstVecList.Count; i++) CreateDebris(speed, i);
    }

    /// <summary>
    /// 破片の生成
    /// </summary>
    /// <param name="Power"></param>
    /// <param name="num"></param>
    private void CreateDebris(float Power, int num)
    {
        GameObject obj = Instantiate(DebrisObj);
        obj.transform.position = GetCreatePosition(1, num);
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) 
        {
            Debug.LogWarning("null component!");
            return;
        }
        Vector3 vector = GetFlyVector(Power, num);
        rb.AddForce(vector, ForceMode.Force);
    }

    /// <summary>
    /// 破片の生成位置を取得する
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private Vector3 GetCreatePosition(float radius,int num) 
    {
        Vector3 Value = new Vector3();
        Vector3 vec = GetFlyVector(1, num);
        Value += this.transform.position;
        Value += vec;
        Value += CreateOffsetPosition;
        return Value;
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
    /// 飛ぶ方向を取得
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private Vector3 GetFlyVector(float radius,int num)
    {
        bool IsOver = BurstVecList.Count <= num;
        bool IsUnder = num < 0;
        bool IsOutsideArray = IsOver && IsUnder;
        if (IsOutsideArray) return Vector3.zero;
        return BurstVecList[num].normalized * radius;
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
        //if (shouldDestroy) Destroy(this.gameObject);
    }


#if UNITY_EDITOR
    
    /// <summary>
    /// 選択時表示
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color =new Color(0,1,0,0.5f);
        for (int i = 0; i < BurstVecList.Count; i++) 
        {
            
            Gizmos.DrawLineStrip(DrawDebris(i).ToArray(), false);
        }
    }

    private List<Vector3> DrawDebris(int num) 
    {
        float deltaTime = 0.04167f;
        float power = 1.0f + PressureValList[PressureNumber];
        float speed = BurstSpeed * power;
        List<Vector3> Points = new List<Vector3>();

        // 初期位置設定
        Vector3 position = GetCreatePosition(1, num);
        Points.Add(this.transform.position + CreateOffsetPosition);
        Points.Add(position);
        // 初速度の設定
        Vector3 Velocity = (GetFlyVector(speed, num) * deltaTime * deltaTime) * 0.5f;
        RaycastHit hit  = new RaycastHit();
        // ぶつかるまでの軌道の線を引く
        for (float time = 0.0f; time < 2; time += deltaTime) 
        {
            Velocity += Vector3.down * ((9.8f * time * time) * 0.5f);
            Ray ray = new Ray(position,Velocity.normalized);
            bool IsHit = Physics.Raycast(ray, out hit, Velocity.magnitude);
            position += Velocity;
            if (IsHit) 
            {
                Points.Add(hit.point);
                break; 
            }
            Points.Add(position);
        }
        // 着地点表示
        Gizmos.DrawWireSphere(hit.point,0.5f);
        Gizmos.DrawSphere(hit.point,0.5f);

        return Points;
    }

    /// <summary>
    /// <summary>
    /// インスペクター更新時
    /// </summary>
    private void OnValidate()
    {

       

    }
#endif // UNITY_EDITOR
}