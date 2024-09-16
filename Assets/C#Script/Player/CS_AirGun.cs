//------------------------------------------
// 空気砲/直刺しの関数定義
// あとでプレイヤーに合成
//------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_AirGun : MonoBehaviour
{
    [SerializeField, Header("空気砲の攻撃力")]
    private float AirAttackPowar = 1.0f;

    [SerializeField, Header("空気砲の弾オブジェクト")]
    private GameObject AirBall;

    [SerializeField, Header("直刺しの注入間隔")]
    [Header("※攻撃力/注入間隔")]
    private const float Injection_Interval = 0.5f;

    private const float MaxPressure = 3.0f; //最大圧力

    //注入間隔計算用
    private float Injection_IntarvalTime = 0.0f;

    private void FixedUpdate()
    {
        //やってみる
        AirGun(KeyCode.E, false);
    }

    //----------------------------
    // 空気砲関数
    // 引数:入力キー,オブジェクトに近づいているか
    // 戻り値：なし
    //----------------------------
    void AirGun(KeyCode key, bool ObjDistance)
    {
        //発射可能か(キーが押された瞬間&オブジェクトに近づいていない)
        bool StartShooting = Input.GetKeyDown(key) && !ObjDistance;
        
        if (!StartShooting) { return; }

        //入力があれば弾を生成
        //ポインタの位置から　Instantiate(AirBall,transform.pointa);
        GameObject ballobj = Instantiate(AirBall);

    }

    //----------------------------
    // 直刺し(空気注入)関数
    // 引数:入力キー,近づいているか,近づいているオブジェクトの圧力,近づいているオブジェクトの耐久値
    // 戻り値：なし
    //----------------------------
    void AirInjection(KeyCode key,bool ObjDistance,float ObjPressure,float ObjDurability)
    {
        //注入可能か(キーが入力されていてオブジェクトに近づいている)
        bool Injection = Input.GetKey(key) && ObjDistance;
        
        if (!Injection) { return; }

        //時間計測
        Injection_IntarvalTime += Time.deltaTime;
        bool TimeProgress = Injection_IntarvalTime > Injection_Interval;   //注入間隔分時間経過しているか
        if (!TimeProgress) { return; }

        Injection_IntarvalTime = 0.0f;  //時間をリセット

        bool StartInjection = ObjPressure > MaxPressure;                   //攻撃開始か(圧力が最大か)

        //時間経過したら攻撃力を追加
        if (!StartInjection) { ObjPressure += AirAttackPowar * Time.deltaTime; }

        //圧力が最大になれば耐久値を減少させる
        ObjDurability -= AirAttackPowar * Time.deltaTime; 

    }


}
