using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_Title : MonoBehaviour
{
    [SerializeField,Tooltip("ゲーム開始時のシーンの名前")]
    private string GameSceneName;

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void StartGame()
    {
        //フェーズ1の開始
        SceneManager.LoadScene(GameSceneName);
    }


    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void EndGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 

    }


}
