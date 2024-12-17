using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CS_EnemyManager : MonoBehaviour
{

    [SerializeField]
    private List<CS_Cofine> Approachcofins = new List<CS_Cofine>();

    [SerializeField]
    private List<CS_Cofine.Cofin_State> statelist = new List<CS_Cofine.Cofin_State>();

    //-------- 敵が参照すべき情報 --------------
    [SerializeField, Header("CS_Core")]
    private CS_Core csCore;

    [SerializeField, Header("CoreのTransform")]
    private Transform CoreTrans;

    [SerializeField, Header("PlayerのTransform")]
    private Transform PlayerTrans;

    //--------- 参照すべき情報のそれぞれのゲッター --------------
    public CS_Core GetCS_Core() => csCore;
    public Transform GetCoreTrans() => CoreTrans;

    public Transform GetPlayerTrans() => PlayerTrans;

    [SerializeField]bool AttackLottery = false; //攻撃していいか

    private Coroutine CurrentCoroutine;

    /// <summary>
    /// プレイヤー近づいたコフィンの数保存
    /// </summary>
    /// <param name="cofin"></param>
    public void AddApproachCofin(CS_Cofine cofin)
    {
        //同じものがいなければ追加
        bool Add = !Approachcofins.Contains(cofin);
        if (Add) { Approachcofins.Add(cofin); }
    }

    //削除
    public void DeleteApproachCofin(CS_Cofine cofin)
    {
        //同じものがいれば削除
        bool Delete = Approachcofins.Contains(cofin);
        if (Delete) { Approachcofins.Remove(cofin); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<CS_Cofine.Cofin_State> state = new List<CS_Cofine.Cofin_State>();
        foreach (CS_Cofine cofin in Approachcofins)
        {
            state.Add(cofin.GetState());
        }

        statelist = state;

        //攻撃していいか
        if (!AttackLottery) 
        {
            if(Approachcofins.Count > 0) { AttackWait(3f); }
            return; 
        }

        //プレイヤーに近づいたコフィンがいたら
        if(Approachcofins.Count > 0 && !statelist.Contains(CS_Cofine.Cofin_State.KANAMEATTACK))
        {
            //攻撃するやつを抽選
            int RandamAttack = Random.Range(0, Approachcofins.Count);
            Approachcofins[RandamAttack].SetState(CS_Cofine.Cofin_State.KANAMEATTACK);
            //攻撃待機
            AttackLottery = false;
            DeleteApproachCofin(Approachcofins[RandamAttack]);
           
        }
    }

    public void AttackWait(float time)
    {
        if (CurrentCoroutine != null) { return; }
        //攻撃待機コルーチンセット
        CurrentCoroutine = StartCoroutine(AttackWaitCoroutine(time));

    }

    //攻撃待機コルーチン
    private IEnumerator AttackWaitCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        CurrentCoroutine = null;
        AttackLottery = true;   //再度抽選へ

    }


}
