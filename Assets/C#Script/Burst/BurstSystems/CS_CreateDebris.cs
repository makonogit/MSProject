using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CS_CreateDebris
{
    [SerializeField, Tooltip("破片プレハブ:\n")]
    private List<GameObject> DebrisObjs = new List<GameObject>();
    [SerializeField, Tooltip("破片の方向:\n")]
    private List<Vector3> BurstVecList = new List<Vector3>();
#if UNITY_EDITOR

    [SerializeField, Tooltip("オブジェクト:\n")]
    private List<GameObject> DebrisPositions = new List<GameObject>();
    [SerializeField, Tooltip("到達時間:\n")]
    private float time = 2.0f;

#endif // UNITY_EDITOR

    
    /// <summary>
    /// コンストラクタ 
    /// </summary>
    public CS_CreateDebris() { }
    
    /// <summary>
    /// デストラクタ
    /// </summary>
    ~CS_CreateDebris() 
    {
        DebrisObjs.Clear();
        BurstVecList.Clear();
    }

    /// <summary>
    /// 破片が飛ぶ処理
    /// </summary>
    public void BurstDebris(float Power, Vector3 position, Vector3 offset)
    {
        float speed = Power;
        for (int i = 0; i < BurstVecList.Count; i++) CreateDebris(speed, i, position, offset);
    }

    /// <summary>
    /// 破片の生成
    /// </summary>
    /// <param name="Power"></param>
    /// <param name="num"></param>
    private void CreateDebris(float Power, int num, Vector3 position, Vector3 offset)
    {
        GameObject obj = Object.Instantiate(GetDebrisObject(num));
        obj.transform.position = GetCreatePosition(1, num,position,offset);

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("null component!");
            return;
        }
        Vector3 vector = GetFlyVector(num) * Power;
        rb.AddForce(vector, ForceMode.Force);
    }

    /// <summary>
    /// リストから一つのオブジェクトを返す
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private GameObject GetDebrisObject(int num)
    {
        int number = Mathf.Min(Mathf.Max(0, num), DebrisObjs.Count - 1);
        if (DebrisObjs.Count == 0) return new GameObject("null");
        if (DebrisObjs[number] == null) return new GameObject("null");
        if (num >= DebrisObjs.Count) return DebrisObjs[Random.Range(0, DebrisObjs.Count - 1)];
        return DebrisObjs[number];
    }

    /// <summary>
    /// 破片の生成位置を取得する
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private Vector3 GetCreatePosition(float radius, int num,Vector3 position ,Vector3 offset)
    {
        Vector3 Value = new Vector3();
        Value += position;
        Value += GetFlyVector(num).normalized;
        Value += offset;
        return Value;
    }

    /// <summary>
    /// 飛ぶ方向を取得する
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="maxNum"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private Vector3 GetFlyVector(float radius, int maxNum, int num)
    {
        float phi = Mathf.Acos(1 - 2 * (num + 0.5f) / maxNum);
        float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * num;

        float x = radius * Mathf.Cos(theta) * Mathf.Sin(phi);
        float y = radius * Mathf.Sin(theta) * Mathf.Sin(phi);
        float z = radius * Mathf.Cos(phi);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 飛ぶ方向を取得
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    private Vector3 GetFlyVector(int num)
    {
        bool IsOver = BurstVecList.Count <= num;
        bool IsUnder = num < 0;
        bool IsOutsideArray = IsOver && IsUnder;
        if (IsOutsideArray) return Vector3.zero;
        return BurstVecList[num];
    }

#if UNITY_EDITOR
    /// <summary>
    /// 破片の予測線を表示する
    /// </summary>
    /// <param name="power">圧力</param>
    /// <param name="position">オブジェクト</param>
    /// <param name="offset"></param>
    public void DrawDebrisLine(float power,Vector3 position, Vector3 offset) 
    {
        for (int i = 0; i < BurstVecList.Count; i++) 
            Gizmos.DrawLineStrip(ExpectedDebrisPoints(i,power,position, offset).ToArray(), false);
    }

    /// <summary>
    /// 予測の位置リストを返す
    /// 落ちる位置の表示
    /// </summary>
    /// <param name="num"></param>
    /// <param name="power"></param>
    /// <param name="objPosition"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private List<Vector3> ExpectedDebrisPoints(int num,float power, Vector3 objPosition, Vector3 offset)
    {
        float deltaTime = 0.04167f;

        List<Vector3> Points = new List<Vector3>();

        // 初期位置設定
        Vector3 position = GetCreatePosition(1, num, objPosition, offset);
        Points.Add(objPosition + offset);
        Points.Add(position);
        // 初速度の設定
        Vector3 Velocity = (GetFlyVector(num) * deltaTime * deltaTime * 0.5f) * power;
        RaycastHit hit = new RaycastHit();
        // ぶつかるまでの軌道の線を引く
        for (float time = 0.0f; time < 10; time += deltaTime)
        {
            Velocity += Vector3.down * (9.81f * deltaTime * deltaTime);
            Ray ray = new Ray(position, Velocity.normalized);
            bool IsHit = Physics.Raycast(ray, out hit, Velocity.magnitude);
            position += Velocity;
            if (IsHit)
            {
                Points.Add(hit.point);
                break;
            }
            Points.Add(position);
        }
        // 着地点表示
        Gizmos.DrawWireSphere(hit.point, 0.5f);
        Gizmos.DrawSphere(hit.point, 0.5f);

        return Points;
    }

    /// <summary>
    /// 目標地点表示
    /// </summary>
    public void DrawDestination() 
    {
        foreach (GameObject obj in DebrisPositions) Gizmos.DrawWireSphere(obj.transform.position, 0.5f);
    }

    /// <summary>
    /// 加速度と方向の再設定
    /// </summary>
    /// <param name="power"></param>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    public void ResetVelocity(float power, Vector3 position, Vector3 offset)
    {
        for (int i = 0; i < DebrisPositions.Count; i++)
        {
            if (DebrisPositions[i] == null) continue;
            if (BurstVecList.Count <= i) BurstVecList.Add(Vector3.zero);
            BurstVecList[i] = GetAcceleration(i,power,position,offset);
        }
        for (int i = BurstVecList.Count; i > DebrisPositions.Count; i--) BurstVecList.RemoveAt(i - 1);
    }

    /// <summary>
    /// 加速度の方向の計算
    /// </summary>
    /// <param name="num"></param>
    /// <param name="power"></param>
    /// <param name="position"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Vector3 GetAcceleration(int num,float power, Vector3 position, Vector3 offset)
    {
        float deltaTime = 0.033334f;
        float gravityA = 9.81f;

        Vector3 firstPos = position + offset;
        Vector3 endPos = DebrisPositions[num].transform.position;

        Vector3 accelerationVec = endPos - firstPos;
        accelerationVec.y += (gravityA * time * time);
        accelerationVec /= power;

        accelerationVec /= time * deltaTime;
        return accelerationVec;
    }
#endif // UNITY_EDITOR

}
