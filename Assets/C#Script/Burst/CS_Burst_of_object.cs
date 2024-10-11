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

    [Header("―――――――――――――――――――――――――――――――――――――――――――")]
    [Header("圧力")]
    
    [SerializeField, Tooltip("圧力番号:\n番号から下記の圧力配列の値を参照する為の変数です。")]
    [Range(0,10)]
    private int PressureNumber;

    [SerializeField, Tooltip("圧力配列:\n数値は、1 = 100%として扱われます。")]
    private List<float> PressureValList = new List<float>();

    [SerializeField, Tooltip("圧力配列:\n数値は、1 = 100%として扱われます。")]
    private float ExplosionVolume = 0.9f;
    [Header("―――――――――――――――――――――――――――――――――――――――――――")]

    [Header("破片")]
    [SerializeField, Tooltip("破片プレハブ:\n")]
    private List<GameObject> DebrisObjs = new List<GameObject>();
    [SerializeField, Tooltip("破片生成位置調整:\n")]
    protected Vector3 CreateOffsetPosition = Vector3.zero;
    [SerializeField, Tooltip("破片の方向:\n")]
    private List<Vector3> BurstVecList = new List<Vector3>();
#if UNITY_EDITOR
    [SerializeField, Tooltip("オブジェクト:\n")]
    private List<GameObject> DebrisPositions = new List<GameObject>();
    [SerializeField, Tooltip("到達時間:\n")]
    private float time = 2.0f;
    [SerializeField, Tooltip("予測線表示:\n")]
    private bool DebrisShow = false;
    [SerializeField, Tooltip("圧力時の予測線表示:\n")]
    private bool ForecastPress = false;
    [SerializeField, Tooltip("想定圧力:\n")]
    [Range(0,10)]
    private int ExpectedPressure;
#endif // UNITY_EDITOR

    [Header("―――――――――――――――――――――――――――――――――――――――――――")]



    [Header("衝撃波")]
    [SerializeField, Tooltip("衝撃波プレハブ:\n")]
    private GameObject ShockWaveObj;

    [SerializeField, Tooltip("衝撃波の力:\n他オブジェクトに当たった時の加える力")]
    private float ShockPower;

    [SerializeField, Tooltip("衝撃波範囲:\n")]
    private float WaveSize;
    [SerializeField, Tooltip("衝撃波速度:\n")]
    private float WaveSpeed;

    [SerializeField, Tooltip("エフェクト")]
    private VisualEffect ExplosionEffect;
#if UNITY_EDITOR
    [SerializeField, Tooltip("衝撃波:\n")]
    private bool DrawShockWaveFlag = false;
#endif // UNITY_EDITOR

    [Header("―――――――――――――――――――――――――――――――――――――――――――")]

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
    /// Update
    /// </summary>
    private void Update() {}

    /// <summary>
    /// FixedUpdate
    /// </summary>
    private void FixedUpdate()
    {
        Explosion();
        UntilDestroyMyself();
    }


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

        float power =Pressure(PressureNumber);

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
        float speed = Power;
        for (int i = 0; i < BurstVecList.Count; i++) CreateDebris(speed, i);
    }

    /// <summary>
    /// 破片の生成
    /// </summary>
    /// <param name="Power"></param>
    /// <param name="num"></param>
    private void CreateDebris(float Power, int num)
    {
        GameObject obj = Instantiate(GetDebrisObject(num));
        obj.transform.position = GetCreatePosition(1, num);
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) 
        {
            Debug.LogWarning("null component!");
            return;
        }
        Vector3 vector = GetFlyVector(num) * Power;
        rb.AddForce(vector, ForceMode.Force);
    }

    /// <summary>
    /// リストから一つのオブジェクトを返す
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private GameObject GetDebrisObject(int num) 
    {
        int number = Mathf.Min(Mathf.Max(0, num), DebrisObjs.Count - 1);
        if(DebrisObjs.Count == 0) return new GameObject("null");
        if (DebrisObjs[number] == null) return new GameObject("null");
        if (num >= DebrisObjs.Count) return DebrisObjs[Random.Range(0, DebrisObjs.Count - 1)];
        return DebrisObjs[number];
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
        Value += this.transform.position;
        Value += GetFlyVector(num).normalized;
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
    private Vector3 GetFlyVector(int num)
    {
        bool IsOver = BurstVecList.Count <= num;
        bool IsUnder = num < 0;
        bool IsOutsideArray = IsOver && IsUnder;
        if (IsOutsideArray) return Vector3.zero;
        return BurstVecList[num];
    }



    /// <summary>
    /// 衝撃波を起こす
    /// </summary>
    /// <returns></returns>
    private void ShockWave(float Power)
    {
        float range = WaveSize * Power;
        float speed = WaveSpeed * Power;
        float time = 1.5f;

        GameObject obj = Instantiate(ShockWaveObj);
        obj.transform.position = this.transform.position+CreateOffsetPosition;

        CS_ShockWave sw = obj.GetComponent<CS_ShockWave>();
        if (sw == null)
        {
            Debug.LogWarning("null component!");
            return;
        }

        sw.DestroyTime = time;
        sw.Speed = speed;
        sw.MaxSize = range * 2;
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
    private float Pressure(int num) 
    {
        float value = 1.0f;
        int number = Mathf.Min(Mathf.Max(0, num), PressureValList.Count - 1);
        if (PressureValList.Count == 0) return value;
        return value + PressureValList[number];
    }

#if UNITY_EDITOR
    

    /// <summary>
    /// 選択時表示
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Info();
    }


    public void Info() 
    {
        ResetVelocity();
        // 破片の予測線
        if(DebrisShow)DrawExpectedDebris();

        // 衝撃波
       if(DrawShockWaveFlag) DrawShockWave();
    }
    private void DrawExpectedDebris() 
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        for (int i = 0; i < BurstVecList.Count; i++) Gizmos.DrawLineStrip(DrawDebris(i, ExpectedPressure).ToArray(), false);
        if (ForecastPress) DrawPressureLines();
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        foreach (GameObject obj in DebrisPositions) Gizmos.DrawWireSphere(obj.transform.position, 0.5f);
    }

    /// <summary>
    /// 圧力ごとの予測線表示
    /// </summary>
    private void DrawPressureLines() 
    {
        Gizmos.color = new Color(0, 1, 1, 0.0625f);
        for (int i = 0; i < BurstVecList.Count; i++)
            for (int j = 0; j < PressureValList.Count; j++)
                Gizmos.DrawLineStrip(DrawDebris(i, j).ToArray(), false);
    }

    /// <summary>
    /// 破片の予測線表示
    /// </summary>
    /// <param name="num"></param>
    /// <param name="pressure"></param>
    /// <returns></returns>
    private List<Vector3> DrawDebris(int num,int pressure) 
    {
        float deltaTime = 0.04167f;

        float power = Pressure(pressure);
        
        List<Vector3> Points = new List<Vector3>();

        // 初期位置設定
        Vector3 position = GetCreatePosition(1, num);
        Points.Add(this.transform.position + CreateOffsetPosition);
        Points.Add(position);
        // 初速度の設定
        Vector3 Velocity = (GetFlyVector(num) * deltaTime * deltaTime * 0.5f) * power;
        RaycastHit hit  = new RaycastHit();
        // ぶつかるまでの軌道の線を引く
        for (float time = 0.0f; time < 10; time += deltaTime) 
        {
            Velocity += Vector3.down * (9.81f * deltaTime * deltaTime);
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

    private void DrawShockWave() 
    {
        Gizmos.color = new Color(1, 0, 1, 0.5f);
        Gizmos.DrawWireSphere(this.transform.position+CreateOffsetPosition, WaveSize * Pressure(ExpectedPressure));
        Gizmos.color = new Color(1, 1, 0, 0.0625f);
        for (int i = 0; i < PressureValList.Count; i++) Gizmos.DrawWireSphere(this.transform.position+CreateOffsetPosition, WaveSize * Pressure(i));
    }

    /// <summary>
    /// <summary>
    /// インスペクター更新時
    /// </summary>
    private void OnValidate()=> ResetVelocity();
    

    public void ResetVelocity() 
    {
        for (int i = 0; i < DebrisPositions.Count; i++)
        {
            if (DebrisPositions[i] == null) continue;
            if (BurstVecList.Count <= i) BurstVecList.Add(Vector3.zero);
            BurstVecList[i] = GetAcceleration(i);
        }
        for (int i = BurstVecList.Count; i > DebrisPositions.Count; i--) BurstVecList.RemoveAt(i - 1);
    }
    private Vector3 GetAcceleration(int num) 
    {
        float deltaTime = 0.033334f;
        float gravityA = 9.81f;
        
        Vector3 firstPos = this.transform.position + CreateOffsetPosition;
        Vector3 endPos = DebrisPositions[num].transform.position;

        Vector3 accelerationVec = endPos - firstPos;
        accelerationVec.y += (gravityA * time * time);
        accelerationVec /= Pressure(ExpectedPressure);
        
        accelerationVec /= time * deltaTime;
        return accelerationVec;
    }

#endif // UNITY_EDITOR
}