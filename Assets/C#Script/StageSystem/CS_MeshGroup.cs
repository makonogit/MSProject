using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 担当:菅　メッシュをグループ化する
/// </summary>
public class CS_MeshGroup : MonoBehaviour
{
    [Header("破壊用パラメータ設定用")]
    [SerializeField] MeshFilter fragmentPrefab;
    [SerializeField] int numberOfPoints = 50;
    [SerializeField] float scaleRadius = 0.6f;
    [SerializeField] float minFragmentSize = 0.1f;
    [SerializeField] float remainingTime = 5f;
    [SerializeField] float fadeDuration = 5f;
    [SerializeField] AnimationCurve fadeCurve;

    private enum CollType
    {
        Box,
        Sphere,
        Capsule,
        Mesh,
    }

    [SerializeField, Header("付与するコライダーのタイプ")]
    private CollType type;

    // Start is called before the first frame update
    void Start()
    {
        //子オブジェクトのメッシュグループを探す
        
        List<Transform> ChildGroup = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            //メッシュグループじゃなければ結合グループに追加しない
            bool Group = transform.GetChild(i).childCount == 0;
            if (Group) { continue; }

            ChildGroup.Add(transform.GetChild(i).GetComponent<Transform>()); 
        }

        for (int i = 0; i < ChildGroup.Count; i++)
        {
            //メッシュグループ化
            MeshGrouping(ChildGroup[i]);
        }

        //子オブジェクトを全てグループ化したら全て統合
        MeshGrouping(ChildGroup);

        switch(type)
        {
            case CollType.Box:
                gameObject.AddComponent<BoxCollider>();
                break;
            case CollType.Sphere:
                gameObject.AddComponent<SphereCollider>();
                break;
            case CollType.Capsule:
                gameObject.AddComponent<CapsuleCollider>();
                break;
            case CollType.Mesh:
                gameObject.AddComponent<MeshCollider>();
                //GetComponent<MeshCollider>().convex = true;
                break;
            default:
                Debug.LogWarning("コライダーが設定されてないよー！");
                break;
        }
        
        gameObject.AddComponent<SimplestarGame.VoronoiFragmenter>();
        GetComponent<SimplestarGame.VoronoiFragmenter>().InitVoronoiFragment(
            fragmentPrefab, numberOfPoints, scaleRadius, minFragmentSize, remainingTime, fadeDuration, fadeCurve);
    }

    /// <summary>
    /// メッシュグループ化
    /// </summary>
    /// <param 親オブジェクト="ParentObj"></param>
    private void MeshGrouping(Transform ParentObj)
    {
        // 親オブジェクトのワールド変換行列（逆行列を取得）
        Matrix4x4 parentTransform = ParentObj.worldToLocalMatrix;

        List<MeshFilter> MeshMembers = new List<MeshFilter>();
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        //グループ化するオブジェクトのメッシュ
        for (int i = 0; i < ParentObj.transform.childCount; i++)
        {
            Transform child = ParentObj.GetChild(i);
            child.TryGetComponent(out MeshFilter filter);
            child.TryGetComponent(out MeshRenderer renderer);
            if (!filter || !renderer) { return; }
            MeshMembers.Add(filter);
            meshRenderers.Add(renderer);
        }

        CombineInstance[] Combine = new CombineInstance[MeshMembers.Count];
        Material[] materials = new Material[meshRenderers.Count];

        for (int i = 0; i < MeshMembers.Count; i++)
        {

            Combine[i].mesh = MeshMembers[i].sharedMesh;
            Combine[i].transform = parentTransform * MeshMembers[i].transform.localToWorldMatrix;

            // マテリアルを保存
            materials[i] = meshRenderers[i].sharedMaterial;

            //元の子オブジェクトを非表示
            MeshMembers[i].gameObject.SetActive(false);
        }

        // 結合されたメッシュを新しいオブジェクトに適用
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(Combine, true); //サブメッシュを統合

        // このオブジェクトにMeshFilterとMeshRendererを設定
        MeshFilter meshFilter = ParentObj.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshrenderer = ParentObj.gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = combinedMesh;
        meshrenderer.materials = materials;
        ParentObj.gameObject.SetActive(true);

    }

    
    /// <summary>
    /// 結合したメッシュをさらに結合する
    /// </summary>
    /// <param name="ChildObjList"></param>
    private void MeshGrouping(List<Transform> ChildObjList)
    {
        // 親オブジェクトのワールド変換行列（逆行列を取得）
        Matrix4x4 parentTransform = transform.worldToLocalMatrix;

        List<MeshFilter> MeshMembers = new List<MeshFilter>();
        List<Material> materialList = new List<Material>();
        List<CombineInstance> CombineInstances = new List<CombineInstance>();

        // グループ化するオブジェクトのメッシュを取得
        foreach (Transform child in ChildObjList)
        {
            if (child.TryGetComponent(out MeshFilter filter) && child.TryGetComponent(out MeshRenderer renderer))
            {
                Mesh mesh = filter.sharedMesh;
                Material[] materials = renderer.sharedMaterials;

                // サブメッシュごとにCombineInstanceを作成
                for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
                {
                    CombineInstance combineInstance = new CombineInstance
                    {
                        mesh = mesh,
                        subMeshIndex = subMeshIndex,
                        transform = parentTransform * filter.transform.localToWorldMatrix
                    };
                    CombineInstances.Add(combineInstance);

                    // マテリアルを収集
                    materialList.Add(materials[subMeshIndex]);
                }

                // 元の子オブジェクトを非表示
                child.gameObject.SetActive(false);
            }
        }

        // 結合されたメッシュを新しいオブジェクトに適用
        Mesh combinedMesh = new Mesh();
        //{
        //    indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 // これで65,535以上の頂点に対応
        //};
        combinedMesh.CombineMeshes(CombineInstances.ToArray(), true); //サブメッシュを統合

        // 新しいメッシュを現在のオブジェクトに適用
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = combinedMesh;
        meshRenderer.materials = materialList.ToArray(); // サブメッシュ対応のマテリアルを設定

        // オブジェクトを再表示
        gameObject.SetActive(true);
    }

}
