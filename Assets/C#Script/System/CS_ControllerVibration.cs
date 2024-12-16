using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * コントローラー振動　担当：藤原昂祐
 */
public class CS_ControllerVibration : MonoBehaviour
{
    private static Gamepad gamepad;

    [SerializeField,Header("振動の周波数の種類")]
    private AnimationCurve[] curve;
    private static AnimationCurve[] vibrationCurve;

    [SerializeField, Header("振動の強さの種類")]
    private float[] power;
    private static float[] vibrationPower;

    // ループ状態
    private static bool isLoop = false;
    public static void SetLoop(bool flg) { isLoop = flg; }
    public static bool GetLoop() {  return isLoop; }

    private void Start()
    {
        // ゲームパッドが接続されているか確認
        if(gamepad == null)
        {
            gamepad = Gamepad.current;
        }

        if (vibrationCurve == null)
        {
            // 振動カーブをコピー
            vibrationCurve = new AnimationCurve[curve.Length];
            for (int i = 0; i < curve.Length; i++)
            {
                vibrationCurve[i] = new AnimationCurve(curve[i].keys);
            }
        }

        if(vibrationPower == null)
        {
            // 振動強さをコピー
            vibrationPower = new float[power.Length];
            power.CopyTo(vibrationPower, 0);
        }
    }

    /*
     * コントローラーを振動させる関数
     */
    public static void StartVibrationWithCurve(
            float duration,         // 振動の長さ
            int powerType,          // 振動の強さ（4段階）
            int curveType,          // 振動の周波数
            int repetition          // 繰り返し回数
        )
    {
        // コルーチンを開始
        GameObject vibrationObject = new GameObject();
        MonoBehaviour.Instantiate(vibrationObject).AddComponent<CS_ControllerVibration>().StartCoroutine(StartVibrationCoroutine(duration, powerType, curveType, repetition, vibrationObject));
    }

    // 振動の強さを時間で補間するコルーチン
    private static IEnumerator StartVibrationCoroutine(
            float duration,         // 振動の長さ
            int powerType,          // 振動の強さ（4段階）
            int curveType,          // 振動の周波数
            int repetition,         // 繰り返し回数
            GameObject vibrationObject // 作成したGameObjectの参照
        )
    {
        if (curveType >= 0 && curveType < vibrationCurve.Length)
        {
            curveType = vibrationCurve.Length - 1;
        }
        if (powerType >= 0 && powerType < vibrationPower.Length)
        {
            powerType = vibrationPower.Length - 1;
        }

        float elapsedTime = 0f; // 経過時間

        // 繰り返し回数だけ実行
        for (int i = 0; (i < repetition || isLoop); i++)
        {
            // 時間の経過に従って振動強さを補間
            while (elapsedTime < duration)
            {
                // 時間に基づいて曲線を評価
                float t = elapsedTime / duration; // 進行度 (0.0f から 1.0f)
                float vibrationStrength = Mathf.Clamp01(vibrationCurve[curveType - 1].Evaluate(t)); // 曲線に基づく振動強さ

                // 上限を適用
                vibrationStrength = Mathf.Min(vibrationStrength, vibrationPower[powerType - 1]);

                // 振動の強さを設定
                if (gamepad != null)
                {
                    gamepad.SetMotorSpeeds(vibrationStrength, vibrationStrength);
                }

                // 経過時間を更新
                elapsedTime += Time.deltaTime;

                // 次のフレームまで待機
                yield return null;
            }

            // 終了時に振動を停止
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
                elapsedTime = 0f;
            }
        }

        // コルーチンが終了したらオブジェクトを破棄
        Destroy(vibrationObject);
    }
}



