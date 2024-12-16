using Assets.C_Script.UI.Result;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 担当：菅　デカ缶詰のシステム
/// </summary>
public class CS_BigCan : MonoBehaviour
{
    [SerializeField, Header("崩壊システム")]
    private CS_Break BreakSystem;
    [SerializeField, Header("ステータス表示スクリプト")]
    private CS_StageInfo StatusInfo;

    [Header("各パラメータ")]
    [SerializeField, Tooltip("HP")]
    private float HP = 5f;
    private float NowHP;
    [SerializeField, Tooltip("崩壊最長停止時間")]
    private float StopMaxTime = 5f;
    [SerializeField, Tooltip("崩壊停止最大距離(エリア何個分?)")]
    private int StopDistance = 5;

    [Header("触らないでーーー！")]
    [SerializeField, Header("バリアコライダー")]
    private SphereCollider BarrierCollider;
    [SerializeField, Header("デカ缶詰コライダー")]
    private SphereCollider BigCanCollider;
    
    [SerializeField, Header("HPゲージキャンバス")]
    private GameObject HPCanvas;
    [SerializeField, Header("HPゲージ")]
    private Image HPGage;
    private bool BreakFlg = false;   //壊れたかどうか

    //--------タイマー関連---------
    private Coroutine currentCoroutine; //現在計測しているコルーチン

    private void Start()
    {
        //HPゲージの処理
        NowHP = HP;
        HPGage.fillAmount = NowHP / HP;
    }

    private IEnumerator EndStopBreak(float time)
    {
        yield return new WaitForSeconds(time);

        //ステータスの表示
        StatusInfo.SetStatus(CS_StageInfo.StageStatus.BreakStart);
        //再び再開
        BreakSystem.ArartStop(false);

    }

    /// <summary>
    /// HPゲージの表示
    /// </summary>
    public void ViewHPGage(Transform PlayerTrans)
    {
        if (HPCanvas == null) { return; }
        HPCanvas.transform.LookAt(PlayerTrans); //HPゲージを常にプレイヤーの方へ
        HPCanvas.SetActive(true);
        if(currentCoroutine != null) { return; }
        currentCoroutine = StartCoroutine(EndViewHP());//HPが表示されていたら消す
    }


    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        currentCoroutine = null;
        //再び非表示に
        if(HPCanvas)HPCanvas.SetActive(false);

    }

    //当たり判定

    private void OnTriggerEnter(Collider other)
    {
        //バリアが壊れていない場合の処理
        if (BreakFlg) { return; }
       
        //-----衝突した弾が敵の弾じゃなければHPを減らす------
        other.transform.TryGetComponent<CS_AirBall>(out CS_AirBall ball);
        bool Attack = other.gameObject.tag == "Attack" && ball != null && !ball.GetEnemyType();
        if (!Attack) { return; }

        NowHP -= ball.Power;

        //HPゲージを処理
        HPGage.fillAmount = NowHP / HP;

        //バリアの当たり判定OFF、デカ缶詰の当たり判定をON
        if (NowHP <= 0)
        {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(HPCanvas);
            BreakFlg = true;
            BarrierCollider.enabled = false;
            BigCanCollider.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       
        if(BreakFlg)
        {
            //プレイヤーと衝突したら取得、停止処理
            bool PlayerHit = collision.transform.tag == "Player";
            if (!PlayerHit) { return; }

            //非表示にする
            TryGetComponent<MeshRenderer>(out MeshRenderer renderer);
            renderer.enabled = false;
            BigCanCollider.enabled = false;
    
            //破壊エリアとの差を取得
            int BreakDistance = BreakSystem.GetBreakAreaDistance();
            //最大距離より離れていたら停止しない
            if(BreakDistance > StopDistance)
            {
                Destroy(this.gameObject);
                return; 
            }

            //ステータスの表示
            StatusInfo.SetStatus(CS_StageInfo.StageStatus.BreakStop);
            BreakSystem.ArartStop(true);    //崩壊システムの停止

            //距離から停止時間を求める
            float StopTime = StopMaxTime * (1 - ((float)BreakDistance / (float)StopDistance));

            //一定時間停止する
            StartCoroutine(EndStopBreak(StopTime));

            // 所持数を増加する             // * 追加：中川
            CSGE_Result.GettingBigCan();    // * 追加：中川
        }
    }

    
}
