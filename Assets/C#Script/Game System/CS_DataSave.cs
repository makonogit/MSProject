using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DataSave : MonoBehaviour
{
    [SerializeField, Header("�X�e�[�W�ԍ�")]
    private int StageNum = 1;

    //�N���A�ς݃X�e�[�W
    private int ClearStageNum;  

    private string SaveDataName = "ClearStage";
   
    private void Awake()
    {
        //���݂̃f�[�^���擾
        ClearStageNum = GetClearStage();
    }


    /// <summary>
    /// �N���A�X�e�[�W�̕ۑ�
    /// </summary>
    public void StageClearSave()
    {
        //���ɃN���A�ς݂̏ꍇ�̓Z�[�u���Ȃ�
        if(StageNum <= ClearStageNum) { return; }

        PlayerPrefs.SetInt(SaveDataName,ClearStageNum + 1);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// �N���A�X�e�[�W���X�g���擾
    /// </summary>
    /// <returns></returns>
    public int GetClearStage()
    {
        //�Z�[�u�f�[�^���Ȃ��Ȃ�0
        if (!PlayerPrefs.HasKey(SaveDataName)) { return 0; }

        return PlayerPrefs.GetInt(SaveDataName);

    }

    /// <summary>
    /// �Z�[�u�f�[�^���Z�b�g
    /// </summary>
    public void ResetSaveData()
    {
        if(ClearStageNum == 0) { return; }

        //�f�[�^��������
        PlayerPrefs.SetInt(SaveDataName, 0);

        PlayerPrefs.Save();

    }


    /// <summary>
    /// �ő�܂ŉ������R�}���h
    /// </summary>
    public void MaxClearCommand()
    {
        PlayerPrefs.SetInt(SaveDataName, 4);

        PlayerPrefs.Save();
    }
}
