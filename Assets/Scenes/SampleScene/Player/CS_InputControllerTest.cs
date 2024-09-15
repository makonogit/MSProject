using UnityEngine;

// コントローラー入力の確認用
public class CS_InputControllerTest : MonoBehaviour
{
    //**
    //* 更新
    //**
    void Update()
    {
        // コントローラー入力をチェックしてログに出力する

        // ステック
        LogStickInputs();
        // 十字ボタン
        LogDPadInputs();
        // トリガー
        LogTriggerInputs();
        // ボタン
        LogButtonInputs();
    }

    //**
    //* スティックの入力をログに出力する
    //**
    void LogStickInputs()
    {
        // 左スティック
        float lsh = Input.GetAxis("LStick X");
        float lsv = Input.GetAxis("LStick Y");
        if (lsh != 0 || lsv != 0)
        {
            Debug.Log("L stick: " + lsh + ", " + lsv);
        }

        // 右スティック
        float rsh = Input.GetAxis("RStick X");
        float rsv = Input.GetAxis("RStick Y");
        if (rsh != 0 || rsv != 0)
        {
            Debug.Log("R stick: " + rsh + ", " + rsv);
        }
    }

    //**
    //* D-Padの入力をログに出力する
    //**
    void LogDPadInputs()
    {
        float dph = Input.GetAxis("DPad X");
        float dpv = Input.GetAxis("DPad Y");
        if (dph != 0 || dpv != 0)
        {
            Debug.Log("D Pad: " + dph + ", " + dpv);
        }
    }

    //**
    //* トリガーの入力をログに出力する
    //**
    void LogTriggerInputs()
    {
        float tri = Input.GetAxis("LRTrigger");
        if (tri > 0)
        {
            Debug.Log("L trigger: " + tri);
        }
        else if (tri < 0)
        {
            Debug.Log("R trigger: " + tri);
        }
        else
        {
            Debug.Log("  trigger: none");
        }
    }

    //**
    //* ボタンの入力をログに出力する
    //**
    void LogButtonInputs()
    {
        if (Input.GetButtonDown("ButtonA"))
        {
            Debug.Log("A");
        }
        if (Input.GetButtonDown("ButtonB"))
        {
            Debug.Log("B");
        }
        if (Input.GetButtonDown("ButtonY"))
        {
            Debug.Log("Y");
        }
        if (Input.GetButtonDown("ButtonX"))
        {
            Debug.Log("X");
        }
        if (Input.GetButtonDown("ButtonL"))
        {
            Debug.Log("L");
        }
        if (Input.GetButtonDown("ButtonR"))
        {
            Debug.Log("R");
        }
    }

}