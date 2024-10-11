using UnityEngine;

[System.Serializable]
public class CS_CreateShockWave
{
    [SerializeField, Tooltip("衝撃波プレハブ:\n")]
    private GameObject ShockWaveObj;

    [SerializeField, Tooltip("衝撃波の力:\n他オブジェクトに当たった時の加える力")]
    private float ShockPower = 1f;

    [SerializeField, Tooltip("衝撃波範囲:\n")]
    private float WaveSize = 1f;
    [SerializeField, Tooltip("衝撃波速度:\n")]
    private float WaveSpeed= 1f;


    // コンストラクタ
    public CS_CreateShockWave() {}
    // デストラクタ
    ~CS_CreateShockWave() {}

    /// <summary>
    /// 衝撃波を起こす
    /// </summary>
    /// <returns></returns>
    public void ShockWave(float Power,Vector3 position,Vector3 offset)
    {
        float range = WaveSize * Power;
        float speed = WaveSpeed * Power;
        float time = 1.5f;

        GameObject obj = Object.Instantiate(ShockWaveObj);
        obj.transform.position = position + offset;

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
    /// 範囲描画
    /// </summary>
    /// <param name="power"></param>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    public void DrawShockWave(float power,Vector3 position,Vector3 offset)
    {
        Gizmos.DrawWireSphere(position + offset, WaveSize * power);
    }

}