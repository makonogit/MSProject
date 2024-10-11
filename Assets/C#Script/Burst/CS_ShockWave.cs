using System;
using System.Net.NetworkInformation;
using UnityEngine;

public class CS_ShockWave : MonoBehaviour
{
    [SerializeField, Tooltip("消滅時間:\n消滅するまでの時間")]
    private float destroyTime;

    public float DestroyTime { set { destroyTime = value; } }

    [SerializeField, Tooltip("速度:\n")]
    private float speed;
    public float Speed { set { speed = value; } }

    [SerializeField, Tooltip("攻撃力:\n")]
    private float power;
    public float Power { set { power = value; } }

    [SerializeField, Tooltip("速度:\n")]
    private float maxSize;
    public float MaxSize { set { maxSize = value; } }

    [SerializeField, Tooltip("現サイズ:\n")]
    private float nowSize = 0.01f;
    /// <summary>
    /// Start
    /// </summary>
    private void Start() { }

    private void FixedUpdate()
    {
        ChangeSize();
        UntilDestroyMyself();
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update() { }

    /// <summary>
    /// サイズ変更
    /// </summary>
    private void ChangeSize() 
    {
        nowSize += (Time.deltaTime * speed);
        Vector3 size = Vector3.one * nowSize;
        this.transform.localScale = size;
    }

    /// <summary>
    /// 消滅するまでの処理
    /// </summary>
    private void UntilDestroyMyself()
    {
        // 破棄する
        bool shouldDestroy = maxSize <= nowSize;
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
            burst.HitDamage(power);
        }
    }
    private void OnValidate()
    {
        Vector3 size = Vector3.one * nowSize;
        this.transform.localScale = size;
    }
}
