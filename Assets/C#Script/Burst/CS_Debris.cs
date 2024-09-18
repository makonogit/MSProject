using System;
using UnityEngine;

public class CS_Debris : MonoBehaviour
{
    [Header("消滅")]
    [SerializeField, Tooltip("消滅フラグ:")]
    private bool DestroyFlag;

    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float DestroyTime;


    private void Start()
    {
    }
    private void Update()
    {
        UntilDestroyMyself();
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
        if (shouldDestroy) Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DestroyFlag = true;
    }
}

