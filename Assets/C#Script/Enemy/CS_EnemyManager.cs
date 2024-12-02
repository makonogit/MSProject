using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CS_EnemyManager : MonoBehaviour
{

    [SerializeField]
    private List<CS_CofineAI> Approachcofins = new List<CS_CofineAI>();

    [SerializeField]
    private List<CS_CofineAI.Cofin_State> statelist = new List<CS_CofineAI.Cofin_State>();

    [SerializeField,Header("�v���C���[�ɍU�����鋗��")]
    private float PlayerApproachDistance = 1f;


    //-------- �G���Q�Ƃ��ׂ���� --------------
    [SerializeField, Header("CS_Core")]
    private CS_Core csCore;

    [SerializeField, Header("Core��Transform")]
    private Transform CoreTrans;

    [SerializeField, Header("Player��Transform")]
    private Transform PlayerTrans;

    //--------- �Q�Ƃ��ׂ����̂��ꂼ��̃Q�b�^�[ --------------
    public CS_Core GetCS_Core() => csCore;
    public Transform GetCoreTrans() => CoreTrans;

    public Transform GetPlayerTrans() => PlayerTrans;



    /// <summary>
    /// �v���C���[�߂Â����R�t�B���̐��ۑ�
    /// </summary>
    /// <param name="cofin"></param>
    public void AddApproachCofin(CS_CofineAI cofin)
    {
        //�������̂����Ȃ���Βǉ�
        bool Add = !Approachcofins.Contains(cofin);
        if (Add) { Approachcofins.Add(cofin); }
    }

    //�폜
    public void DeleteApproachCofin(CS_CofineAI cofin)
    {
        //�������̂�����΍폜
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


        //�v���C���[�ɋ߂Â����R�t�B����������
        if(Approachcofins.Count > 0 && !statelist.Contains(CS_CofineAI.Cofin_State.KANAMEATTACK))
        {
            //�U�������𒊑I
            int RandamAttack = Random.Range(0, Approachcofins.Count);
            Approachcofins[RandamAttack].Attack();

        }
    }
}
