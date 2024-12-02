using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 担当：菅　破壊オブジェクトを取得
/// </summary>
public class CS_BreakSearch : MonoBehaviour
{

    [SerializeField, Header("破壊システム")]
    private CS_Break BreakSystem;

    // Start is called before the first frame update
    void Start()
    {
        List<GameObject> allChildren = GetAllChildren(transform);
        List<GameObject> BreakObjList = new List<GameObject>();

        foreach (GameObject child in allChildren)
        {
            if (child.name == "BreakArea")
            {
                BreakObjList.Add(child);
            }

        }

        BreakSystem.SetBreakList(BreakObjList);
    }


    List<GameObject> GetAllChildren(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in parent)
        {
            children.Add(child.gameObject);
            // 再帰的に子オブジェクトを追加
            children.AddRange(GetAllChildren(child));
        }

        return children;
    }


}
