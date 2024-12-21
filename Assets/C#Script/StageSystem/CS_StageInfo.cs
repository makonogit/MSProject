using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �S���ҁF���@�X�e�[�W�̒ʒm�HUI
/// </summary>
public class CS_StageInfo : MonoBehaviour
{
    //�X�e�[�^�X�\���p
    [SerializeField] private GameObject StatusImageObj;
    [SerializeField] private Image StatusImage;

    [SerializeField, Header("��ԃX�v���C�g")]
    private List<Sprite> StatusSprite;
    
  

    public enum StageStatus
    {
        
        BreakStop = 0,  //�����~
        BreakStart = 1, //����J�n
        CoreSteal = 2,  //�R�A�𓐂܂ꂽ
        CoreGet = 3,    //�R�A���擾
    }


    //���݂̏�ԎQ��
    [SerializeField] StageStatus currentstatus;

    public StageStatus GetCurrentStatus() => currentstatus;

    /// <summary>
    /// �X�e�[�^�X��\��
    /// </summary>
    /// <param �\��������="status"></param>
    public void SetStatus(StageStatus status)
    {
        if (StatusImageObj.activeSelf) { StatusImageObj.SetActive(false); }
        //�X�v���C�g��ݒ肵�ĕ\���A�A�j���[�V���������Đ�
        currentstatus = status;
        StatusImage.sprite = StatusSprite[(int)status];
        StatusImageObj.SetActive(true);
        StartCoroutine(EndViewStatus());    
    }


    /// <summary>
    /// �X�e�[�^�X�\���I���R���[�`��
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndViewStatus()
    {
        yield return new WaitForSeconds(2.0f);

        //�Ăє�\����
        StatusImageObj.SetActive(false);

    }

}
