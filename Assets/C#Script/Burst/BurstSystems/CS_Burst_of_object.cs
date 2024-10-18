using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class CS_Burst_of_object : MonoBehaviour
{
    //-変数-
    private Collider thisCollider;

    private AudioSource thisAudioSource;

    private Vector3 defaultScale;

    private MeshRenderer thisMeshRenderer;

    [SerializeField, Tooltip("壊れた後")]
    private GameObject brokenObj;

    [SerializeField, Tooltip("爆破エフェクト")]
    private VisualEffect ExplosionEffect;
    [SerializeField, Tooltip("破片のエフェクト")]
    private ParticleSystem DebrisParticle;

    [SerializeField, Tooltip("地雷設定")]
    private bool MineFlag = false;

    [Header("体力")]
    [SerializeField, Tooltip("耐久力")]
    private float Health;
    
    [Header("圧力")]
    
    [SerializeField, Tooltip("圧力番号:\n番号から下記の圧力配列の値を参照する為の変数です。")]
    [Range(0,10)]
    private int PressureNumber;
    [SerializeField, Tooltip("圧力配列:\n数値は、1 = 100%として扱われます。")]
    private List<float> PressureValList = new List<float>();
    [SerializeField, Tooltip("圧力配列:\n数値は、1 = 100%として扱われます。")]
    private float ExplosionVolume = 0.9f;


    [Header("消滅")]
    [SerializeField, Tooltip("消滅フラグ:")]
    private bool DestroyFlag;
    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float DestroyTime;

    [Header("破片")]
    [SerializeField, Tooltip("破片:\n")]
    private CS_CreateDebris Debris = new CS_CreateDebris();
   
    [Header("衝撃波")]
    [SerializeField, Tooltip("衝撃波:\n")]
    private CS_CreateShockWave shockWave = new CS_CreateShockWave();
    
    [SerializeField, Tooltip("生成位置調整:\n破片や衝撃波の生成位置をずらす為のオフセット")]
    protected Vector3 CreateOffset = Vector3.zero;
    
#if UNITY_EDITOR
    [Header("―――――――――――――――――――――――――――――――――――――――――――")]
    [Header("表示関係")]

    [SerializeField, Tooltip("想定圧力:\n想定圧力を基準に予測線表示や加速度が決まる")]
    [Range(0, 10)]
    private int ExpectedPressure;
    [SerializeField, Tooltip("破片の予測線表示")]
    private bool DebrisShow = false;
    [SerializeField, Tooltip("各圧力の破片の予測線表示")]
    private bool DebrisPressureShow = false;
    [SerializeField, Tooltip("破片の目標地点の表示")]
    public bool DestinationShow = false;

    [SerializeField, Tooltip("衝撃波範囲表示:\n")]
    private bool ShockWaveShow = false;
    [SerializeField, Tooltip("衝撃波各圧力範囲表示:\n")]
    private bool ShockWavePressureShow = false;
    [Header("予測線の色")]
    [SerializeField, Tooltip("破片予測線の色")]
    private Color DebrisColor = new Color(0, 1, 0, 0.5f);
    [SerializeField, Tooltip("各圧力破片予測線の色")]
    private Color DebrisPressureColor = new Color(0, 1, 1, 0.0625f);
    public Color DestinationColor = new Color(1, 0, 0, 0.25f);

    [SerializeField, Tooltip("破片予測線の色")]
    private Color ShockWaveColor = new Color(1, 0, 1, 0.5f);
    [SerializeField, Tooltip("各圧力破片予測線の色")]
    private Color ShockWavePressureColor = new Color(1, 1, 0, 0.0625f);

#endif // UNITY_EDITOR

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

        if (ExplosionEffect == null) Debug.LogError("null explosion effect");
        ExplosionEffect.Stop();
        if (DebrisParticle == null) Debug.LogError("null debris particle");
        DebrisParticle.Stop();
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
        Debris.BurstDebris(power,this.transform.position,CreateOffset);
        
        // 衝撃波を出す
        shockWave.ShockWave(power,this.transform.position,CreateOffset);
        // 破裂音の再生
        MakeBurstSounds(power);
        // 爆破エフェクトの生成
        CreateExplosionEffect();
        // モデル変更
        ChangeModel();

        // 消失フラグを立てる
        DestroyFlag = true;
    }
    /// <summary>
    /// 爆破エフェクト
    /// </summary>
    private void CreateExplosionEffect()
    {
        ExplosionEffect.Play();
        DebrisParticle.Play();
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
    {
        // モデル配置
        GameObject obj = Instantiate(brokenObj);
        obj.transform.position = this.transform.position;
        // 自オブジェクトのメッシュ非表示
        thisMeshRenderer.enabled = false;
    }

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

    /// <summary>
    /// 
    /// </summary>
    public void Info() 
    {
        ResetVelocity();
        // 破片の予測線    
        if(DebrisShow)DrawExpectedDebris();

        // 衝撃波
        if (ShockWaveShow) DrawShockWave();
    }
    /// <summary>
    /// 
    /// </summary>
    private void DrawExpectedDebris() 
    {
        Gizmos.color = DebrisColor;
        Debris.DrawDebrisLine(Pressure(ExpectedPressure),this.transform.position,CreateOffset);
        // 圧力ごとに予測線
        if (DebrisPressureShow) DrawPressureLines();
        Gizmos.color = DestinationColor;
        // 目的地表示
        if(DestinationShow)Debris.DrawDestination();
    }

    /// <summary>
    /// 圧力ごとの予測線表示
    /// </summary>
    private void DrawPressureLines() 
    {
        Gizmos.color = DebrisPressureColor;
        for (int i = 0; i < PressureValList.Count; i++) Debris.DrawDebrisLine(Pressure(i),this.transform.position,CreateOffset);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DrawShockWave() 
    {
        
        // 現在の想定範囲表示
        Gizmos.color = ShockWaveColor;
        shockWave.DrawShockWave(Pressure(ExpectedPressure),this.transform.position, CreateOffset);
        // 圧力ごとに範囲表示
        if (ShockWavePressureShow) 
        {
            Gizmos.color = ShockWavePressureColor;
            for (int i = 0; i < PressureValList.Count; i++) 
                shockWave.DrawShockWave(Pressure(i), this.transform.position, CreateOffset);
        }
        
    }

    /// <summary>
    /// <summary>
    /// インスペクター更新時
    /// </summary>
    private void OnValidate()=> ResetVelocity();
    
    /// <summary>
    /// 
    /// </summary>
    public void ResetVelocity() => Debris.ResetVelocity(Pressure(ExpectedPressure),this.transform.position,CreateOffset);
    
    
#endif // UNITY_EDITOR
}