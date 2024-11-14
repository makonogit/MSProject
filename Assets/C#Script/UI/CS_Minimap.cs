using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField, Header("検知対象Layer")]
    private LayerMask DetectionLayer;

    [Space(10)]
    [Header("=========サワルナキケン===========")]
    [SerializeField,Header("敵アイコンのPrefab")]
    private GameObject EnemyIconPrefab;
    [SerializeField, Header("アイテムアイコンのPrefab")]
    private GameObject ItemIconPrefab;
    [SerializeField, Header("MinmapUITrans")]
    private RectTransform MinimapRect;
    [SerializeField, Header("アイコンMaskObject")]
    private RectTransform IconMaskObj;

    private List<GameObject> activeIcons = new List<GameObject>(); // アイコンリスト

    [SerializeField, Header("アイコンのプール数")]
    private int IconPoolNum = 30;

    private Queue<GameObject> IconPool = new Queue<GameObject>(); // アイコン用のプール

    // Start is called before the first frame update
    void Start()
    {
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
        for (int i = 0; i < IconPoolNum; i++) // 50個のアイコンをプールに追加する例
        {
            GameObject icon = Instantiate(EnemyIconPrefab);
            icon.SetActive(false); // 最初は非表示
            icon.transform.SetParent(IconMaskObj);
            icon.GetComponent<RectTransform>().localScale = new Vector2(1.5f,1.5f);
            IconPool.Enqueue(icon);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(PlayerTrans.position, DetectionRadius, DetectionLayer);

        // アイコンの表示
        List<GameObject> currentIcons = new List<GameObject>(); // 現在表示するアイコンを保持

        foreach (var hit in colliders)
        {
            Vector3 relativePos = hit.transform.position - PlayerTrans.position; // プレイヤーからの相対位置
            float distance = relativePos.magnitude;

            // 相対位置をミニマップの座標に変換
            Vector2 minimapPosition = new Vector2(-relativePos.x, -relativePos.z) / DetectionRadius * (MinimapRect.sizeDelta.x / 3);

            // アイコンの生成または再利用
            GameObject icon = GetIconFromPool();
            icon.SetActive(true);
            icon.GetComponent<RectTransform>().anchoredPosition = minimapPosition;

            currentIcons.Add(icon);
        }

        // プールに戻すアイコンを非表示にする
        foreach (var icon in activeIcons)
        {
            if (!currentIcons.Contains(icon))
            {
                icon.SetActive(false);
                ReturnIconToPool(icon);
            }
        }

        // 現在のアイコンリストを更新
        activeIcons = currentIcons;

    }

    /// <summary>
    /// プールからアイコンを取得
    /// </summary>
    /// <returns></returns>

    GameObject GetIconFromPool()
    {
        if (IconPool.Count > 0)
        {
            return IconPool.Dequeue(); // プールからアイコンを取得
        }
        else
        {
            // プールが空なら新しいアイコンを生成
            GameObject icon = Instantiate(EnemyIconPrefab);
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
        IconPool.Enqueue(icon); // プールに戻す
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
