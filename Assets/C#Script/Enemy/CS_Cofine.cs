using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �S���F���@�U�R�G
/// </summary>
public class CS_Cofine : MonoBehaviour
{
    public enum Cofin_State
    {
        IDEL,       //�ҋ@
        KANAMECHASE,    //�J�i����ǂ�������
        ENEMYCHASE,     //����Enemy��ǂ�������    
        CORECHASE,      //�R�A��ǂ�������
        KANAMEATTACK,   //��т��U��
        CORESTEAL,      //�R�A���擾
        GOHOME,         //�A��
        INTIMIDATION,   //�Њd
        FALL,           //����
        DETH,           //���S
    }

    [SerializeField,Tooltip("ENEMYSTATE")]
    private Cofin_State state;

    [Header("-----------------------------------------------")]

    [Header("�Ώی��m�p")]
    [SerializeField, Header("�^�[�Q�b�g���m����(��)")]
    private float TargetDistance = 3.0f;
    [SerializeField,Header("�R�A��D�悷�鋗��(��)")]
    private float CoreDistance = 3.0f;
    [SerializeField, Header("�v���C���[���U�����鋗��(��)")]
    private float PlayerDistance = 3.0f;

    [SerializeField, Tooltip("��Enemy���m���C���[")]
    private LayerMask EnemyLayer;

    [Header("-----------------------------------------------")]


    private Transform CoreTrans;    //�R�A�̈ʒu
    private CS_Core Corestate;      //�R�A�̏��
    private Transform PlayerTrans;  //�v���C���[�̈ʒu

    [SerializeField, Header("�X�e�[�W�X�^�[�g�ʒu(�A��ʒu)")]
    private Vector3 StartPos;

    private Vector3 TargetPos;      //�ǐՃ^�[�Q�b�g�̍��W

    [SerializeField,Header("�ړ����x")]
    private float MoveSpeed = 1.0f;
    
    [SerializeField,Header("�m�b�N�o�b�N��")]
    private float KnockBackForce = 1.0f;

    [SerializeField,Tooltip("NavMesh")]
    private NavMeshAgent navmeshAgent;  //�ǐ՗pNavMesh
    [SerializeField, Tooltip("Animator")]
    private Animator CofinAnim;

    // Start is called before the first frame update
    void Start()
    {
        state = Cofin_State.IDEL;
        navmeshAgent.speed = MoveSpeed;

        //�R�A�ƃv���C���[�̏�Ԃ��X�|�i�[����擾
        Corestate = CS_SpawnerInfo.GetCoreState();
        CoreTrans = CS_SpawnerInfo.GetCoreTrans();
        PlayerTrans = CS_SpawnerInfo.GetPlayerTrans();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //�Ώی��m���Ēǐ�
        TargetDetection();

        Attack();

    }

    /// <summary>
    /// �W�����v
    /// </summary>
    private void Jump()
    {

    }


    /// <summary>
    /// ����/���S
    /// </summary>
    private void Fall()
    {
        bool FallFlg = navmeshAgent.isOnNavMesh == false;

        if (!FallFlg) { return; }

        //�����郂�[�V����
        if (!CofinAnim.GetBool("Fall")) { CofinAnim.SetBool("Fall", true); }

        float dethHieght = 0.0f; //���S���鍂��

        if(transform.position.y < dethHieght)
        {
            state = Cofin_State.DETH;
        }

    }


    /// <summary>
    /// �U��
    /// </summary>
    private void Attack()
    {
        //�J�i�����U��
        if (state == Cofin_State.KANAMECHASE)
        {
            float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
            bool Playerattack = playerdistance < PlayerDistance;

            if (!Playerattack) { return; }

            CofinAnim.SetTrigger("Attack");
            state = Cofin_State.KANAMEATTACK;
            //hit.transform.GetComponent<CS_Player>()
            //�v���C���[�̏����擾���čU��
        }

        //�R�A���擾
        if(state == Cofin_State.CORECHASE)
        {
            float coredistance = Vector3.Distance(transform.position, CoreTrans.position);
            bool Coreget = coredistance < CoreDistance;

            if (!Coreget) { return; }

            state = Cofin_State.CORESTEAL;

            //�R�A�̏�Ԃ�ύX
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
            //�R�A���W���Œ�
            CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
            //�q�I�u�W�F�N�g�ɂ���
            //transform.SetParent(hit.transform.parent);

        }

    }


    /// <summary>
    /// �Ώی��m�p
    /// </summary>
    private void TargetDetection()
    {

        //�R�A���������A��
        if (state == Cofin_State.CORESTEAL)
        {
            //�R�A���W���Œ�
            CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
            //�R�A�̏�Ԃ�ύX
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;

            SetTarget(StartPos, Cofin_State.GOHOME);
        }

        //SetTarget(PlayerTrans.position

        float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
        float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

        Collider[] Enemyhit = Physics.OverlapSphere(transform.position, TargetDistance, EnemyLayer);

        //�͈͓��ɂ��邩
        bool PlayerInRange = playerdistance < TargetDistance;
        bool CoreInRange = coredistance < TargetDistance;

        //�R�A�h���b�v���Ă�����
        if (Corestate.STATE == CS_Core.CORE_STATE.DROP)
        {
            //�R�A��ǂ�������
            if (CoreInRange) { SetTarget(CoreTrans.position,Cofin_State.CORECHASE); }
            //�G��ǂ�������
            else if(Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
            //�v���C���[��ǂ�������
            else if (PlayerInRange){ SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
        }

        if(Corestate.STATE == CS_Core.CORE_STATE.HAVEPLAYER)
        {
            //�v���C���[��ǂ�������
            if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
            //�G��ǂ�������
            else if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
        }

        if (Corestate.STATE == CS_Core.CORE_STATE.HAVEENEMY && state != Cofin_State.GOHOME)�@
        {
            //�G��ǂ�������
            if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
            //�v���C���[��ǂ�������
            else if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }

        }

    }


    /// <summary>
    /// �ǐՑΏۂ�ݒ�
    /// </summary>
    /// <param �Ώۍ��W="targetpos"></param>
    public void SetTarget(Vector3 targetpos,Cofin_State changestate)
    {
        //�X�e�[�g���Ⴄ��
        bool Set = state != changestate;

        if (!Set) { return; }

        state = changestate;
        //NavMesh�������ł��Ă��邩���f
        bool setnav = navmeshAgent.pathStatus != NavMeshPathStatus.PathInvalid;
        if (setnav) { navmeshAgent.SetDestination(targetpos); }
        
    }


    /// <summary>
    /// Ray�̕\��
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, TargetDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, PlayerDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, CoreDistance);
    }



    private void OnCollisionEnter(Collision collision)
    {
        bool Attack = collision.gameObject.tag == "Attack";
        //�U�����ꂽ��
        if(Attack)
        {
            Rigidbody rb;
            TryGetComponent<Rigidbody>(out rb);

            if (rb != null)
            {
                // �Փ˕����̔��Ε����ɗ͂�������
                Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                rb.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
            }

            //�Њd���[�V����
            state = Cofin_State.INTIMIDATION;
            CofinAnim.SetTrigger("Intimidation");
        }

    }


}
