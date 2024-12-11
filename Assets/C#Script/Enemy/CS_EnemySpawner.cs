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

    [SerializeField, Header("EnemyManagerのオブジェクト")]
    private GameObject EnemyManager;

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
    [SerializeField, Header("接近時のスポーン距離(高さ)")]
    private float SpawnYDistance = 1f;

    [SerializeField, Header("通過用の判定バーの長さ")]
    private float PathSwitchBarLength = 5f;
    [SerializeField, Header("通過判定Layer")]
    private LayerMask PathLayer;

    [Header("-------触らないで---------")]
   [SerializeField, Tooltip("プレイヤーの位置")]
    private Transform PlayerTrans;

    //時間計測用
    private float SpawnStartTime = 0.0f;
    private float SpawnSpaceTime = 0.0f;
    private int CurrentSpawnNum = 0;    //現在のスポーン数
    private bool SpawnSwitch = false;   //スポーンスイッチ
    private bool end = false;

    // Start is called before the first frame update
    void Start()
    {
        if (SpawnonAwake) { SpawnSwitch = true;  }

        //1回目生成時は無視するように設定
        SpawnSpaceTime = SpawnSpace;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (end) { return; }

        //スイッチオンになったら生成
        if (SpawnSwitch) { Spawn(); }

        else
        {
            if (conditions == SpawnConditions.APPROACH)
            {
                Vector3 thispos = transform.position;
                Vector3 PlayerPos = PlayerTrans.position;

                // Y軸の距離
                float verticalDistance = Mathf.Abs(thispos.y - PlayerPos.y);
                
                //XZ平面の距離
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

                // Raycastの結果を保持するための変数
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
        //最大数生成していたら終了
        if (CurrentSpawnNum >= MaxSpawnNum) { end = true; return; }//{ Destroy(this.gameObject); }

        //スポーン開始時間を計測
        bool Start = SpawnStartTime >= SpawnDeley;
        if (!Start) 
        {
            SpawnStartTime += Time.deltaTime;
            return;
        }

        //スポーン感覚を設定
        bool Space = SpawnSpaceTime >= SpawnSpace;
        if (!Space)
        {
            SpawnSpaceTime += Time.deltaTime;
            return;
        }

        SpawnSpaceTime = 0f;

        //向き指定して生成
        for (int i = 0; i < SynchroSpawnNum; i++)
        {
            Quaternion rotate = Quaternion.LookRotation(SpawnDirection);
            GameObject enemy = Instantiate(SpawnEnemy,transform.position + transform.forward * 2f,rotate);
            enemy.transform.SetParent(EnemyManager.transform);   //子オブジェクトに設定
            CurrentSpawnNum++;
        }


      
        
    }

}
