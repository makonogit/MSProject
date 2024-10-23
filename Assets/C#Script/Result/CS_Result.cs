using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

/// <summary>
/// 担当:菅　リザルトシステム
/// </summary>
public class CS_Result : MonoBehaviour
{


    [SerializeField, Header("リザルト開始フラグ")]
    private bool ResultStart;

    [Header("------------------------------------------")]

    [SerializeField, Header("UIAnimator")]
    private Animator Anim;

    [SerializeField, Header("InputSystem")]
    private CS_InputSystem CSInput;

    [Header("表示するText情報")]
    [SerializeField, Tooltip("表示する文字")]
    private List<Image> ViewTexts;

    [SerializeField, Header("表示するテキストUI")]
    private List<GameObject> ViewTextUIObj;

    //[SerializeField, Tooltip("表示する数字")]
    //private List<Image> ViewNums;

    //[SerializeField,Tooltip("表示する数字UI")]
    //private List<GameObject> ViewNumUIObj;

    [SerializeField, Tooltip("表示する早さ")]
    private float ViewSpeed = 1f;

    [SerializeField, Header("変更するマテリアル")]
    private Material ChangeMat;

    [SerializeField, Tooltip("数字スプライト")]
    private List<Sprite> NumSprite;

    [SerializeField, Tooltip("ランクスプライト")]
    private List<Sprite> LankSprite;

    [SerializeField, Header("遷移するセレクトシーンの名前")]
    private string SelectSceneName;

    //マテリアルのフレームレート
    private float MatFrameRate;

    //テキスト表示状態
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

    //表示状態
    private ViewState viewState = ViewState.ClearText;

    // Start is called before the first frame update
    void Start()
    {
        ViewTexts[0].material.SetFloat("_FrameRate", 50f);

        //マテリアルをインスタンス化して個別に変更するように設定
        for(int i = 0; i<ViewTexts.Count-1;i++)
        {
            //データがあるか調べる
            bool data = ViewTexts[i];
            if(data)
            {
                Material mat = ViewTexts[i].material;
                ViewTexts[i].material = new Material(mat);

            }
        }

        //各パラメータを設定

        //コアの残量を取得

        //ステージの崩落度を取得

        //缶詰の数を取得

    }

    // Update is called once per frame
    void Update()
    {

        if (!ResultStart) { return; }

        //手動で触った時用
        if (!Anim.enabled) { Anim.enabled = true; }

        //配列要素分表示されたら終了
        bool EndView = viewState > ViewState.RankText;
        bool EndButton = CSInput.GetButtonATriggered() || Input.GetKeyDown(KeyCode.Return);
        //Aボタンでシーン遷移
        if (EndButton) { SceneManager.LoadScene(SelectSceneName); }
        if (EndView)  { return; }

        //グリッチエフェクトを止めていく
        //数字表示フラグ
        bool NumView = viewState >= ViewState.CoreLifeNum && viewState <= ViewState.KanCorectNum;
        if(NumView)
        {
            //UIの表示
            bool ViewUI = ViewTextUIObj[(int)viewState].activeSelf && ViewTextUIObj[(int)viewState + 1].activeSelf && ViewTextUIObj[(int)viewState + 2].activeSelf;
            if (!ViewUI)
            {
                return;
            }

            //数字のエフェクトを一気に止める
            bool NumEffectStop =　StopNumGlitchEffect(ViewTexts[(int)viewState].material, ViewTexts[(int)viewState + 1].material, ViewTexts[(int)viewState + 2].material);
            if (NumEffectStop) { viewState += 3; }    //表示数字分止める 
        }
        else
        {
            //UIの表示
            bool ViewUI = ViewTextUIObj[(int)viewState].activeSelf;
            if (!ViewUI) { return; }

            //テキストのエフェクトを止める
            bool TextEffectStop = StopGlithEffect(ViewTexts[(int)viewState].material);
            if (!TextEffectStop) { return; }
            viewState++;
        }
       

    }


    /// <summary>
    /// テキストのグリッチエフェクトを止める
    /// </summary>
    /// <param name="mat"></param>
    /// false:動作中<returns> true:終了</returns>
    private bool StopGlithEffect(Material mat)
    {
        if(mat == null) { return true; }
        //フレームレートを取得
        MatFrameRate = mat.GetFloat("_FrameRate");

        //フレームレートが1より大きいとグリッチエフェクトが作動している
        bool GlitchMove = MatFrameRate >= 1f;

        //フレームレートを下げて動きをゆっくりにしていく
        if(GlitchMove)
        {
            MatFrameRate -= ViewSpeed * Time.deltaTime;
            mat.SetFloat("_FrameRate", MatFrameRate);
        }
        else
        {
            //終了
            ViewTexts[(int)viewState].material = null;
            return true;
        }

        return false;

    }

    /// <summary>
    /// 数字のグリッチエフェクトを止める、複数あるので同時に
    /// </summary>
    /// <param じゅうのくらい="ten"></param>
    /// <param いちのくらい="one"></param>
    /// <param たんい="unit"></param>
    /// <returns></returns>
    private bool StopNumGlitchEffect(Material ten, Material one, Material unit)
    {
        //フレームレートを取得
        MatFrameRate = ten.GetFloat("_FrameRate");

        //フレームレートが1より大きいとグリッチエフェクトが作動している
        bool GlitchMove = MatFrameRate >= 1f;

        //フレームレートを下げて動きをゆっくりにしていく
        if (GlitchMove)
        {
            MatFrameRate -= ViewSpeed * Time.deltaTime;
            ten.SetFloat("_FrameRate", MatFrameRate);
            one.SetFloat("_FrameRate", MatFrameRate);
            unit.SetFloat("_FrameRate", MatFrameRate);
        }
        else
        {
            //終了
            ViewTexts[(int)viewState].material = null;
            ViewTexts[(int)viewState + 1].material = null;
            ViewTexts[(int)viewState + 2].material = null;
            return true;
        }

        return false;
    }

    /// <summary>
    /// リザルトアニメーション開始
    /// </summary>
    public void StartResult()
    {
        ResultStart = true;
        Anim.enabled = true;
    }

}
