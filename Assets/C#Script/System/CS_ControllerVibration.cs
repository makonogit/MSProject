using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * �R���g���[���[�U���@�S���F�����V�S
 */
public class CS_ControllerVibration : MonoBehaviour
{
    private static Gamepad gamepad;

    //[SerializeField,Header("�U���̎��g���̎��")]
    //private AnimationCurve[] curve;
    //private static AnimationCurve[] vibrationCurve;

    [SerializeField, Header("�U���̋����̎��")]
    private float[] power;
    private static float[] vibrationPower;

    // ���[�v���
    private static bool isLoop = false;
    public static void SetLoop(bool flg) { isLoop = flg; }
    public static bool GetLoop() {  return isLoop; }

    private static bool isAllStop = false;

    private void Start()
    {
        // �Q�[���p�b�h���ڑ�����Ă��邩�m�F
        if(gamepad == null)
        {
            gamepad = Gamepad.current;
        }

        //if (vibrationCurve == null)
        //{
        //    // �U���J�[�u���R�s�[
        //    vibrationCurve = new AnimationCurve[curve.Length];
        //    for (int i = 0; i < curve.Length; i++)
        //    {
        //        vibrationCurve[i] = new AnimationCurve(curve[i].keys);
        //    }
        //}

        if(vibrationPower == null)
        {
            // �U���������R�s�[
            vibrationPower = new float[power.Length];
            power.CopyTo(vibrationPower, 0);
        }
    }

    // �|�[�Y��ʒ��͍Đ����~
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (!isAllStop)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
                isAllStop = true;
            }
        }
        else
        {
            isAllStop = false;
        }
    }

    /*
     * �R���g���[���[��U��������֐�
     */
    public static void StartVibrationWithCurve(
            float duration,         // �U���̒���
            int powerType,          // �U���̋����i4�i�K�j
            AnimationCurve curve,   // �U���̎��g��
            int repetition          // �J��Ԃ���
        )
    {
        // �R���[�`�����J�n
        GameObject vibrationObject = new GameObject();
        MonoBehaviour.Instantiate(vibrationObject).AddComponent<CS_ControllerVibration>().StartCoroutine(StartVibrationCoroutine(duration, powerType, curve, repetition, vibrationObject));
    }

    // �U���̋��������Ԃŕ�Ԃ���R���[�`��
    private static IEnumerator StartVibrationCoroutine(
            float duration,         // �U���̒���
            int powerType,          // �U���̋����i4�i�K�j
            AnimationCurve curve,   // �U���̎��g��
            int repetition,         // �J��Ԃ���
            GameObject vibrationObject // �쐬����GameObject�̎Q��
        )
    {
        //if (curveType >= 0 && curveType < vibrationCurve.Length)
        //{
        //    curveType = vibrationCurve.Length - 1;
        //}
        if (powerType >= 0 && powerType < vibrationPower.Length)
        {
            powerType = vibrationPower.Length - 1;
        }

        float elapsedTime = 0f; // �o�ߎ���

        // �J��Ԃ��񐔂������s
        for (int i = 0; (i < repetition || isLoop); i++)
        {
            if (isAllStop)
            {
                break;
            }

            // ���Ԃ̌o�߂ɏ]���ĐU����������
            while (elapsedTime < duration)
            {
                // ���ԂɊ�Â��ċȐ���]��
                float t = elapsedTime / duration; // �i�s�x
                float vibrationStrength = Mathf.Clamp01(curve.Evaluate(t));
                vibrationStrength = Mathf.Min(vibrationStrength, vibrationPower[powerType - 1]);

                // �U���̋�����ݒ�
                if (gamepad != null)
                {
                    gamepad.SetMotorSpeeds(vibrationStrength, vibrationStrength);
                }

                // �o�ߎ��Ԃ��X�V
                float deltaTime = Time.unscaledDeltaTime;
                elapsedTime += deltaTime;

                // ���̃t���[���܂őҋ@
                yield return null;
            }

            // �I�����ɐU�����~
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
                elapsedTime = 0f;
            }
        }

        // �R���[�`�����I��������I�u�W�F�N�g��j��
        // ���񓯊��^�X�N����Destroy���Ă���̂�Hierarchy���疼�O�������Ȃ����Ƃ�����
        Destroy(vibrationObject);
    }
}



