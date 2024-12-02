using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S���F���@�j��I�u�W�F�N�g���擾
/// </summary>
public class CS_BreakSearch : MonoBehaviour
{

    [SerializeField, Header("�j��V�X�e��")]
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
            // �ċA�I�Ɏq�I�u�W�F�N�g��ǉ�
            children.AddRange(GetAllChildren(child));
        }

        return children;
    }


}
