using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CS_EnemyManager : MonoBehaviour
{

    [SerializeField]
    private List<CS_Cofine> Approachcofins = new List<CS_Cofine>();

    [SerializeField]
    private List<CS_Cofine.Cofin_State> statelist = new List<CS_Cofine.Cofin_State>();

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

    [SerializeField]bool AttackLottery = false; //�U�����Ă�����

    private Coroutine CurrentCoroutine;

    /// <summary>
    /// �v���C���[�߂Â����R�t�B���̐��ۑ�
    /// </summary>
    /// <param name="cofin"></param>
    public void AddApproachCofin(CS_Cofine cofin)
    {
        //�������̂����Ȃ���Βǉ�
        bool Add = !Approachcofins.Contains(cofin);
        if (Add) { Approachcofins.Add(cofin); }
    }

    //�폜
    public void DeleteApproachCofin(CS_Cofine cofin)
    {
        //�������̂�����΍폜
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

        //�U�����Ă�����
        if (!AttackLottery) 
        {
            if(Approachcofins.Count > 0) { AttackWait(3f); }
            return; 
        }

        //�v���C���[�ɋ߂Â����R�t�B����������
        if(Approachcofins.Count > 0 && !statelist.Contains(CS_Cofine.Cofin_State.KANAMEATTACK))
        {
            //�U�������𒊑I
            int RandamAttack = Random.Range(0, Approachcofins.Count);
            Approachcofins[RandamAttack].SetState(CS_Cofine.Cofin_State.KANAMEATTACK);
            //�U���ҋ@
            AttackLottery = false;
            DeleteApproachCofin(Approachcofins[RandamAttack]);
           
        }
    }

    public void AttackWait(float time)
    {
        if (CurrentCoroutine != null) { return; }
        //�U���ҋ@�R���[�`���Z�b�g
        CurrentCoroutine = StartCoroutine(AttackWaitCoroutine(time));

    }

    //�U���ҋ@�R���[�`��
    private IEnumerator AttackWaitCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        CurrentCoroutine = null;
        AttackLottery = true;   //�ēx���I��

    }


}
