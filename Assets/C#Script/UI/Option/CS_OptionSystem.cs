using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 担当：菅　オプションシステムver2
/// </summary>
public class CS_OptionSystem : MonoBehaviour
{

    [SerializeField, Tooltip("オプション用カーソル")]
    private RectTransform OptionCursor;
    [SerializeField, Tooltip("カーソル移動位置List")]
    private List<RectTransform> OptionTransList;

    private enum OptionState
    {
        NONE,   //なにもしていない
        SOUND,  //サウンド
        CAMERA, //カメラ
        GRAPHIC,//グラフィック
    }

    [SerializeField, Header("現在の状態")]
    private OptionState CurrentState;

    //項目選択中か
    private bool Select = false;
    
    //オプション項目選択中か
    private bool OptionSelect = false;

    //決定とキャンセルを保持
    private bool Decision = false;
    private bool Cancel = false;

    [Header("=============システム===============")]
    [SerializeField, Tooltip("入力")]
    private CS_InputSystem InputSystem;
    [SerializeField, Tooltip("決定時の色")]
    private Color DecisionColor;
    [SerializeField, Tooltip("未選択時の色")]
    private Color NormalColor;
    [SerializeField, Tooltip("設定スライダー移動速度")]
    private float SliderSpeed = 1;
    //スライダー加速度
    private float SliderAccelerationSpeed = 1;


    [Header("=============サウンド===============")]
    [SerializeField, Tooltip("AudioMixer")]
    private AudioMixer Mixer;
    [SerializeField, Tooltip("設定用カーソル")]
    private RectTransform SoundCursor;
    [SerializeField, Tooltip("カーソル移動用座標")]
    private List<RectTransform> SoundCursorPos;
    [SerializeField, Tooltip("設定用スライダー")]
    private List<Image> SoundSlider;
    private int SoundSelectNum = 0; //0:SE 1:BGM

    [Header("=============カメラ===============")]
    [SerializeField, Tooltip("視野角設定")]
    private Cinemachine.CinemachineVirtualCamera CameraFov;
    [SerializeField, Tooltip("カメラ感度設定用")]
    private CS_TpsCamera CameraMove;
    [SerializeField, Tooltip("設定用カーソル")]
    private RectTransform CameraCursor;
    [SerializeField, Tooltip("カーソル移動用座標")]
    private List<RectTransform> CameraCursorPos;
    [SerializeField, Tooltip("設定用スライダー")]
    private List<Image> CameraSlider;
    private float CameraMaxSpeed = 200f;    //Cameraの最大移動速度
    private float CameraMaxFov = 120f;     //Cameraの最大視野角
    private int CameraSelectNum = 0; //0:感度 1:視野角

    [Header("=============画質設定===============")]
    [SerializeField, Tooltip("設定用カーソル")]
    private RectTransform GraphicCursor;
    [SerializeField, Tooltip("カーソル移動用座標")]
    private List<RectTransform> GraphicCursorPos;
    [SerializeField, Tooltip("設定項目テキスト")]
    private List<Text> GraphicSettingTextList;
    private int CurrentGraphicSettingNum = 2; //画質設定番号 0~低画質　4~高画質
    private int GraphicSettingNum;  //現在設定中の画質


    [SerializeField, Header("設定用パネル")]
    private List<GameObject> SettingPanels;    //表示用パネル

    private bool End = false;

    /// <summary>
    /// オプション終了できる状態か
    /// </summary>
    /// true 終了<returns>  false 継続 </returns>
    public bool GetOptionBack()
    {
        return End;
    }

    // Start is called before the first frame update
    void Start()
    {
        GraphicSettingTextList[CurrentGraphicSettingNum].color = DecisionColor;
        GraphicSettingNum = CurrentGraphicSettingNum;
    }

    
    private void OnEnable()
    {
        End = false;
        CurrentState = OptionState.SOUND;
        SettingPanels[(int)CurrentState - 1].SetActive(true);
        //if(CurrentState != OptionState.NONE) { return; }
        //CurrentState = OptionState.SOUND;
    }


    /// <summary>
    /// カメラ設定
    /// </summary>
    private void Camera()
    {
        
        if (!OptionSelect)
        {
            //オプション項目の選択中にする
            if (Decision) 
            {
               CameraCursorPos[CameraSelectNum].GetComponent<Text>().color = DecisionColor;
               OptionSelect = true; 
            }
            //オプションをキャンセル
            if (Cancel)
            {
                CameraSelectNum = 0;
                Select = false;
                //CurrentState = OptionState.NONE;

                OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = NormalColor;
            }

            //入力(上下)
            bool DownButton = InputSystem.GetDpadDownTriggered();
            bool UpButton = InputSystem.GetDpadUpTriggered();
            if (DownButton || UpButton) { CameraSelectNum = CameraSelectNum == 0 ? 1 : 0; }

            //カーソル移動
            CameraCursor.anchoredPosition = CameraCursorPos[CameraSelectNum].anchoredPosition;

        }
        else
        {
            //左スティックのx方向を取得
            float SideStick = InputSystem.GetLeftStick().x;
            bool Left = SideStick < -0.5f && CameraSlider[CameraSelectNum].fillAmount >= 0;
            bool Right = SideStick > 0.5f && CameraSlider[CameraSelectNum].fillAmount <= 1;
            if (Left) { CameraSlider[CameraSelectNum].fillAmount -= SliderAccelerationSpeed * Time.deltaTime; }
            if (Right) { CameraSlider[CameraSelectNum].fillAmount += SliderAccelerationSpeed * Time.deltaTime; }

            //スライダー速度を加速させてく
            if (Left || Right) { SliderAccelerationSpeed += SliderSpeed * Time.deltaTime; }
            else { SliderAccelerationSpeed = SliderSpeed; }

            //感度設定
            if (CameraSelectNum == 0)
            {
                if(CameraMove) CameraMove.CameraSpeed = CameraSlider[CameraSelectNum].fillAmount / CameraMaxSpeed;
            }
            //視野角設定
            else
            {
                if(CameraFov) CameraFov.m_Lens.FieldOfView = CameraSlider[CameraSelectNum].fillAmount / CameraMaxFov;
            }

            //オプション項目設定終了
            if (Cancel) 
            {
               CameraCursorPos[CameraSelectNum].GetComponent<Text>().color = NormalColor;
               OptionSelect = false; 
            }
        }
    }

    /// <summary>
    /// サウンド設定
    /// </summary>
    private void Sound()
    {

        
        if (!OptionSelect)
        {
            //オプション項目の選択中にする
            if (Decision) 
            {
                SoundCursorPos[SoundSelectNum].GetComponent<Text>().color = DecisionColor;
                OptionSelect = true; 
            }
            //オプションをキャンセル
            if (Cancel)
            {
                SoundSelectNum = 0;
                Select = false;

                //CurrentState = OptionState.NONE;

                OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = NormalColor;
            }

            //入力(上下)
            bool DownButton = InputSystem.GetDpadDownTriggered();
            bool UpButton = InputSystem.GetDpadUpTriggered();
            if (DownButton || UpButton) { SoundSelectNum = SoundSelectNum == 0 ? 1 : 0; }

            //カーソル移動
            SoundCursor.anchoredPosition = SoundCursorPos[SoundSelectNum].anchoredPosition;

        }
        else
        {
            //左スティックのx方向を取得
           
            float SideStick = InputSystem.GetLeftStick().x;
            bool Left = SideStick < -0.5f && CameraSlider[CameraSelectNum].fillAmount >= 0;
            bool Right = SideStick > 0.5f && CameraSlider[CameraSelectNum].fillAmount <= 1;
            if (Left) { SoundSlider[SoundSelectNum].fillAmount -= SliderAccelerationSpeed * Time.deltaTime; }
            if (Right) { SoundSlider[SoundSelectNum].fillAmount += SliderAccelerationSpeed * Time.deltaTime; }

            //スライダー速度を加速させてく
            if (Left || Right) { SliderAccelerationSpeed += SliderSpeed * Time.deltaTime; }
            else { SliderAccelerationSpeed = SliderSpeed; }

            //SE設定
            if (SoundSelectNum == 0)
            {
                // スライダーの値を-80dBから0dBに変換
                float volume = Mathf.Lerp(-80f, 0f, SoundSlider[SoundSelectNum].fillAmount);
                Mixer.SetFloat("SE", volume);

            }
            //BGM設定
            else
            {
                // スライダーの値を-80dBから0dBに変換
                float volume = Mathf.Lerp(-80f, 0f, SoundSlider[SoundSelectNum].fillAmount);
                Mixer.SetFloat("BGM", volume);
            }

            //オプション項目設定終了
            if (Cancel) 
            {
                SoundCursorPos[SoundSelectNum].GetComponent<Text>().color = NormalColor;
                OptionSelect = false; 
            }
        }
    }

    /// <summary>
    /// 画質設定
    /// </summary>
    private void Graphic()
    {
        
        if (Cancel)
        {
            CurrentGraphicSettingNum = 0;
            Select = false;

            //CurrentState = OptionState.NONE;

            OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = NormalColor;
        }

        //入力(上下)
        bool DownButton = InputSystem.GetDpadDownTriggered();
        bool UpButton = InputSystem.GetDpadUpTriggered();

        if (DownButton) { CurrentGraphicSettingNum--; }
        if (UpButton) { CurrentGraphicSettingNum++; }

        //上下限の設定
        if (CurrentGraphicSettingNum > GraphicCursorPos.Count - 1) { CurrentGraphicSettingNum = 0; }
        if (CurrentGraphicSettingNum < 0) { CurrentGraphicSettingNum = GraphicCursorPos.Count - 1; }

        //カーソル移動
        GraphicCursor.anchoredPosition = GraphicCursorPos[CurrentGraphicSettingNum].anchoredPosition;
      
        if (!Decision) { return; }

        QualitySettings.SetQualityLevel(CurrentGraphicSettingNum + 1);

        //選択中の画質を更新
        GraphicSettingTextList[GraphicSettingNum].color = NormalColor;
        GraphicSettingTextList[CurrentGraphicSettingNum].color = DecisionColor;
        GraphicSettingNum = CurrentGraphicSettingNum;
    }

    private void SelectAction()
    {
        switch (CurrentState)
        {
            case OptionState.NONE:      //なにもしていない
                break;
            case OptionState.SOUND:     //サウンド
                Sound();
                break;
            case OptionState.CAMERA:    //カメラ
                Camera();
                break;
            case OptionState.GRAPHIC:   //画質
                Graphic();
                break;
        }


    }

    // Update is called once per frame
    void Update()
    {                                  
        if (Decision) { Select = true; }//項目選択中かの状態
        
        Cancel = InputSystem.GetButtonBTriggered();　//戻る
        Decision = InputSystem.GetButtonATriggered(); //決定

        if (Select) //選択行動
        {
            OptionTransList[(int)CurrentState - 1].GetChild(0).GetComponent<Text>().color = DecisionColor;
            SelectAction(); 
        }
        else  //カーソル移動
        {
            CursorMove();
            if (!OptionSelect && Cancel) 
            {
                SettingPanels[(int)CurrentState - 1].SetActive(false);
                End = true; 
            }
        }
    }

    /// <summary>
    /// カーソル移動
    /// </summary>
    private void CursorMove()
    {
        //前回の状態
        OptionState oldstate = CurrentState;

        //入力(上下)
        bool DownButton = InputSystem.GetDpadDownTriggered();
        bool UpButton = InputSystem.GetDpadUpTriggered();

        if (DownButton) { CurrentState++; }
        if (UpButton) { CurrentState--; }

        //入力上下限の設定
        if (CurrentState < OptionState.SOUND) { CurrentState = OptionState.GRAPHIC; }
        if (CurrentState > OptionState.GRAPHIC) { CurrentState = OptionState.SOUND; }

        //カーソル移動
        OptionCursor.anchoredPosition = OptionTransList[(int)CurrentState - 1].anchoredPosition;

        //パネルの表示非表示
        if (oldstate == CurrentState) { return; }
        if (!SettingPanels[(int)CurrentState - 1].activeSelf) { SettingPanels[(int)CurrentState -1].SetActive(true); }
        if(oldstate < OptionState.SOUND || oldstate > OptionState.GRAPHIC) { return; }
        if (SettingPanels[(int)oldstate - 1].activeSelf) { SettingPanels[(int)oldstate - 1].SetActive(false); }



    }


}
