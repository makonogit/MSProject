using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S���F���@Enemy�̃X�|�i�[
/// </summary>
public class CS_EnemySpawner : MonoBehaviour
{
    [SerializeField, Header("��������Enemy")]
    private GameObject SpawnEnemy;

    [SerializeField, Header("�Q�[���J�n������X�|�[�����邩")]
    private bool SpawnonAwake = false;

    [SerializeField, Header("EnemyManager�̃I�u�W�F�N�g")]
    private GameObject EnemyManager;

    private enum SpawnConditions
    {
        APPROACH,   //�ڋ�
        PASSTHROUGH,//�ʉ�
    }

    [SerializeField, Header("�X�|�[������")]
    private SpawnConditions conditions;

    [SerializeField, Header("�X�|�[����")]
    private int MaxSpawnNum = 1;

    [SerializeField, Header("�����X�|�[����")]
    private int SynchroSpawnNum = 1;

    [SerializeField, Header("�X�|�[���Ԋu")]
    private float SpawnSpace = 1.0f;

    [SerializeField, Header("�X�|�[���f�B���C")]
    private float SpawnDeley = 0.0f;

    [SerializeField, Header("�X�|�[�����̃G�l�~�[�̌���")]
    private Vector3 SpawnDirection = Vector3.forward;

    [SerializeField, Header("�ڋߎ��̃X�|�[������")]
    private float SpawnDistance = 1f;
    [SerializeField, Header("�ڋߎ��̃X�|�[������(����)")]
    private float SpawnYDistance = 1f;

    [SerializeField, Header("�ʉߗp�̔���o�[�̒���")]
    private float PathSwitchBarLength = 5f;
    [SerializeField, Header("�ʉߔ���Layer")]
    private LayerMask PathLayer;

    [Header("-------�G��Ȃ���---------")]
   [SerializeField, Tooltip("�v���C���[�̈ʒu")]
    private Transform PlayerTrans;

    //���Ԍv���p
    private float SpawnStartTime = 0.0f;
    private float SpawnSpaceTime = 0.0f;
    private int CurrentSpawnNum = 0;    //���݂̃X�|�[����
    private bool SpawnSwitch = false;   //�X�|�[���X�C�b�`
    private bool end = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SpawnonAwake) { SpawnSwitch = true;  }

        //1��ڐ������͖�������悤�ɐݒ�
        SpawnSpaceTime = SpawnSpace;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (end) { return; }

        //�X�C�b�`�I���ɂȂ����琶��
        if (SpawnSwitch) { Spawn(); }

        else
        {
            if (conditions == SpawnConditions.APPROACH)
            {
                Vector3 thispos = transform.position;
                Vector3 PlayerPos = PlayerTrans.position;

                // Y���̋���
                float verticalDistance = Mathf.Abs(thispos.y - PlayerPos.y);
                
                //XZ���ʂ̋���
                thispos.y = 0;
                PlayerPos.y = 0;
                float Distance = Vector3.Distance(thispos, PlayerPos);
                bool Approach = Distance < SpawnDistance && verticalDistance < SpawnYDistance;
                if (Approach) { SpawnSwitch = true; }
            }

            if (conditions == SpawnConditions.PASSTHROUGH)
            {
                //Vector3 direction = transform.right;
                //Ray ray = new Ray(transform.position, direction);

                // Raycast�̌��ʂ�ێ����邽�߂̕ϐ�
                RaycastHit hit;

                Vector3 start = transform.position - transform.right * (PathSwitchBarLength / 2);
                Vector3 end = transform.position + transform.right * (PathSwitchBarLength / 2);

                bool Path = Physics.Linecast(start, end, out hit, PathLayer);//Physics.Raycast(transform.position,Vector3.right,out hit, PathSwitchBarLength, PathLayer);

                if (Path) { SpawnSwitch = true; }

            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 start = transform.position - transform.right * (PathSwitchBarLength / 2);
        Vector3 end = transform.position + transform.right * (PathSwitchBarLength / 2);

        Gizmos.DrawLine(start,end);

        Debug.DrawRay(transform.position, Vector3.down * SpawnYDistance, Color.red);

        Gizmos.DrawSphere(transform.position, 1.0f);

        Gizmos.DrawWireSphere(transform.position, SpawnDistance);
    }

    private void Spawn()
    {
        //�ő吔�������Ă�����I��
        if (CurrentSpawnNum >= MaxSpawnNum) { end = true; return; }//{ Destroy(this.gameObject); }

        //�X�|�[���J�n���Ԃ��v��
        bool Start = SpawnStartTime >= SpawnDeley;
        if (!Start) 
        {
            SpawnStartTime += Time.deltaTime;
            return;
        }

        //�X�|�[�����o��ݒ�
        bool Space = SpawnSpaceTime >= SpawnSpace;
        if (!Space)
        {
            SpawnSpaceTime += Time.deltaTime;
            return;
        }

        SpawnSpaceTime = 0f;

        //�����w�肵�Đ���
        for (int i = 0; i < SynchroSpawnNum; i++)
        {
            Quaternion rotate = Quaternion.LookRotation(SpawnDirection);
            GameObject enemy = Instantiate(SpawnEnemy,transform.position + transform.forward * 2f,rotate);
            enemy.transform.SetParent(EnemyManager.transform);   //�q�I�u�W�F�N�g�ɐݒ�
            CurrentSpawnNum++;
        }


      
        
    }

}
