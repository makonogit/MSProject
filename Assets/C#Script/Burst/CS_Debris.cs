using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CS_Debris : MonoBehaviour
{
    private AudioSource thisAS;
    private Rigidbody thisRB;

    private const float MIN_UNDER = -50.0f;

    [Header("消滅")]
    [SerializeField, Tooltip("消滅フラグ:")]
    private bool DestroyFlag;

    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float DestroyTime;


    [SerializeField, Tooltip("攻撃力:\n")]
    private float Power;

    [Header("サウンド関係")]
    [SerializeField, Tooltip("速度の範囲:\n X ＝ 下限　Y ＝ 上限\n速度によってピッチの値が変わる範囲。")]
    private Vector2 VelocityRange = new Vector2(0.5f, 10.0f);
    [SerializeField, Tooltip("ピッチの範囲:\n X ＝ 下限　Y ＝ 上限\n")]
    private Vector2 PitchRange = new Vector2(0.75f, 2.0f);

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        thisAS = GetComponent<AudioSource>();
        if (thisAS == null) Debug.LogError("null component");
        thisRB = GetComponent<Rigidbody>();
        if (thisRB == null) Debug.LogError("null component");
    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
    private void FixedUpdate()
    {
        UntilDestroyMyself();
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update() { }

    /// <summary>
    /// 消滅するまでの処理
    /// </summary>
    private void UntilDestroyMyself()
    {
        bool isOutArea = transform.position.y < MIN_UNDER;
        if (isOutArea) DestroyFlag = true;

        // 破壊しない
        if (!DestroyFlag) return;
        DestroyTime -= Time.deltaTime;
        // 破棄する
        bool shouldDestroy = DestroyTime <= 0;
        if (shouldDestroy) Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {

        DestroyFlag = true;
        bool isHitBurstObj = collision.gameObject.tag == "Burst";
        bool isHitAttackTag = collision.gameObject.tag == this.tag;
        bool canPlaySound = thisRB != null;

        if (isHitBurstObj)
        {
            CS_Burst_of_object burst = collision.transform.GetComponent<CS_Burst_of_object>();
            if (burst == null) { Debug.LogWarning("null component"); return; }
            burst.HitDamage(Power);
        }

        if (isHitAttackTag) return;

        if (canPlaySound) PlaySound();
    }

    /// <summary>
    /// 音の再生
    /// </summary>
    private void PlaySound()
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

