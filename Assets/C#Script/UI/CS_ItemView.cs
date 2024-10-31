using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �S��:���@�A�C�e��UI�V�X�e��
/// </summary>
public class CS_ItemView : MonoBehaviour
{
    [SerializeField, Header("�A�j���[�^�[")]
    private Animator ItemAnim;

    [SerializeField, Header("���͊֌W")]
    private CS_InputSystem csInput;

    [SerializeField, Header("�����A�C�e��")]
    private List<Image> HaveItems;

    private int HaveItemNum = 0;    //���ݎ����Ă���A�C�e���ԍ�

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //�A�C�e���\��
        bool RBButton = csInput.GetButtonRPressed();
        if (RBButton) { ItemAnim.SetBool("SelectView",true); }
        else 
        { 
            ItemAnim.SetBool("SelectView", false);
            return;
        }

        //�O��̏����A�C�e��
        int olditem = HaveItemNum;

        //�A�C�e���ؑ�
        bool LeftButton = csInput.GetDpadLeftTriggered();
        bool RightButton = csInput.GetDpadRightTriggered();

        if (LeftButton) { HaveItemNum++; Debug.Log(LeftButton); }
        if (RightButton) { HaveItemNum--; Debug.Log(RightButton); }

        if (LeftButton || RightButton) { ItemAnim.SetTrigger("SelectMove"); }

        bool left = olditem < HaveItemNum;

        //�㉺��
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
