using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Q�[��UI�V�X�e��
/// </summary>
public class CS_GameUIManager : MonoBehaviour
{
    [SerializeField, Header("�Q��Q�[�W�}�X�N")]
    private RectMask2D HungerGageMask;
    private float MaskMAXValue; //�}�X�N�ő�l
    private float MaxPlayerHP; //�v���C���[�ő�HP
    //�Q��Q�[�W�̃}�X�N�T�C�Y�ƃv���C���[�̋Q�샊�\�[�X���X�P�[�������l
    private float HungerGageScale;

    //private CS_Player player;   //�v���C���[�̏��(���\�[�X���擾������)

    [Header("�ʋlText")]
    [SerializeField, Tooltip("�擾�ʋl")]
    private Text CanText;
    [SerializeField, Tooltip("�擾�󂫊�")]
    private Text EnptyCanText;


    [SerializeField, Header("�V���[�g�J�b�g�p�l��")]
    private GameObject CraftPanel;

    [SerializeField, Header("���e�B�N��")]
    private GameObject Reticle;

    [SerializeField, Header("TPS�J����")]
    private CS_TpsCamera tpscamera;
    private float CamSpeed = 0;

    [SerializeField, Header("���͊֌W")]
    private CS_InputSystem CS_Input;

    [SerializeField, Header("CS_PlayerManager")]
    private CS_PlayerManager playermanager;

    private void Start()
    {
        //�}�X�N�̍ő�T�C�Y(Left)���擾
        MaskMAXValue = HungerGageMask.padding.w;
        HungerGageMask.padding = Vector4.zero;

        //�v���C���[�̏��擾
        MaxPlayerHP = playermanager.GetHP();

        //�ʋl�̐����擾(��U99)
        string CanNum = 0.ToString("00");
        //if(value < 10) { //10��菬����������01�Ƃ��ɂ��� }
        CanText.text = CanNum;
        EnptyCanText.text = CanNum;

        //�J�����̃X�s�[�h��ۑ�
        CamSpeed = tpscamera.CameraSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�̗̑͂��擾���Ĕ��f������
        float Hpprogress = playermanager.GetHP() / MaxPlayerHP;
        HungerGageScale = MaskMAXValue - (MaskMAXValue * Hpprogress);
        UpdateHungerGage(HungerGageScale);

        //�ʂ̐��𔽉f
        string CanNum = playermanager.GetItemStock().ToString("00");
        CanText.text = CanNum;
        string EmptyCanNum = playermanager.GetIngredientsStock().ToString("00");
        EnptyCanText.text = EmptyCanNum;

        bool LBTriggered = CS_Input.GetButtonLTriggered();
        bool LBButton = CS_Input.GetButtonLPressed();
        
        //LB�ŃN���t�g�\��,���e�B�N����\��
        if (LBButton) 
        {
            //�J�����̉�]���~�߂�
            tpscamera.CameraSpeed = 0f;
            Reticle.SetActive(false);
            CraftPanel.SetActive(true); 
        }
        else
        {
            tpscamera.CameraSpeed = CamSpeed;
            Reticle.SetActive(true);
            CraftPanel.SetActive(false); 
        }


    }

    /// <summary>
    /// �Q�[�W�̒l�X�V����
    /// </summary>
    /// <param �V�����Q�[�W�̒l="value"></param>
    private void UpdateHungerGage(float value)
    {
        HungerGageMask.padding = new Vector4(0, 0, 0, value);
    }

}
