using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// �S���F���@�h���[���̓GAI
/// </summary>
public class CS_DrawnAI : MonoBehaviour
{
    [Header("�e�p�����[�^")]
    [SerializeField, Tooltip("HP")]
    private float HP = 80.0f;
    private float NowHP;        //���݂�HP
    [SerializeField, Tooltip("�ړ����x")]
    private float MoveSpeed = 3f;
    [SerializeField, Tooltip("�U���Ԋu")]
    private float AttackSpace = 10.0f;
    [SerializeField, Tooltip("�ړ����x�ቺ�t�^����")]
    private float MoveSpeedDownTime = 3f;
    [SerializeField, Tooltip("�������苗��")]
    private float RunAwayDistance = 5f;
    [SerializeField, Tooltip("�ő哦������")]
    private float MaxRanAwayTime = 3f;
    [SerializeField, Tooltip("�ǐՋ���")]
    private float TrackingDistance = 7f;
    [SerializeField, Tooltip("�e���\�����̕\������")]
    private float ViewForecastLineChangeTime = 3f;
    [SerializeField, Tooltip("�U���ҋ@��ԂɂȂ��Ă���J�n�܂ł̎���")]
    private float AttackWaitTime = 1f;

    [Header("-------------------------------------------------")]

    [Header("�ڍאݒ�")]
    [SerializeField, Tooltip("�Ώی��m���̐F")]
    private Color TargetColor = Color.white;
    [SerializeField, Tooltip("�U���ҋ@���̐F")]
    private Color AttackWaitColor = Color.red;
    [SerializeField, Tooltip("�e���\�������ˏo���鑬�x")]
    private float ForecastLineViewSpeed = 5f;
    private float progress = 0f;              // �e���\�����\������
    [SerializeField, Tooltip("�ő哦������")]
    private float MaxRanAwayDistance = 15f;
    [SerializeField, Tooltip("���񂾎��̋󂫊ʐ�����")]
    private int DethCanNum = 3;

    [Header("------------�T�����i�L�P��--------------")]
    [SerializeField, Tooltip("NavMesh")]
    private NavMeshAgent agent;
    [SerializeField, Tooltip("�ePrefab")]
    private GameObject BallObj;
    [SerializeField, Tooltip("�e�𐶐����鋗��")]
    private float CreateBallDistance = 0.5f;
    [SerializeField, Tooltip("�󂫊�")]
    private GameObject Can;
    [SerializeField, Tooltip("HP�Q�[�W")]
    private Image HPGage;
    [SerializeField,Tooltip("�Q�[�W�L�����o�X")]
    private GameObject HPCanvas;
    [SerializeField, Tooltip("Ray����������LineRenderer")] 
    private LineRenderer lineRenderer;

    //--------�^�C�}�[�֘A---------
    private Coroutine currentCoroutine; //���݌v�����Ă���R���[�`��

    private bool CreateBallTrigger = false; //�e�𐶐������g���K�[
    private Vector3 CreateVec;              //�����������

    private int CurrentPathNum = 1;     //���݂̌o�H�̃C���f�b�N�X(1�n�܂�)

    private Transform PlayerTrans;      //�v���C���[�̈ʒu

    //�h���[���̏��
    private enum DrawnState
    {
        IDEL,       //�ҋ@
        TRACKING,   //�ǐ�
        FLEEING,    //����
        CHARGE, //�`���[�W
        ATTACK, //�U��
        DETH    //���S
    }

    [SerializeField]private DrawnState state;   //���݂̏��
    private Rigidbody ThisRb;   //������RigitBody
  
    //�U�����
    private enum DrawnAttackState
    {
        NONE,               //�������Ă��Ȃ�
        VIEWLINE,           //�\�����\��
        ATTACKWAIT,         //�U���ҋ@
        NEXTATTACKSPACE,    //���̍U���܂ł̊Ԋu
    }

    [SerializeField] private DrawnAttackState attackstate;

    // Start is called before the first frame update
    void Start()
    {
        //HP��ۑ�
        NowHP = HP;

        //�n�_�ƏI�_��������
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position); // �ŏ��͎n�_�̂�

        //��Ԃ�������
        state = DrawnState.IDEL;
        attackstate = DrawnAttackState.NONE;

        //RigitBody���擾
        TryGetComponent<Rigidbody>(out ThisRb);

        //�v���C���[�̏�Ԃ��X�|�i�[����擾
        PlayerTrans = CS_SpawnerInfo.GetPlayerTrans();

        //���O�̈ړ����s�����߂�Agent�̎����X�V�𖳌���
        agent.updatePosition = false;
        agent.updateRotation = false;

        currentCoroutine = null;

        //�ڕW��ݒ�
        agent.SetDestination(PlayerTrans.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //HP�Q�[�W�̏���
        HPGage.fillAmount = NowHP / HP;
        HPCanvas.transform.LookAt(PlayerTrans);
        if (HPCanvas.activeSelf)
        {
            StartCoroutine(EndViewHP());
        }

        ActionTable();

        //����ł��珈�����Ȃ�
        if (state == DrawnState.DETH) { return; }

        //-----�v���C���[�Ƃ̋����ɂ���čs��-----
        //���������߂�(�����͍l�����Ȃ�)
        Vector3 targetpos = new Vector3(PlayerTrans.position.x, 0, PlayerTrans.position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        float Distance = Vector3.Distance(pos, targetpos);
        bool Tracking = Distance >= TrackingDistance;    //�w�苗���v���C���[���痣�ꂽ��ǐ�
        bool RanAway = Distance <= RunAwayDistance;      //�v���C���[���߂Â����瓦����

        //��苗���܂Œǐ�
        if (Tracking && state == DrawnState.IDEL) { ChangeState(DrawnState.TRACKING); }
        //�ǐՒ��Ɉ�苗���܂ŋ߂Â�����`���[�W
        if(!Tracking && state == DrawnState.TRACKING) { ChangeState(DrawnState.CHARGE); }
        
        //����(�`���[�W���ƍU�����͓����Ȃ��H)
        if (RanAway && (state != DrawnState.ATTACK || state != DrawnState.CHARGE )) { ChangeState(DrawnState.FLEEING); }

        
        if (state != DrawnState.ATTACK || state != DrawnState.CHARGE) { Move(); }//�ړ�
    }


    /// <summary>
    /// ���O�̈ړ�����
    /// </summary>
    private void Move()
    {
        if(agent.path.corners.Length < 2) { return; }

        //�`���[�W���ƍU�����͓����Ȃ�
        if (attackstate != DrawnAttackState.NEXTATTACKSPACE && (state == DrawnState.CHARGE || state == DrawnState.ATTACK))
        {
            //�v���C���[�̌���������
            if (attackstate != DrawnAttackState.ATTACKWAIT){ transform.LookAt(PlayerTrans); }

            ThisRb.velocity = Vector3.zero;
            return; 
        }

        Vector3 currentPosition = transform.position;
        Vector3 nextWaypoint = agent.path.corners[CurrentPathNum];  // �ŏ��̈ړ���
        Vector3 direction = (nextWaypoint - currentPosition);

        // �G�L�����N�^�[��i�s�����Ɉړ�������
        float forward_x = transform.forward.x * MoveSpeed;
        float forward_z = transform.forward.z * MoveSpeed;

        ThisRb.velocity = new Vector3(forward_x, ThisRb.velocity.y, forward_z);

        // �i�s�����Ɍ����ĉ�]������
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion moveRotation = Quaternion.LookRotation(direction, Vector3.up);
            moveRotation.x = 0;
            moveRotation.z = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, 0.1f);
        }

        // ���̌o�H�_�ɓ��B�����ꍇ�A���̌o�H�_�֐i��
        if (Vector3.Distance(currentPosition, nextWaypoint) < 0.1f)
        {
            // ���̌o�H�_�ɐi�ނ��߂ɃC���f�b�N�X���X�V
            if (CurrentPathNum < agent.path.corners.Length - 1)
            {
                CurrentPathNum++;  // �C���f�b�N�X��i�߂�
            }
            else
            {
                CurrentPathNum = 1;
            }
        }

    }

    /// <summary>
    /// ��ԑJ��
    /// </summary>
    /// <param ���="_state"></param>
    private void ChangeState(DrawnState _state)
    {
        //������ԂȂ�X�V���Ȃ�
        if(state == _state) { return; }

        //���Ԍv������������~�߂�
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        agent.SetDestination(transform.position);

        //�\�����̏�����
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);

        //�\�����J�n�F�ɐF�ύX
        lineRenderer.startColor = TargetColor;
        lineRenderer.endColor = TargetColor;

        //�U����Ԃ̏�����
        if (CreateBallTrigger)
        {
            CreateBallTrigger = false;
            progress = 0;
            ChangeAttackState(DrawnAttackState.NONE);
        }
        state = _state;

    }

    /// <summary>
    /// �A�N�V�����e�[�u��
    /// </summary>
    private void ActionTable()
    {
        switch(state)
        {
            case DrawnState.IDEL:       //�ҋ@(�s���I��)
                break;
            case DrawnState.TRACKING:   //�ǐ�
                Tracking();
                break;
            case DrawnState.FLEEING:    //����
                RanAway();
                break;
            case DrawnState.CHARGE:     //�`���[�W
                ViewLine(PlayerTrans.position);
                break;
            case DrawnState.ATTACK:     //�U��
                Attack();
                break;
            case DrawnState.DETH:       //���S
                DETH();
                break;
        }

    }

    /// <summary>
    /// ����
    /// </summary>
    private void RanAway()
    {

        //������������v�Z
        Vector3 fleeDirection = (transform.position - PlayerTrans.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * MaxRanAwayDistance;

        // NavMesh��̓K�؂ȃ|�C���g���v�Z
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, MaxRanAwayDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        //���ԑ҂�
        if (currentCoroutine != null) { return; }
        currentCoroutine = StartCoroutine(Timer(MaxRanAwayTime,() =>
        {
            ChangeState(DrawnState.IDEL);
        }));

       

    }

    /// <summary>
    /// �ǐ�
    /// </summary>
    private void Tracking()
    {
        //�ڕW��ݒ�
        agent.SetDestination(PlayerTrans.position);
    }

    /// <summary>
    /// �\�����\��(�`���[�W)
    /// </summary>
    /// <param �ˏo��="TargetPos"></param>
    /// false:�������@true:�I�� <returns></returns>
    private void ViewLine(Vector3 TargetPos)
    {

        float LineOffsetY = 1f;

        switch(attackstate)
        {
            case DrawnAttackState.NONE:

                //�\�����̕\��
                lineRenderer.SetPosition(0, transform.position);    // �n�_

                //�����ˏo����`�ŕ\��
                if (progress < 1f)
                {
                    progress += ForecastLineViewSpeed * Time.deltaTime;
                    //���݂̈ʒu���Ԍv�Z
                    Vector3 currentEndPoint = Vector3.Lerp(transform.position, TargetPos, progress);
                    currentEndPoint.y += LineOffsetY;    //������ƍ����o����
                                                  //LineRenderer �̏I�_���X�V
                    lineRenderer.SetPosition(1, currentEndPoint);

                    return;
                }
                else
                {
                    ChangeAttackState(DrawnAttackState.VIEWLINE);
                }

                break;
            case DrawnAttackState.VIEWLINE:             //�\�����\��
                
                TargetPos.y += LineOffsetY;
                lineRenderer.SetPosition(1, TargetPos);

                if (currentCoroutine != null) { return; }
                currentCoroutine = StartCoroutine(Timer(ViewForecastLineChangeTime, () =>
                 {
                     //�U���ҋ@���ɐF�ύX
                     lineRenderer.startColor = AttackWaitColor;
                     lineRenderer.endColor = AttackWaitColor;
                     CreateVec = PlayerTrans.position;  //�����������m��
                     ChangeAttackState(DrawnAttackState.ATTACKWAIT);
                 }));
                break;
            case DrawnAttackState.ATTACKWAIT:           //�U���ҋ@

                if (currentCoroutine != null) { return; }
                currentCoroutine = StartCoroutine(Timer(AttackWaitTime, () =>
                {
                    progress = 0f;                  //����������
                    ChangeState(DrawnState.ATTACK);
                }));

                break;
            case DrawnAttackState.NEXTATTACKSPACE:      //���̍U���܂őҋ@
                //ATTACK�֐��܂�
                break;

        }

    }

    /// <summary>
    /// �U��
    /// </summary>
    private void Attack()
    {
        if(attackstate != DrawnAttackState.ATTACKWAIT) { return; }

        CreateBallTrigger = false;

        //�����̑O���ɐ����ʒu���v�Z
        Vector3 spawnPosition = transform.position + transform.forward * CreateBallDistance;

        //�ڕW�̕������v�Z(�v���C���[)
        Vector3 directionToTarget = (CreateVec - spawnPosition).normalized;

        //��������I�u�W�F�N�g�̉�]���v�Z
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

        //�e�I�u�W�F�N�g�𐶐�
        Instantiate(BallObj, spawnPosition, rotationToTarget);

        CreateBallTrigger = true;

        //�\�����Ă����\����������
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        //�����I�������玟�̍U���܂őҋ@
        ChangeAttackState(DrawnAttackState.NEXTATTACKSPACE);

        //�U���ҋ@�A���݂܂��񌩂ɂ�����
        if (attackstate == DrawnAttackState.NEXTATTACKSPACE)
        {
            //�\�����͏�Ɏ����̏�
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);

            if (currentCoroutine != null) { return; }
            currentCoroutine = StartCoroutine(Timer(AttackSpace, () =>
            {
                //�\�����J�n�F�ɐF�ύX
                lineRenderer.startColor = TargetColor;
                lineRenderer.endColor = TargetColor;
                ChangeAttackState(DrawnAttackState.NONE);
                ChangeState(DrawnState.CHARGE);
            }));
        }


    }

    /// <summary>
    /// ���S
    /// </summary>
    private void DETH()
    {

        //Y���Œ���������ė��Ƃ�
        if ((ThisRb.constraints & RigidbodyConstraints.FreezePositionY) != 0)
        {
            ThisRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }

        //���ԑ҂��ď����@�Ȃ񂩈�U3�b
        if (currentCoroutine != null) { return; }
        currentCoroutine = StartCoroutine(Timer(3f, () =>
        {
            Destroy(this.gameObject);
        }));
     
    }

    private void OnTriggerEnter(Collider other)
    {
        //����ł��疳��
        if(state == DrawnState.DETH) { return; }
        bool AttackHit = other.tag == "Attack";

        //�e�ɓ���������HP����
        if(AttackHit)
        {
            other.TryGetComponent<CS_AirBall>(out CS_AirBall ball);
            if (!ball) { return; }
            NowHP -= ball.Power;

            //���S
            if(NowHP <= 0) 
            {
                //�ʂ𐶐�
                for(int i = 0; i<DethCanNum;i++)
                {
                    Instantiate(Can,transform.position,Quaternion.identity);
                }
                ChangeState(DrawnState.DETH); 
            }
        }

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //����łĂǂ����ɓ������������
    //    if(state == DrawnState.DETH) { Destroy(this.gameObject); }
    //}


    /// <summary>
    /// �U����Ԃ̕ύX
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeAttackState(DrawnAttackState newState)
    {
        if(attackstate == newState) { return; }
        attackstate = newState;
        currentCoroutine = null; // ���̏�ԂŐV�����R���[�`�����J�n�ł���悤�ɂ���
    }

    /// <summary>
    /// �^�C�}�[
    /// </summary>
    /// <param ����="time"></param>
    /// <param �ҋ@��̏���="onComplete"></param>
    /// <returns></returns>

    private IEnumerator Timer(float time, System.Action onComplete)
    {
        //�w�肵���b���҂�
        yield return new WaitForSeconds(time);

        onComplete?.Invoke();
        currentCoroutine = null;
    }



    /// <summary>
    /// HP��\������R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        //�Ăє�\����
        HPCanvas.SetActive(false);

    }

    /// <summary>
    /// HP�Q�[�W�̕\��
    /// </summary>
    public void ViewHPGage()
    {
        HPCanvas.SetActive(true);
    }




}