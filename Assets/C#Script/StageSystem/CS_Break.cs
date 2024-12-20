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

    private int PlayerOnBreakAtraNum = 0;   //プレイヤーが立っているエリア

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
    [SerializeField, Header("崩壊演出UI表示最大距離")]
    private int BreakEffectMaxDistance = 5;

    [Header("==============サワルナキケン==============")]
    [SerializeField, Header("崩壊アラートPrefab")]
    private GameObject BreakAlartBord;
    [SerializeField, Header("アラート演出UI")]
    private Image ArartEffectUI;
    //アラート関係
    private float EffectAlpha = 0f;
    private bool AlphaChange = false;

    [SerializeField, Header("崩壊するオブジェクトのLayer")]
    private LayerMask breakLayer;

    [SerializeField, Header("PlayerのTransform")]
    private Transform PlayerTrans;
  
    private float BreakTimeMesure = 0.0f;   //崩壊時間計測用
   
    private GameObject CurrentAlartObj;     //現在再生中のアラートObj


    /// <summary>
    /// 崩壊を停止させる
    /// </summary>
    /// <param trueで停止falseで解除="stopflg"></param>
    public void ArartStop(bool stopflg)
    {
        StopBreak = stopflg;
        //停止
        if (stopflg)
        {
            ArartEffectUI.enabled = false;
            if (CurrentAlartObj == null) { return; }
            //アラートのAnimatorを取得して一時停止
            Animator anim;
            CurrentAlartObj.TryGetComponent<Animator>(out anim);
            anim.speed = 0;
        }
        //再生
        else
        {
            ArartEffectUI.enabled = true;
            if (CurrentAlartObj == null) { return; }
            //アラートのAnimatorを取得して再生
            Animator anim;
            CurrentAlartObj.TryGetComponent<Animator>(out anim);
            anim.speed = 1;
        }
    }

    //現在の崩壊エリア番号を取得
    public int GetBreakAreaDistance() => PlayerOnBreakAtraNum - CurrentBreakAreaNum;


    /// <summary>
    /// エリアの番号を取得
    /// </summary>
    /// <param BreakAreaオブジェクト="breakarea"></param>
    /// <returns></returns>
    public int GetBreakAreaNum(GameObject breakarea)
    {
        return BreakArea.IndexOf(breakarea);
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
        if (StopBreak) 
        {
            return; 
        }

        //プレイヤーが居てるエリアを取得
        {
            int OldDistance = GetBreakAreaDistance();

            //下向きにRayを飛ばして床Get
            Vector3 Pos = PlayerTrans.position;
            Pos.y += 1f;
            Ray ray = new Ray(Pos, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray, 5f, breakLayer);
            bool AreaHit = hits.Length > 0 && hits[0].collider.transform.childCount > 0;

            if (AreaHit)
            {

                PlayerOnBreakAtraNum = GetBreakAreaNum(hits[0].collider.transform.GetChild(0).gameObject);
            }

            //アラートUIのα値編集
            ChangeAlpha();
        }

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

    private void ChangeAlpha()
    {
        //最大距離より小さければ
        int BreakAreaDistance = GetBreakAreaDistance();
        bool ViewEffect = BreakEffectMaxDistance >= BreakAreaDistance;

        if (ViewEffect)
        {
            float alpha = 1 - ((float)BreakAreaDistance / (float)BreakEffectMaxDistance);

            //αの値を直接変更、大きさによって速度を変える
            if (!AlphaChange) { EffectAlpha += (alpha / 1f) * Time.deltaTime; }
            else { EffectAlpha -= (alpha / 1f) * Time.deltaTime; }
            if (EffectAlpha >= alpha) { AlphaChange = true; }
            if(EffectAlpha <= 0) { AlphaChange = false; }

            ArartEffectUI.color = new Color(1, 0, 0, EffectAlpha);
        }
        else
        {
            ArartEffectUI.color = new Color(1, 0, 0, 0);
        }

    }

    /// <summary>
    /// 破壊システム
    /// </summary>
    /// <param 破壊エリアのTransform="areatrans"></param>
    private void BreakStage(Transform areatrans)
    {
        RaycastHit[] hits = Physics.BoxCastAll(areatrans.position, areatrans.localScale * 0.5f, Vector3.one, areatrans.rotation, 1f, breakLayer);
        if (hits.Length <= 0) { return; }

        hits[0].transform.TryGetComponent<Renderer>(out Renderer renderer);

        RaycastHit[] Area = Physics.BoxCastAll(renderer.bounds.center, renderer.bounds.size * 0.1f, Vector3.one, areatrans.rotation, 1f, breakLayer);

        if(Area.Length <= 0) { return; }

        //衝突したオブジェクト全てを破壊
        for (int i = 0; i< Area.Length;i++)
        {

            SimplestarGame.VoronoiFragmenter voronoi;
            //階段の処理、子オブジェクトになっているので
            if (Area[i].collider.gameObject.name == "Collider")
            {
                Area[i].transform.parent.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
                Destroy(Area[i].collider.gameObject);
            }
            else
            {
                Area[i].collider.TryGetComponent<SimplestarGame.VoronoiFragmenter>(out voronoi);
            }

            Area[i].point = renderer.bounds.center;
            
            if (voronoi) { voronoi.Fragment(Area[i]); }
            //else { Debug.Log(hits[i].transform.name); }
        }

    }

    /// <summary>
    /// アラートの生成
    /// </summary>
    /// <param 破壊エリアのTransform="areatrans"></param>
    private void CreateAlart(Transform areatrans)
    {
        RaycastHit[] hits = Physics.BoxCastAll(areatrans.position, areatrans.localScale * 0.5f, Vector3.one, areatrans.rotation, 1f, breakLayer);
        if (hits.Length <= 0) { return; }

        hits[0].transform.TryGetComponent<Renderer>(out Renderer renderer);
        Vector3 HalfScale = renderer.bounds.size * 0.5f;

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
            Vector3 Pos = renderer.bounds.center;
            WorldCorners[i] = Pos + ArartPos[i];
        }

        //Debug.Log("4隅の座標" + "\n左下" + WorldCorners[0] + "\n左上" + WorldCorners[1] + "\n右下" + WorldCorners[2] + "\n右上" + WorldCorners[3]);

        //アラートの生成
        CurrentAlartObj = Instantiate(BreakAlartBord);
        
        //アラートの位置を設定
        for(int i=0;i<CurrentAlartObj.transform.childCount;i++)
        {
            Transform childtrans = CurrentAlartObj.transform.GetChild(i).transform;
            childtrans.position = WorldCorners[i];
            childtrans.localScale = new Vector3(renderer.bounds.size.x * 0.08f, childtrans.localScale.y, childtrans.localScale.x * 0.08f);
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
