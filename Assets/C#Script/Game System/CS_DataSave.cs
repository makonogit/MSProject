using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_DataSave : MonoBehaviour
{
    [SerializeField, Header("ステージ番号")]
    private int StageNum = 1;

    //クリア済みステージ
    private int ClearStageNum;  

    private string SaveDataName = "ClearStage";
   
    private void Awake()
    {
        //現在のデータを取得
        ClearStageNum = GetClearStage();
    }


    /// <summary>
    /// クリアステージの保存
    /// </summary>
    public void StageClearSave()
    {
        //既にクリア済みの場合はセーブしない
        if(StageNum <= ClearStageNum) { return; }

        PlayerPrefs.SetInt(SaveDataName,ClearStageNum + 1);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// クリアステージリストを取得
    /// </summary>
    /// <returns></returns>
    public int GetClearStage()
    {
        //セーブデータがないなら0
        if (!PlayerPrefs.HasKey(SaveDataName)) { return 0; }

        return PlayerPrefs.GetInt(SaveDataName);

    }

    /// <summary>
    /// セーブデータリセット
    /// </summary>
    public void ResetSaveData()
    {
        if(ClearStageNum == 0) { return; }

        //データを初期化
        PlayerPrefs.SetInt(SaveDataName, 0);

        PlayerPrefs.Save();

    }


    /// <summary>
    /// 最大まで解放するコマンド
    /// </summary>
    public void MaxClearCommand()
    {
        PlayerPrefs.SetInt(SaveDataName, 4);

        PlayerPrefs.Save();
    }
}
