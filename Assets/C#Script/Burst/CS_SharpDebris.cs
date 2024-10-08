using Unity.VisualScripting;
using UnityEngine;

public class CS_SharpDebris : CS_Debris
{
    [SerializeField, Tooltip("刺さるための必要速度")]
    private float StabSpeed;

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
    /// 
    /// </summary>
    private void FixedUpdate() 
    {
       // 飛ぶスピードが速いとトリガーになる
        thisCollider.isTrigger = IsOverSpeed;
        // 削除処理
        if (ShouldDestroyExecution && !IsSpecificLayer(default)) UntilDestroyMyself();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(HitCollisionsEnter(other)) UntilStab();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision) 
    {
        if (HitCollisionsEnter(collision)) DestroyFlag = true;
    }
    
    /// <summary>
    /// 刺さる処理
    /// </summary>
    private void UntilStab()
    { 
        // 回転と位置を固定
        thisRB.constraints = RigidbodyConstraints.FreezeAll;
        thisRB.velocity = Vector3.zero;// 念のため
        thisCollider.isTrigger = false;
 
        // レイヤー変更
        this.gameObject.layer = LayerMask.NameToLayer(default);
    }

    /// <summary>
    /// 指定したレイヤーか
    /// </summary>
    /// <param name="layerName"></param>
    /// <returns></returns>
    private bool IsSpecificLayer(string layerName) 
    {
        return this.gameObject.layer == LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    /// 設定された速度より速いか
    /// </summary>
    private bool IsOverSpeed
    {
        get
        {
            return thisRB.velocity.magnitude >= StabSpeed;
        }
    }
}
