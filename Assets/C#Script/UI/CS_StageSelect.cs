using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.C_Script.GameEvent;

/// <summary>
/// �S���F���@�X�e�[�W�Z���N�gUI�V�X�e��
/// </summary>
public class CS_StageSelectUI : MonoBehaviour
{
    [SerializeField, Header("AnimtorController")]
    private RuntimeAnimatorController SelectanimatorController;
    [SerializeField, Header("Animtor�t�^Obj")]
    private GameObject AnimObj;

    private Animator SelectAnim;

    [SerializeField, Header("����")]
    private CS_InputSystem csinput;

    [SerializeField, Header("�X�e�[�W���X�g")]
    private List<Image> StageImageList;
    [SerializeField, Header("�X�e�[�WName�摜")]
    private List<Sprite>�@StageSprite;

    [SerializeField, Header("�J�ڃV�[��")]
    private List<string> SceneName;

    private int currentstage = 1;
    [SerializeField]private int maxstage = 1;   //�z���0�n�܂�(�����)

    //====�m�F��ʊ֌W======
    [SerializeField, Header("�m�F�E�B���h�E")]
    private GameObject ConfirmationWindow;

    [SerializeField, Header("�I��Image")]
    private List<Image> SelectImages;
    private int SelectNum = 0;      //�I��ԍ�
    private bool Selected = false;  //�I�𒆂�

    [SerializeField, Header("�N���A���")]
    private CS_DataSave Save;

    [SerializeField, Header("AudioSource")]
    private AudioSource audio;

    [Header("SE")]
    [SerializeField]
    private AudioClip CursorSE;
    [SerializeField]
    private AudioClip SelectSE;
    [SerializeField]
    private AudioClip CancelSE;

    // Start is called before the first frame update
    void Start()
    {

        maxstage = Save.GetClearStage();   //�N���A�����X�e�[�W�ԍ����擾

        
        for (int i = 0; i < StageImageList.Count; i++)
        {
            //�N���A�ς�
            if (i <= maxstage) { StageImageList[i].sprite = StageSprite[0]; }
            else
            {
                //Debug.Log(StageImageList[i]);
                //���J��
                //StageImageList[i].sprite = StageSprite[1];
                StageImageList[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        SelectAnim = AnimObj.AddComponent<Animator>();
        SelectAnim.runtimeAnimatorController = SelectanimatorController;

        SelectImages[SelectNum].color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Selected)
        {
            bool DownButton = csinput.GetDpadDownTriggered();
            bool UpButton = csinput.GetDpadUpTriggered();

            int oldstage = currentstage;

            if (DownButton) { currentstage++; audio.PlayOneShot(CursorSE); }
            if (UpButton) { currentstage--; audio.PlayOneShot(CursorSE); }

            //�㉺��
            if (currentstage < 1) { currentstage = maxstage + 1;  }
            if (currentstage > maxstage + 1) { currentstage = 1; }
            if (maxstage == 0) { currentstage = 1; }

            
            //�ύX������Animator��ԕύX
            bool Change = oldstage != currentstage;
            if (Change)
            {
                SelectAnim.SetInteger("StageNum", currentstage);
            }

            //�I��������
            bool SelectButton = csinput.GetButtonATriggered();
            if (SelectButton)
            {
                audio.PlayOneShot(SelectSE);
                Selected = true;
                ConfirmationWindow.SetActive(true); //�m�F��ʂ̕\��
            }
        }
        else
        {
            bool LeftButton = csinput.GetDpadLeftTriggered();
            bool RightButton = csinput.GetDpadRightTriggered();

            int oldselect = SelectNum;

            if (RightButton) { SelectNum++; audio.PlayOneShot(CursorSE); }
            if (LeftButton) { SelectNum--; audio.PlayOneShot(CursorSE); }

            if (SelectNum < 0) { SelectNum = 1; }
            if(SelectNum > 1) { SelectNum = 0; }

            bool Change = oldselect != SelectNum;

            if(Change)
            {
                SelectImages[oldselect].color = Color.clear;
                SelectImages[SelectNum].color = Color.white;
            }

            //�I��������
            bool SelectButton = csinput.GetButtonATriggered();

            if (!SelectButton) { return; }

            if (SelectNum == 0) { Selected = false; ConfirmationWindow.SetActive(false); audio.PlayOneShot(CancelSE); }    //�L�����Z��
            if(SelectNum == 1) { audio.PlayOneShot(SelectSE); SceneManager.LoadScene(SceneName[currentstage]); CSGE_GameOver.InitRespawn(); }   //�V�[���J��
        }
    }
}
