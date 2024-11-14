using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    private BREAKSTATE CurrentBreakState;   //現在の崩壊状態
    [SerializeField]
    private int CurrentBreakAreaNum = 0;    //現在の崩壊エリア番号


    [SerializeField, Header("崩壊するエリア※順番に登録")]
    private List<GameObject> BreakArea;

    [SerializeField, Header("崩壊スピード")]
    private float BreakSpeed = 1.0f;
    [SerializeField, Header("崩壊時間間隔")]
    private float BreakTime = 5.0f;
    [SerializeField, Header("アラートを表示してから壊れるまでの時間")]
    private float AlartTime = 3.0f;

    [Header("==============サワルナキケン==============")]
    [SerializeField, Header("崩壊アラートPrefab")]
    private GameObject BreakAlartBord;
    [SerializeField, Header("崩壊するオブジェクトのLayer")]
    private LayerMask breakLayer;

    private float BreakTimeMesure = 0.0f;   //崩壊時間計測用
    
   
    private GameObject CurrentAlartObj;     //現在再生中のアラートObj

    // Start is called before the first frame update
    void Start()
    {
        CurrentBreakState = BREAKSTATE.NONE;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //時間計測
        BreakTimeMesure += Time.deltaTime * BreakSpeed;

        if(BreakTimeMesure < BreakTime) { return; }

        switch(CurrentBreakState)
        {
            case BREAKSTATE.NONE:
                //アラートを生成
                CreateAlart(BreakArea[CurrentBreakAreaNum].transform);
                CurrentBreakState = BREAKSTATE.ALART;
                break;
            case BREAKSTATE.ALART:
                //再生終了したらアラートObjectが消えるので消えたら崩壊
                bool AlartEnd = CurrentAlartObj == null;
                if (AlartEnd) { CurrentBreakState = BREAKSTATE.BREAK; }
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
            hits[i].collider.gameObject.SetActive(false);
            //SimplestarGame.VoronoiFragmenter voronoi;
            //hits[i].collider.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
            //if (voronoi) { voronoi.Fragment(hits[i]); }
        }

    }

    /// <summary>
    /// アラートの生成
    /// </summary>
    /// <param 破壊エリアのTransform="areatrans"></param>
    private void CreateAlart(Transform areatrans)
    {
        //アラートの生成
        CurrentAlartObj = Instantiate(BreakAlartBord);
        CurrentAlartObj.transform.position = areatrans.position;

        //アラートのAnimatorを取得して設定、再生する
        Animator anim;
        CurrentAlartObj.TryGetComponent<Animator>(out anim);
        anim.SetBool("Alart",true);
        anim.SetFloat("AlartTime", AlartTime);
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(BreakArea[CurrentBreakAreaNum].transform.position, BreakArea[CurrentBreakAreaNum].transform.localScale);
    }

}
