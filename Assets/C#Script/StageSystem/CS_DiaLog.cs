//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class CS_DiaLog : MonoBehaviour
//{
//    [SerializeField, Header("表示したいダイアログImege")]
//    private List<Sprite> DialogSprite;

//    [SerializeField, Tooltip("ダイアログ")]
//    private Image DiaLogImage;
//    [SerializeField, Tooltip("ダイアログTrans")]
//    private RectTransform DiaLogTrans;

//    //ダイアログのサイズ倍率(開始時に取得)
//    private Vector3 DialogScale;
//    [SerializeField, Tooltip("プレイヤーの検知範囲")]
//    private float DitictionRadius = 5f;
//    [SerializeField, Tooltip("プレイヤーの検知layer")]
//    private LayerMask PlayerLayer;
//    [SerializeField, Tooltip("ダイアログを開くスピード")]
//    private float OpenSpeed = 1f;

//    //表示フラグ
//    [SerializeField] private bool ViewFlg = false;

//    //現在表示しているスプライト
//    private int CurrentDialogSprite = 0;

//    [SerializeField, Header("入力関係")]
//    private CS_InputSystem Input;

//    // Start is called before the first frame update
//    void Start()
//    {
//        //最初のダイアログ画像を設定
//        DiaLogImage.sprite = DialogSprite[CurrentDialogSprite];
//        DialogScale = DiaLogTrans.localScale;
//        //ダイアログを非表示にしておく
//        DiaLogTrans.localScale = Vector3.zero;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//        //プレイヤーを検知して表示、非表示の切り替え
//        Collider[] hits = Physics.OverlapSphere(transform.position, DitictionRadius, PlayerLayer);

//        foreach (Collider hit in hits)
//        {
//            Debug.Log(hit);
//            bool PlayerHit = hit.CompareTag("Player");
//            if (PlayerHit && !ViewFlg)
//            {
//                ViewFlg = true;
//                Time.timeScale = 0f;    //一時停止する
//            }
//            if (!PlayerHit && ViewFlg)
//            {
//                ViewFlg = false;
//            }
//        }

//        //表示されてなかったら処理しなーい
//        if (!ViewFlg) { return; }

//        //ダイアログの表示(サイズを大きくしていく)
//        DiaLogTrans.localScale = Vector3.Lerp(DiaLogTrans.localScale, DialogScale, OpenSpeed * 0.01f);

//        //ダイアログの表示
//        DialogInfo();

//    }

//    /// <summary>
//    /// ダイアログの表示更新
//    /// </summary>
//    private void DialogInfo()
//    {
     
//        bool BButton = Input.GetButtonBTriggered();

//        if (!BButton) { return; }

//        //全て表示したらダイアログ表示終了
//        if (CurrentDialogSprite >= DialogSprite.Count)
//        {
//            Time.timeScale = 1f;
//            DiaLogTrans.localScale = Vector3.zero;
//        }

//        //スプライトの更新
//        CurrentDialogSprite++;
//        DiaLogImage.sprite = DialogSprite[CurrentDialogSprite];

//    }


//    private void OnDrawGizmos()
//    {

//        Gizmos.color = Color.red;

//        Gizmos.DrawWireSphere(transform.position, DitictionRadius);

//    }

//    //private void OnTriggerEnter(Collider other)
//    //{
//    //    //プレイヤーが入ったら表示
//    //    bool PlayerHit = other.tag == "Player";
//    //    if(PlayerHit)
//    //    {
//    //        ViewFlg = true;
//    //        Time.timeScale = 0f;    //一時停止する
//    //    }
//    //}

//    //private void OnCollisionEnter(Collision collision)
//    //{
//    //    //プレイヤーが入ったら表示
//    //    bool PlayerHit = collision.transform.tag == "Player";
//    //    if (PlayerHit)
//    //    {
//    //        ViewFlg = true;
//    //        Time.timeScale = 0f;    //一時停止する
//    //    }
//    //}
//}
