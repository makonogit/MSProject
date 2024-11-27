using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraftItemBase : MonoBehaviour
{
    public bool isSetUp;// �ݒu�����t���O
    public bool GetSetUp() {  return isSetUp; }

    private CS_InputSystem inputSystem;     // �C���v�b�g�V�X�e��
    public CS_InputSystem GetInputSystem() => inputSystem;

    protected virtual void Start()
    {
        inputSystem = GameObject.Find("InputSystem")?.GetComponent<CS_InputSystem>();
        if (inputSystem == null)
        {
            UnityEngine.Debug.LogError("InputSystem��Hierarchy�ɒǉ����Ă��������B");
        }
    }
}
