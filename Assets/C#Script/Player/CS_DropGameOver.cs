using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.C_Script.GameEvent;

public class CS_DropGameOver : MonoBehaviour
{
    [SerializeField, Header("�J�����}�l�[�W���[")]
    private CS_CameraManager cameraManager;     // �J�����}�l�[�W���[
    [SerializeField, Header("TPS�J����")]
    private GameObject tpsCamera;               // TPS�J����
    [SerializeField, Header("�������J����")]
    private GameObject fallingCamera;           // �������J����
    [SerializeField, Header("�J�ڂ܂ł̑ҋ@����")]
    private float delay = 0.5f;           // �J�ڂ܂ł̑ҋ@����


    private CS_Countdown countdown; // �J�E���g�_�E���I�u�W�F�N�g

    private bool isGameOver = false; //�Q�[���I�[�o�[�t���O

    // Start is called before the first frame update
    void Start()
    {
        countdown = gameObject.AddComponent<CS_Countdown>();
    }

    void FixedUpdate()
    {
        if (isGameOver && countdown.IsCountdownFinished())
        {
            CSGE_GameOver.GameOver();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // ���C���J�����̈ʒu�����g�ɐݒ�
            fallingCamera.transform.position = tpsCamera.transform.position;
            fallingCamera.transform.rotation = tpsCamera.transform.rotation;

            // �J�����̐؂�ւ�
            cameraManager.SwitchingCamera(1);

            // �Q�[���I�[�o�[��ԂɕύX
            isGameOver = true;

            // �ҋ@���ăQ�[���I�[�o�[�ɑJ��
            countdown.Initialize(delay);
        }
    }
}
