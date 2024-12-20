using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_DebguFPS : MonoBehaviour
{

    private float deltaTime = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        // フレーム間の経過時間を取得
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void Update()
    {
        // フレーム間の経過時間を計算
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    void OnGUI()
    {
        // 現在のFPSを計算
        float fps = 1.0f / deltaTime;

        // 描画用のテキスト
        string text = string.Format("{0:0.} FPS", fps);

        // スタイルを設定
        GUIStyle style = new GUIStyle
        {
            fontSize = 24, // フォントサイズ
            normal = { textColor = Color.white } // テキスト色
        };

        // 表示位置
        Rect rect = new Rect(10, 10, 200, 40); // 画面左上
        GUI.Label(rect, text, style);
    }
}
    
