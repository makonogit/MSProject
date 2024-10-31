using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 担当：菅　ザコ敵
/// </summary>
public class CS_Cofine : MonoBehaviour
{
    public enum Cofin_State
    {
        IDEL,       //待機
        KANAMECHASE,    //カナメを追いかける
        ENEMYCHASE,     //他のEnemyを追いかける    
        CORECHASE,      //コアを追いかける
        KANAMEATTACK,   //飛びつき攻撃
        CORESTEAL,      //コアを取得
        GOHOME,         //帰宅
        INTIMIDATION,   //威嚇
        FALL,           //落下
        DETH,           //死亡
    }

    [SerializeField,Tooltip("ENEMYSTATE")]
    private Cofin_State state;

    [Header("-----------------------------------------------")]

    [Header("対象検知用")]
    [SerializeField, Header("ターゲット検知距離(赤)")]
    private float TargetDistance = 3.0f;
    [SerializeField,Header("コアを奪取する距離(緑)")]
    private float CoreDistance = 3.0f;
    [SerializeField, Header("プレイヤーを攻撃する距離(青)")]
    private float PlayerDistance = 3.0f;

    [SerializeField, Tooltip("他Enemy検知レイヤー")]
    private LayerMask EnemyLayer;

    [Header("-----------------------------------------------")]


    private Transform CoreTrans;    //コアの位置
    private CS_Core Corestate;      //コアの状態
    private Transform PlayerTrans;  //プレイヤーの位置

    [SerializeField, Header("ステージスタート位置(帰宅位置)")]
    private Vector3 StartPos;

    private Vector3 TargetPos;      //追跡ターゲットの座標

    [SerializeField,Header("移動速度")]
    private float MoveSpeed = 1.0f;
    
    [SerializeField,Header("ノックバック力")]
    private float KnockBackForce = 1.0f;

    [SerializeField,Tooltip("NavMesh")]
    private NavMeshAgent navmeshAgent;  //追跡用NavMesh
    [SerializeField, Tooltip("Animator")]
    private Animator CofinAnim;

    // Start is called before the first frame update
    void Start()
    {
        state = Cofin_State.IDEL;
        navmeshAgent.speed = MoveSpeed;

        //コアとプレイヤーの状態をスポナーから取得
        Corestate = CS_SpawnerInfo.GetCoreState();
        CoreTrans = CS_SpawnerInfo.GetCoreTrans();
        PlayerTrans = CS_SpawnerInfo.GetPlayerTrans();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //対象検知して追跡
        TargetDetection();

        Attack();

    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    private void Jump()
    {

    }


    /// <summary>
    /// 落下/死亡
    /// </summary>
    private void Fall()
    {
        bool FallFlg = navmeshAgent.isOnNavMesh == false;

        if (!FallFlg) { return; }

        //落ちるモーション
        if (!CofinAnim.GetBool("Fall")) { CofinAnim.SetBool("Fall", true); }

        float dethHieght = 0.0f; //死亡する高さ

        if(transform.position.y < dethHieght)
        {
            state = Cofin_State.DETH;
        }

    }


    /// <summary>
    /// 攻撃
    /// </summary>
    private void Attack()
    {
        //カナメを攻撃
        if (state == Cofin_State.KANAMECHASE)
        {
            float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
            bool Playerattack = playerdistance < PlayerDistance;

            if (!Playerattack) { return; }

            CofinAnim.SetTrigger("Attack");
            state = Cofin_State.KANAMEATTACK;
            //hit.transform.GetComponent<CS_Player>()
            //プレイヤーの情報を取得して攻撃
        }

        //コアを取得
        if(state == Cofin_State.CORECHASE)
        {
            float coredistance = Vector3.Distance(transform.position, CoreTrans.position);
            bool Coreget = coredistance < CoreDistance;

            if (!Coreget) { return; }

            state = Cofin_State.CORESTEAL;

            //コアの状態を変更
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;
            //コア座標を固定
            CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
            //子オブジェクトにする
            //transform.SetParent(hit.transform.parent);

        }

    }


    /// <summary>
    /// 対象検知用
    /// </summary>
    private void TargetDetection()
    {

        //コアを取ったら帰宅
        if (state == Cofin_State.CORESTEAL)
        {
            //コア座標を固定
            CoreTrans.position = new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z);
            //コアの状態を変更
            Corestate.STATE = CS_Core.CORE_STATE.HAVEENEMY;

            SetTarget(StartPos, Cofin_State.GOHOME);
        }

        //SetTarget(PlayerTrans.position

        float playerdistance = Vector3.Distance(transform.position, PlayerTrans.position);
        float coredistance = Vector3.Distance(transform.position, CoreTrans.position);

        Collider[] Enemyhit = Physics.OverlapSphere(transform.position, TargetDistance, EnemyLayer);

        //範囲内にあるか
        bool PlayerInRange = playerdistance < TargetDistance;
        bool CoreInRange = coredistance < TargetDistance;

        //コアドロップしていたら
        if (Corestate.STATE == CS_Core.CORE_STATE.DROP)
        {
            //コアを追いかける
            if (CoreInRange) { SetTarget(CoreTrans.position,Cofin_State.CORECHASE); }
            //敵を追いかける
            else if(Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
            //プレイヤーを追いかける
            else if (PlayerInRange){ SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
        }

        if(Corestate.STATE == CS_Core.CORE_STATE.HAVEPLAYER)
        {
            //プレイヤーを追いかける
            if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }
            //敵を追いかける
            else if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
        }

        if (Corestate.STATE == CS_Core.CORE_STATE.HAVEENEMY && state != Cofin_State.GOHOME)　
        {
            //敵を追いかける
            if (Enemyhit.Length > 0) { SetTarget(Enemyhit[0].transform.position,Cofin_State.ENEMYCHASE); }
            //プレイヤーを追いかける
            else if (PlayerInRange) { SetTarget(PlayerTrans.position,Cofin_State.KANAMECHASE); }

        }

    }


    /// <summary>
    /// 追跡対象を設定
    /// </summary>
    /// <param 対象座標="targetpos"></param>
    public void SetTarget(Vector3 targetpos,Cofin_State changestate)
    {
        //ステートが違うか
        bool Set = state != changestate;

        if (!Set) { return; }

        state = changestate;
        //NavMeshが準備できているか判断
        bool setnav = navmeshAgent.pathStatus != NavMeshPathStatus.PathInvalid;
        if (setnav) { navmeshAgent.SetDestination(targetpos); }
        
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



    private void OnCollisionEnter(Collision collision)
    {
        bool Attack = collision.gameObject.tag == "Attack";
        //攻撃されたら
        if(Attack)
        {
            Rigidbody rb;
            TryGetComponent<Rigidbody>(out rb);

            if (rb != null)
            {
                // 衝突方向の反対方向に力を加える
                Vector3 knockbackDirection = (transform.position - collision.transform.position).normalized;
                rb.AddForce(knockbackDirection * KnockBackForce, ForceMode.Impulse);
            }

            //威嚇モーション
            state = Cofin_State.INTIMIDATION;
            CofinAnim.SetTrigger("Intimidation");
        }

    }


}
