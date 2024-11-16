using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 落下ギミック
/// 担当：菅
/// </summary>
public class CS_Falling : MonoBehaviour
{
    [SerializeField, Header("落下物生成中心Trans")]
    private Transform CenterTrans;

    [SerializeField, Header("落下オブジェクトプレハブ")]
    private List<GameObject> FallingObj;

    [Header("落下計算用")]
    [SerializeField,Tooltip("落下物生成間隔")]
    private float FallingSpace;
    [SerializeField, Tooltip("落下空気抵抗\n大きいほど落ちるの遅くなるよ")]
    private float FallingDrag;
    [SerializeField, Tooltip("落下物生成範囲(半径)")]
    private float FallingRange;

    //時間計測用
    private float TimeMeasure = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //時間計測
        TimeMeasure += Time.deltaTime;
        bool CreateTime = TimeMeasure > FallingSpace;

        if (CreateTime)
        {
            //生成オブジェクトを決める
            int CreateObj = Random.Range(0, FallingObj.Count);

            //範囲内のランダムな座標を指定
            Vector3 FallingPos = Random.insideUnitCircle * FallingRange;
            FallingPos.x += CenterTrans.position.x;
            FallingPos.z += CenterTrans.position.z;
            FallingPos.y = CenterTrans.position.y + 5.0f;   //生成位置とりあえず高さ+5
            //生成
            GameObject obj = Instantiate(FallingObj[CreateObj]);
            obj.transform.position = FallingPos;
            
            Rigidbody objrigid = obj.GetComponent<Rigidbody>();
            objrigid.drag = FallingDrag;

            //なんか回転させてみた
            objrigid.AddTorque(1, 0.5f, 0);
            
            //次の生成まで時間をリセット
            TimeMeasure = 0.0f;
        }
    }
}
