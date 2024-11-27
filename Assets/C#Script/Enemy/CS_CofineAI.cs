using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// �S���F���@�U�R�G
/// </summary>
public class CS_CofineAI : MonoBehaviour
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

    [SerializeField, Tooltip("ENEMYSTATE")]
    private Cofin_State state;

    [Header("-----------------------------------------------")]

    [Header("�Ώی��m�p")]
    [SerializeField, Header("�^�[�Q�b�g���m����(��)")]
    private float TargetDistance = 3.0f;
    [SerializeField, Header("�R�A��D�悷�鋗��(��)")]
    private float CoreDistance = 3.0f;
    [SerializeField, Header("�v���C���[���U�����鋗��(��)")]
    private float PlayerDistance = 3.0f;
    [SerializeField, Tooltip("�ڋߍŏ�����")]
    [Range(0.01f, 100f)] private float DistanceStop = 0.1f;

    [SerializeField, Tooltip("��Enemy���m���C���[")]
    private LayerMask EnemyLayer;

    [Header("-----------------------------------------------")]

    [SerializeField, Header("�X�e�[�W�X�^�[�g�ʒu(�A��ʒu)")]
    private Vector3 StartPos;

    //--------�ǐՃ^�[�Q�b�g�̏��----------
    private Transform CoreTrans;    //�R�A�̈ʒu
    private CS_Core Corestate;      //�R�A�̏��
    private Transform PlayerTrans;  //�v���C���[�̈ʒu

    private Vector3 TargetPos;                        //�ǐՃ^�[�Q�b�g�̍��W
    //private Vector3 TargetDirection;                 //�ǐՃ^�[�Q�b�g�̌���
    //private Quaternion CurrentDestinationsRotate;    //���݂̖ڕW����

    private bool CoreGet = false;       //�R�A���擾�������ǂ���
    [SerializeField, Tooltip("������RigitBody")]
    private Rigidbody ThisRd;           //�ړ��pRigidBody

    [Header("�e�p�����[�^�[")]
    [SerializeField, Tooltip("�ړ����x")]
    private float MoveSpeed = 1.0f;
    [SerializeField, Tooltip("�����]�����x")]
    private float RotationSpeed = 1.0f;
    [SerializeField,Tooltip("�m�b�N�o�b�N��")]
    private float KnockBackForce = 1.0f;
    [SerializeField, Tooltip("�U����")]
    private float AttackPower = 1.0f;
    [SerializeField, Tooltip("�U���Ԋu")]
    private float AttackSpace = 3f;
    [SerializeField, Tooltip("HP")]
    private float HP = 30.0f;
    private float NowHP;    //���݂�HP

    [Header("-----------------------------------------------")]
  

    [SerializeField, Tooltip("NavMesh")]
    private NavMeshAgent navmeshAgent;  //�ǐ՗pNavMesh
    private int CurrentPathNum = 1;     //���݂̌o�H�̃C���f�b�N�X(1�n�܂�)

    [SerializeField, Tooltip("Animator")]
    private Animator CofinAnim;
    [SerializeField, Tooltip("Light")]
    private Light SpotLight;
    [SerializeField, Tooltip("�������̖��邳")]
    private float LightBrightness = 100;
    [SerializeField, Tooltip("�󂫊�")]
    private GameObject Can;
    [SerializeField, Tooltip("��������󂫊ʂ̐�")]
    private int CanNum = 3;
    [SerializeField, Tooltip("HPGage")]
    private Image HPGage;
    [SerializeField]
    private GameObject HPCanvas;


    // Start is called before the first frame update
    void Start()
    {
        state = Cofin_State.IDEL;
       
        //�R�A�ƃv���C���[�̏�Ԃ��X�|�i�[����擾
        Corestate = CS_SpawnerInfo.GetCoreState();
        CoreTrans = CS_SpawnerInfo.GetCoreTrans();
        PlayerTrans = CS_SpawnerInfo.GetPlayerTrans();

        //HP�Q�[�W��ݒ�
        NowHP = HP;
        HPGage.fillAmount = NowHP / HP;

        //HP�Q�[�W���\��
        //HPSliderObj.SetActive(false);

        // ���O�̈ړ����s�����߂�Agent�̎����X�V�𖳌���
        navmeshAgent.updatePosition = false;
        navmeshAgent.updateRotation = false;

    }
    
    // Update is called once per frame
    void FixedUpdate()
    {

        //�Ώی��m���Ēǐ�
        TargetDetection();

        //�ړ�
        Move();

        //�U��
        Attack();

        //HP�Q�[�W�̏���
        HPGage.fillAmount = NowHP / HP;
        if(NowHP <= 0) 
        {
            for(int i = 0;i<CanNum;i++)
            {
                //�ʂ̐���
                Instantiate(Can, transform.position, Quaternion.identity);
            }
            Destroy(this.gameObject); 
        }
        HPCanvas.transform.LookAt(PlayerTrans);
        if (HPCanvas.activeSelf) { StartCoroutine(EndViewHP()); }    //HP���\������Ă��������
    }

    /// <summary>
    ///�@�ړ�
    /// </summary>
    private void Move()
    {
        if (navmeshAgent.path.corners.Length < 2) { return; }  // �o�H���Ȃ��ꍇ�͏I��
        
        Vector3 currentPosition = transform.position;
        Vector3 nextWaypoint = navmeshAgent.path.corners[CurrentPathNum];  // �ŏ��̈ړ���
        Vector3 direction = (nextWaypoint - currentPosition);
       

        // �G�L�����N�^�[��i�s�����Ɉړ�������
        float forward_x = transform.forward.x * MoveSpeed;
        float forward_z = transform.forward.z * MoveSpeed;

        ThisRd.velocity = new Vector3(forward_x, ThisRd.velocity.y, forward_z);

        // �i�s�����Ɍ����ĉ�]������
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion moveRotation = Quaternion.LookRotation(direction, Vector3.up);
            moveRotation.x = 0;
            moveRotation.z = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, 0.1f);
        }

        // ���̌o�H�_�ɓ��B�����ꍇ�A���̌o�H�_�֐i��
        if (Vector3.Distance(currentPosition, nextWaypoint) < DistanceStop)
        {
            // ���̌o�H�_�ɐi�ނ��߂ɃC���f�b�N�X���X�V
            if (CurrentPathNum < navmeshAgent.path.corners.Length - 1)
            {
                CurrentPathNum++;  // �C���f�b�N�X��i�߂�
            }
            else
            {
                CurrentPathNum = 1;
            }
        }

        // �f�o�b�O�p�Ɍo�H����\��
        Debug.DrawLine(currentPosition, nextWaypoint, Color.red);

    }

    /// <summary>
    /// �W�����v
    /// </summary>
    private void Jump()
    {

    }


    /// <summary>
    /// HP�Q�[�W�̕\��
    /// </summary>
    public void ViewHPGage()
    {
        HPCanvas.SetActive(true);
    }


    /// <summary>
    /// ����/���S
    /// </summary>
    private void Fall()
    {
       // bool FallFlg = navmeshAgent.isOnNavMesh == false;

        //if (!FallFlg) { return; }

        ////�����郂�[�V����
        //if (!CofinAnim.GetBool("Fall")) { CofinAnim.SetBool("Fall", true); }

        //float dethHieght = 0.0f; //���S���鍂��

        //if (transform.position.y < dethHieght)
        //{
        //    state = Cofin_State.DETH;
        //}

    }


    /// <summary>
    /// �U��
    /// </summary>
    private void Attack()
    {
        //�J�i�����U��
        if (state == Cofin_State.KANAMECHASE)
        {
            navmeshAgent.SetDestination(PlayerTrans.position);

            float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
            bool Playerattack = playerdistance < PlayerDistance;

            if (!Playerattack) { return; }

            CofinAnim.SetTrigger("Attack");
            state = Cofin_State.KANAMEATTACK;
            //hit.transform.GetComponent<CS_Player>()
            //�v���C���[�̏����擾���čU��

            state = Cofin_State.KANAMECHASE;
        }

        //�R�A���擾
        //if (state == Cofin_State.CORECHASE)
        //{
        //    float coredistance = Vector3.Distance(transform.position, CoreTrans.position);
        //    CoreGet = coredistance < CoreDistance;

        //    if (!CoreGet) { return; }

        //    //�R�A���W���Œ�
        //    CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        //    //�q�I�u�W�F�N�g�ɂ���
        //    //transform.SetParent(hit.transform.parent);

        //}

    }


    /// <summary>
    /// �Ώی��m�p
    /// </summary>
    private void TargetDetection()
    {

        if (CoreGet)
        {
            state = Cofin_State.GOHOME;
            //�R�A�̏�Ԃ�ύX
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
            //�R�A���W���Œ�
            CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

        }
        
        if (state == Cofin_State.GOHOME )
        {
            navmeshAgent.SetDestination(StartPos);
        }


        //�G�̓����蔻��
        Collider[] Enemyhit = Physics.OverlapSphere(transform.position, TargetDistance, EnemyLayer);
        
        float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
        float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

        //�R�A�ǐ�
        bool CoreTracking = coredistance < TargetDistance;
        //�v���C���[�ǐ�
        bool PlayerTracking = playerdistance < TargetDistance;

        //�R�A�h���b�v���Ă�����
        if (Corestate.STATE == CS_Core.CORE_STATE.DROP)
        {
            //�R�A��ǐ�
            if (CoreTracking) { SetTarget(CoreTrans.position, Cofin_State.CORECHASE); }
            //�G��ǐ�
            //else if (Enemyhit.Length > 0 && Enemyhit[0].gameObject != gameObject) { SetTarget(Enemyhit[0].transform.position, Cofin_State.ENEMYCHASE); }
            //�v���C���[��ǐ�
            else if (PlayerTracking) { SetTarget(PlayerTrans.position, Cofin_State.KANAMECHASE); }
           
        }

        //�v���C���[���R�A�������Ă�����
        if (Corestate.STATE == CS_Core.CORE_STATE.HAVEPLAYER)
        {
            //�v���C���[��ǂ�������
            if (PlayerTracking) { SetTarget(PlayerTrans.position, Cofin_State.KANAMECHASE); }
            //�G��ǂ�������
            //else if (Enemyhit.Length > 0 && Enemyhit[0].gameObject != gameObject) { SetTarget(Enemyhit[0].transform.position, Cofin_State.ENEMYCHASE); }
        
        }


        //���̓G�������Ă�����
        if (Corestate.STATE == CS_Core.CORE_STATE.HAVEENEMY)
        {
            //�X�^�[�g�n�_�ɋA��
            SetTarget(StartPos, Cofin_State.GOHOME);
        }

        
        if(!CoreTracking && !PlayerTracking)
        {
            Destroy(this.gameObject);   //�f�X�|�[��
        }
      
    }


    /// <summary>
    /// �ǐՑΏۂ�ݒ�
    /// </summary>
    /// <param �Ώۍ��W="targetpos"></param>
    public void SetTarget(Vector3 targetpos, Cofin_State changestate)
    {
        //�X�e�[�g���Ⴄ��
        bool Set = state != changestate;

        if (!Set) { return; }

        state = changestate;

        //�ړI�n��ݒ�
        if (navmeshAgent && navmeshAgent.enabled)
        {
            navmeshAgent.SetDestination(targetpos);
        }
           
        TargetPos = targetpos;
        //TargetDirection = (targetpos - transform.position).normalized;
        
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);


        ////NavMesh�������ł��Ă��邩���f
        //if (navmeshAgent.enabled)
        //{
        //    TargetPos = targetpos;
        //    bool setnav = navmeshAgent.pathStatus != NavMeshPathStatus.PathInvalid;
        //    if (setnav) { navmeshAgent.SetDestination(targetpos); }
        //}



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

        //// �����̏����O���̈ʒu���v�Z
        //Vector3 RayPos = transform.position + transform.forward * FloorRayFowardOffset;
        //Debug.DrawRay(RayPos, Vector3.down * FloorDistance, Color.red);
    }



    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.tag == "EnergyCore")
        {
            //state = Cofin_State.CORESTEAL;
            CoreGet = true;

            //�R�A�̏�Ԃ�ύX
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;

            SetTarget(StartPos, Cofin_State.GOHOME);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        bool Attack = other.gameObject.tag == "Attack";
        //�U�����ꂽ��
        if (Attack)
        {
            //navmeshAgent.enabled = false;

            other.transform.TryGetComponent<CS_AirBall>(out CS_AirBall airBall);
            if (airBall != null)
            {
                NowHP -= airBall.Power;                    //HP�����炷

                SpotLight.intensity = LightBrightness;  //����������
                StartCoroutine(EndLight());
            }

            if (ThisRd != null)
            {
                //navmeshAgent.enabled = false;
                // �Փ˕����̔��Ε����ɗ͂�������
                Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
                ThisRd.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
                // �m�b�N�o�b�N�̏I����ҋ@����R���[�`�����J�n
                StartCoroutine(EndKnockback());
            }

            if (CoreGet)
            {
                Corestate.STATE = CS_Core.CORE_STATE.DROP;
                state = Cofin_State.IDEL;
                CoreGet = false;

                ////�Њd���[�V����
                //state = Cofin_State.INTIMIDATION;
                //CofinAnim.SetTrigger("Intimidation");

            }

        }


    }

  

    private IEnumerator EndKnockback()
    {
        yield return new WaitForSeconds(1f);

        //navmeshAgent.enabled = true;
        // �ړI�n���Đݒ�i��Ƃ��Č��̖ړI�n�ɖ߂��j
        navmeshAgent.SetDestination(TargetPos);

    }

    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        //�Ăє�\����
        HPCanvas.SetActive(false);

    }


    private IEnumerator EndLight()
    {
        yield return new WaitForSeconds(0.2f);

        //���邳�����Z�b�g
        SpotLight.intensity = 0f;

    }


}
