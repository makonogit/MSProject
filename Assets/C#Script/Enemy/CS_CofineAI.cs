using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// �S���F���@�U�R�G
/// </summary>
public class CS_CofineAI : CS_Cofine
{
    //public enum Cofin_State
    //{
    //    IDEL,                //�ҋ@
    //    MOVE,                //�ړ����
    //    KANAMECHASE,         //�J�i����ǂ�������
    //    ENEMYCHASE,          //����Enemy��ǂ�������    
    //    CORECHASE,           //�R�A��ǂ�������
    //    KANAMEATTACKLOTTERY, //�U�����I
    //    KANAMEATTACK,        //��т��U��
    //    CORESTEAL,           //�R�A���擾
    //    GOHOME,              //�A��
    //    INTIMIDATION,        //�Њd
    //    FALL,                //����
    //    DETH,                //���S
    //}

    //[SerializeField, Tooltip("ENEMYSTATE")]
    //private Cofin_State state;

    //public Cofin_State GetState() => state; //��Ԃ��擾

    //[Header("-----------------------------------------------")]

    //[Header("�Ώی��m�p")]
    //[SerializeField, Header("�^�[�Q�b�g���m����(��)")]
    //private float TargetDistance = 3.0f;
    //[SerializeField, Header("�R�A��D�悷�鋗��(��)")]
    //private float CoreDistance = 3.0f;
    //[SerializeField, Header("�v���C���[���U�����鋗��(��)")]
    //private float PlayerDistance = 3.0f;
    //[SerializeField, Tooltip("�ڋߍŏ�����")]
    //[Range(0.01f, 100f)] private float DistanceStop = 0.1f;

    //[SerializeField, Tooltip("��Enemy���m���C���[")]
    //private LayerMask EnemyLayer;

    //[Header("-----------------------------------------------")]

    //[SerializeField, Header("�X�e�[�W�X�^�[�g�ʒu(�A��ʒu)")]
    //private Vector3 StartPos;

    ////--------�ǐՃ^�[�Q�b�g�̏��----------
    //private Transform CoreTrans;    //�R�A�̈ʒu
    //private CS_Core Corestate;      //�R�A�̏��
    //private Transform PlayerTrans;  //�v���C���[�̈ʒu

    //private Vector3 TargetPos;                        //�ǐՃ^�[�Q�b�g�̍��W

    ////private Vector3 TargetDirection;                 //�ǐՃ^�[�Q�b�g�̌���
    ////private Quaternion CurrentDestinationsRotate;    //���݂̖ڕW����

    //private bool CoreGet = false;       //�R�A���擾�������ǂ���
    //[SerializeField, Tooltip("������RigitBody")]
    //private Rigidbody ThisRd;           //�ړ��pRigidBody

    //[Header("�e�p�����[�^�[")]
    //[SerializeField, Tooltip("�ړ����x")]
    //private float MoveSpeed = 1.0f;
    //[SerializeField, Tooltip("�����]�����x")]
    //private float RotationSpeed = 1.0f;
    //[SerializeField,Tooltip("�m�b�N�o�b�N��")]
    //private float KnockBackForce = 1.0f;
    //[SerializeField, Tooltip("�U����")]
    //private float AttackPower = 1.0f;
    //[SerializeField, Tooltip("�U���Ԋu")]
    //private float AttackSpace = 3f;
    //[SerializeField, Tooltip("HP")]
    //private float HP = 30.0f;
    //private float NowHP;    //���݂�HP

    //[Header("-----------------------------------------------")]
  

    //[SerializeField, Tooltip("NavMesh")]
    //private NavMeshAgent navmeshAgent;  //�ǐ՗pNavMesh
    //private int CurrentPathNum = 1;     //���݂̌o�H�̃C���f�b�N�X(1�n�܂�)

    //[SerializeField, Tooltip("Animator")]
    //private Animator CofinAnim;
    //[SerializeField, Tooltip("Light")]
    //private Light SpotLight;
    //[SerializeField, Tooltip("�������̖��邳")]
    //private float LightBrightness = 100;
    //[SerializeField, Tooltip("�󂫊�")]
    //private GameObject Can;
    //[SerializeField, Tooltip("��������󂫊ʂ̐�")]
    //private int CanNum = 3;
    //[SerializeField, Tooltip("HPGage")]
    //private Image HPGage;
    //[SerializeField]
    //private GameObject HPCanvas;

    ////SE�Đ��p

    //[SerializeField, Tooltip("SEAudioSource")]
    //private AudioSource SE;
    //[SerializeField, Tooltip("SEList")]
    //private�@List<AudioClip> SEList;


    //private CS_EnemyManager EnemyManager;        //�G�̊Ǘ��p�X�N���v�g

    //// Start is called before the first frame update
    //void Start()
    //{
    //    state = Cofin_State.MOVE;

    //    //�e����}�l�[�W���[���擾
    //    transform.parent.TryGetComponent<CS_EnemyManager>(out EnemyManager);
       
    //    //�R�A�ƃv���C���[�̏�Ԃ��}�l�[�W���[����擾
    //    Corestate = EnemyManager.GetCS_Core();
    //    CoreTrans = EnemyManager.GetCoreTrans();
    //    PlayerTrans = EnemyManager.GetPlayerTrans();

    //    //HP�Q�[�W��ݒ�
    //    NowHP = HP;
    //    HPGage.fillAmount = NowHP / HP;

    //    //HP�Q�[�W���\��
    //    //HPSliderObj.SetActive(false);

    //    // ���O�̈ړ����s�����߂�Agent�̎����X�V�𖳌���
    //    navmeshAgent.updatePosition = false;
    //    navmeshAgent.updateRotation = false;


    //}
    
    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    //HP�Q�[�W�̏���
    //    HPGage.fillAmount = NowHP / HP;
    //    if (HPCanvas.activeSelf) { StartCoroutine(EndViewHP()); }    //HP���\������Ă��������
    //    if(state == Cofin_State.DETH) { return;  }
    //    //��莞�ԑҋ@
    //    if (state == Cofin_State.IDEL)
    //    {
          
    //        StartCoroutine(EndStop());
    //        return;
    //    }

    //    //�Ώی��m���Ēǐ�
    //    TargetDetection();

    //    //�ړ�
    //    Move();

    //    //�U��
    //    //Attack();

    
    //}

    ///// <summary>
    /////�@�ړ�
    ///// </summary>
    //private void Move()
    //{
    //    if (navmeshAgent.path.corners.Length < 2) { return; }  // �o�H���Ȃ��ꍇ�͏I��

    //    Vector3 currentPosition = transform.position;
    //    Vector3 nextWaypoint = navmeshAgent.path.corners[CurrentPathNum];  // �ŏ��̈ړ���
    //    Vector3 direction = (nextWaypoint - currentPosition);
       

    //    // �G�L�����N�^�[��i�s�����Ɉړ�������
    //    float forward_x = transform.forward.x * MoveSpeed;
    //    float forward_z = transform.forward.z * MoveSpeed;

    //    ThisRd.velocity = new Vector3(forward_x, ThisRd.velocity.y, forward_z);

    //    // �i�s�����Ɍ����ĉ�]������
    //    if (direction.sqrMagnitude > 0.01f)
    //    {
    //        Quaternion moveRotation = Quaternion.LookRotation(direction, Vector3.up);
    //        moveRotation.x = 0;
    //        moveRotation.z = 0;
    //        transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, 0.1f);
    //    }

    //    // ���̌o�H�_�ɓ��B�����ꍇ�A���̌o�H�_�֐i��
    //    currentPosition.y = 0;
    //    nextWaypoint.y = 0;
    //    float dis = Vector3.Distance(currentPosition, nextWaypoint);
    //    if (dis < 0.5f)
    //    {
    //        // ���̌o�H�_�ɐi�ނ��߂ɃC���f�b�N�X���X�V
    //        if (CurrentPathNum < navmeshAgent.path.corners.Length - 1)
    //        {
    //            CurrentPathNum++;  // �C���f�b�N�X��i�߂�
    //        }

    //    }
    //    // �f�o�b�O�p�Ɍo�H����\��
    //    Debug.DrawLine(currentPosition, nextWaypoint, Color.red);

    //}

    ///// <summary>
    ///// �W�����v
    ///// </summary>
    //private void Jump()
    //{

    //}


    ///// <summary>
    ///// HP�Q�[�W�̕\��
    ///// </summary>
    //public void ViewHPGage(Transform PlayerTrans)
    //{
    //    HPCanvas.transform.LookAt(PlayerTrans);
    //    HPCanvas.SetActive(true);
    //}


    ///// <summary>
    ///// ����/���S
    ///// </summary>
    //private void Fall()
    //{
    //   // bool FallFlg = navmeshAgent.isOnNavMesh == false;

    //    //if (!FallFlg) { return; }

    //    ////�����郂�[�V����
    //    //if (!CofinAnim.GetBool("Fall")) { CofinAnim.SetBool("Fall", true); }

    //    //float dethHieght = 0.0f; //���S���鍂��

    //    //if (transform.position.y < dethHieght)
    //    //{
    //    //    state = Cofin_State.DETH;
    //    //}

    //}


    ///// <summary>
    ///// �U��
    ///// </summary>
    //public void Attack()
    //{
    //    SetTarget(PlayerTrans.position, Cofin_State.KANAMEATTACK);
    
    //}


    ///// <summary>
    ///// �Ώی��m�p
    ///// </summary>
    //private void TargetDetection()
    //{
    //    PlaySE(1, true);

    //    if (CoreGet)
    //    {
    //        state = Cofin_State.GOHOME;
    //        //�R�A�̏�Ԃ�ύX
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
    //        //�R�A���W���Œ�
    //        CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

    //    }
        
    //    if (state == Cofin_State.GOHOME )
    //    {
    //        SetTarget(StartPos, state);
    //    }


    //    //�G�̓����蔻��
    //   // Collider[] Enemyhit = Physics.OverlapSphere(transform.position, TargetDistance, EnemyLayer);
        
       
    //    float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
    //    float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

    //    //�R�A�ǐ�
    //    bool CoreTracking = coredistance < TargetDistance;
    //    //�v���C���[�ǐ�
    //    bool PlayerTracking = playerdistance < TargetDistance;

    //    //�R�A�h���b�v���Ă�����
    //    if (Corestate.STATE == CS_Core.CORE_STATE.DROP)
    //    {
    //        //�R�A��ǐ�
    //        if (CoreTracking) { SetTarget(CoreTrans.position, Cofin_State.CORECHASE); }
    //        //�G��ǐ�
    //        //else if (Enemyhit.Length > 0 && Enemyhit[0].gameObject != gameObject) { SetTarget(Enemyhit[0].transform.position, Cofin_State.ENEMYCHASE); }
    //        //�v���C���[��ǐ�
    //        else if (PlayerTracking && state != Cofin_State.KANAMEATTACK) { SetTarget(PlayerTrans.position, Cofin_State.KANAMECHASE); }
           
    //    }

    //    //�v���C���[���R�A�������Ă�����
    //    if (Corestate.STATE == CS_Core.CORE_STATE.HAVEPLAYER && state != Cofin_State.KANAMEATTACK)
    //    {
    //        //�v���C���[��ǂ�������
    //        if (PlayerTracking) { SetTarget(PlayerTrans.position, Cofin_State.KANAMECHASE); }
    //        //�G��ǂ�������
    //        //else if (Enemyhit.Length > 0 && Enemyhit[0].gameObject != gameObject) { SetTarget(Enemyhit[0].transform.position, Cofin_State.ENEMYCHASE); }
        
    //    }


    //    //���̓G�������Ă�����
    //    if (Corestate.STATE == CS_Core.CORE_STATE.HAVEENEMY)
    //    {
    //        //�X�^�[�g�n�_�ɋA��
    //        SetTarget(StartPos, Cofin_State.GOHOME);
    //    }

    //    //�J�i�����U��(���I��)
    //    if (state == Cofin_State.KANAMECHASE)
    //    {
    //        //�w�苗�����ꂽ�ꏊ��ݒ�
    //        Vector3 PlayerFoward = transform.forward;
    //        Vector3 distancepos = PlayerTrans.position + (PlayerFoward.normalized * PlayerDistance);
    //        SetTarget(distancepos, state);
    //        //navmeshAgent.SetDestination(PlayerTrans.position);

    //        bool Playerattack = playerdistance <= PlayerDistance;

    //        if (!Playerattack) 
    //        {
    //            EnemyManager.DeleteApproachCofin(this);
    //            return; 
    //        }

    //        //���I����
    //        EnemyManager.AddApproachCofin(this);
    //        state = Cofin_State.KANAMEATTACKLOTTERY;
            
    //    }


    //    if (!CoreTracking && !PlayerTracking)
    //    {
    //        Destroy(this.gameObject);   //�f�X�|�[��
    //    }
      
    //}


    ///// <summary>
    ///// �ǐՑΏۂ�ݒ�
    ///// </summary>
    ///// <param �Ώۍ��W="targetpos"></param>
    //public void SetTarget(Vector3 targetpos, Cofin_State changestate)
    //{
    //    //�ړI�n��ݒ�
    //    NavMeshHit hit;
    //    bool Target = NavMesh.SamplePosition(targetpos, out hit, 100, NavMesh.AllAreas);
    //    if (Target)
    //    {
    //        if (navmeshAgent != null) 
    //        {
    //            navmeshAgent.SetDestination(targetpos);
    //            TargetPos = targetpos;
    //        }
    //    }


    //    //�X�e�[�g���Ⴄ��
    //    bool Set = state != changestate;

    //    if (!Set) { return; }

    //    state = changestate;
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

    //    //// �����̏����O���̈ʒu���v�Z
    //    //Vector3 RayPos = transform.position + transform.forward * FloorRayFowardOffset;
    //    //Debug.DrawRay(RayPos, Vector3.down * FloorDistance, Color.red);
    //}



    //private void OnCollisionEnter(Collision collision)
    //{
      

    //    if (collision.gameObject.tag == "EnergyCore")
    //    {
    //        //state = Cofin_State.CORESTEAL;
    //        CoreGet = true;

    //        //�R�A�̏�Ԃ�ύX
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;

    //        SetTarget(StartPos, Cofin_State.GOHOME);
    //    }

    //    if (collision.gameObject.tag == "Player" && state == Cofin_State.KANAMEATTACK)
    //    {
    //        PlaySE(4, false);
    //        CofinAnim.SetTrigger("Attack");
    //        // EnemyManager.DeleteApproachCofin(this); //�U���I�����������
    //        state = Cofin_State.IDEL;
    //        CofinAnim.SetBool("Idle", true);
    //    }

         

    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    bool Attack = other.gameObject.tag == "Attack";
    //    //�U�����ꂽ��
    //    if (Attack)
    //    {
    //        PlaySE(7, false);
    //        //navmeshAgent.enabled = false;
    //        EnemyManager.DeleteApproachCofin(this);
    //        other.transform.TryGetComponent<CS_AirBall>(out CS_AirBall airBall);
    //        if (airBall != null)
    //        {
    //            NowHP -= airBall.Power;                    //HP�����炷

    //            SpotLight.intensity = LightBrightness;  //����������
    //            StartCoroutine(EndLight());

    //            //���S����
    //            if(NowHP <= 0)
    //            {
    //                state = Cofin_State.DETH;
    //                //�R���C�_�[�𖳌���
    //                transform.tag = "Untagged";
    //                for (int i = 0; i < CanNum; i++)
    //                {
    //                    //�ʂ̐���
    //                    Instantiate(Can, transform.position, Quaternion.identity);
    //                }
    //                CofinAnim.SetBool("Deth",true);
    //                PlaySE(6, false);

    //            }
    //        }

    //        if (ThisRd != null)
    //        {
    //            //navmeshAgent.enabled = false;
    //            // �Փ˕����̔��Ε����ɗ͂�������
    //            Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
    //            ThisRd.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
    //            //CofinAnim.SetTrigger("KnockBack");
    //            // �m�b�N�o�b�N�̏I����ҋ@����R���[�`�����J�n
    //            state = Cofin_State.IDEL;
    //            CofinAnim.SetBool("Idle", true);
    //        }

    //        if (CoreGet)
    //        {
    //            Corestate.STATE = CS_Core.CORE_STATE.DROP;
    //            state = Cofin_State.IDEL;
    //            CofinAnim.SetBool("Idle", true);
    //            CoreGet = false;

    //            ////�Њd���[�V����
    //            //state = Cofin_State.INTIMIDATION;
    //            //CofinAnim.SetTrigger("Intimidation");

    //        }

    //    }


    //}

    ///// <summary>
    ///// ���S�����@�A�j���[�V�����ŌĂяo��
    ///// </summary>
    //public void Deth()
    //{
    //    Destroy(this.gameObject);
    //}

    ////��莞�ԑҋ@���Ĉړ���Ԃ�
    //private IEnumerator EndStop()
    //{
    //    yield return new WaitForSeconds(AttackSpace);
       
       
    //    state = Cofin_State.MOVE;
    //    CofinAnim.SetBool("Idle", false);
       
    //}


    //private IEnumerator EndViewHP()
    //{
    //    yield return new WaitForSeconds(3f);

    //    //�Ăє�\����
    //    HPCanvas.SetActive(false);

    //}


    //private IEnumerator EndLight()
    //{
    //    yield return new WaitForSeconds(0.2f);

    //    //���邳�����Z�b�g
    //    SpotLight.intensity = 0f;

    //}

    //private void PlaySE(int PlayNum,bool Loop)
    //{
    //    if(SE.clip == SEList[PlayNum]) { return; }

    //    if (!Loop) 
    //    {
    //        SE.Stop();
    //        SE.clip = null;
    //        SE.PlayOneShot(SEList[PlayNum]);
    //        return;
    //    }

    //    if (SE.isPlaying) { return; }
    //    SE.clip = SEList[PlayNum];
    //    SE.Play();
    //}
}
