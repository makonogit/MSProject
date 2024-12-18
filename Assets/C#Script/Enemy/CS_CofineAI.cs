using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// 担当：菅　ザコ敵
/// </summary>
public class CS_CofineAI : CS_Cofine
{
    //public enum Cofin_State
    //{
    //    IDEL,                //待機
    //    MOVE,                //移動状態
    //    KANAMECHASE,         //カナメを追いかける
    //    ENEMYCHASE,          //他のEnemyを追いかける    
    //    CORECHASE,           //コアを追いかける
    //    KANAMEATTACKLOTTERY, //攻撃抽選
    //    KANAMEATTACK,        //飛びつき攻撃
    //    CORESTEAL,           //コアを取得
    //    GOHOME,              //帰宅
    //    INTIMIDATION,        //威嚇
    //    FALL,                //落下
    //    DETH,                //死亡
    //}

    //[SerializeField, Tooltip("ENEMYSTATE")]
    //private Cofin_State state;

    //public Cofin_State GetState() => state; //状態を取得

    //[Header("-----------------------------------------------")]

    //[Header("対象検知用")]
    //[SerializeField, Header("ターゲット検知距離(赤)")]
    //private float TargetDistance = 3.0f;
    //[SerializeField, Header("コアを奪取する距離(緑)")]
    //private float CoreDistance = 3.0f;
    //[SerializeField, Header("プレイヤーを攻撃する距離(青)")]
    //private float PlayerDistance = 3.0f;
    //[SerializeField, Tooltip("接近最小距離")]
    //[Range(0.01f, 100f)] private float DistanceStop = 0.1f;

    //[SerializeField, Tooltip("他Enemy検知レイヤー")]
    //private LayerMask EnemyLayer;

    //[Header("-----------------------------------------------")]

    //[SerializeField, Header("ステージスタート位置(帰宅位置)")]
    //private Vector3 StartPos;

    ////--------追跡ターゲットの情報----------
    //private Transform CoreTrans;    //コアの位置
    //private CS_Core Corestate;      //コアの状態
    //private Transform PlayerTrans;  //プレイヤーの位置

    //private Vector3 TargetPos;                        //追跡ターゲットの座標

    ////private Vector3 TargetDirection;                 //追跡ターゲットの向き
    ////private Quaternion CurrentDestinationsRotate;    //現在の目標向き

    //private bool CoreGet = false;       //コアを取得したかどうか
    //[SerializeField, Tooltip("自分のRigitBody")]
    //private Rigidbody ThisRd;           //移動用RigidBody

    //[Header("各パラメーター")]
    //[SerializeField, Tooltip("移動速度")]
    //private float MoveSpeed = 1.0f;
    //[SerializeField, Tooltip("方向転換速度")]
    //private float RotationSpeed = 1.0f;
    //[SerializeField,Tooltip("ノックバック力")]
    //private float KnockBackForce = 1.0f;
    //[SerializeField, Tooltip("攻撃力")]
    //private float AttackPower = 1.0f;
    //[SerializeField, Tooltip("攻撃間隔")]
    //private float AttackSpace = 3f;
    //[SerializeField, Tooltip("HP")]
    //private float HP = 30.0f;
    //private float NowHP;    //現在のHP

    //[Header("-----------------------------------------------")]
  

    //[SerializeField, Tooltip("NavMesh")]
    //private NavMeshAgent navmeshAgent;  //追跡用NavMesh
    //private int CurrentPathNum = 1;     //現在の経路のインデックス(1始まり)

    //[SerializeField, Tooltip("Animator")]
    //private Animator CofinAnim;
    //[SerializeField, Tooltip("Light")]
    //private Light SpotLight;
    //[SerializeField, Tooltip("発光時の明るさ")]
    //private float LightBrightness = 100;
    //[SerializeField, Tooltip("空き缶")]
    //private GameObject Can;
    //[SerializeField, Tooltip("生成する空き缶の数")]
    //private int CanNum = 3;
    //[SerializeField, Tooltip("HPGage")]
    //private Image HPGage;
    //[SerializeField]
    //private GameObject HPCanvas;

    ////SE再生用

    //[SerializeField, Tooltip("SEAudioSource")]
    //private AudioSource SE;
    //[SerializeField, Tooltip("SEList")]
    //private　List<AudioClip> SEList;


    //private CS_EnemyManager EnemyManager;        //敵の管理用スクリプト

    //// Start is called before the first frame update
    //void Start()
    //{
    //    state = Cofin_State.MOVE;

    //    //親からマネージャーを取得
    //    transform.parent.TryGetComponent<CS_EnemyManager>(out EnemyManager);
       
    //    //コアとプレイヤーの状態をマネージャーから取得
    //    Corestate = EnemyManager.GetCS_Core();
    //    CoreTrans = EnemyManager.GetCoreTrans();
    //    PlayerTrans = EnemyManager.GetPlayerTrans();

    //    //HPゲージを設定
    //    NowHP = HP;
    //    HPGage.fillAmount = NowHP / HP;

    //    //HPゲージを非表示
    //    //HPSliderObj.SetActive(false);

    //    // 自前の移動を行うためにAgentの自動更新を無効化
    //    navmeshAgent.updatePosition = false;
    //    navmeshAgent.updateRotation = false;


    //}
    
    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    //HPゲージの処理
    //    HPGage.fillAmount = NowHP / HP;
    //    if (HPCanvas.activeSelf) { StartCoroutine(EndViewHP()); }    //HPが表示されていたら消す
    //    if(state == Cofin_State.DETH) { return;  }
    //    //一定時間待機
    //    if (state == Cofin_State.IDEL)
    //    {
          
    //        StartCoroutine(EndStop());
    //        return;
    //    }

    //    //対象検知して追跡
    //    TargetDetection();

    //    //移動
    //    Move();

    //    //攻撃
    //    //Attack();

    
    //}

    ///// <summary>
    /////　移動
    ///// </summary>
    //private void Move()
    //{
    //    if (navmeshAgent.path.corners.Length < 2) { return; }  // 経路がない場合は終了

    //    Vector3 currentPosition = transform.position;
    //    Vector3 nextWaypoint = navmeshAgent.path.corners[CurrentPathNum];  // 最初の移動先
    //    Vector3 direction = (nextWaypoint - currentPosition);
       

    //    // 敵キャラクターを進行方向に移動させる
    //    float forward_x = transform.forward.x * MoveSpeed;
    //    float forward_z = transform.forward.z * MoveSpeed;

    //    ThisRd.velocity = new Vector3(forward_x, ThisRd.velocity.y, forward_z);

    //    // 進行方向に向けて回転させる
    //    if (direction.sqrMagnitude > 0.01f)
    //    {
    //        Quaternion moveRotation = Quaternion.LookRotation(direction, Vector3.up);
    //        moveRotation.x = 0;
    //        moveRotation.z = 0;
    //        transform.rotation = Quaternion.Lerp(transform.rotation, moveRotation, 0.1f);
    //    }

    //    // 次の経路点に到達した場合、次の経路点へ進む
    //    currentPosition.y = 0;
    //    nextWaypoint.y = 0;
    //    float dis = Vector3.Distance(currentPosition, nextWaypoint);
    //    if (dis < 0.5f)
    //    {
    //        // 次の経路点に進むためにインデックスを更新
    //        if (CurrentPathNum < navmeshAgent.path.corners.Length - 1)
    //        {
    //            CurrentPathNum++;  // インデックスを進める
    //        }

    //    }
    //    // デバッグ用に経路線を表示
    //    Debug.DrawLine(currentPosition, nextWaypoint, Color.red);

    //}

    ///// <summary>
    ///// ジャンプ
    ///// </summary>
    //private void Jump()
    //{

    //}


    ///// <summary>
    ///// HPゲージの表示
    ///// </summary>
    //public void ViewHPGage(Transform PlayerTrans)
    //{
    //    HPCanvas.transform.LookAt(PlayerTrans);
    //    HPCanvas.SetActive(true);
    //}


    ///// <summary>
    ///// 落下/死亡
    ///// </summary>
    //private void Fall()
    //{
    //   // bool FallFlg = navmeshAgent.isOnNavMesh == false;

    //    //if (!FallFlg) { return; }

    //    ////落ちるモーション
    //    //if (!CofinAnim.GetBool("Fall")) { CofinAnim.SetBool("Fall", true); }

    //    //float dethHieght = 0.0f; //死亡する高さ

    //    //if (transform.position.y < dethHieght)
    //    //{
    //    //    state = Cofin_State.DETH;
    //    //}

    //}


    ///// <summary>
    ///// 攻撃
    ///// </summary>
    //public void Attack()
    //{
    //    SetTarget(PlayerTrans.position, Cofin_State.KANAMEATTACK);
    
    //}


    ///// <summary>
    ///// 対象検知用
    ///// </summary>
    //private void TargetDetection()
    //{
    //    PlaySE(1, true);

    //    if (CoreGet)
    //    {
    //        state = Cofin_State.GOHOME;
    //        //コアの状態を変更
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
    //        //コア座標を固定
    //        CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

    //    }
        
    //    if (state == Cofin_State.GOHOME )
    //    {
    //        SetTarget(StartPos, state);
    //    }


    //    //敵の当たり判定
    //   // Collider[] Enemyhit = Physics.OverlapSphere(transform.position, TargetDistance, EnemyLayer);
        
       
    //    float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
    //    float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

    //    //コア追跡
    //    bool CoreTracking = coredistance < TargetDistance;
    //    //プレイヤー追跡
    //    bool PlayerTracking = playerdistance < TargetDistance;

    //    //コアドロップしていたら
    //    if (Corestate.STATE == CS_Core.CORE_STATE.DROP)
    //    {
    //        //コアを追跡
    //        if (CoreTracking) { SetTarget(CoreTrans.position, Cofin_State.CORECHASE); }
    //        //敵を追跡
    //        //else if (Enemyhit.Length > 0 && Enemyhit[0].gameObject != gameObject) { SetTarget(Enemyhit[0].transform.position, Cofin_State.ENEMYCHASE); }
    //        //プレイヤーを追跡
    //        else if (PlayerTracking && state != Cofin_State.KANAMEATTACK) { SetTarget(PlayerTrans.position, Cofin_State.KANAMECHASE); }
           
    //    }

    //    //プレイヤーがコアを持っていたら
    //    if (Corestate.STATE == CS_Core.CORE_STATE.HAVEPLAYER && state != Cofin_State.KANAMEATTACK)
    //    {
    //        //プレイヤーを追いかける
    //        if (PlayerTracking) { SetTarget(PlayerTrans.position, Cofin_State.KANAMECHASE); }
    //        //敵を追いかける
    //        //else if (Enemyhit.Length > 0 && Enemyhit[0].gameObject != gameObject) { SetTarget(Enemyhit[0].transform.position, Cofin_State.ENEMYCHASE); }
        
    //    }


    //    //他の敵が持っていたら
    //    if (Corestate.STATE == CS_Core.CORE_STATE.HAVEENEMY)
    //    {
    //        //スタート地点に帰る
    //        SetTarget(StartPos, Cofin_State.GOHOME);
    //    }

    //    //カナメを攻撃(抽選へ)
    //    if (state == Cofin_State.KANAMECHASE)
    //    {
    //        //指定距離離れた場所を設定
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

    //        //抽選中へ
    //        EnemyManager.AddApproachCofin(this);
    //        state = Cofin_State.KANAMEATTACKLOTTERY;
            
    //    }


    //    if (!CoreTracking && !PlayerTracking)
    //    {
    //        Destroy(this.gameObject);   //デスポーン
    //    }
      
    //}


    ///// <summary>
    ///// 追跡対象を設定
    ///// </summary>
    ///// <param 対象座標="targetpos"></param>
    //public void SetTarget(Vector3 targetpos, Cofin_State changestate)
    //{
    //    //目的地を設定
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


    //    //ステートが違うか
    //    bool Set = state != changestate;

    //    if (!Set) { return; }

    //    state = changestate;
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

    //    //// 自分の少し前方の位置を計算
    //    //Vector3 RayPos = transform.position + transform.forward * FloorRayFowardOffset;
    //    //Debug.DrawRay(RayPos, Vector3.down * FloorDistance, Color.red);
    //}



    //private void OnCollisionEnter(Collision collision)
    //{
      

    //    if (collision.gameObject.tag == "EnergyCore")
    //    {
    //        //state = Cofin_State.CORESTEAL;
    //        CoreGet = true;

    //        //コアの状態を変更
    //        Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;

    //        SetTarget(StartPos, Cofin_State.GOHOME);
    //    }

    //    if (collision.gameObject.tag == "Player" && state == Cofin_State.KANAMEATTACK)
    //    {
    //        PlaySE(4, false);
    //        CofinAnim.SetTrigger("Attack");
    //        // EnemyManager.DeleteApproachCofin(this); //攻撃終了したら消去
    //        state = Cofin_State.IDEL;
    //        CofinAnim.SetBool("Idle", true);
    //    }

         

    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    bool Attack = other.gameObject.tag == "Attack";
    //    //攻撃されたら
    //    if (Attack)
    //    {
    //        PlaySE(7, false);
    //        //navmeshAgent.enabled = false;
    //        EnemyManager.DeleteApproachCofin(this);
    //        other.transform.TryGetComponent<CS_AirBall>(out CS_AirBall airBall);
    //        if (airBall != null)
    //        {
    //            NowHP -= airBall.Power;                    //HPを減らす

    //            SpotLight.intensity = LightBrightness;  //発光させる
    //            StartCoroutine(EndLight());

    //            //死亡処理
    //            if(NowHP <= 0)
    //            {
    //                state = Cofin_State.DETH;
    //                //コライダーを無効に
    //                transform.tag = "Untagged";
    //                for (int i = 0; i < CanNum; i++)
    //                {
    //                    //缶の生成
    //                    Instantiate(Can, transform.position, Quaternion.identity);
    //                }
    //                CofinAnim.SetBool("Deth",true);
    //                PlaySE(6, false);

    //            }
    //        }

    //        if (ThisRd != null)
    //        {
    //            //navmeshAgent.enabled = false;
    //            // 衝突方向の反対方向に力を加える
    //            Vector3 knockbackDirection = (transform.position - other.transform.position).normalized;
    //            ThisRd.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
    //            //CofinAnim.SetTrigger("KnockBack");
    //            // ノックバックの終了を待機するコルーチンを開始
    //            state = Cofin_State.IDEL;
    //            CofinAnim.SetBool("Idle", true);
    //        }

    //        if (CoreGet)
    //        {
    //            Corestate.STATE = CS_Core.CORE_STATE.DROP;
    //            state = Cofin_State.IDEL;
    //            CofinAnim.SetBool("Idle", true);
    //            CoreGet = false;

    //            ////威嚇モーション
    //            //state = Cofin_State.INTIMIDATION;
    //            //CofinAnim.SetTrigger("Intimidation");

    //        }

    //    }


    //}

    ///// <summary>
    ///// 死亡処理　アニメーションで呼び出し
    ///// </summary>
    //public void Deth()
    //{
    //    Destroy(this.gameObject);
    //}

    ////一定時間待機して移動状態へ
    //private IEnumerator EndStop()
    //{
    //    yield return new WaitForSeconds(AttackSpace);
       
       
    //    state = Cofin_State.MOVE;
    //    CofinAnim.SetBool("Idle", false);
       
    //}


    //private IEnumerator EndViewHP()
    //{
    //    yield return new WaitForSeconds(3f);

    //    //再び非表示に
    //    HPCanvas.SetActive(false);

    //}


    //private IEnumerator EndLight()
    //{
    //    yield return new WaitForSeconds(0.2f);

    //    //明るさをリセット
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
