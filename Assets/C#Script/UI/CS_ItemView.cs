using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 担当:菅　アイテムUIシステム
/// </summary>
public class CS_ItemView : MonoBehaviour
{
    [SerializeField, Header("アニメーター")]
    private Animator ItemAnim;

    [SerializeField, Header("入力関係")]
    private CS_InputSystem csInput;

    [SerializeField, Header("所持アイテム")]
    private List<Image> HaveItems;

    private int HaveItemNum = 0;    //現在持っているアイテム番号

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //アイテム表示
        bool RBButton = csInput.GetButtonRPressed();
        if (RBButton) { ItemAnim.SetBool("SelectView",true); }
        else 
        { 
            ItemAnim.SetBool("SelectView", false);
            return;
        }

        //前回の所持アイテム
        int olditem = HaveItemNum;

        //アイテム切替
        bool LeftButton = csInput.GetDpadLeftTriggered();
        bool RightButton = csInput.GetDpadRightTriggered();

        if (LeftButton) { HaveItemNum++; Debug.Log(LeftButton); }
        if (RightButton) { HaveItemNum--; Debug.Log(RightButton); }

        if (LeftButton || RightButton) { ItemAnim.SetTrigger("SelectMove"); }

        bool left = olditem < HaveItemNum;

        //上下限
        if(HaveItemNum < 0) { HaveItemNum = HaveItems.Count - 1; }
        if(HaveItemNum > HaveItems.Count - 1) { HaveItemNum = 0; }

        bool ChangeItemflg = olditem != HaveItemNum;
        
        if (ChangeItemflg) { ChangeItem(left); }

    }

    private void ChangeItem(bool left)
    {
        
        if (left)
        {
            Sprite itemimage = HaveItems[0].sprite;
            HaveItems[0].sprite = HaveItems[1].sprite;
            HaveItems[1].sprite = HaveItems[2].sprite;
            HaveItems[2].sprite = itemimage;
        }
        else
        {
            Sprite itemimage = HaveItems[0].sprite;
            HaveItems[0].sprite = HaveItems[2].sprite;
            HaveItems[2].sprite = HaveItems[1].sprite;
            HaveItems[1].sprite = itemimage;
        }

    }

}
