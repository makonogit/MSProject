using System;
using UnityEngine;

public class CS_ShockWave : MonoBehaviour 
{
    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float DestroyTime;

    [SerializeField, Tooltip("速度:\n")]
    private float Speed;

    [SerializeField, Tooltip("攻撃力:\n")]
    private float Power;

    /// <summary>
    /// Start
    /// </summary>
    private void Start(){}

    private void FixedUpdate()
    {
        Vector3 size = Vector3.one;
        this.transform.localScale += size * (Time.deltaTime * Speed);
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
        DestroyTime -= Time.deltaTime;
        // 破棄する
        bool shouldDestroy = DestroyTime <= 0;
        if (shouldDestroy) Destroy(this.gameObject);
    }
 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        
        bool GimmickHit = other.gameObject.tag == "Burst";

        if (GimmickHit)
        {
            
            CS_Burst_of_object burst = other.transform.GetComponent<CS_Burst_of_object>();
            if (burst == null) { Debug.LogWarning("null component"); return; }
            burst.HitDamage(Power);
        }
    }
}
