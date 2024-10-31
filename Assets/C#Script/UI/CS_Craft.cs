using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �N���t�g�p�̃X�N���v�g
/// </summary>
public class CS_Craft : MonoBehaviour
{
    [SerializeField, Header("�N���t�g�J�[�\��")]
    private RectTransform CraftCursor;

    [SerializeField, Header("���͊֌W")]
    private CS_InputSystem csInput;

    [SerializeField, Header("�N���t�g�E�B���h�E")]
    private List<GameObject> CraftWindow;

    [SerializeField, Header("�N���t�g�I���E�B���h�E")]
    private List<Image> CraftSelectWindow;

    //[SerializeField, Tooltip("�I�𒆃N���t�g�E�B���h�E�X�v���C�g")]
    //private Sprite SelectionCraftWindowSprite;

    //�X�e�B�b�N�̓|���������l�@������傫���Ɠ|���Ă��锻��(0.7���ő傩��)
    private float StickInputThreshold = 0.5f;

    //�N���t�g�ԍ�0:��1:�E2:��
    private int CraftWindowNum = -1;

    private Color oldcolor;

    // Update is called once per frame
    void Update()
    {
        //�E�X�e�B�b�N�̓��͂��擾
        Vector2 StickValue = csInput.GetRightStick();

        //臒l�𒴂�����X�e�B�b�N���͔���
        bool StickInput = Mathf.Abs(StickValue.x) > StickInputThreshold || Mathf.Abs(StickValue.y) > StickInputThreshold;

        int oldWindowNum = CraftWindowNum;

        //���͂��Ă��鎞������]�𔽉f
        if(StickInput){ CraftWindowNum = StickRotationtoObject(StickValue.x, StickValue.y); }
        else{ CraftWindowNum = -1; }
        
        //�O��̒l�ƕς���Ă�����I���E�B���h�E��ݒ�
        bool WindowChange = oldWindowNum != CraftWindowNum;
        if (WindowChange) { SetSelectionCraftWindow(oldWindowNum, CraftWindowNum); }

    }


    /// <summary>
    /// �X�e�B�b�N�̉�]���I�u�W�F�N�g�ɔ��f������
    /// </summary>
    /// <param X������="StickX"></param>
    /// <param Y������="StickY"></param>
    /// �Z�N�V�����ԍ�0:�^��1:�E2:��
    private int StickRotationtoObject(float StickX,float StickY)
    {
        // ���͂����]�p�x���v�Z
        float angle = Mathf.Atan2(StickY, StickX) * Mathf.Rad2Deg;

        //�摜�̃J�[�\���ʒu��90�x����Ă�̂�-90�x
        angle -= 90f;

        //�I�u�W�F�N�g�ɉ�]�𔽉f
        CraftCursor.localRotation = Quaternion.Euler(0, 0, angle);

        //���݂̊p�x����Z�N�V�����ԍ������߂�(Y����3����)
        int Section = -1;
        if (angle <= 60f && angle > -60f) { Section = 0; } //�^��
        else if(angle <= -60f && angle > -180f) { Section = 1; } //�E
        else { Section = 2; }  //��

        return Section;

    }


    /// <summary>
    /// �I�����Ă��鍀�ڂɑI�𒆃N���t�g�E�B���h�E��ݒ肷��
    /// </summary>
    /// <param �N���t�g�ԍ�="CraftNum"></param>
    /// �N���t�g�ԍ��͈ȉ��ɏ�����
    /// 0:�� 1:�E 2:��
    private void SetSelectionCraftWindow(int OldCraftNum,int CraftNum)
    {
        //�ԍ����͈͓����m�F���Ĕ��f
        bool active = OldCraftNum > -1 && OldCraftNum < CraftWindow.Count;
        if (active) 
        {
            CraftSelectWindow[OldCraftNum].color = oldcolor;
            CraftWindow[OldCraftNum].SetActive(false); 
        }

        active = CraftNum > -1 && CraftNum < CraftWindow.Count;
        if (active)
        {
            oldcolor = CraftSelectWindow[CraftNum].color;
            CraftSelectWindow[CraftNum].color = Color.clear;
            CraftWindow[CraftNum].SetActive(true); 
        }
    }
}
