using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 担当：菅　Enemyのスポナー
/// </summary>
public class CS_EnemySpawner : MonoBehaviour
{
    [SerializeField, Header("生成するEnemy")]
    private GameObject SpawnEnemy;

    [SerializeField, Header("ゲーム開始時からスポーンするか")]
    private bool SpawnonAwake = false;

    private enum SpawnConditions
    {
        APPROACH,   //接近
        PASSTHROUGH,//通過
    }

    [SerializeField, Header("スポーン条件")]
    private SpawnConditions conditions;

    [SerializeField, Header("スポーン数")]
    private int MaxSpawnNum = 1;

    [SerializeField, Header("同時スポーン数")]
    private int SynchroSpawnNum = 1;

    [SerializeField, Header("スポーン間隔")]
    private float SpawnSpace = 1.0f;

    [SerializeField, Header("スポーンディレイ")]
    private float SpawnDeley = 0.0f;

    [SerializeField, Header("スポーン時のエネミーの向き")]
    private Vector3 SpawnDirection = Vector3.forward;

    [SerializeField, Header("接近時のスポーン距離")]
    private float SpawnDistance = 1f;

    [SerializeField, Header("通過用の判定バーの長さ")]
    private float PathSwitchBarLength = 5f;
    [SerializeField, Header("通過判定Layer")]
    private LayerMask PathLayer;

    [Header("-------触らないで---------")]
    [SerializeField, Tooltip("コアの場所")]
    private Transform CoreTrans;
    [SerializeField, Tooltip("コアの状態")]
    private CS_Core CoreState;
    [SerializeField, Tooltip("プレイヤーの位置")]
    private Transform PlayerTrans;

    //時間計測用
    private float SpawnStartTime = 0.0f;
    private float SpawnSpaceTime = 0.0f;
    private int CurrentSpawnNum = 0;    //現在のスポーン数
    private bool SpawnSwitch = false;   //スポーンスイッチ

    // Start is called before the first frame update
    void Start()
    {
        //各項目を設定
        CS_SpawnerInfo.SetCoreState(CoreState);
        CS_SpawnerInfo.SetCoreTrans(CoreTrans);
        CS_SpawnerInfo.SetPlayerTrans(PlayerTrans);

        if (SpawnonAwake) { SpawnSwitch = true;  }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //スイッチオンになったら生成
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

                // Raycastの結果を保持するための変数
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
        //最大数生成していたら終了
        if(CurrentSpawnNum >= MaxSpawnNum) { Destroy(this.gameObject); }

        //スポーン開始時間を計測
        bool Start = SpawnStartTime > SpawnDeley;
        if (!Start) 
        {
            SpawnStartTime += Time.deltaTime;
            return;
        }

        //スポーン感覚を設定
        bool Space = SpawnSpaceTime > SpawnSpace;
        if (!Space)
        {
            SpawnSpaceTime += Time.deltaTime;
            return; 
        }
        
        //向き指定して生成
        for(int i = 0; i < SynchroSpawnNum; i++)
        {
            Quaternion rotate = Quaternion.LookRotation(SpawnDirection);
            Instantiate(SpawnEnemy,transform.position,rotate);
            CurrentSpawnNum++;
        }

        SpawnSpaceTime = 0f;
        
    }

}
