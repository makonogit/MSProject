using System;
using UnityEngine;

public class CS_Debris : MonoBehaviour
{
    [Header("消滅")]
    [SerializeField, Tooltip("消滅フラグ:")]
    private bool DestroyFlag;

    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float DestroyTime;


    [SerializeField, Tooltip("攻撃力:\n")]
    private float Power;
    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
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
    private void Update(){}

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

    private void OnCollisionEnter(Collision collision)
    {
        DestroyFlag = true;
        bool GimmickHit = collision.gameObject.tag == "Burst";

        if (GimmickHit)
        {
            CS_Burst_of_object burst = collision.transform.GetComponent<CS_Burst_of_object>();
            if (burst == null) { Debug.LogWarning("null component"); return; }
            burst.HitDamage(Power);
        }
    }
}

