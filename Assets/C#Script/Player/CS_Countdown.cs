using UnityEngine;

public class CS_Countdown : MonoBehaviour
{
    private float countdownTime;

    // コンストラクタは使用しないが、初期化メソッドを作成
    public void Initialize(float time)
    {
        countdownTime = time;
        StartCoroutine(StartCountdown());
    }

    private System.Collections.IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime; // フレーム毎に時間を減少
            yield return null; // 次のフレームまで待機
        }
    }

    public bool IsCountdownFinished()
    {
        return countdownTime <= 0;
    }
}

