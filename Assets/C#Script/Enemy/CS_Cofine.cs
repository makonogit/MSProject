using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// �S���F���@�U�R�G
/// </summary>
public class CS_Cofine : MonoBehaviour
{
    public enum Cofin_State
    {
        IDLE,                //�ҋ@
        TARGETSEARCH,        //�ڕW����
        KANAMECHASE,         //�J�i����ǂ�������
        ENEMYCHASE,          //����Enemy��ǂ�������    
        CORECHASE,           //�R�A��ǂ�������
        KANAMEATTACKLOTTERY, //�U�����I
        KANAMEATTACK,        //��т��U��
        CORESTEAL,           //�R�A���擾
        GOHOME,              //�A��
        INTIMIDATION,        //�Њd
        FALL,                //����
        DETH,                //���S
    }

    [SerializeField, Tooltip("ENEMYSTATE")]
    private Cofin_State state;
    public Cofin_State GetState() => state; //��Ԃ��擾
    public void SetState(CS_Cofine.Cofin_State _state) { state = _state; }

    private bool CoreGet = false;       //�R�A���擾�������ǂ���

    [Header("-----------------------------------------------")]

    [Header("�e�p�����[�^�[")]
    [SerializeField, Tooltip("�ړ����x")]
    private float MoveSpeed = 1.0f;
    [SerializeField, Tooltip("�����]�����x")]
    private float RotationSpeed = 1.0f;
    [SerializeField, Tooltip("�m�b�N�o�b�N��")]
    private float KnockBackForce = 1.0f;
    [SerializeField, Tooltip("�U����")]
    private float AttackPower = 1.0f;
    [SerializeField, Tooltip("�U���Ԋu")]
    private float AttackSpace = 3f;
    [SerializeField, Tooltip("HP")]
    private float HP = 30.0f;
    private float NowHP;    //���݂�HP
    [SerializeField, Header("�R�A�����炷��/s")]
    private float EnelgyStealPower = 1f;

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
    private CS_EnergyCure CoreEnelgy; //�R�A�̃G�l���M�\�擾
    private CS_StageInfo Status;        //�X�e�[�^�X�\��

    private Vector3 CurrentTargetPos;      //���݂̒ǐՃ^�[�Q�b�g�̍��W

    private int CurrentPathNum = 1;     �@ //���݂̌o�H�̃C���f�b�N�X(1�n�܂�)

    [Header("-----------------------------------------------")]
    [Header("�ݒ荀��")]
    [SerializeField, Tooltip("������RigitBody")]
    private Rigidbody ThisRd;           //�ړ��pRigidBody
    [SerializeField, Tooltip("NavMesh")]
    private NavMeshAgent navmeshAgent;  //�ǐ՗pNavMesh
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
    private int CurrentCanNum = 0;  //���݂̊ʋl�̐�
    [SerializeField, Tooltip("HPGage")]
    private Image HPGage;
    [SerializeField]
    private GameObject HPCanvas;
    //SE�Đ��p
    [SerializeField, Tooltip("SEAudioSource")]
    private AudioSource SE;
    [SerializeField, Tooltip("SEList")]
    private List<AudioClip> SEList;

    private CS_EnemyManager EnemyManager;        //�G�̊Ǘ��p�X�N���v�g

    // Start is called before the first frame update
    void Start()
    {
        state = Cofin_State.IDLE;

        //�e����}�l�[�W���[���擾
        transform.parent.TryGetComponent<CS_EnemyManager>(out EnemyManager);

        //�R�A�ƃv���C���[�̏�Ԃ��}�l�[�W���[����擾
        Corestate = EnemyManager.GetCS_Core();
        CoreTrans = EnemyManager.GetCoreTrans();
        PlayerTrans = EnemyManager.GetPlayerTrans();
        Status = EnemyManager.GetStageInfo();

        CoreTrans.TryGetComponent<CS_EnergyCure>(out CoreEnelgy);

        //HP�Q�[�W��ݒ�
        NowHP = HP;
        HPGage.fillAmount = NowHP / HP;

        //HP�Q�[�W���\��
        HPCanvas.SetActive(false);

        // ���O�̈ړ����s�����߂�Agent�̎����X�V�𖳌���
        navmeshAgent.updatePosition = false;
        navmeshAgent.updateRotation = false;

        //�ڕW��ݒ�
        TargetDetection();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //HP�Q�[�W�̏���
        HPGage.fillAmount = NowHP / HP;
        if (HPCanvas.activeSelf) { StartCoroutine(EndViewHP()); }    //HP���\������Ă��������
    
        if(state == Cofin_State.DETH) { return; }

        ActionTable();
    }

    /// <summary>
    ///�@�ړ�
    /// </summary>
    private void Move()
    {
        if (navmeshAgent.path.corners.Length < 2) { return; }  // �o�H���Ȃ��ꍇ�͏I��

        //PlaySE(2, true);

        Vector3 currentPosition = transform.position;
        Vector3 nextWaypoint = navmeshAgent.path.corners[CurrentPathNum];  // �ŏ��̈ړ���
        Vector3 direction = (nextWaypoint - currentPosition);

        //// �������
        //Vector3 avoidance = Vector3.zero;
        //float avoidRadius = 2.0f; // ������锼�a
        //Collider[] nearbyAgents = Physics.OverlapSphere(currentPosition, avoidRadius); // ���͈͓��̃G�[�W�F���g�����m

        //foreach (Collider collider in nearbyAgents)
        //{
        //    // �������g��NavMeshAgent�͖���
        //    NavMeshAgent otherAgent = collider.GetComponent<NavMeshAgent>();
        //    if (otherAgent != null && otherAgent != navmeshAgent)
        //    {
        //        Vector3 toOtherAgent = otherAgent.transform.position - currentPosition;
        //        float distance = toOtherAgent.magnitude;

        //        // �����ɉ���������x�N�g�����v�Z�i�߂��قǋ�������j
        //        if (distance < avoidRadius)
        //        {
        //            avoidance -= toOtherAgent.normalized / distance;
        //        }
        //    }
        //}

        // ���������ړ������ɉ��Z
        //Vector3 finalDirection = (direction + avoidance).normalized;


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
        currentPosition.y = 0;
        nextWaypoint.y = 0;
        float dis = Vector3.Distance(currentPosition, nextWaypoint);
        if (dis < 0.5f)
        {
            // ���̌o�H�_�ɐi�ނ��߂ɃC���f�b�N�X���X�V
            if (CurrentPathNum < navmeshAgent.path.corners.Length - 1)
            {
                CurrentPathNum++;  // �C���f�b�N�X��i�߂�
            }

        }
        // �f�o�b�O�p�Ɍo�H����\��
        Debug.DrawLine(currentPosition, nextWaypoint, Color.red);

    }


    /// <summary>
    /// ��Ԃ��Ƃ̍s��
    /// </summary>
    private void ActionTable()
    {
        
        switch(state)
        {
            case Cofin_State.IDLE:                  //�ҋ@
                StartCoroutine(EndStop());
                break;
            case Cofin_State.KANAMECHASE:           //�J�i���Ǐ]
                TargetDetection();
                //�w�苗�����ꂽ�ꏊ��ݒ�
                Vector3 PlayerFoward = transform.forward;
                Vector3 distancepos = PlayerTrans.position + (PlayerFoward.normalized * PlayerDistance);
                SetTarget(distancepos);
                float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
                bool Playerattack = playerdistance <= PlayerDistance;
                //���I����
                EnemyManager.AddApproachCofin(this);
                state = Cofin_State.KANAMEATTACKLOTTERY;
                Move();
                break;
            case Cofin_State.ENEMYCHASE:            //�G�Ǐ]
                break;
            case Cofin_State.CORECHASE:             //�R�A�Ǐ]
                TargetDetection();
                SetTarget(CoreTrans.position);
                Move();
                break;
            case Cofin_State.KANAMEATTACKLOTTERY:   //�U�����I
                playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
                Playerattack = playerdistance > PlayerDistance + 2f;
                //�U���͈͓��ɂ��Ȃ��Ȃ�����ēx�ڕW����
                if (Playerattack)
                {
                    EnemyManager.DeleteApproachCofin(this);
                    TargetDetection();
                }
                break;
            case Cofin_State.KANAMEATTACK:          //�U��
                EnemyManager.AttackWait(AttackSpace);
                SetTarget(PlayerTrans.position);
                Move();
                break;
            case Cofin_State.CORESTEAL:             //�R�A��D��
                 //�R�A���W���Œ�
                CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
                CoreEnelgy.SetEnergy(CoreEnelgy.GetEnergy() - EnelgyStealPower * Time.deltaTime);
                SetTarget(StartPos);
                Move();
                break;
            case Cofin_State.GOHOME:                //�ƂɋA��
                SetTarget(StartPos);
                //�R�A�𗎂Ƃ����肵����ēx�ڕW����
                if(Corestate.STATE != CS_Core.CORE_STATE.HAVEENEMY) { TargetDetection(); }
                Move();
                break;
            case Cofin_State.INTIMIDATION:          //�Њd
                break;
            case Cofin_State.FALL:                  //����
                break;
            case Cofin_State.DETH:                  //���S
                //�U���ҋ@�ɓo�^����Ă������
                EnemyManager.DeleteApproachCofin(this); 
                break;
        }
    }

    /// <summary>
    /// NavMeshAgent�̖ړI�n�ݒ�
    /// </summary>
    /// <param �ڕW���W="target"></param>
    public void SetTarget(Vector3 target)
    {
        if(navmeshAgent == null || !navmeshAgent.isActiveAndEnabled) { return; }
        navmeshAgent.SetDestination(target);
        //CurrentPathNum = 0; // �o�H�����Z�b�g
    }

    /// <summary>
    /// �Ώی��m�p
    /// </summary>
    private void TargetDetection()
    {

        float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
        float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

        //�R�A�ǐ�
        bool CoreTracking = coredistance < TargetDistance;
        //�v���C���[�ǐ�
        bool PlayerTracking = playerdistance < TargetDistance;

        switch(Corestate.STATE)
        {
            case CS_Core.CORE_STATE.DROP:
                if (CoreTracking) { state = Cofin_State.CORECHASE; }
                else if (PlayerTracking) { state = Cofin_State.KANAMECHASE; }
                break;
            case CS_Core.CORE_STATE.HAVEPLAYER:
                state = Cofin_State.KANAMECHASE;   //�J�i����ǂ�������
                break;
            case CS_Core.CORE_STATE.HAVEENEMY:
                state = Cofin_State.GOHOME;        //�ƂɋA��
                break;
        }

        //�͈͓��ɉ����Ȃ����
        if (!CoreTracking && !PlayerTracking)
        {
            Destroy(this.gameObject);   //�f�X�|�[��
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state == Cofin_State.DETH) { return; }
        //�R�A�̎擾
        if (collision.gameObject.tag == "EnergyCore")
        {
            //���ꂽ�\��
            if (Status.GetCurrentStatus() != CS_StageInfo.StageStatus.CoreSteal)
            {
                Status.SetStatus(CS_StageInfo.StageStatus.CoreSteal);
            }

            state = Cofin_State.CORESTEAL;
            //CoreGet = true;
            //�R�A���W���Œ�
            CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

            //�R�A�̏�Ԃ�ύX
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
        }

        bool PlayerHit = collision.gameObject.tag == "Player";

        //�U����ԂŃv���C���[�ƏՓ˂�����
        if (PlayerHit && state == Cofin_State.KANAMEATTACK)
        {
            PlaySE(4, false);
            CofinAnim.SetTrigger("Attack");
            state = Cofin_State.IDLE;
            CofinAnim.SetBool("Idle", true);
            
        }

        //if ( &&)
        //{
        //    PlaySE(4, false);
        //    CofinAnim.SetTrigger("Attack");
        //    // EnemyManager.DeleteApproachCofin(this); //�U���I�����������
        //    state = Cofin_State.IDEL;
        //    CofinAnim.SetBool("Idle", true);
        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        if(state == Cofin_State.DETH) { return; }
        bool Attack = other.gameObject.tag == "Attack";
        //�U�����ꂽ��
        if (Attack)
        {
            //PlaySE(7, false);
            //navmeshAgent.enabled = false;
            //EnemyManager.DeleteApproachCofin(this);
            other.transform.TryGetComponent<CS_AirBall>(out CS_AirBall airBall);
            
            //�e�ɏՓ˂�����
            if (airBall != null && !airBall.GetEnemyType())
            {
                NowHP -= airBall.Power;                    //HP�����炷

                SpotLight.intensity = LightBrightness;  //����������
                StartCoroutine(EndLight());             //���΂炭�����甭����~

                //���S����
                if (NowHP <= 0 && CurrentCanNum < CanNum)
                {
                    state = Cofin_State.DETH;

                    //�R���C�_�[�𖳌���
                    transform.tag = "Untagged";
                    for (; CurrentCanNum < CanNum; CurrentCanNum++)
                    {
                        //�ʂ̐���
                        Instantiate(Can, transform.position, Quaternion.identity);
                    }
                    CofinAnim.SetBool("Deth", true);
                  �@PlaySE(6, false);

                }
            }


            //�m�b�N�o�b�N����
            if (ThisRd != null)
            {
                 // �Փ˕����̔��Ε����ɗ͂�������
                Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
                ThisRd.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
                //CofinAnim.SetTrigger("KnockBack");
                // �m�b�N�o�b�N�̏I����ҋ@����R���[�`�����J�n
                state = Cofin_State.IDLE;
            }

            //�R�A�𗎂Ƃ�
            if (state == Cofin_State.CORESTEAL)
            {
                Corestate.STATE = CS_Core.CORE_STATE.DROP;
                state = Cofin_State.IDLE;

            }

            //if (CoreGet)
            //{
            //    Corestate.STATE = CS_Core.CORE_STATE.DROP;
            //    state = Cofin_State.IDLE;
            //    CoreGet = false;

            //    ////�Њd���[�V����
            //    //state = Cofin_State.INTIMIDATION;
            //    //CofinAnim.SetTrigger("Intimidation");

            //}

        }


    }

    /// <summary>
    /// ���S�����@�A�j���[�V�����ŌĂяo��
    /// </summary>
    public void Deth()
    {
        Destroy(this.gameObject);
    }



    /// <summary>
    /// HP�Q�[�W�̕\��
    /// </summary>
    public void ViewHPGage(Transform PlayerTrans)
    {
        HPCanvas.transform.LookAt(PlayerTrans);
        HPCanvas.SetActive(true);
    }


    /// <summary>
    /// HP�Q�[�W�\���R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        //�Ăє�\����
        HPCanvas.SetActive(false);

    }

    
    /// <summary>
    /// �ҋ@
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndStop()
    {
        yield return new WaitForSeconds(AttackSpace);
        CofinAnim.SetBool("Idle", false);
        TargetDetection();  //���̖ڕW�����߂�

    }


    /// <summary>
    ///�@��莞�Ԕ����R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndLight()
    {
        yield return new WaitForSeconds(0.2f);

        //���邳�����Z�b�g
        SpotLight.intensity = 0f;

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

    private void PlaySE(int PlayNum, bool Loop)
    {
        if (SE.clip == SEList[PlayNum]) { return; }

        if (!Loop)
        {
            SE.Stop();
            SE.clip = null;
            SE.PlayOneShot(SEList[PlayNum]);
            return;
        }

        if (SE.isPlaying) { return; }
        SE.clip = SEList[PlayNum];
        SE.Play();
    }


    //public enum Cofin_State
    //{
    //    IDEL,       //�ҋ@
    //    KANAMECHASE,    //�J�i����ǂ�������
    //    ENEMYCHASE,     //����Enemy��ǂ�������    
    //    CORECHASE,      //�R�A��ǂ�������
    //    KANAMEATTACK,   //��т��U��
    //    CORESTEAL,      //�R�A���擾
    //    GOHOME,         //�A��
    //    INTIMIDATION,   //�Њd
    //    FALL,           //����
    //    DETH,           //���S
    //}

    //[SerializeField,Tooltip("ENEMYSTATE")]
    //private Cofin_State state;

    //[Header("-----------------------------------------------")]

    //[Header("�Ώی��m�p")]
    //[SerializeField, Header("�^�[�Q�b�g���m����(��)")]
    //private float TargetDistance = 3.0f;
    //[SerializeField,Header("�R�A��D�悷�鋗��(��)")]
    //private float CoreDistance = 3.0f;
    //[SerializeField, Header("�v���C���[���U�����鋗��(��)")]
    //private float PlayerDistance = 3.0f;

    //[SerializeField, Tooltip("��Enemy���m���C���[")]
    //private LayerMask EnemyLayer;

    //[Header("-----------------------------------------------")]


    //private Transform CoreTrans;    //�R�A�̈ʒu
    //private CS_Core Corestate;      //�R�A�̏��
    //private Transform PlayerTrans;  //�v���C���[�̈ʒu

    //[SerializeField, Header("�X�e�[�W�X�^�[�g�ʒu(�A��ʒu)")]
    //private Vector3 StartPos;

    //private Vector3 TargetPos;      //�ǐՃ^�[�Q�b�g�̍��W

    //[SerializeField,Header("�ړ����x")]
    //private float MoveSpeed = 1.0f;

    //[SerializeField,Header("�m�b�N�o�b�N��")]
    //private float KnockBackForce = 1.0f;

    //[SerializeField,Tooltip("NavMesh")]
    //private NavMeshAgent navmeshAgent;  //�ǐ՗pNavMesh
    //[SerializeField, Tooltip("Animator")]
    //private Animator CofinAnim;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    state = Cofin_State.IDEL;
    //    navmeshAgent.speed = MoveSpeed;

    //    //�R�A�ƃv���C���[�̏�Ԃ��X�|�i�[����擾
    //    Corestate = CS_SpawnerInfo.GetCoreState();
    //    CoreTrans = CS_SpawnerInfo.GetCoreTrans();
    //    PlayerTrans = CS_SpawnerInfo.GetPlayerTrans();

    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{

    //    //�Ώی��m���Ēǐ�
    //    TargetDetection();

    //    Attack();

    //}

    ///// <summary>
    ///// �W�����v
    ///// </summary>
    //private void Jump()
    //{

    //}


    ///// <summary>
    ///// ����/���S
    ///// </summary>
    //private void Fall()
    //{
    //    bool FallFlg = navmeshAgent.isOnNavMesh == false;

    //    if (!FallFlg) { return; }

    //    //�����郂�[�V����
    //    if (!CofinAnim.GetBool("Fall")) { CofinAnim.SetBool("Fall", true); }

    //    float dethHieght = 0.0f; //���S���鍂��

    //    if(transform.position.y < dethHieght)
    //    {
    //        state = Cofin_State.DETH;
    //    }

    //}


    ///// <summary>
    ///// �U��
    ///// </summary>
    //private void Attack()
    //{
    //    //�J�i�����U��
    //    if (state == Cofin_State.KANAMECHASE)
    //    {
    //        float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
    //        bool Playerattack = playerdistance < PlayerDistance;

    //        if (!Playerattack) { return; }

    //        CofinAnim.SetTrigger("Attack");
    //        state = Cofin_State.KANAMEATTACK;
    //        //hit.transform.GetComponent<CS_Player>()
    //        //�v���C���[�̏����擾���čU��
    //    }

    //    //�R�A���擾
    //    if(state == Cofin_State.CORECHASE)
    //    {
    //        float coredistance = Vector3.Distance(transform.position, CoreTrans.position);
    //        bool Coreget = coredistance < CoreDistance;

    //        if (!Coreget) { return; }

    //        state = Cofin_State.CORESTEAL;

    //        //�R�A�̏�Ԃ�ύX
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
    //        //�R�A���W���Œ�
    //        CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
    //        //�q�I�u�W�F�N�g�ɂ���
    //        //transform.SetParent(hit.transform.parent);

    //    }

    //}


    ///// <summary>
    ///// �Ώی��m�p
    ///// </summary>
    //private void TargetDetection()
    //{

    //    //�R�A���������A��

    //    if (state == Cofin_State.CORESTEAL)
    //    {
    //        //�R�A���W���Œ�
    //        CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
    //        //�R�A�̏�Ԃ�ύX
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;

    //        SetTarget(StartPos, Cofin_State.GOHOME);
    //    }

    //    //SetTarget(PlayerTrans.position

    //    float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
    //    float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

    //    Collider[] Enemyhit = Physics.OverlapSphere(transform.position, TargetDistance, EnemyLayer);

    //    //�͈͓��ɂ��邩
    //    bool PlayerInRange = playerdistance < TargetDistance;
    //    bool CoreInRange = coredistance < TargetDistance;

    //    //�R�A�h���b�v���Ă�����
    //    if (Corestate.STATE == CS_Core.CORE_STATE.DROP)
    //    {
    //        //�R�A��ǂ�������
    //        if (CoreInRange) { SetTarget(CoreTrans.position,Cofin_State.CORECHASE); }
    //        //�G��ǂ�������
    //        else if(Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
    //        //�v���C���[��ǂ�������
    //        else if (PlayerInRange){ SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
    //    }

    //    if(Corestate.STATE == CS_Core.CORE_STATE.HAVEPLAYER)
    //    {
    //        //�v���C���[��ǂ�������
    //        if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
    //        //�G��ǂ�������
    //        else if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
    //    }

    //    if (Corestate.STATE == CS_Core.CORE_STATE.HAVEENEMY && state != Cofin_State.GOHOME)�@
    //    {
    //        //�G��ǂ�������
    //        if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
    //        //�v���C���[��ǂ�������
    //        else if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }

    //    }

    //}


    ///// <summary>
    ///// �ǐՑΏۂ�ݒ�
    ///// </summary>
    ///// <param �Ώۍ��W="targetpos"></param>
    //public void SetTarget(Vector3 targetpos,Cofin_State changestate)
    //{
    //    //�X�e�[�g���Ⴄ��
    //    bool Set = state != changestate;

    //    if (!Set) { return; }

    //    state = changestate;
    //    //NavMesh�������ł��Ă��邩���f
    //    if(navmeshAgent.enabled)
    //    {
    //        TargetPos = targetpos;
    //        bool setnav = navmeshAgent.pathStatus != NavMeshPathStatus.PathInvalid;
    //        if (setnav) { navmeshAgent.SetDestination(targetpos); }
    //    }



    //}


    ///// <summary>
    ///// Ray�̕\��
    ///// </summary>
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, TargetDistance);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, PlayerDistance);

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, CoreDistance);
    //}



    //private void OnCollisionEnter(Collision collision)
    //{
    //    bool Attack = collision.gameObject.tag == "Attack";
    //    //�U�����ꂽ��
    //    if(Attack)
    //    {
    //        navmeshAgent.enabled = false;

    //        Rigidbody rb;
    //        TryGetComponent<Rigidbody>(out rb);

    //        if (rb != null)
    //        {
    //            // �Փ˕����̔��Ε����ɗ͂�������
    //            Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
    //            rb.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
    //            // �m�b�N�o�b�N�̏I����ҋ@����R���[�`�����J�n
    //            StartCoroutine(EndKnockback());
    //        }

    //        //�Њd���[�V����
    //        state = Cofin_State.INTIMIDATION;
    //        CofinAnim.SetTrigger("Intimidation");
    //    }

    //}

    //private IEnumerator EndKnockback()
    //{
    //    yield return new WaitForSeconds(1f);

    //    // NavMeshAgent���ēx�L���ɂ���
    //    navmeshAgent.enabled = true;

    //    // �ړI�n���Đݒ�i��Ƃ��Č��̖ړI�n�ɖ߂��j
    //    navmeshAgent.SetDestination(TargetPos);

    //}


}
