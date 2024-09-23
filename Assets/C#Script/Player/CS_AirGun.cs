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
        //AirGun(KeyCode.E, false);
    }


    //**
    //* 音声ファイルが再生されているか
    //*
    //* in：無し
    //* out:再生されているか bool
    //**

    //bool IsPlayingSound(int indexSource)
    //{
    //    return audioSource[indexSource].isPlaying;
    //}


    //----------------------------
    // 空気砲関数
    // 引数:入力キー,オブジェクトに近づいているか
    // 戻り値：なし
    //----------------------------
    void AirGun(string button)
    {
        //発射可能か(キーが押された瞬間&オブジェクトに近づいていない)
        bool StartShooting = Input.GetButtonDown(button); //&& !HitBurstObjFlag;

        if (!StartShooting) { return; }

        //SEが再生されていたら止める
        //if (IsPlayingSound(1)) { StopPlayingSound(1); }

        //PlaySoundEffect(1, 3);

        Vector3 forwardVec = transform.forward; // = cameraTransform.forward;

        //入力があれば弾を生成
        //ポインタの位置から　Instantiate(AirBall,transform.pointa);
        GameObject ballobj = Instantiate(AirBall);

        Vector3 pos = Vector3.zero;
        float scaler = 2.0f;
        Vector3 offset = new Vector3(0, 1, 0);

        pos = this.transform.position;
        pos += offset;
        pos += forwardVec * scaler;
        ballobj.transform.position = pos;
        ballobj.transform.forward = forwardVec;

    }


    //----------------------------
    // 直刺し(空気注入)関数
    // 引数:入力キー
    // 戻り値：なし
    //----------------------------
    void AirInjection(string button)
    {

        ////注入可能か(キーが入力されていてオブジェクトに近づいている)
        //bool Injection = Input.GetButtonDown(button) && HitBurstObjFlag;

        //if (!Injection)
        //{
        //    return;
        //}
        //else
        //{
        //    StopPlayingSound(1);    //音が鳴っていたら止める
        //    PlaySoundEffect(1, 4);  //挿入SE
        //    InjectionState = true;  //注入中のフラグをOn
        //}

        ////注入中じゃなければ終了
        //if (!InjectionState)
        //{
        //    return;
        //}

        //PlaySoundEffect(1, 6);  //挿入SE

        ////ボタンが離された or 対象が消滅したら終了
        //InjectionState = !(Input.GetButtonUp(button) || !csButstofObj);

        ////時間計測
        //Injection_IntarvalTime += Time.deltaTime;
        //bool TimeProgress = Injection_IntarvalTime > Injection_Interval;   //注入間隔分時間経過しているか
        //if (!TimeProgress) { return; }

        //Injection_IntarvalTime = 0.0f;  //時間をリセット

        //if (!csButstofObj)
        //{
        //    Debug.LogWarning("null");
        //    return;
        //}

        ////圧力が最大になったら↓
        //bool MaxPressure = true;
        //csButstofObj.AddPressure(InjectionPower);

        ////最大になったら注入終了
        //if (MaxPressure)
        //{
        //    InjectionState = false;
        //}
    }

}
