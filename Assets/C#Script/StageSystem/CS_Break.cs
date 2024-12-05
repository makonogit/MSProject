using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 担当:菅　崩壊
/// </summary>
public class CS_Break : MonoBehaviour
{
    private enum BREAKSTATE
    {
        NONE = 0,
        ALART = 1,
        BREAK = 2,
    }

    [Header("確認用......")]
    [SerializeField]
    private BREAKSTATE CurrentBreakState;   //現在の崩壊状態
    [SerializeField]
    private bool StopBreak = false;     //崩壊しているか
    [SerializeField]
    private int CurrentBreakAreaNum = 0;    //現在の崩壊エリア番号

    [SerializeField, Header("崩壊するエリア※順番に登録")]
    private List<GameObject> BreakArea;

    public void SetBreakList(List<GameObject> list) { BreakArea = list; }

    [Header("----------------------------------------------------")]


    [SerializeField, Header("崩壊スピード")]
    private float BreakSpeed = 1.0f;
    [SerializeField, Header("崩壊時間間隔")]
    private float BreakTime = 5.0f;
    [SerializeField, Header("アラートを表示してから壊れるまでの時間")]
    private float AlartTime = 3.0f;
    [SerializeField, Header("アラートの表示する高さ")]
    private float ArartHeight = 2.4f;
    [SerializeField, Header("アラートがどれだけ内側に来るか")]
    private float ArartInnerOffset = 1.0f;

    [Header("==============サワルナキケン==============")]
    [SerializeField, Header("崩壊アラートPrefab")]
    private GameObject BreakAlartBord;
    [SerializeField, Header("アラート演出UI")]
    private GameObject ArartEffectUI;
    [SerializeField, Header("崩壊するオブジェクトのLayer")]
    private LayerMask breakLayer;

    private float BreakTimeMesure = 0.0f;   //崩壊時間計測用
    
   
    private GameObject CurrentAlartObj;     //現在再生中のアラートObj


    /// <summary>
    /// 崩壊を停止させる
    /// </summary>
    /// <param trueで停止falseで解除="stopflg"></param>
    public void ArartStop(bool stopflg)
    {
        StopBreak = stopflg;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentBreakState = BREAKSTATE.NONE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //停止中は更新しない
        if (StopBreak) { return; }
 
        //時間計測
        BreakTimeMesure += Time.deltaTime * BreakSpeed;

        if(BreakTimeMesure < BreakTime) { return; }

        switch(CurrentBreakState)
        {
            case BREAKSTATE.NONE:
                //アラートを生成
                CreateAlart(BreakArea[CurrentBreakAreaNum].transform);
                CurrentBreakState = BREAKSTATE.ALART;
                ArartEffectUI.SetActive(true);
                break;
            case BREAKSTATE.ALART:
                //再生終了したらアラートObjectが消えるので消えたら崩壊
                bool AlartEnd = CurrentAlartObj == null;
                if (AlartEnd) { ArartEffectUI.SetActive(false); CurrentBreakState = BREAKSTATE.BREAK; }
                break;
            case BREAKSTATE.BREAK:
                BreakStage(BreakArea[CurrentBreakAreaNum].transform);
                //初期化と更新
                BreakTimeMesure = 0f;
                CurrentBreakAreaNum++;
                CurrentBreakState = BREAKSTATE.NONE;
                break;
        }
    }


    /// <summary>
    /// 破壊システム
    /// </summary>
    /// <param 破壊エリアのTransform="areatrans"></param>
    private void BreakStage(Transform areatrans)
    {
        RaycastHit[] hits = Physics.BoxCastAll(areatrans.position, areatrans.localScale * 0.5f, Vector3.one,areatrans.rotation, 1f, breakLayer);
        if(hits.Length <= 0) { return; }

        //衝突したオブジェクト全てを破壊
        for(int i = 0; i< hits.Length;i++)
        {
            //if(hits[i].collider)
            //hits[i].collider.gameObject.SetActive(false);


            SimplestarGame.VoronoiFragmenter voronoi;
            //階段の処理、子オブジェクトになっているので
            if (hits[i].collider.gameObject.name == "Collider")
            {
                hits[i].transform.parent.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
                Destroy(hits[i].collider.gameObject);
            }
            else
            {
                hits[i].collider.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
            }

            hits[i].point = hits[i].collider.transform.position;
            
            if (voronoi) { voronoi.Fragment(hits[i]); }
            //else { Debug.Log(hits[i].transform.name); }
        }

    }

    /// <summary>
    /// アラートの生成
    /// </summary>
    /// <param 破壊エリアのTransform="areatrans"></param>
    private void CreateAlart(Transform areatrans)
    {
        Vector3 HalfScale = areatrans.localScale * 0.5f;

        Vector3[] ArartPos = new Vector3[]
        {
            new Vector3(0f                               , ArartHeight,-(HalfScale.z - ArartInnerOffset)), // 上
            new Vector3(-(HalfScale.x - ArartInnerOffset), ArartHeight,0f                               ), // 右
            new Vector3(0f                               , ArartHeight,HalfScale.z - ArartInnerOffset   ), // 下
            new Vector3(HalfScale.x - ArartInnerOffset   , ArartHeight,0f                               )  // 左
        };

        // 各コーナーのワールド座標を計算
        Vector3[] WorldCorners = new Vector3[ArartPos.Length];
        for (int i = 0; i < ArartPos.Length; i++)
        {
            WorldCorners[i] = areatrans.position + ArartPos[i];
        }

        //Debug.Log("4隅の座標" + "\n左下" + WorldCorners[0] + "\n左上" + WorldCorners[1] + "\n右下" + WorldCorners[2] + "\n右上" + WorldCorners[3]);

        //アラートの生成
        CurrentAlartObj = Instantiate(BreakAlartBord);
        
        //アラートの位置を設定
        for(int i=0;i<CurrentAlartObj.transform.childCount;i++)
        {
            Transform childtrans = CurrentAlartObj.transform.GetChild(i).transform;
            childtrans.position = WorldCorners[i];
            childtrans.localScale = new Vector3(areatrans.localScale.x * 0.08f, childtrans.localScale.y, childtrans.localScale.x * 0.08f);
            //アラートのサイズ倍率0.025f
        }

        //CurrentAlartObj.transform.position = areatrans.position;

        //アラートのAnimatorを取得して設定、再生する
        Animator anim;
        CurrentAlartObj.TryGetComponent<Animator>(out anim);
        anim.SetBool("Alart",true);
        anim.SetFloat("AlartTime", AlartTime);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(BreakArea.Count > 0)Gizmos.DrawCube(BreakArea[CurrentBreakAreaNum].transform.position, BreakArea[CurrentBreakAreaNum].transform.localScale);
    }

}
