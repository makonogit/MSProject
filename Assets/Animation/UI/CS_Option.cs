using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class CS_Option : MonoBehaviour
{

    //[SerializeField, Header("OptionAnimators")]
    //private List<Animator> OptionAnimators;

    [SerializeField, Header("OptionRectTrans")]
    private List<RectTransform> OptionRectTrans;

    [SerializeField, Header("InputSystem")]
    private CS_InputSystem InputSystem;

    [SerializeField, Header("カーソルRectTrans")]
    private RectTransform CursorRectTrans;

    [Header("オプション用カーソル情報")]
    [SerializeField]
    private GameObject OptionCursor;
    [SerializeField]
    private RectTransform OptionCursorTrans;

    [Header("オプション用選択肢項目")]
    [SerializeField,Tooltip("音量")]
    private List<RectTransform> VolumeRectTrans;
    [SerializeField, Tooltip("カメラ")]
    private List<RectTransform> CameraRectTrans;
    [SerializeField, Tooltip("画質")]
    private RectTransform QualityRectTrans;


    [SerializeField, Tooltip("音量スライダー")]
    private List<Slider> VolumeSlider;
    [SerializeField, Tooltip("カメラスライダー")]
    private List<Slider> CameraSlider;
    [SerializeField, Tooltip("画質項目")]
    private List<RectTransform> QualityLevelRectTrans;
    [SerializeField, Tooltip("画質選択中カーソル")]
    private RectTransform QualitySelectCursor;

    [SerializeField, Header("スライダー調整スピード")]
    private float SliderSpeed = 1000.0f;
    [SerializeField, Header("スライダー加速度")]
    private float Slideracceleration = 0.01f;

    private float SliderAccelerationSpeed = 0.0f;

    private int QualityLevel = 0;

    //private Slider QualitySlider;

    [SerializeField, Header("視野角設定")]
    private Cinemachine.CinemachineVirtualCamera CameraFov;
    [SerializeField, Header("カメラ感度設定用")]
    private CS_TpsCamera CameraMove;

    [SerializeField, Header("音量設定")]
    private AudioMixer Audio;
    //[SerializeField, Header("BGM設定")]
    //private AudioSource BGMSource;
    //[SerializeField, Header("SE設定")]
    //private AudioSource SESource;

    //オプション選択番号
    private int OptionSelectNum = 0;

    //オプション状態
    private enum OptionState
    {
        Volume = 0,     //音量
        Camera = 1,     //カメラ
        Quality = 2,    //画質
        Story = 3,      //ストーリー
    }

    private OptionState CurrentOption;

    [SerializeField, Header("OptionPanels")]
    private List<GameObject> OptionPanels;

    private int MaxOption = 0;

    private bool OptionSelect = false;  //オプション選択中か

    private bool SliderSelect = false;  //調整バーを選択しているか
    

    // Start is called before the first frame update
    void Start()
    {
        //画質のスライダーを設定
        //QualitySlider.maxValue = QualitySettings.names.Length - 1;

        QualityLevel = QualitySettings.GetQualityLevel() - 1;

        QualitySelectCursor.transform.SetParent(QualityLevelRectTrans[QualityLevel]);
        CopyRectTransform(QualitySelectCursor, QualityLevelRectTrans[QualityLevel]);

        //スライダーが更新されたときのCallBack
        //QualitySlider.onValueChanged.AddListener(ChangeQuality);

        //音量のスライダーを設定
        float BGMVolume = 0.75f;
        float SEVolume = 0.75f;
        //Audio.GetFloat("BGM", out BGMVolume);
        //Audio.GetFloat("SE", out SEVolume);

        //VolumeSlider[0].value = Mathf.Pow(10, BGMVolume / 20);
        //VolumeSlider[1].value = Mathf.Pow(10, SEVolume / 20);

        //音量が変更されたときのCallBack
        //VolumeSlider[0].onValueChanged.AddListener(SetBGMVolume);
        //VolumeSlider[1].onValueChanged.AddListener(SetSEVolume);

        //最低値と最高値を設定して視野角を設定
        float minfov = 60f;
        float maxfov = 200f;
        CameraSlider[0].minValue = minfov;
        CameraSlider[0].maxValue = maxfov;
        CameraSlider[0].value = minfov;
        //CameraSlider[0].onValueChanged.AddListener(SetFov);

        //カメラ感度設定
        //CameraSlider[1].value = CameraMove.moveSpeed;
        //CameraSlider[1].onValueChanged.AddListener(SetMoveSpeed);
        

        //オプション項目数
        MaxOption = OptionRectTrans.Count - 1;
    }

    /// <summary>
    /// オプション全体アクション
    /// </summary>
    /// <returns></returns>
    public bool OptionAction()
    {
        //各ボタンを押したか判定
        bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
        bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);
        bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);

        //オプション終了
        bool EndOption = true;

        //オプションの設定
        if (OptionSelect) { SelectOption(); }
        else
        {
            //入力がなければ終了
            //if (!DownButton && !UpButton && !SelectButton) { return; }

            //メニュー番号を更新
            int oldOption = (int)CurrentOption;
            if (DownButton) { CurrentOption++; }
            if (UpButton) { CurrentOption--; }

            //カーソル移動(オプション選択中じゃなければカーソルが動く)
            bool ChangeOption = oldOption != (int)CurrentOption;
            if (ChangeOption) { CursorMove(oldOption); }

            //キャンセルしたか
            bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

            if (CanselButton)
            {
                CurrentOption = OptionState.Volume;
                //カーソルの移動
                OptionCursor.transform.SetParent(OptionRectTrans[(int)CurrentOption]);
                CopyRectTransform(OptionCursorTrans, OptionRectTrans[(int)CurrentOption]);
                OptionSelectNum = 0;
                EndOption = false; 
            }
        }


        //オプション選択状態に
        if (SelectButton)
        {
            OptionInfo();   //対象のオプションを表示
            OptionSelect = true;
        }


        return EndOption;

    }

    /// <summary>
    /// カーソル移動
    /// </summary>
    /// <param name="old"></param>
    private void CursorMove(int old)
    {
        //上下限
        if ((int)CurrentOption > MaxOption) { CurrentOption = 0; }
        if ((int)CurrentOption < 0) { CurrentOption = (OptionState)MaxOption; }

        //今回と前回のAnimatorの状態を保存
        //OptionAnimators[(int)CurrentOption].SetBool("OnCursor", true);
        //OptionAnimators[old].SetBool("OnCursor", false);

        //カーソルの位置を設定する
        OptionCursor.transform.SetParent(OptionRectTrans[(int)CurrentOption]);    //子オブジェクトに設定
        CopyRectTransform(OptionCursorTrans, OptionRectTrans[(int)CurrentOption]);
        //OptionCursorTrans = OptionRectTrans[(int)CurrentOption];
        //Vector3 pos = OptionRectTrans[(int)CurrentOption].anchoredPosition;
        //pos.y -= CursorSize;
        //CursorRectTrans.anchoredPosition = pos;
    }

    /// <summary>
    /// 対象のオプションを表示
    /// </summary>
    private void OptionInfo()
    {
        OptionPanels[(int)CurrentOption].SetActive(true);
        //カーソルの位置を合わせる
        OptionCursor.transform.SetParent(VolumeRectTrans[OptionSelectNum]);
        CopyRectTransform(OptionCursorTrans, VolumeRectTrans[OptionSelectNum]);
    }

    /// <summary>
    /// オプション詳細選択
    /// </summary>
    private void SelectOption()
    {
        //OptionAnimators[(int)CurrentOption].SetBool("IsSelect", true);

        //キャンセルしたか
        bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

       
        switch (CurrentOption)
        {
            case OptionState.Volume:
                VolumeAction();
                break;
            case OptionState.Camera:
                CameraAction();
                break;
            case OptionState.Quality:
                QualityAction();
                break;
            case OptionState.Story:
                StoryAction();
                break;
        }

        //オプション選択解除
        if (CanselButton)
        {
           
            
        }

    }

    /// <summary>
    /// オプション項目の初期化
    /// </summary>

    private void OptionInitialize()
    {
        //キャンセルしたか
        bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

        if (!CanselButton) { return; }

        //オプション項目を初期化
        OptionCursor.transform.SetParent(OptionRectTrans[(int)CurrentOption]);
        CopyRectTransform(OptionCursorTrans, OptionRectTrans[(int)CurrentOption]);
        OptionSelectNum = 0;

        OptionPanels[(int)CurrentOption].SetActive(false);
        OptionSelect = false;
    }

    
    /// <summary>
    /// 音量設定
    /// </summary>
    private void VolumeAction()
    {
        if (!SliderSelect)
        {
            //カーソルの位置を合わせる
            OptionCursor.transform.SetParent(VolumeRectTrans[OptionSelectNum]);
            CopyRectTransform(OptionCursorTrans, VolumeRectTrans[OptionSelectNum]);
            //OptionCursorTrans = VolumeRectTrans[OptionSelectNum];
            //Vector3 pos = VolumeRectTrans[OptionSelectNum].anchoredPosition;
            //OptionCursorTrans.anchoredPosition = pos;

           //上下ボタンを取得
            bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
            bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

            //オプション項目番号を更新
            if (DownButton) { OptionSelectNum++; }
            if (UpButton) { OptionSelectNum--; }

            //上下限
            if (OptionSelectNum > VolumeRectTrans.Count - 1) { OptionSelectNum = 0; }
            if (OptionSelectNum < 0) { OptionSelectNum = VolumeRectTrans.Count - 1; }

            bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
            if (SelectButton) 
            {
                SliderAccelerationSpeed = (VolumeSlider[OptionSelectNum].maxValue - VolumeSlider[OptionSelectNum].minValue) / SliderSpeed;
                SliderSelect = true; 
            }
            
            //オプションの初期化
            OptionInitialize();

        }
        else
        {
            //左スティックのx方向を取得
            float SideStick = InputSystem.GetLeftStick().x;

            //スライダーに反映
            if (SideStick > 0.5f) { VolumeSlider[OptionSelectNum].value += SliderAccelerationSpeed; }
            else if (SideStick < -0.5f) { VolumeSlider[OptionSelectNum].value -= SliderAccelerationSpeed; }
            else {SliderAccelerationSpeed = (VolumeSlider[OptionSelectNum].maxValue - VolumeSlider[OptionSelectNum].minValue) / SliderSpeed; }

            float acceleration = (VolumeSlider[OptionSelectNum].maxValue - VolumeSlider[OptionSelectNum].minValue) / Slideracceleration;
            SliderAccelerationSpeed += acceleration;

            //キャンセルしたか
            bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);
            if (CanselButton) { SliderSelect = false; }

        }
        //float SliderValue = (SideStick + 1) / 2;    //0～1の数値に変換
        //VolumeSlider[OptionSelectNum].value = SliderValue;

    }

    /// <summary>
    /// BGM変更用
    /// </summary>
    /// <param name="value"></param>
    private void SetBGMVolume(float value)
    {
        // スライダーの値を-80dBから0dBに変換してAudioMixerに設定
        float volume = Mathf.Log10(value) * 20;
        Audio.SetFloat("BGM", volume);
    }

    /// <summary>
    /// SE変更用
    /// </summary>
    /// <param name="value"></param>
    private void SetSEVolume(float value)
    {
        // スライダーの値を-80dBから0dBに変換してAudioMixerに設定
        float volume = Mathf.Log10(value) * 20;
        Audio.SetFloat("SE", volume);
    }

    /// <summary>
    /// カメラ設定
    /// </summary>
    private void CameraAction()
    {
        //カーソルの位置を合わせる
        OptionCursor.transform.SetParent(CameraRectTrans[OptionSelectNum]);
        CopyRectTransform(OptionCursorTrans, CameraRectTrans[OptionSelectNum]);
        //OptionCursorTrans = CameraRectTrans[OptionSelectNum];
        // Vector3 pos = CameraRectTrans[OptionSelectNum].anchoredPosition;
        // OptionCursorTrans.anchoredPosition = pos;

        if (!SliderSelect)
        {
            //上下ボタンを取得
            bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
            bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

            //オプション項目番号を更新
            if (DownButton) { OptionSelectNum++; }
            if (UpButton) { OptionSelectNum--; }

            //上下限
            if (OptionSelectNum > CameraRectTrans.Count - 1) { OptionSelectNum = 0; }
            if (OptionSelectNum < 0) { OptionSelectNum = CameraRectTrans.Count - 1; }


            bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
            if (SelectButton) 
            {
                SliderAccelerationSpeed = (CameraSlider[OptionSelectNum].maxValue - CameraSlider[OptionSelectNum].minValue) / SliderSpeed;
                SliderSelect = true;
            }

            //オプションの初期化
            OptionInitialize();

        }
        else
        {
            //左スティックのx方向を取得
            float SideStick = InputSystem.GetLeftStick().x;

            //スライダーに反映
            if (SideStick > 0.5f) { CameraSlider[OptionSelectNum].value += SliderAccelerationSpeed; }
            else if (SideStick < -0.5f) { CameraSlider[OptionSelectNum].value -= SliderAccelerationSpeed; }
            else { SliderAccelerationSpeed = (CameraSlider[OptionSelectNum].maxValue - CameraSlider[OptionSelectNum].minValue) / SliderSpeed; }

            float acceleration = (CameraSlider[OptionSelectNum].maxValue - CameraSlider[OptionSelectNum].minValue) / Slideracceleration;
            SliderAccelerationSpeed += acceleration;

            //キャンセルしたか
            bool CanselButton = InputSystem.GetButtonBTriggered() || Input.GetKeyDown(KeyCode.Backspace);

            if (CanselButton) { SliderSelect = false; }

        }

        //float SliderValue = (SideStick + 1) / 2;    //0～1の数値に変換
        //CameraSlider[OptionSelectNum].value = QualitySlider.value;
    }


    /// <summary>
    /// Fov設定
    /// </summary>
    /// <param name="value"></param>
    private void SetFov(float value)
    {
        CameraFov.m_Lens.FieldOfView = value;
    }

    /// <summary>
    /// 画質設定
    /// </summary>
    private void QualityAction()
    {
        //カーソルの位置を合わせる
        //OptionCursor.transform.SetParent(QualityRectTrans);
        //CopyRectTransform(OptionCursorTrans, QualityRectTrans);
        //OptionCursorTrans = QualityRectTrans;
        //Vector3 pos = QualityRectTrans.anchoredPosition;
        //OptionCursorTrans.anchoredPosition = pos;

        //上下ボタンを取得
        bool DownButton = InputSystem.GetDpadDownTriggered() || Input.GetKeyDown(KeyCode.DownArrow);
        bool UpButton = InputSystem.GetDpadUpTriggered() || Input.GetKeyDown(KeyCode.UpArrow);

        if (DownButton) { QualityLevel--; }
        if (UpButton) { QualityLevel++; }

        //上下限
        int MaxLevel = QualityLevelRectTrans.Count - 1;
        if (QualityLevel > MaxLevel) { QualityLevel = 0; }
        if(QualityLevel < 0) { QualityLevel = MaxLevel; }

        //カーソルの移動
        OptionCursor.transform.SetParent(QualityLevelRectTrans[QualityLevel]);
        CopyRectTransform(OptionCursorTrans, QualityLevelRectTrans[QualityLevel]);

        bool SelectButton = InputSystem.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);

        if(SelectButton)
        {
            QualitySelectCursor.transform.SetParent(QualityLevelRectTrans[QualityLevel]);
            CopyRectTransform(QualitySelectCursor, QualityLevelRectTrans[QualityLevel]);
            //画質変更
            QualitySettings.SetQualityLevel(QualityLevel + 1);
        }

        OptionInitialize();
        //左スティックのx方向を取得
        //float SideStick = InputSystem.GetLeftStick().x;

        //スライダーに反映
        //if(SideStick > 0) { QualitySlider.value += 0.01f; }
        //if(SideStick < 0) { QualitySlider.value -= 0.01f; }
        //QualitySlider.value = (SideStick + 1) / 2;

        //QualitySettings.SetQualityLevel((int)QualitySlider.value);
        //Debug.Log(QualitySettings.GetQualityLevel());
    }


    private void ChangeQuality(int value)
    {
        QualitySettings.SetQualityLevel((int)value);
    }

    /// <summary>
    /// ストーリーの表示
    /// </summary>
    private void StoryAction()
    {
        OptionPanels[(int)CurrentOption].SetActive(true);

        OptionInitialize();
    }

    
    /// <summary>
    /// RectTransFotmをコピー
    /// </summary?>
    /// <returns></returns>
    private void CopyRectTransform(RectTransform a,RectTransform b)
    {
        a.localPosition = Vector3.zero;
        a.localRotation = Quaternion.identity;
        a.localScale = Vector3.one;
        a.pivot = Vector2.zero;//b.pivot;
        a.anchorMin = Vector2.zero;//a.anchorMin;//new Vector2(0.5f,0.5f);
        a.anchorMax = Vector2.zero;//new Vector2(0.5f, 0.5f);
        a.anchoredPosition = Vector2.zero;
       
    }

}
