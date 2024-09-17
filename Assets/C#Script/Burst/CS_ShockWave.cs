using System;
using UnityEngine;

public class CS_ShockWave : MonoBehaviour 
{
    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float DestroyTime;

    [SerializeField, Tooltip("速度:\n")]
    private float Speed;

    private void Update()
    {
        Vector3 size = Vector3.one;

        this.transform.localScale += size * (Time.deltaTime * Speed);

        UntilDestroyMyself();
    }

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
}
