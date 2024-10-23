using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

/// <summary>
/// �S��:���@���U���g�V�X�e��
/// </summary>
public class CS_Result : MonoBehaviour
{


    [SerializeField, Header("���U���g�J�n�t���O")]
    private bool ResultStart;

    [Header("------------------------------------------")]

    [SerializeField, Header("UIAnimator")]
    private Animator Anim;

    [SerializeField, Header("InputSystem")]
    private CS_InputSystem CSInput;

    [Header("�\������Text���")]
    [SerializeField, Tooltip("�\�����镶��")]
    private List<Image> ViewTexts;

    [SerializeField, Header("�\������e�L�X�gUI")]
    private List<GameObject> ViewTextUIObj;

    //[SerializeField, Tooltip("�\�����鐔��")]
    //private List<Image> ViewNums;

    //[SerializeField,Tooltip("�\�����鐔��UI")]
    //private List<GameObject> ViewNumUIObj;

    [SerializeField, Tooltip("�\�����鑁��")]
    private float ViewSpeed = 1f;

    [SerializeField, Header("�ύX����}�e���A��")]
    private Material ChangeMat;

    [SerializeField, Tooltip("�����X�v���C�g")]
    private List<Sprite> NumSprite;

    [SerializeField, Tooltip("�����N�X�v���C�g")]
    private List<Sprite> LankSprite;

    [SerializeField, Header("�J�ڂ���Z���N�g�V�[���̖��O")]
    private string SelectSceneName;

    //�}�e���A���̃t���[�����[�g
    private float MatFrameRate;

    //�e�L�X�g�\�����
    private enum ViewState
    {
        ClearText = 0,
        CoreLifeText = 1,
        StageLifeText = 2,
        KanCorectText = 3,
        CoreLifeNum = 4,
        StageLifeNum = 7,
        KanCorectNum = 10,
        RankText = 13,
    }

    //�\�����
    private ViewState viewState = ViewState.ClearText;

    // Start is called before the first frame update
    void Start()
    {
        ViewTexts[0].material.SetFloat("_FrameRate", 50f);

        //�}�e���A�����C���X�^���X�����ČʂɕύX����悤�ɐݒ�
        for(int i = 0; i<ViewTexts.Count-1;i++)
        {
            //�f�[�^�����邩���ׂ�
            bool data = ViewTexts[i];
            if(data)
            {
                Material mat = ViewTexts[i].material;
                ViewTexts[i].material = new Material(mat);

            }
        }

        //�e�p�����[�^��ݒ�

        //�R�A�̎c�ʂ��擾

        //�X�e�[�W�̕����x���擾

        //�ʋl�̐����擾

    }

    // Update is called once per frame
    void Update()
    {

        if (!ResultStart) { return; }

        //�蓮�ŐG�������p
        if (!Anim.enabled) { Anim.enabled = true; }

        //�z��v�f���\�����ꂽ��I��
        bool EndView = viewState > ViewState.RankText;
        bool EndButton = CSInput.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
        //A�{�^���ŃV�[���J��
        if (EndButton) { SceneManager.LoadScene(SelectSceneName); }
        if (EndView)  { return; }

        //�O���b�`�G�t�F�N�g���~�߂Ă���
        //�����\���t���O
        bool NumView = viewState >= ViewState.CoreLifeNum && viewState <= ViewState.KanCorectNum;
        if(NumView)
        {
            //UI�̕\��
            bool ViewUI = ViewTextUIObj[(int)viewState].activeSelf && ViewTextUIObj[(int)viewState + 1].activeSelf && ViewTextUIObj[(int)viewState + 2].activeSelf;
            if (!ViewUI)
            {
                return;
            }

            //�����̃G�t�F�N�g����C�Ɏ~�߂�
            bool NumEffectStop =�@StopNumGlitchEffect(ViewTexts[(int)viewState].material, ViewTexts[(int)viewState + 1].material, ViewTexts[(int)viewState + 2].material);
            if (NumEffectStop) { viewState += 3; }    //�\���������~�߂� 
        }
        else
        {
            //UI�̕\��
            bool ViewUI = ViewTextUIObj[(int)viewState].activeSelf;
            if (!ViewUI) { return; }

            //�e�L�X�g�̃G�t�F�N�g���~�߂�
            bool TextEffectStop = StopGlithEffect(ViewTexts[(int)viewState].material);
            if (!TextEffectStop) { return; }
            viewState++;
        }
       

    }


    /// <summary>
    /// �e�L�X�g�̃O���b�`�G�t�F�N�g���~�߂�
    /// </summary>
    /// <param name="mat"></param>
    /// false:���쒆<returns> true:�I��</returns>
    private bool StopGlithEffect(Material mat)
    {
        if(mat == null) { return true; }
        //�t���[�����[�g���擾
        MatFrameRate = mat.GetFloat("_FrameRate");

        //�t���[�����[�g��1���傫���ƃO���b�`�G�t�F�N�g���쓮���Ă���
        bool GlitchMove = MatFrameRate >= 1f;

        //�t���[�����[�g�������ē������������ɂ��Ă���
        if(GlitchMove)
        {
            MatFrameRate -= ViewSpeed * Time.deltaTime;
            mat.SetFloat("_FrameRate", MatFrameRate);
        }
        else
        {
            //�I��
            ViewTexts[(int)viewState].material = null;
            return true;
        }

        return false;

    }

    /// <summary>
    /// �����̃O���b�`�G�t�F�N�g���~�߂�A��������̂œ�����
    /// </summary>
    /// <param ���イ�̂��炢="ten"></param>
    /// <param �����̂��炢="one"></param>
    /// <param ����="unit"></param>
    /// <returns></returns>
    private bool StopNumGlitchEffect(Material ten, Material one, Material unit)
    {
        //�t���[�����[�g���擾
        MatFrameRate = ten.GetFloat("_FrameRate");

        //�t���[�����[�g��1���傫���ƃO���b�`�G�t�F�N�g���쓮���Ă���
        bool GlitchMove = MatFrameRate >= 1f;

        //�t���[�����[�g�������ē������������ɂ��Ă���
        if (GlitchMove)
        {
            MatFrameRate -= ViewSpeed * Time.deltaTime;
            ten.SetFloat("_FrameRate", MatFrameRate);
            one.SetFloat("_FrameRate", MatFrameRate);
            unit.SetFloat("_FrameRate", MatFrameRate);
        }
        else
        {
            //�I��
            ViewTexts[(int)viewState].material = null;
            ViewTexts[(int)viewState + 1].material = null;
            ViewTexts[(int)viewState + 2].material = null;
            return true;
        }

        return false;
    }

    /// <summary>
    /// ���U���g�A�j���[�V�����J�n
    /// </summary>
    public void StartResult()
    {
        ResultStart = true;
        Anim.enabled = true;
    }

}
