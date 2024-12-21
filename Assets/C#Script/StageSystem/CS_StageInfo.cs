using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 担当者：菅　ステージの通知？UI
/// </summary>
public class CS_StageInfo : MonoBehaviour
{
    //ステータス表示用
    [SerializeField] private GameObject StatusImageObj;
    [SerializeField] private Image StatusImage;

    [SerializeField, Header("状態スプライト")]
    private List<Sprite> StatusSprite;
    
  

    public enum StageStatus
    {
        
        BreakStop = 0,  //崩壊停止
        BreakStart = 1, //崩壊開始
        CoreSteal = 2,  //コアを盗まれた
        CoreGet = 3,    //コアを取得
    }


    //現在の状態参照
    [SerializeField] StageStatus currentstatus;

    public StageStatus GetCurrentStatus() => currentstatus;

    /// <summary>
    /// ステータスを表示
    /// </summary>
    /// <param 表示する状態="status"></param>
    public void SetStatus(StageStatus status)
    {
        if (StatusImageObj.activeSelf) { StatusImageObj.SetActive(false); }
        //スプライトを設定して表示、アニメーション自動再生
        currentstatus = status;
        StatusImage.sprite = StatusSprite[(int)status];
        StatusImageObj.SetActive(true);
        StartCoroutine(EndViewStatus());    
    }


    /// <summary>
    /// ステータス表示終了コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndViewStatus()
    {
        yield return new WaitForSeconds(2.0f);

        //再び非表示に
        StatusImageObj.SetActive(false);

    }

}
