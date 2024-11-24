using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 担当：菅　Minimapシステム
/// </summary>
public class CS_Minimap : MonoBehaviour
{
    
    [SerializeField, Header("プレイヤー位置")]
    private Transform PlayerTrans;

    [Header("検知範囲")]
    [SerializeField,Tooltip("半径")]
    private float DetectionRadius = 1.0f;
    //[SerializeField, Tooltip("高さ")]
    //private float DetectionHeight = 1.0f;

    
    [Header("敵のセンサー強度割合")]
    [Header("※強度を最大距離(検知範囲)の何パーセントの割合からとするのか※")]

    [Tooltip("薄い赤")]
    [Range(0, 100)] public float EnemySencorCautionRatio = 75f; // Safe の割合（例：最大距離の75%）
    [Tooltip("赤")]
    [Range(0, 100)] public float EnemySencorWarningRatio = 50f; // Caution の割合（例：最大距離の50%）
    [Tooltip("濃い赤")]
    [Range(0, 100)] public float EnemySencorDangerRatio = 25f; // Warning の割合（例：最大距離の25%）

   
    [SerializeField, Header("検知対象Layer")]
    private LayerMask DetectionLayer;

    [Space(10)]
    [Header("=========サワルナキケン===========")]
    [SerializeField,Header("敵センサーの管理オブジェクト")]
    private GameObject EnemySencorGroup;
    [SerializeField, Header("敵センサーの中心")]
    private GameObject EnemySencorCenter;
    [SerializeField, Header("缶詰アイコンのPrefab")]
    private GameObject CanIconPrefab;
    [SerializeField, Header("コアアイコンのUI")]
    private GameObject CoreIconImage;
    [SerializeField, Header("MinmapUITrans")]
    private RectTransform MinimapRect;
    [SerializeField, Header("アイコンMaskObject")]
    private RectTransform IconMaskObj;

    [SerializeField,Header("ミニマップの枠")]
    private RectTransform MinimapFrame;

    [Header("検知対象のTag名")]
    [SerializeField,Tooltip("敵")]
    private string EnemyTagName;
    [SerializeField, Tooltip("缶詰")]
    private string CanTagName;
    [SerializeField, Tooltip("コア")]
    private string CoreTagName;

    [SerializeField, Header("アイコンのプール数")]
    private int IconPoolNum = 20;

    private List<GameObject> activeIcons = new List<GameObject>(); // アイコンリスト
    private List<int> activeEnemySencor = new List<int>(); //エネミーセンサーリスト

    private Queue<GameObject> CanIconPool = new Queue<GameObject>();    // 缶詰アイコン用のプール
   
    //16分割のセンサーUI
    private GameObject[] EnemySencorSection = new GameObject[16];

    // Start is called before the first frame update
    void Start()
    {
        //敵センサー用UIを取得して非アクティブにしておく
        for(int i = 0; i< EnemySencorSection.Length;i++)
        {
            EnemySencorSection[i] = EnemySencorGroup.transform.GetChild(i).gameObject;
            EnemySencorSection[i].SetActive(false);
        }

        //センサーの角度

        //// プレイヤーの現在のY座標を割合に変換
        //float progress = Mathf.InverseLerp(StartPosition.y, EndPosition.y, PlayerTrans.position.y);

        //// Minimap上の位置を設定
        //Vector2 markerPosition = MinimapSize.rect.min + new Vector2(0, MinimapSize.rect.height * progress);
        //MiniPlayerPin.anchoredPosition = markerPosition;

        ////崩壊UIのサイズを設定
        //BreakUImaxsize = BreakNowPos.sizeDelta.y;
        //BreakNowPos.sizeDelta = new Vector2(BreakNowPos.sizeDelta.x, 0f);
        //BreakedPos.sizeDelta = new Vector2(BreakedPos.sizeDelta.x, 0f);

        // 初期化: アイコンをプールに事前に追加しておく
        for (int i = 0; i < IconPoolNum; i++) 
        {
            GameObject icon = Instantiate(CanIconPrefab);
            icon.SetActive(false); // 最初は非表示
            icon.transform.SetParent(IconMaskObj);
            icon.GetComponent<RectTransform>().localScale = new Vector2(1.5f,1.5f);
            CanIconPool.Enqueue(icon);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //ミニマップの枠回しちゃったりして
        MinimapFrame.localRotation = Quaternion.Euler(0, 0, PlayerTrans.eulerAngles.y);


        Collider[] colliders = Physics.OverlapSphere(PlayerTrans.position, DetectionRadius, DetectionLayer);

        // アイコンの表示
        List<GameObject> currentIcons = new List<GameObject>(); // 現在表示するアイコンを保持
        List<int> currentEnemyIndex = new List<int>();          //現在表示している敵のセンサー番号

        //コアの非表示
        if (CoreIconImage.activeSelf) { CoreIconImage.SetActive(false); }
        //中央センサーの非表示
        if (EnemySencorCenter.activeSelf) { EnemySencorCenter.SetActive(false); }

        foreach (var hit in colliders)
        {
            //検知対象を検索
            bool Enemyhit = hit.tag == EnemyTagName;
            bool CanHit = hit.tag == CanTagName;
            bool CoreHit = hit.tag == CoreTagName;


            Vector3 relativePos = hit.transform.position - PlayerTrans.position; // プレイヤーからの相対位置
            float distance = relativePos.magnitude;


            // オブジェクトの角度を計算
            Vector3 forward = PlayerTrans.forward; // プレイヤーの前方向
            forward.y = 0; // 水平面での計算に限定
            relativePos.y = 0; // 水平面での計算に限定
            float angle = Vector3.SignedAngle(forward, relativePos, Vector3.up);

            // 相対位置をミニマップの座標に変換
            Vector2 minimapPosition = new Vector2(-relativePos.x, -relativePos.z) / DetectionRadius * (MinimapRect.sizeDelta.x / 3);
            // 角度を利用して位置を補正する
            minimapPosition = new Vector2(-minimapPosition.x, -minimapPosition.y * Mathf.Cos(angle * Mathf.Deg2Rad));

            // 敵を検知した場合はセンサーを表示
            if (Enemyhit)
            {

                // 角度を0°～360°に変換
                if (angle < 0)
                {
                    angle += 360;
                }

                // 表示するセンサー番号を計算（セクションは22.5°刻み）
                int index = Mathf.FloorToInt(angle / 22.5f);
                
                
                // 対象のセンサーを表示
                EnemySencorSection[index].SetActive(true);
                currentEnemyIndex.Add(index);

                // -------------距離によって透明度を変更する--------------

                // 最大検知距離に対する割合を計算 (0～1の値)
                float distanceRatio = Mathf.Clamp01(distance / DetectionRadius);
                // パーセント表記に変換
                float distancePercent = distanceRatio * 100;


                Image SencorImage = EnemySencorSection[index].GetComponent<Image>();

                if (distancePercent >= EnemySencorCautionRatio) //薄い赤
                {
                    SencorImage.color = new Color(1, 1, 1, 0.25f);
                }
                else if (distancePercent >= EnemySencorWarningRatio)   //赤？
                {
                    SencorImage.color = new Color(1, 1, 1, 0.5f);
                }
                else if (distancePercent >= EnemySencorDangerRatio) //濃い赤
                {
                    SencorImage.color = new Color(1, 1, 1, 0.9f);
                }
                else
                {
                    EnemySencorSection[index].SetActive(false);
                    currentEnemyIndex.Remove(index);
                    EnemySencorCenter.SetActive(true);          //中心
                }

                    
                
            }

            //缶詰の場合はプールから
            if (CanHit)
            {
                // アイコンの生成または再利用
                GameObject icon = GetIconFromPool();
                icon.SetActive(true);
                icon.GetComponent<RectTransform>().anchoredPosition = minimapPosition;

                currentIcons.Add(icon);
            }

            //コアの場合は位置を変更
            if (CoreHit)
            {
                CoreIconImage.SetActive(true);
                CoreIconImage.GetComponent<RectTransform>().anchoredPosition = minimapPosition;
            }


        }

        //範囲外に出ている場合には非表示にする
        {
            // プールに戻すアイコンを非表示にする
            foreach (var icon in activeIcons)
            {
                if (!currentIcons.Contains(icon))
                {
                    icon.SetActive(false);
                    ReturnIconToPool(icon);
                }
            }

            //エネミーを非表示
            foreach(int index in activeEnemySencor)
            {
                if(!currentEnemyIndex.Contains(index))
                {
                    //EnemySencorSection[index].TryGetComponent<Image>(out Image image);
                    
                    EnemySencorSection[index].SetActive(false);
                }
            }

            // 現在のアイコンリストを更新
            activeIcons = currentIcons;
            activeEnemySencor = currentEnemyIndex;

            
            
        }
    }

    /// <summary>
    /// プールからアイコンを取得
    /// </summary>
    /// <returns></returns>

    GameObject GetIconFromPool()
    {
        if (CanIconPool.Count > 0)
        {
            return CanIconPool.Dequeue(); // プールからアイコンを取得
        }
        else
        {
            // プールが空なら新しいアイコンを生成
            GameObject icon = Instantiate(CanIconPrefab);
            icon.transform.SetParent(MinimapRect);
            return icon;
        }
    }

    /// <summary>
    /// 生成アイコンをプールに戻す
    /// </summary>
    /// <param アイコンObject="icon"></param>
    void ReturnIconToPool(GameObject icon)
    {
        CanIconPool.Enqueue(icon); // プールに戻す
    }

    /// <summary>
    /// Rayを表示
    /// </summary>
    private void OnDrawGizmos()
    {
        //プレイヤー検知範囲
        Gizmos.color = Color.yellow;
        //Gizmos.matrix = Matrix4x4.TRS(, transform.rotation, Vector3.one);
        Gizmos.DrawWireSphere(PlayerTrans.position,DetectionRadius);

        //DrawCylinderGizmo(PlayerTrans.position, DetectionRadius, DetectionHeight);
    }


    void DrawCylinderGizmo(Vector3 position, float radius, float height)
    {
        // 円柱の底面と上面を描画
        Gizmos.DrawWireSphere(position, radius);  // 底面
        Gizmos.DrawWireSphere(position + Vector3.up * height, radius);  // 上面

        // 上面と底面をつなぐ縁を描画
        int segmentCount = 36; // 円周を分割する数（多いほど滑らか）
        for (int i = 0; i < segmentCount; i++)
        {
            float angleA = i * Mathf.PI * 2 / segmentCount;
            float angleB = (i + 1) * Mathf.PI * 2 / segmentCount;
            Vector3 pointA = position + new Vector3(Mathf.Cos(angleA) * radius, 0, Mathf.Sin(angleA) * radius);
            Vector3 pointB = position + new Vector3(Mathf.Cos(angleB) * radius, 0, Mathf.Sin(angleB) * radius);
            Gizmos.DrawLine(pointA, pointB);  // 底面の円周を描画

            pointA = position + new Vector3(Mathf.Cos(angleA) * radius, height, Mathf.Sin(angleA) * radius);
            pointB = position + new Vector3(Mathf.Cos(angleB) * radius, height, Mathf.Sin(angleB) * radius);
            Gizmos.DrawLine(pointA, pointB);  // 上面の円周を描画

            Gizmos.DrawLine(position + new Vector3(Mathf.Cos(angleA) * radius, 0, Mathf.Sin(angleA) * radius),
                            position + new Vector3(Mathf.Cos(angleA) * radius, height, Mathf.Sin(angleA) * radius)); // 上下を繋ぐライン
        }
    }

}
