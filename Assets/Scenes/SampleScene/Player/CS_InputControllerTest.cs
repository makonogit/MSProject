using UnityEngine;

// �R���g���[���[���͂̊m�F�p
public class CS_InputControllerTest : MonoBehaviour
{
    //**
    //* �X�V
    //**
    void Update()
    {
        // �R���g���[���[���͂��`�F�b�N���ă��O�ɏo�͂���

        // �X�e�b�N
        LogStickInputs();
        // �\���{�^��
        LogDPadInputs();
        // �g���K�[
        LogTriggerInputs();
        // �{�^��
        LogButtonInputs();
    }

    //**
    //* �X�e�B�b�N�̓��͂����O�ɏo�͂���
    //**
    void LogStickInputs()
    {
        // ���X�e�B�b�N
        float lsh = Input.GetAxis("LStick X");
        float lsv = Input.GetAxis("LStick Y");
        if (lsh != 0 || lsv != 0)
        {
            Debug.Log("L stick: " + lsh + ", " + lsv);
        }

        // �E�X�e�B�b�N
        float rsh = Input.GetAxis("RStick X");
        float rsv = Input.GetAxis("RStick Y");
        if (rsh != 0 || rsv != 0)
        {
            Debug.Log("R stick: " + rsh + ", " + rsv);
        }
    }

    //**
    //* D-Pad�̓��͂����O�ɏo�͂���
    //**
    void LogDPadInputs()
    {
        float dph = Input.GetAxis("DPad X");
        float dpv = Input.GetAxis("DPad Y");
        if (dph != 0 || dpv != 0)
        {
            Debug.Log("D Pad: " + dph + ", " + dpv);
        }
    }

    //**
    //* �g���K�[�̓��͂����O�ɏo�͂���
    //**
    void LogTriggerInputs()
    {
        float tri = Input.GetAxis("LRTrigger");
        if (tri > 0)
        {
            Debug.Log("L trigger: " + tri);
        }
        else if (tri < 0)
        {
            Debug.Log("R trigger: " + tri);
        }
        else
        {
            Debug.Log("  trigger: none");
        }
    }

    //**
    //* �{�^���̓��͂����O�ɏo�͂���
    //**
    void LogButtonInputs()
    {
        if (Input.GetButtonDown("ButtonA"))
        {
            Debug.Log("A");
        }
        if (Input.GetButtonDown("ButtonB"))
        {
            Debug.Log("B");
        }
        if (Input.GetButtonDown("ButtonY"))
        {
            Debug.Log("Y");
        }
        if (Input.GetButtonDown("ButtonX"))
        {
            Debug.Log("X");
        }
        if (Input.GetButtonDown("ButtonL"))
        {
            Debug.Log("L");
        }
        if (Input.GetButtonDown("ButtonR"))
        {
            Debug.Log("R");
        }
    }

}