using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CraftItemBase : MonoBehaviour
{
    public bool isSetUp;// 設置完了フラグ
    public bool GetSetUp() {  return isSetUp; }

    private CS_InputSystem inputSystem;     // インプットシステム
    public CS_InputSystem GetInputSystem() => inputSystem;

    protected virtual void Start()
    {
        inputSystem = GameObject.Find("InputSystem")?.GetComponent<CS_InputSystem>();
        if (inputSystem == null)
        {
            UnityEngine.Debug.LogError("InputSystemをHierarchyに追加してください。");
        }
    }
}
