using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// InputSystem��c#�ň������߂̃X�N���v�g
public class CS_InputSystem : MonoBehaviour
{
    //**
    //* �C���v�b�g�V�X�e��
    //**

    // �C���X�^���X
    private InputSystem inputSystem;

    private void Awake()
    {
        // �A�N�V�����A�Z�b�g���琶�����ꂽ�N���X���C���X�^���X��
        inputSystem = new InputSystem();
    }

    private void OnEnable()
    {
        // �A�N�V�����}�b�v��L����
        inputSystem.Enable();
    }

    private void OnDisable()
    {
        // �A�N�V�����}�b�v�𖳌���
        inputSystem.Disable();
    }

    // Dpad
    public bool IsDpadUpPressed() => inputSystem.Controller.Dpad_up.ReadValue<float>() > 0.1f;
    public bool IsDpadDownPressed() => inputSystem.Controller.Dpad_down.ReadValue<float>() > 0.1f;
    public bool IsDpadRightPressed() => inputSystem.Controller.Dpad_right.ReadValue<float>() > 0.1f;
    public bool IsDpadLeftPressed() => inputSystem.Controller.Dpad_left.ReadValue<float>() > 0.1f;

    public bool IsDpadUpTriggered() => inputSystem.Controller.Dpad_up.triggered;
    public bool IsDpadDownTriggered() => inputSystem.Controller.Dpad_down.triggered;
    public bool IsDpadRightTriggered() => inputSystem.Controller.Dpad_right.triggered;
    public bool IsDpadLeftTriggered() => inputSystem.Controller.Dpad_left.triggered;

    // �g���K�[
    public float GetLeftTrigger() => inputSystem.Controller.Trigger_L.ReadValue<float>();
    public float GetRightTrigger() => inputSystem.Controller.Trigger_R.ReadValue<float>();

    // �{�^��
    public bool IsButtonAPressed() => inputSystem.Controller.Button_A.ReadValue<float>() > 0.1f;
    public bool IsButtonBPressed() => inputSystem.Controller.Button_B.ReadValue<float>() > 0.1f;
    public bool IsButtonYPressed() => inputSystem.Controller.Button_Y.ReadValue<float>() > 0.1f;
    public bool IsButtonXPressed() => inputSystem.Controller.Button_X.ReadValue<float>() > 0.1f;

    public bool IsButtonATriggered() => inputSystem.Controller.Button_A.triggered;
    public bool IsButtonBTriggered() => inputSystem.Controller.Button_B.triggered;
    public bool IsButtonYTriggered() => inputSystem.Controller.Button_Y.triggered;
    public bool IsButtonXTriggered() => inputSystem.Controller.Button_X.triggered;

    // �X�e�B�b�N
    public bool IsLeftStickActive(float min)
    {
        var input = inputSystem.Controller.Stick_L.ReadValue<Vector2>();
        return Mathf.Abs(input.x) > min || Mathf.Abs(input.y) > min;
    }

    public bool IsRightStickActive(float min)
    {
        var input = inputSystem.Controller.Stick_R.ReadValue<Vector2>();
        return Mathf.Abs(input.x) > min || Mathf.Abs(input.y) > min;
    }

    public Vector2 GetLeftStick() => inputSystem.Controller.Stick_L.ReadValue<Vector2>();
    public Vector2 GetRightStick() => inputSystem.Controller.Stick_R.ReadValue<Vector2>();

}
