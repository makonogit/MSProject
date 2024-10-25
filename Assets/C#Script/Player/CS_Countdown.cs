using UnityEngine;

public class CS_Countdown : MonoBehaviour
{
    private float countdownTime;

    // �R���X�g���N�^�͎g�p���Ȃ����A���������\�b�h���쐬
    public void Initialize(float time)
    {
        countdownTime = time;
        StartCoroutine(StartCountdown());
    }

    private System.Collections.IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime; // �t���[�����Ɏ��Ԃ�����
            yield return null; // ���̃t���[���܂őҋ@
        }
    }

    public bool IsCountdownFinished()
    {
        return countdownTime <= 0;
    }
}

