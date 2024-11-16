using System;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class CS_Debris : MonoBehaviour
{
    protected AudioSource thisAS;
    protected Rigidbody thisRB;
    protected Collider thisCollider;

    protected const float MIN_UNDER = -50.0f;



    [Header("消滅")]
    [SerializeField, Tooltip("消滅フラグ:")]
    protected bool DestroyFlag;

    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    protected float DestroyTime;

    [SerializeField, Tooltip("攻撃力:\n")]
    protected float Power;

    [Header("サウンド関係")]
    [SerializeField, Tooltip("速度の範囲:\n X ＝ 下限　Y ＝ 上限\n速度によってピッチの値が変わる範囲。")]
    protected Vector2 VelocityRange = new Vector2(0.5f, 10.0f);
    [SerializeField, Tooltip("ピッチの範囲:\n X ＝ 下限　Y ＝ 上限\n")]
    protected Vector2 PitchRange = new Vector2(0.75f, 2.0f);

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        thisAS = GetComponent<AudioSource>();
        if (thisAS == null) Debug.LogError("null component");
        thisRB = GetComponent<Rigidbody>();
        if (thisRB == null) Debug.LogError("null component");
        thisCollider = GetComponent<Collider>();
        if (thisCollider == null) Debug.LogError("null component");
    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
    private void FixedUpdate()
    {
        if (ShouldDestroyExecution) UntilDestroyMyself();
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
        if (HitCollisionsEnter(collision)) DestroyFlag = true;
    }
    
    /// <summary>
    /// 削除処理の実行条件
    /// </summary>
    protected bool ShouldDestroyExecution 
    {
        get 
        {
            bool IsUnderWorld = transform.position.y < MIN_UNDER;
            if (IsUnderWorld) return true;
            if (DestroyFlag) return true;
            return false;
        }
    }

    /// <summary>
    /// 消滅するまでの処理
    /// </summary>
    protected void UntilDestroyMyself()
    {
        DestroyTime -= Time.deltaTime;
        // 破棄する
        bool isTimeOver = DestroyTime <= 0;
        bool isStopSound = !thisAS.isPlaying;
        bool shouldDestroy =isTimeOver && isStopSound;
        if (shouldDestroy) Destroy(this.gameObject);
    }

    /// <summary>
    /// オブジェクトに当たった処理
    /// </summary>
    /// <param name="collision"></param>
    /// <returns>同タグなら false  別タグなら true </returns>
    protected bool HitCollisionsEnter(Collider collision)
    {
        bool isHitAttackTag = collision.gameObject.tag == this.tag;
        bool isHitBurstObj = collision.gameObject.tag == "Burst";
        bool isGetRigidbody = thisRB != null;
        bool canPlaySound = isGetRigidbody && !DestroyFlag;

        // 同じタグなら抜ける
        if (isHitAttackTag) return false;
        // はじけるオブジェクトならダメージを与える
        if (isHitBurstObj) InflictDamage(collision);
        // 効果音を鳴らす
        if (canPlaySound) PlaySound();

        return true;
    }
    protected  bool HitCollisionsEnter(Collision collision) => HitCollisionsEnter(collision.collider);

    

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="collision"></param>
    protected void InflictDamage(Collider collision)
    {
        CS_Burst_of_object burst = collision.transform.GetComponent<CS_Burst_of_object>();
        if (burst == null) { Debug.LogWarning("null component"); return; }
        burst.HitDamage(Power);
    }
    protected void InflictDamage(Collision collision) => InflictDamage(collision.collider);





    // 以下より効果音関係の関数のみ

    /// <summary>
    /// 音の再生
    /// </summary>
    protected void PlaySound()
    {
        thisAS.pitch = GetPitch(PitchRange);
        thisAS.Play();
    }

    /// <summary>
    /// 速度に合わせったピッチを返す
    /// </summary>
    /// <param name="range">範囲</param>
    /// <returns>範囲内</returns>
    private float GetPitch(Vector2 range) => GetPitch(range.x, range.y);

    /// <summary>
    ///  値から指定した範囲の割合(パーセンテージ)を返す
    /// </summary>
    /// <param name="range">範囲</param>
    /// <param name="value">値</param>
    /// <returns> 0～1.0f </returns>
    private float GetProportion(Vector2 range, float value) => GetProportion(range.x, range.y, value);


    /// <summary>
    /// 速度に合わせったピッチを返す
    /// </summary>
    /// <param name="min">下限</param>
    /// <param name="max">上限</param>
    /// <returns>下限～上限</returns>
    private float GetPitch(float min, float max)
    {
        float velocityMag = thisRB.velocity.magnitude;
        float value = GetProportion(VelocityRange, velocityMag);
        float ratio = max - min;
        value *= ratio;
        value += min;

        return value;
    }


    /// <summary>
    ///  値から指定した範囲の割合(パーセンテージ)を返す 
    /// </summary>
    /// <param name="min">下限</param>
    /// <param name="max">上限</param>
    /// <param name="value">値</param>
    /// <returns> 0～1.0f </returns>
    private float GetProportion(float min, float max, float value)
    {
        float subMax = max - min;
        float val = value - min;
        val /= subMax;

        val = Mathf.Min(1.0f, val);
        val = Mathf.Max(0, val);
        return val;
    }

}

