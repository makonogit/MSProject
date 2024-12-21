using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// 担当：菅　ザコ敵
/// </summary>
public class CS_Cofine : MonoBehaviour
{
    public enum Cofin_State
    {
        IDLE,                //待機
        TARGETSEARCH,        //目標検索
        KANAMECHASE,         //カナメを追いかける
        ENEMYCHASE,          //他のEnemyを追いかける    
        CORECHASE,           //コアを追いかける
        KANAMEATTACKLOTTERY, //攻撃抽選
        KANAMEATTACK,        //飛びつき攻撃
        CORESTEAL,           //コアを取得
        GOHOME,              //帰宅
        INTIMIDATION,        //威嚇
        FALL,                //落下
        DETH,                //死亡
    }

    [SerializeField, Tooltip("ENEMYSTATE")]
    private Cofin_State state;
    public Cofin_State GetState() => state; //状態を取得
    public void SetState(CS_Cofine.Cofin_State _state) { state = _state; }

    private bool CoreGet = false;       //コアを取得したかどうか

    [Header("-----------------------------------------------")]

    [Header("各パラメーター")]
    [SerializeField, Tooltip("移動速度")]
    private float MoveSpeed = 1.0f;
    [SerializeField, Tooltip("方向転換速度")]
    private float RotationSpeed = 1.0f;
    [SerializeField, Tooltip("ノックバック力")]
    private float KnockBackForce = 1.0f;
    [SerializeField, Tooltip("攻撃力")]
    private float AttackPower = 1.0f;
    [SerializeField, Tooltip("攻撃間隔")]
    private float AttackSpace = 3f;
    [SerializeField, Tooltip("HP")]
    private float HP = 30.0f;
    private float NowHP;    //現在のHP
    [SerializeField, Header("コアを減らす量/s")]
    private float EnelgyStealPower = 1f;

    [Header("-----------------------------------------------")]

    [Header("対象検知用")]
    [SerializeField, Header("ターゲット検知距離(赤)")]
    private float TargetDistance = 3.0f;
    [SerializeField, Header("コアを奪取する距離(緑)")]
    private float CoreDistance = 3.0f;
    [SerializeField, Header("プレイヤーを攻撃する距離(青)")]
    private float PlayerDistance = 3.0f;
    [SerializeField, Tooltip("接近最小距離")]
    [Range(0.01f, 100f)] private float DistanceStop = 0.1f;

    [SerializeField, Tooltip("他Enemy検知レイヤー")]
    private LayerMask EnemyLayer;

    [Header("-----------------------------------------------")]

    [SerializeField, Header("ステージスタート位置(帰宅位置)")]
    private Vector3 StartPos;

    //--------追跡ターゲットの情報----------
    private Transform CoreTrans;    //コアの位置
    private CS_Core Corestate;      //コアの状態
    private Transform PlayerTrans;  //プレイヤーの位置
    private CS_EnergyCure CoreEnelgy; //コアのエネルギ―取得
    private CS_StageInfo Status;        //ステータス表示

    private Vector3 CurrentTargetPos;      //現在の追跡ターゲットの座標

    private int CurrentPathNum = 1;     　 //現在の経路のインデックス(1始まり)

    [Header("-----------------------------------------------")]
    [Header("設定項目")]
    [SerializeField, Tooltip("自分のRigitBody")]
    private Rigidbody ThisRd;           //移動用RigidBody
    [SerializeField, Tooltip("NavMesh")]
    private NavMeshAgent navmeshAgent;  //追跡用NavMesh
    [SerializeField, Tooltip("Animator")]
    private Animator CofinAnim;
    [SerializeField, Tooltip("Light")]
    private Light SpotLight;
    [SerializeField, Tooltip("発光時の明るさ")]
    private float LightBrightness = 100;
    [SerializeField, Tooltip("空き缶")]
    private GameObject Can;
    [SerializeField, Tooltip("生成する空き缶の数")]
    private int CanNum = 3;
    private int CurrentCanNum = 0;  //現在の缶詰の数
    [SerializeField, Tooltip("HPGage")]
    private Image HPGage;
    [SerializeField]
    private GameObject HPCanvas;
    //SE再生用
    [SerializeField, Tooltip("SEAudioSource")]
    private AudioSource SE;
    [SerializeField, Tooltip("SEList")]
    private List<AudioClip> SEList;

    private CS_EnemyManager EnemyManager;        //敵の管理用スクリプト

    // Start is called before the first frame update
    void Start()
    {
        state = Cofin_State.IDLE;

        //親からマネージャーを取得
        transform.parent.TryGetComponent<CS_EnemyManager>(out EnemyManager);

        //コアとプレイヤーの状態をマネージャーから取得
        Corestate = EnemyManager.GetCS_Core();
        CoreTrans = EnemyManager.GetCoreTrans();
        PlayerTrans = EnemyManager.GetPlayerTrans();
        Status = EnemyManager.GetStageInfo();

        CoreTrans.TryGetComponent<CS_EnergyCure>(out CoreEnelgy);

        //HPゲージを設定
        NowHP = HP;
        HPGage.fillAmount = NowHP / HP;

        //HPゲージを非表示
        HPCanvas.SetActive(false);

        // 自前の移動を行うためにAgentの自動更新を無効化
        navmeshAgent.updatePosition = false;
        navmeshAgent.updateRotation = false;

        //目標を設定
        TargetDetection();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //HPゲージの処理
        HPGage.fillAmount = NowHP / HP;
        if (HPCanvas.activeSelf) { StartCoroutine(EndViewHP()); }    //HPが表示されていたら消す
    
        if(state == Cofin_State.DETH) { return; }

        ActionTable();
    }

    /// <summary>
    ///　移動
    /// </summary>
    private void Move()
    {
        if (navmeshAgent.path.corners.Length < 2) { return; }  // 経路がない場合は終了

        //PlaySE(2, true);

        Vector3 currentPosition = transform.position;
        Vector3 nextWaypoint = navmeshAgent.path.corners[CurrentPathNum];  // 最初の移動先
        Vector3 direction = (nextWaypoint - currentPosition);

        //// 回避処理
        //Vector3 avoidance = Vector3.zero;
        //float avoidRadius = 2.0f; // 回避する半径
        //Collider[] nearbyAgents = Physics.OverlapSphere(currentPosition, avoidRadius); // 一定範囲内のエージェントを検知

        //foreach (Collider collider in nearbyAgents)
        //{
        //    // 自分自身のNavMeshAgentは無視
        //    NavMeshAgent otherAgent = collider.GetComponent<NavMeshAgent>();
        //    if (otherAgent != null && otherAgent != navmeshAgent)
        //    {
        //        Vector3 toOtherAgent = otherAgent.transform.position - currentPosition;
        //        float distance = toOtherAgent.magnitude;

        //        // 距離に応じた回避ベクトルを計算（近いほど強く回避）
        //        if (distance < avoidRadius)
        //        {
        //            avoidance -= toOtherAgent.normalized / distance;
        //        }
        //    }
        //}

        // 回避方向を移動方向に加算
        //Vector3 finalDirection = (direction + avoidance).normalized;


        // 敵キャラクターを進行方向に移動させる
        float forward_x = transform.forward.x * MoveSpeed;
        float forward_z = transform.forward.z * MoveSpeed;

        ThisRd.velocity = new Vector3(forward_x, ThisRd.velocity.y, forward_z);

        // 進行方向に向けて回転させる
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion moveRotation = Quaternion.LookRotation(direction, Vector3.up);
            moveRotation.x = 0;
            moveRotation.z = 0;
            transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, 0.1f);
        }

        // 次の経路点に到達した場合、次の経路点へ進む
        currentPosition.y = 0;
        nextWaypoint.y = 0;
        float dis = Vector3.Distance(currentPosition, nextWaypoint);
        if (dis < 0.5f)
        {
            // 次の経路点に進むためにインデックスを更新
            if (CurrentPathNum < navmeshAgent.path.corners.Length - 1)
            {
                CurrentPathNum++;  // インデックスを進める
            }

        }
        // デバッグ用に経路線を表示
        Debug.DrawLine(currentPosition, nextWaypoint, Color.red);

    }


    /// <summary>
    /// 状態ごとの行動
    /// </summary>
    private void ActionTable()
    {
        
        switch(state)
        {
            case Cofin_State.IDLE:                  //待機
                StartCoroutine(EndStop());
                break;
            case Cofin_State.KANAMECHASE:           //カナメ追従
                TargetDetection();
                //指定距離離れた場所を設定
                Vector3 PlayerFoward = transform.forward;
                Vector3 distancepos = PlayerTrans.position + (PlayerFoward.normalized * PlayerDistance);
                SetTarget(distancepos);
                float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
                bool Playerattack = playerdistance <= PlayerDistance;
                //抽選中へ
                EnemyManager.AddApproachCofin(this);
                state = Cofin_State.KANAMEATTACKLOTTERY;
                Move();
                break;
            case Cofin_State.ENEMYCHASE:            //敵追従
                break;
            case Cofin_State.CORECHASE:             //コア追従
                TargetDetection();
                SetTarget(CoreTrans.position);
                Move();
                break;
            case Cofin_State.KANAMEATTACKLOTTERY:   //攻撃抽選
                playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
                Playerattack = playerdistance > PlayerDistance + 2f;
                //攻撃範囲内にいなくなったら再度目標検索
                if (Playerattack)
                {
                    EnemyManager.DeleteApproachCofin(this);
                    TargetDetection();
                }
                break;
            case Cofin_State.KANAMEATTACK:          //攻撃
                EnemyManager.AttackWait(AttackSpace);
                SetTarget(PlayerTrans.position);
                Move();
                break;
            case Cofin_State.CORESTEAL:             //コアを奪う
                 //コア座標を固定
                CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
                CoreEnelgy.SetEnergy(CoreEnelgy.GetEnergy() - EnelgyStealPower * Time.deltaTime);
                SetTarget(StartPos);
                Move();
                break;
            case Cofin_State.GOHOME:                //家に帰る
                SetTarget(StartPos);
                //コアを落としたりしたら再度目標検索
                if(Corestate.STATE != CS_Core.CORE_STATE.HAVEENEMY) { TargetDetection(); }
                Move();
                break;
            case Cofin_State.INTIMIDATION:          //威嚇
                break;
            case Cofin_State.FALL:                  //落下
                break;
            case Cofin_State.DETH:                  //死亡
                //攻撃待機に登録されてたら消す
                EnemyManager.DeleteApproachCofin(this); 
                break;
        }
    }

    /// <summary>
    /// NavMeshAgentの目的地設定
    /// </summary>
    /// <param 目標座標="target"></param>
    public void SetTarget(Vector3 target)
    {
        if(navmeshAgent == null || !navmeshAgent.isActiveAndEnabled) { return; }
        navmeshAgent.SetDestination(target);
        //CurrentPathNum = 0; // 経路をリセット
    }

    /// <summary>
    /// 対象検知用
    /// </summary>
    private void TargetDetection()
    {

        float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
        float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

        //コア追跡
        bool CoreTracking = coredistance < TargetDistance;
        //プレイヤー追跡
        bool PlayerTracking = playerdistance < TargetDistance;

        switch(Corestate.STATE)
        {
            case CS_Core.CORE_STATE.DROP:
                if (CoreTracking) { state = Cofin_State.CORECHASE; }
                else if (PlayerTracking) { state = Cofin_State.KANAMECHASE; }
                break;
            case CS_Core.CORE_STATE.HAVEPLAYER:
                state = Cofin_State.KANAMECHASE;   //カナメを追いかける
                break;
            case CS_Core.CORE_STATE.HAVEENEMY:
                state = Cofin_State.GOHOME;        //家に帰る
                break;
        }

        //範囲内に何もなければ
        if (!CoreTracking && !PlayerTracking)
        {
            Destroy(this.gameObject);   //デスポーン
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(state == Cofin_State.DETH) { return; }
        //コアの取得
        if (collision.gameObject.tag == "EnergyCore")
        {
            //取られた表示
            if (Status.GetCurrentStatus() != CS_StageInfo.StageStatus.CoreSteal)
            {
                Status.SetStatus(CS_StageInfo.StageStatus.CoreSteal);
            }

            state = Cofin_State.CORESTEAL;
            //CoreGet = true;
            //コア座標を固定
            CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

            //コアの状態を変更
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
        }

        bool PlayerHit = collision.gameObject.tag == "Player";

        //攻撃状態でプレイヤーと衝突したら
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
        //    // EnemyManager.DeleteApproachCofin(this); //攻撃終了したら消去
        //    state = Cofin_State.IDEL;
        //    CofinAnim.SetBool("Idle", true);
        //}

    }

    private void OnTriggerEnter(Collider other)
    {
        if(state == Cofin_State.DETH) { return; }
        bool Attack = other.gameObject.tag == "Attack";
        //攻撃されたら
        if (Attack)
        {
            //PlaySE(7, false);
            //navmeshAgent.enabled = false;
            //EnemyManager.DeleteApproachCofin(this);
            other.transform.TryGetComponent<CS_AirBall>(out CS_AirBall airBall);
            
            //弾に衝突したら
            if (airBall != null && !airBall.GetEnemyType())
            {
                NowHP -= airBall.Power;                    //HPを減らす

                SpotLight.intensity = LightBrightness;  //発光させる
                StartCoroutine(EndLight());             //しばらくしたら発光停止

                //死亡処理
                if (NowHP <= 0 && CurrentCanNum < CanNum)
                {
                    state = Cofin_State.DETH;

                    //コライダーを無効に
                    transform.tag = "Untagged";
                    for (; CurrentCanNum < CanNum; CurrentCanNum++)
                    {
                        //缶の生成
                        Instantiate(Can, transform.position, Quaternion.identity);
                    }
                    CofinAnim.SetBool("Deth", true);
                  　PlaySE(6, false);

                }
            }


            //ノックバック処理
            if (ThisRd != null)
            {
                 // 衝突方向の反対方向に力を加える
                Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
                ThisRd.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
                //CofinAnim.SetTrigger("KnockBack");
                // ノックバックの終了を待機するコルーチンを開始
                state = Cofin_State.IDLE;
            }

            //コアを落とす
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

            //    ////威嚇モーション
            //    //state = Cofin_State.INTIMIDATION;
            //    //CofinAnim.SetTrigger("Intimidation");

            //}

        }


    }

    /// <summary>
    /// 死亡処理　アニメーションで呼び出し
    /// </summary>
    public void Deth()
    {
        Destroy(this.gameObject);
    }



    /// <summary>
    /// HPゲージの表示
    /// </summary>
    public void ViewHPGage(Transform PlayerTrans)
    {
        HPCanvas.transform.LookAt(PlayerTrans);
        HPCanvas.SetActive(true);
    }


    /// <summary>
    /// HPゲージ表示コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        //再び非表示に
        HPCanvas.SetActive(false);

    }

    
    /// <summary>
    /// 待機
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndStop()
    {
        yield return new WaitForSeconds(AttackSpace);
        CofinAnim.SetBool("Idle", false);
        TargetDetection();  //次の目標を決める

    }


    /// <summary>
    ///　一定時間発光コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndLight()
    {
        yield return new WaitForSeconds(0.2f);

        //明るさをリセット
        SpotLight.intensity = 0f;

    }


    /// <summary>
    /// Rayの表示
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
    //    IDEL,       //待機
    //    KANAMECHASE,    //カナメを追いかける
    //    ENEMYCHASE,     //他のEnemyを追いかける    
    //    CORECHASE,      //コアを追いかける
    //    KANAMEATTACK,   //飛びつき攻撃
    //    CORESTEAL,      //コアを取得
    //    GOHOME,         //帰宅
    //    INTIMIDATION,   //威嚇
    //    FALL,           //落下
    //    DETH,           //死亡
    //}

    //[SerializeField,Tooltip("ENEMYSTATE")]
    //private Cofin_State state;

    //[Header("-----------------------------------------------")]

    //[Header("対象検知用")]
    //[SerializeField, Header("ターゲット検知距離(赤)")]
    //private float TargetDistance = 3.0f;
    //[SerializeField,Header("コアを奪取する距離(緑)")]
    //private float CoreDistance = 3.0f;
    //[SerializeField, Header("プレイヤーを攻撃する距離(青)")]
    //private float PlayerDistance = 3.0f;

    //[SerializeField, Tooltip("他Enemy検知レイヤー")]
    //private LayerMask EnemyLayer;

    //[Header("-----------------------------------------------")]


    //private Transform CoreTrans;    //コアの位置
    //private CS_Core Corestate;      //コアの状態
    //private Transform PlayerTrans;  //プレイヤーの位置

    //[SerializeField, Header("ステージスタート位置(帰宅位置)")]
    //private Vector3 StartPos;

    //private Vector3 TargetPos;      //追跡ターゲットの座標

    //[SerializeField,Header("移動速度")]
    //private float MoveSpeed = 1.0f;

    //[SerializeField,Header("ノックバック力")]
    //private float KnockBackForce = 1.0f;

    //[SerializeField,Tooltip("NavMesh")]
    //private NavMeshAgent navmeshAgent;  //追跡用NavMesh
    //[SerializeField, Tooltip("Animator")]
    //private Animator CofinAnim;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    state = Cofin_State.IDEL;
    //    navmeshAgent.speed = MoveSpeed;

    //    //コアとプレイヤーの状態をスポナーから取得
    //    Corestate = CS_SpawnerInfo.GetCoreState();
    //    CoreTrans = CS_SpawnerInfo.GetCoreTrans();
    //    PlayerTrans = CS_SpawnerInfo.GetPlayerTrans();

    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{

    //    //対象検知して追跡
    //    TargetDetection();

    //    Attack();

    //}

    ///// <summary>
    ///// ジャンプ
    ///// </summary>
    //private void Jump()
    //{

    //}


    ///// <summary>
    ///// 落下/死亡
    ///// </summary>
    //private void Fall()
    //{
    //    bool FallFlg = navmeshAgent.isOnNavMesh == false;

    //    if (!FallFlg) { return; }

    //    //落ちるモーション
    //    if (!CofinAnim.GetBool("Fall")) { CofinAnim.SetBool("Fall", true); }

    //    float dethHieght = 0.0f; //死亡する高さ

    //    if(transform.position.y < dethHieght)
    //    {
    //        state = Cofin_State.DETH;
    //    }

    //}


    ///// <summary>
    ///// 攻撃
    ///// </summary>
    //private void Attack()
    //{
    //    //カナメを攻撃
    //    if (state == Cofin_State.KANAMECHASE)
    //    {
    //        float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
    //        bool Playerattack = playerdistance < PlayerDistance;

    //        if (!Playerattack) { return; }

    //        CofinAnim.SetTrigger("Attack");
    //        state = Cofin_State.KANAMEATTACK;
    //        //hit.transform.GetComponent<CS_Player>()
    //        //プレイヤーの情報を取得して攻撃
    //    }

    //    //コアを取得
    //    if(state == Cofin_State.CORECHASE)
    //    {
    //        float coredistance = Vector3.Distance(transform.position, CoreTrans.position);
    //        bool Coreget = coredistance < CoreDistance;

    //        if (!Coreget) { return; }

    //        state = Cofin_State.CORESTEAL;

    //        //コアの状態を変更
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
    //        //コア座標を固定
    //        CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
    //        //子オブジェクトにする
    //        //transform.SetParent(hit.transform.parent);

    //    }

    //}


    ///// <summary>
    ///// 対象検知用
    ///// </summary>
    //private void TargetDetection()
    //{

    //    //コアを取ったら帰宅

    //    if (state == Cofin_State.CORESTEAL)
    //    {
    //        //コア座標を固定
    //        CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
    //        //コアの状態を変更
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;

    //        SetTarget(StartPos, Cofin_State.GOHOME);
    //    }

    //    //SetTarget(PlayerTrans.position

    //    float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
    //    float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

    //    Collider[] Enemyhit = Physics.OverlapSphere(transform.position, TargetDistance, EnemyLayer);

    //    //範囲内にあるか
    //    bool PlayerInRange = playerdistance < TargetDistance;
    //    bool CoreInRange = coredistance < TargetDistance;

    //    //コアドロップしていたら
    //    if (Corestate.STATE == CS_Core.CORE_STATE.DROP)
    //    {
    //        //コアを追いかける
    //        if (CoreInRange) { SetTarget(CoreTrans.position,Cofin_State.CORECHASE); }
    //        //敵を追いかける
    //        else if(Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
    //        //プレイヤーを追いかける
    //        else if (PlayerInRange){ SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
    //    }

    //    if(Corestate.STATE == CS_Core.CORE_STATE.HAVEPLAYER)
    //    {
    //        //プレイヤーを追いかける
    //        if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
    //        //敵を追いかける
    //        else if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
    //    }

    //    if (Corestate.STATE == CS_Core.CORE_STATE.HAVEENEMY && state != Cofin_State.GOHOME)　
    //    {
    //        //敵を追いかける
    //        if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
    //        //プレイヤーを追いかける
    //        else if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }

    //    }

    //}


    ///// <summary>
    ///// 追跡対象を設定
    ///// </summary>
    ///// <param 対象座標="targetpos"></param>
    //public void SetTarget(Vector3 targetpos,Cofin_State changestate)
    //{
    //    //ステートが違うか
    //    bool Set = state != changestate;

    //    if (!Set) { return; }

    //    state = changestate;
    //    //NavMeshが準備できているか判断
    //    if(navmeshAgent.enabled)
    //    {
    //        TargetPos = targetpos;
    //        bool setnav = navmeshAgent.pathStatus != NavMeshPathStatus.PathInvalid;
    //        if (setnav) { navmeshAgent.SetDestination(targetpos); }
    //    }



    //}


    ///// <summary>
    ///// Rayの表示
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
    //    //攻撃されたら
    //    if(Attack)
    //    {
    //        navmeshAgent.enabled = false;

    //        Rigidbody rb;
    //        TryGetComponent<Rigidbody>(out rb);

    //        if (rb != null)
    //        {
    //            // 衝突方向の反対方向に力を加える
    //            Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
    //            rb.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
    //            // ノックバックの終了を待機するコルーチンを開始
    //            StartCoroutine(EndKnockback());
    //        }

    //        //威嚇モーション
    //        state = Cofin_State.INTIMIDATION;
    //        CofinAnim.SetTrigger("Intimidation");
    //    }

    //}

    //private IEnumerator EndKnockback()
    //{
    //    yield return new WaitForSeconds(1f);

    //    // NavMeshAgentを再度有効にする
    //    navmeshAgent.enabled = true;

    //    // 目的地を再設定（例として元の目的地に戻す）
    //    navmeshAgent.SetDestination(TargetPos);

    //}


}
