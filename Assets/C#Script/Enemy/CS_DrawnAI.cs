using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// 担当：菅　ドローンの敵AI
/// </summary>
public class CS_DrawnAI : MonoBehaviour
{
    private CS_EnemyManager EnemyManager;        //敵の管理用スクリプト

    [Header("確認用、触らない！")]
    [SerializeField]
    private Vector3[] path;

    [SerializeField]
    private int CurrentPathNum = 1;     //現在の経路のインデックス(1始まり)

    [Header("各パラメータ")]
    [SerializeField, Tooltip("HP")]
    private float HP = 80.0f;
    private float NowHP;        //現在のHP
    [SerializeField, Tooltip("移動速度")]
    private float MoveSpeed = 3f;
    [SerializeField, Tooltip("攻撃間隔")]
    private float AttackSpace = 10.0f;
    [SerializeField, Tooltip("移動速度低下付与時間")]
    private float MoveSpeedDownTime = 3f;
    [SerializeField, Tooltip("移動速度低下率")]
    [Range(0,1)]private float MoveSpeedDownParsentage = 0.5f;
    [SerializeField, Tooltip("逃走判定距離")]
    private float RunAwayDistance = 5f;
    [SerializeField, Tooltip("最大逃走時間")]
    private float MaxRanAwayTime = 3f;
    [SerializeField, Tooltip("追跡距離")]
    private float TrackingDistance = 7f;
    [SerializeField, Tooltip("弾道予測線の表示時間")]
    private float ViewForecastLineChangeTime = 3f;
    [SerializeField, Tooltip("攻撃待機状態になってから開始までの時間")]
    private float AttackWaitTime = 1f;

    [Header("-------------------------------------------------")]

    [Header("詳細設定")]
    [SerializeField, Tooltip("対象検知時の色")]
    private Color TargetColor = Color.white;
    [SerializeField, Tooltip("攻撃待機時の色")]
    private Color AttackWaitColor = Color.red;
    [SerializeField, Tooltip("弾道予測線を射出する速度")]
    private float ForecastLineViewSpeed = 5f;
    private float progress = 0f;              // 弾道予測線表示割合
    [SerializeField, Tooltip("最大逃走距離")]
    private float MaxRanAwayDistance = 15f;
    [SerializeField, Tooltip("死んだ時の空き缶生成数")]
    private int DethCanNum = 3;
    [SerializeField, Tooltip("プレイヤーから離れて飛ぶ高さ")]
    private float ApploachY = 1f;

    [Header("------------サワルナキケン--------------")]
    [SerializeField, Tooltip("NavMesh")]
    private NavMeshAgent agent;
    [SerializeField, Tooltip("Animator")]
    private Animator anim;
    [SerializeField, Tooltip("弾Prefab")]
    private GameObject BallObj;
    [SerializeField, Tooltip("弾を生成する距離")]
    private float CreateBallDistance = 0.5f;
    [SerializeField, Tooltip("空き缶")]
    private GameObject Can;
    [SerializeField, Tooltip("HPゲージ")]
    private Image HPGage;
    [SerializeField,Tooltip("ゲージキャンバス")]
    private GameObject HPCanvas;
    [SerializeField, Tooltip("Rayを可視化するLineRenderer")] 
    private LineRenderer lineRenderer;
    [SerializeField, Tooltip("SE再生Source")]
    private AudioSource SE;
    [SerializeField, Tooltip("SEList")]
    private　List<AudioClip> SEList;
    [SerializeField, Tooltip("チャージ時のエフェクト")]
    private GameObject ChargeEffect;
    private GameObject CurrentChargeEffect;  //現在表示しているチャージエフェクト

    //--------タイマー関連---------
    private Coroutine currentCoroutine; //現在計測しているコルーチン

    private bool CreateBallTrigger = false; //弾を生成したトリガー
    private Vector3 CreateVec;              //生成する向き

   

    private Transform PlayerTrans;      //プレイヤーの位置

    //ドローンの状態
    private enum DrawnState
    {
        IDEL,       //待機
        TRACKING,   //追跡
        FLEEING,    //逃走
        CHARGE, //チャージ
        ATTACK, //攻撃
        DETH    //死亡
    }

    [SerializeField]private DrawnState state;   //現在の状態
    private Vector3 CurrentTargetPos;   //現在の目標位置
    private Rigidbody ThisRb;   //自分のRigitBody
  
    //攻撃状態
    private enum DrawnAttackState
    {
        NONE,               //何もしていない
        VIEWLINE,           //予測線表示
        ATTACKWAIT,         //攻撃待機
        NEXTATTACKSPACE,    //次の攻撃までの間隔
    }

    [SerializeField] private DrawnAttackState attackstate;


    // Start is called before the first frame update
    void Start()
    {
        //HPを保存
        NowHP = HP;

        //始点と終点を初期化
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position); // 最初は始点のみ

        //状態を初期化
        state = DrawnState.IDEL;
        attackstate = DrawnAttackState.NONE;

        //RigitBodyを取得
        TryGetComponent<Rigidbody>(out ThisRb);

        //親からマネージャーを取得
        transform.parent.TryGetComponent<CS_EnemyManager>(out EnemyManager);

        //プレイヤーの状態をスポナーから取得
        PlayerTrans = EnemyManager.GetPlayerTrans();


        //自前の移動を行うためにAgentの自動更新を無効化
        agent.updatePosition = false;
        agent.updateRotation = false;

        currentCoroutine = null;

        //目標を設定
        SetTarget(PlayerTrans.position);
    }

    /// <summary>
    /// 目標設定
    /// </summary>
    /// <param 目標座標="pos"></param>
    private void SetTarget(Vector3 pos)
    {
        NavMeshHit hit;
        bool Target = NavMesh.SamplePosition(pos, out hit, 100, NavMesh.AllAreas);
        if (Target)
        {
            agent.SetDestination(pos);
            CurrentTargetPos = pos; //現在の目標を設定
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        path = agent.path.corners;

       
        //HPゲージの処理
        HPGage.fillAmount = NowHP / HP;
        HPCanvas.transform.LookAt(PlayerTrans);
        if (HPCanvas.activeSelf)
        {
            StartCoroutine(EndViewHP());
        }

        ActionTable();

        //死んでたら処理しない
        if (state == DrawnState.DETH) { return; }

        //-----プレイヤーとの距離によって行動-----
        //距離を求める(高さは考慮しない)
        Vector3 targetpos = new Vector3(PlayerTrans.position.x, 0, PlayerTrans.position.z);
        Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
        float Distance = Vector3.Distance(pos, targetpos);
        bool Tracking = Distance >= TrackingDistance;    //指定距離プレイヤーから離れたら追跡
        bool RanAway = Distance <= RunAwayDistance;      //プレイヤーが近づいたら逃げる

        //一定距離まで追跡
        if (Tracking && (state == DrawnState.IDEL || attackstate == DrawnAttackState.NEXTATTACKSPACE)) { ChangeState(DrawnState.TRACKING); }
        //追跡中に一定距離まで近づいたらチャージ
        if(!Tracking && state == DrawnState.TRACKING) { ChangeState(DrawnState.CHARGE); }
        
        //逃走(チャージ時と攻撃時は逃げない？)
        if (RanAway && (state != DrawnState.ATTACK || state != DrawnState.CHARGE )) { ChangeState(DrawnState.FLEEING); }


        //Y軸のみ常にプレイヤーの近くを飛行
        {
            // Y軸方向の移動計算
            float targetY = PlayerTrans.position.y + ApploachY;        //プレイヤーのY座標
            float currentY = transform.position.y;              //敵の現在のY座標

            float yDistance = Mathf.Abs(targetY - currentY);        // 距離を計算
            float ySpeed = 0f;
            //近づけてない時だけ更新
            if (yDistance > 0.1f)
            {
                //固定解除
                ThisRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
                float yDirection = Mathf.Sign(targetY - currentY); // 上下方向を計算
                ySpeed = yDirection * MoveSpeed; // Y方向の移動速度を設定
            }
            else
            {
                //Y軸固定
                ThisRb.constraints = RigidbodyConstraints.FreezePositionY;
            }

            ThisRb.velocity = new Vector3(ThisRb.velocity.x, ySpeed, ThisRb.velocity.z);
        }
        if (state != DrawnState.ATTACK || state != DrawnState.CHARGE) { Move(); }//移動
    }


    /// <summary>
    /// 自前の移動処理
    /// </summary>
    private void Move()
    {
        if(agent.path.corners.Length < 2) { return; }

        //チャージ中と攻撃中は動かない
        if ((attackstate == DrawnAttackState.NEXTATTACKSPACE && state == DrawnState.ATTACK) ||
            (attackstate != DrawnAttackState.NEXTATTACKSPACE && (state == DrawnState.CHARGE || state == DrawnState.ATTACK)))
        {
            //プレイヤーの向きを向く
            if (attackstate != DrawnAttackState.ATTACKWAIT){ transform.LookAt(PlayerTrans); }

            ThisRb.velocity = Vector3.zero;
            return; 
        }

        Vector3 currentPosition = transform.position;
        Vector3 nextWaypoint = agent.path.corners[CurrentPathNum];  // 最初の移動先
        Vector3 direction = (nextWaypoint - currentPosition);

        // 敵キャラクターを進行方向に移動させる
        float forward_x = transform.forward.x * MoveSpeed;
        float forward_z = transform.forward.z * MoveSpeed;

        // Y軸方向の移動計算
        float targetY = PlayerTrans.position.y + ApploachY;        //プレイヤーのY座標
        float currentY = transform.position.y;              //敵の現在のY座標
        float yDirection = Mathf.Sign(targetY - currentY);  //上下方向を計算

        float ySpeed = yDirection * MoveSpeed; // Y方向の移動速度を設定（調整可能）

        ThisRb.velocity = new Vector3(forward_x, ThisRb.velocity.y, forward_z);

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
        Debug.Log(dis);
        if (dis < 1.5f)
        {
            // 次の経路点に進むためにインデックスを更新
            if (CurrentPathNum < agent.path.corners.Length - 1)
            {
                CurrentPathNum++;  // インデックスを進める
            }
          
        }
        //else
        //{
        //    Debug.Log(currentPosition + ""+ nextWaypoint);
        //}

    }

    /// <summary>
    /// 状態遷移
    /// </summary>
    /// <param 状態="_state"></param>
    private void ChangeState(DrawnState _state)
    {
        //同じ状態なら更新しない
        if(state == _state) { return; }

        if(CurrentChargeEffect != null) { Destroy(CurrentChargeEffect); }

        //時間計測中だったら止める
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        SetTarget(transform.position);
        //agent.SetDestination(transform.position);

        //予測線の初期化
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);

        //予測線開始色に色変更
        lineRenderer.startColor = TargetColor;
        lineRenderer.endColor = TargetColor;

        //攻撃状態の初期化
        if (CreateBallTrigger)
        {
            CreateBallTrigger = false;
            progress = 0;
            ChangeAttackState(DrawnAttackState.NONE);
        }
        state = _state;

    }

    /// <summary>
    /// アクションテーブル
    /// </summary>
    private void ActionTable()
    {
        anim.SetBool("Idle", state == DrawnState.IDEL);

        switch (state)
        {
            case DrawnState.IDEL:       //待機(行動選択中)
                break;
            case DrawnState.TRACKING:   //追跡
                Tracking();
                break;
            case DrawnState.FLEEING:    //逃走
                RanAway();
                break;
            case DrawnState.CHARGE:     //チャージ
                ViewLine(PlayerTrans.position);
                break;
            case DrawnState.ATTACK:     //攻撃
                Attack();
                break;
            case DrawnState.DETH:       //死亡
                DETH();
                break;
        }

    }

    /// <summary>
    /// 逃走
    /// </summary>
    private void RanAway()
    {

        //逃げる方向を計算
        Vector3 fleeDirection = (transform.position - PlayerTrans.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * MaxRanAwayDistance;

        // NavMesh上の適切なポイントを計算
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, MaxRanAwayDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        //時間待つ
        if (currentCoroutine != null) { return; }
        currentCoroutine = StartCoroutine(Timer(MaxRanAwayTime,() =>
        {
            
            ChangeState(DrawnState.IDEL);
        }));

       

    }

    /// <summary>
    /// 追跡
    /// </summary>
    private void Tracking()
    {
        if(agent.destination == PlayerTrans.position) { return; }
        //目標を設定
        SetTarget(PlayerTrans.position);
        //agent.SetDestination(PlayerTrans.position);
    }

    /// <summary>
    /// 予測線表示(チャージ)
    /// </summary>
    /// <param 射出先="TargetPos"></param>
    /// false:処理中　true:終了 <returns></returns>
    private void ViewLine(Vector3 TargetPos)
    {

        float LineOffsetY = 1f;

        if(CurrentChargeEffect != null)
        {
            Vector3 pos = transform.position;
            pos.y += 1.5f;
            CurrentChargeEffect.transform.position = pos;
        }

        switch(attackstate)
        {
            case DrawnAttackState.NONE:

                //予測線の表示
                Vector3 startpos = transform.position;
                startpos.y += 1.5f; //モデルのずれ
                lineRenderer.SetPosition(0,startpos);    // 始点

                //線を射出する形で表示
                if (progress < 1f)
                {
                    progress += ForecastLineViewSpeed * Time.deltaTime;
                    //現在の位置を補間計算
                    Vector3 currentEndPoint = Vector3.Lerp(transform.position, TargetPos, progress);
                    currentEndPoint.y += LineOffsetY;    //ちょっと高さ出して
                                                  //LineRenderer の終点を更新
                    lineRenderer.SetPosition(1, currentEndPoint);

                    return;
                }
                else
                {
                    Vector3 pos = transform.position;
                    pos.y += 1f;
                    CurrentChargeEffect = Instantiate(ChargeEffect, pos, Quaternion.identity);
                    ChangeAttackState(DrawnAttackState.VIEWLINE);
                }

                break;
            case DrawnAttackState.VIEWLINE:             //予測線表示
                
                TargetPos.y += LineOffsetY;
                lineRenderer.SetPosition(1, TargetPos);

                if (currentCoroutine != null) { return; }
                currentCoroutine = StartCoroutine(Timer(ViewForecastLineChangeTime, () =>
                 {
                     //攻撃待機中に色変更
                     lineRenderer.startColor = AttackWaitColor;
                     lineRenderer.endColor = AttackWaitColor;
                     CreateVec = PlayerTrans.position;  //生成方向を確定
                     CreateVec.y += 0.5f;
                     ChangeAttackState(DrawnAttackState.ATTACKWAIT);
                 }));
                break;
            case DrawnAttackState.ATTACKWAIT:           //攻撃待機

                if (currentCoroutine != null) { return; }
                currentCoroutine = StartCoroutine(Timer(AttackWaitTime, () =>
                {
                   
                    progress = 0f;                  //割合初期化
                    ChangeState(DrawnState.ATTACK);
                }));

                break;
            case DrawnAttackState.NEXTATTACKSPACE:      //次の攻撃まで待機
                //ATTACK関数まで
                break;

        }

    }

    /// <summary>
    /// 攻撃
    /// </summary>
    private void Attack()
    {
        if(attackstate != DrawnAttackState.ATTACKWAIT) { return; }

        CreateBallTrigger = false;

        //自分の前方に生成位置を計算
        Vector3 spawnPosition = transform.position + transform.forward * CreateBallDistance;

        spawnPosition.y += 1.5f;    //モデルのずれ

        //目標の方向を計算(プレイヤー)
        Vector3 directionToTarget = (CreateVec - spawnPosition).normalized;

        //生成するオブジェクトの回転を計算
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
        
        if (CurrentChargeEffect != null) { Destroy(CurrentChargeEffect); }


        //SE再生
        PlaySE(0, false);
       
        //弾オブジェクトを生成
        GameObject ball = Instantiate(BallObj, spawnPosition, rotationToTarget);
        //弾に速度低下を付与
        ball.TryGetComponent<CS_AirBall>(out CS_AirBall airball);
        if (airball) { airball.SetSpeedDown(MoveSpeedDownTime, MoveSpeedDownParsentage); }

        CreateBallTrigger = true;

        //表示していた予測線を消す
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        //生成終了したら次の攻撃まで待機
        ChangeAttackState(DrawnAttackState.NEXTATTACKSPACE);

        //攻撃待機、すみません見にくくて
        if (attackstate == DrawnAttackState.NEXTATTACKSPACE)
        {
            //予測線は常に自分の所
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);

            if (currentCoroutine != null) { return; }
            currentCoroutine = StartCoroutine(Timer(AttackSpace, () =>
            {
                //予測線開始色に色変更
                lineRenderer.startColor = TargetColor;
                lineRenderer.endColor = TargetColor;
                ChangeAttackState(DrawnAttackState.NONE);
                ChangeState(DrawnState.CHARGE);
            }));
        }


    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void DETH()
    {

        //Y軸固定を解除して落とす
        if ((ThisRb.constraints & RigidbodyConstraints.FreezePositionY) != 0)
        {
            
            ThisRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        }
    }

    /// <summary>
    /// 死亡後消去　アニメーションで呼び出し
    /// </summary>
    private void DethDestroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //死んでたら無視
        if(state == DrawnState.DETH) { return; }
        bool AttackHit = other.tag == "Attack";
       
        //弾に当たったらHP減少
        if(AttackHit)
        {
            PlaySE(2, false);
            other.TryGetComponent<CS_AirBall>(out CS_AirBall ball);
            
            if (!ball) { return; }
            if (ball.GetEnemyType()) { return; }    //敵からの弾なら無視
            NowHP -= ball.Power;

            //死亡
            if(NowHP <= 0) 
            {
                //缶を生成
                for(int i = 0; i<DethCanNum;i++)
                {
                    Vector3 pos = transform.position;
                    pos.y += 1.5f;
                    Instantiate(Can,pos,Quaternion.identity);
                }
                ChangeState(DrawnState.DETH);
                anim.SetBool("Deth", true);
                PlaySE(1, false);
            }
        }

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.transform.tag == transform.tag) { return; }
    //    //どっかにあったったら退避
    //    //新しい経路を設定
    //    Vector3 newTarget = transform.position + Vector3.back * 1f; // 後ろに退避
    //    SetTarget(newTarget);

    //    //死んでてどっかに当たったら消去
    //    if (state == DrawnState.DETH) { Destroy(this.gameObject); }
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    //どっかにあったったら退避
    //    //新しい経路を設定
    //    if (collision.transform.tag != "Enemy")
    //    {
    //        Vector3 newTarget = transform.position + Vector3.back * 1f; // 後ろに退避

    //        SetTarget(newTarget);
    //    }
    //}

    /// <summary>
    /// 攻撃状態の変更
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeAttackState(DrawnAttackState newState)
    {
        if(attackstate == newState) { return; }
        attackstate = newState;
        currentCoroutine = null; // 次の状態で新しいコルーチンを開始できるようにする
    }

    /// <summary>
    /// タイマー
    /// </summary>
    /// <param 時間="time"></param>
    /// <param 待機後の処理="onComplete"></param>
    /// <returns></returns>

    private IEnumerator Timer(float time, System.Action onComplete)
    {
        //指定した秒数待つ
        yield return new WaitForSeconds(time);

        onComplete?.Invoke();
        currentCoroutine = null;
    }



    /// <summary>
    /// HPを表示するコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        //再び非表示に
        HPCanvas.SetActive(false);

    }

    /// <summary>
    /// HPゲージの表示
    /// </summary>
    public void ViewHPGage(Transform PlayerTrans)
    {
        HPCanvas.transform.LookAt(PlayerTrans);
        HPCanvas.SetActive(true);
    }


    //SE再生用
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



}
