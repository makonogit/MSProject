using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// 担当;菅　ステージ崩壊システム
/// </summary>
public class CS_StageBreak : MonoBehaviour
{
    [SerializeField, Header("崩壊オブジェクトリスト")]
    private List<GameObject> BreakObjList;

    [SerializeField,Header("あああああ")]
    private int CurrentBreakNum = 0;    //現在壊れているオブジェクト番号
    private GameObject AreaBlock;       //現在壊れているエリアの座標


    [Header("崩壊検知関連")]
    [SerializeField,Tooltip("判定サイズ")]
    private Vector3 BoxSize;
    [SerializeField, Tooltip("判定するArealayer")]
    private LayerMask AreaLayer;
    [SerializeField, Tooltip("判定するlayer")]
    private LayerMask layer;
    
    private int BreakObjCount = 0;     //壊れたオブジェクトの数
    private int MaxBreakObjCount = 0; //壊れるオブジェクトの最大数
    private float BreakRate = 0;  //崩壊度

    [Header("移動関係")]
    [SerializeField, Tooltip("崩壊ルート")]
    private SplineAnimate splineanim;
    //[SerializeField, Tooltip("移動速度")]
    //private float MoveSpeed = 1f;
    

    // Start is called before the first frame update
    void Start()
    {
        //壊れるオブジェクトの数を保存(全て子オブジェクト)
        for (int i= 0; i < BreakObjList.Count; i++)
        {
            for(int j = 0; j < BreakObjList[i].transform.childCount; j++)
            {
                MaxBreakObjCount += BreakObjList[i].transform.GetChild(j).transform.childCount;
            }
        }

        AreaBlock = BreakObjList[CurrentBreakNum];
        //開始時のコライダーを全てアクティブにする

        
        SetBreakArea();

        TemporaryStorage.DataRegistration("Stage", 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        BreakJudgment();
        ChangeArea();

        //データをセーブ
        TemporaryStorage.DataSave("Stage",Mathf.FloorToInt(BreakObjCount / MaxBreakObjCount * 100f));
    }

    /// <summary>
    /// 崩壊検知(Ray)
    /// </summary>
    private void BreakJudgment()
    {
        //あたり判定
        // ボックス内に入っているコライダーを取得
        Collider[] Hits = Physics.OverlapBox(transform.position, BoxSize / 2, transform.rotation, layer);

        bool hitflg = Hits.Length > 0;

        if (!hitflg) { return; }

        //階段だけ応急処置
        if (Hits[0].gameObject.name == "Collider") { Destroy(Hits[0].transform.parent.gameObject); }
        //layerを変更
        //hit.transform.gameObject.layer = Breakedlayer;
        //衝突したオブジェクトを崩壊状態にする
        Destroy(Hits[0].gameObject);   //消す
        BreakObjCount++;                    //壊れたオブジェクトの数をカウント
    }

    /// <summary>
    /// エリア変更
    /// </summary>
    private void ChangeArea()
    {
        // RaycastHit AreaHit;
        // ボックス内に入っているコライダーを取得
        Collider[] AreaHits = Physics.OverlapBox(transform.position, BoxSize / 2, transform.rotation, AreaLayer);

        //あたり判定
        bool Areahitflg = AreaHits.Length > 0;
        if (!Areahitflg) { return; }

       
        GameObject oldarea = AreaBlock; //前回分記録しておく
        AreaBlock = AreaHits[0].gameObject;

        bool ChangeAreaFlg = oldarea != AreaBlock;

        if (ChangeAreaFlg)
        {
            oldarea.GetComponent<BoxCollider>().isTrigger = true;
            if (BreakObjList.Count - 1 > CurrentBreakNum) { CurrentBreakNum++; }
            SetBreakArea(); //エリア変更
        }

    }


    /// <summary>
    /// 壊れる対象のエリアを更新
    /// </summary>
    private void SetBreakArea()
    {
        
        int child = BreakObjList[CurrentBreakNum].transform.childCount;

        for(int i = 0;i<child;i++)
        {
            GameObject breakobj = BreakObjList[CurrentBreakNum].transform.GetChild(i).gameObject;

            //コライダーを有効に
            BoxCollider[] coll = breakobj.GetComponentsInChildren<BoxCollider>();
            foreach (var collider in coll)
            {
                //階段だけ応急処置
                if(collider.gameObject.name == "Stairs_TypeA") { collider.transform.GetChild(0).transform.GetComponent<BoxCollider>().enabled = true; }
                collider.enabled = true;
            }

        }

        //エリアコライダーを無効化
        BoxCollider areacoll;
        BreakObjList[CurrentBreakNum].transform.TryGetComponent<BoxCollider>(out areacoll);
        areacoll.enabled = false;
    }


    /// <summary>
    /// Rayの表示
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, BoxSize);
    }

    /// <summary>
    /// 崩壊度を返す
    /// </summary>
    /// <returns></returns>
    private int GetBreakRate()
    {
       return (int)(MaxBreakObjCount / BreakObjCount);
    }

}
