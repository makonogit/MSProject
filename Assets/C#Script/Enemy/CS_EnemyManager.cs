using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CS_EnemyManager : MonoBehaviour
{

    [SerializeField]
    private List<CS_CofineAI> Approachcofins = new List<CS_CofineAI>();

    [SerializeField]
    private List<CS_CofineAI.Cofin_State> statelist = new List<CS_CofineAI.Cofin_State>();

    [SerializeField,Header("プレイヤーに攻撃する距離")]
    private float PlayerApproachDistance = 1f;


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



    /// <summary>
    /// プレイヤー近づいたコフィンの数保存
    /// </summary>
    /// <param name="cofin"></param>
    public void AddApproachCofin(CS_CofineAI cofin)
    {
        //同じものがいなければ追加
        bool Add = !Approachcofins.Contains(cofin);
        if (Add) { Approachcofins.Add(cofin); }
    }

    //削除
    public void DeleteApproachCofin(CS_CofineAI cofin)
    {
        //同じものがいれば削除
        bool Delete = Approachcofins.Contains(cofin);
        if (Delete) { Approachcofins.Remove(cofin); }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        List<CS_CofineAI.Cofin_State> state = new List<CS_CofineAI.Cofin_State>();
        foreach(CS_CofineAI cofin in Approachcofins)
        {
            state.Add(cofin.GetState());
        }

        statelist = state;


        //プレイヤーに近づいたコフィンがいたら
        if(Approachcofins.Count > 0 && !statelist.Contains(CS_CofineAI.Cofin_State.KANAMEATTACK))
        {
            //攻撃するやつを抽選
            int RandamAttack = Random.Range(0, Approachcofins.Count);
            Approachcofins[RandamAttack].Attack();

        }
    }
}
