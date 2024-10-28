using UnityEngine;

public class CS_Countdown : MonoBehaviour
{
    private float countdownTime;

    public void Initialize(float time)
    {
        if(countdownTime <= 0)
        {
            countdownTime = time;
            StartCoroutine(StartCountdown());
        }
    }

    private System.Collections.IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            yield return null;
        }
    }

    public bool IsCountdownFinished()
    {
        return countdownTime <= 0;
    }
}

