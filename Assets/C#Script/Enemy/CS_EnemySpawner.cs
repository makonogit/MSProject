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

    [SerializeField, Header("�ʉߗp�̔���o�[�̒���")]
    private float PathSwitchBarLength = 5f;
    [SerializeField, Header("�ʉߔ���Layer")]
    private LayerMask PathLayer;

    [Header("-------�G��Ȃ���---------")]
    [SerializeField, Tooltip("�R�A�̏ꏊ")]
    private Transform CoreTrans;
    [SerializeField, Tooltip("�R�A�̏��")]
    private CS_Core CoreState;
    [SerializeField, Tooltip("�v���C���[�̈ʒu")]
    private Transform PlayerTrans;

    //���Ԍv���p
    private float SpawnStartTime = 0.0f;
    private float SpawnSpaceTime = 0.0f;
    private int CurrentSpawnNum = 0;    //���݂̃X�|�[����
    private bool SpawnSwitch = false;   //�X�|�[���X�C�b�`

    // Start is called before the first frame update
    void Start()
    {
        //�e���ڂ�ݒ�
        CS_SpawnerInfo.SetCoreState(CoreState);
        CS_SpawnerInfo.SetCoreTrans(CoreTrans);
        CS_SpawnerInfo.SetPlayerTrans(PlayerTrans);

        if (SpawnonAwake) { SpawnSwitch = true;  }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //�X�C�b�`�I���ɂȂ����琶��
        if (SpawnSwitch) { Spawn(); }

        else
        {
            if (conditions == SpawnConditions.APPROACH)
            {
                float Distance = Vector3.Distance(transform.position, PlayerTrans.position);
                bool Approach = Distance < SpawnDistance;
                if (Approach) { SpawnSwitch = true; }
            }

            if (conditions == SpawnConditions.PASSTHROUGH)
            {
                Vector3 direction = transform.right;
                Ray ray = new Ray(transform.position, direction);

                // Raycast�̌��ʂ�ێ����邽�߂̕ϐ�
                RaycastHit hit;

                bool Path = Physics.Raycast(ray, out hit, PathSwitchBarLength, PathLayer);

                if (Path) { SpawnSwitch = true; }

            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.right * PathSwitchBarLength);
    }

    private void Spawn()
    {
        //�ő吔�������Ă�����I��
        if(CurrentSpawnNum >= MaxSpawnNum) { Destroy(this.gameObject); }

        //�X�|�[���J�n���Ԃ��v��
        bool Start = SpawnStartTime > SpawnDeley;
        if (!Start) 
        {
            SpawnStartTime += Time.deltaTime;
            return;
        }

        //�X�|�[�����o��ݒ�
        bool Space = SpawnSpaceTime > SpawnSpace;
        if (!Space)
        {
            SpawnSpaceTime += Time.deltaTime;
            return; 
        }
        
        //�����w�肵�Đ���
        for(int i = 0; i < SynchroSpawnNum; i++)
        {
            Quaternion rotate = Quaternion.LookRotation(SpawnDirection);
            Instantiate(SpawnEnemy,transform.position,rotate);
            CurrentSpawnNum++;
        }

        SpawnSpaceTime = 0f;
        
    }

}
