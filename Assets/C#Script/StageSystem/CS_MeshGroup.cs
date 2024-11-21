using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S��:���@���b�V�����O���[�v������
/// </summary>
public class CS_MeshGroup : MonoBehaviour
{
    [Header("�j��p�p�����[�^�ݒ�p")]
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

    [SerializeField, Header("�t�^����R���C�_�[�̃^�C�v")]
    private CollType type;

    // Start is called before the first frame update
    void Start()
    {
        //�q�I�u�W�F�N�g�̃��b�V���O���[�v��T��
        
        List<Transform> ChildGroup = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            //���b�V���O���[�v����Ȃ���Ό����O���[�v�ɒǉ����Ȃ�
            bool Group = transform.GetChild(i).childCount == 0;
            if (Group) { continue; }

            ChildGroup.Add(transform.GetChild(i).GetComponent<Transform>()); 
        }

        for (int i = 0; i < ChildGroup.Count; i++)
        {
            //���b�V���O���[�v��
            MeshGrouping(ChildGroup[i]);
        }

        //�q�I�u�W�F�N�g��S�ăO���[�v��������S�ē���
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
                Debug.LogWarning("�R���C�_�[���ݒ肳��ĂȂ���[�I");
                break;
        }
        
        gameObject.AddComponent<SimplestarGame.VoronoiFragmenter>();
        GetComponent<SimplestarGame.VoronoiFragmenter>().InitVoronoiFragment(
            fragmentPrefab, numberOfPoints, scaleRadius, minFragmentSize, remainingTime, fadeDuration, fadeCurve);
    }

    /// <summary>
    /// ���b�V���O���[�v��
    /// </summary>
    /// <param �e�I�u�W�F�N�g="ParentObj"></param>
    private void MeshGrouping(Transform ParentObj)
    {
        // �e�I�u�W�F�N�g�̃��[���h�ϊ��s��i�t�s����擾�j
        Matrix4x4 parentTransform = ParentObj.worldToLocalMatrix;

        List<MeshFilter> MeshMembers = new List<MeshFilter>();
        List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

        //�O���[�v������I�u�W�F�N�g�̃��b�V��
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

            // �}�e���A����ۑ�
            materials[i] = meshRenderers[i].sharedMaterial;

            //���̎q�I�u�W�F�N�g���\��
            MeshMembers[i].gameObject.SetActive(false);
        }

        // �������ꂽ���b�V����V�����I�u�W�F�N�g�ɓK�p
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(Combine, true); //�T�u���b�V���𓝍�

        // ���̃I�u�W�F�N�g��MeshFilter��MeshRenderer��ݒ�
        MeshFilter meshFilter = ParentObj.gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshrenderer = ParentObj.gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = combinedMesh;
        meshrenderer.materials = materials;
        ParentObj.gameObject.SetActive(true);

    }

    
    /// <summary>
    /// �����������b�V��������Ɍ�������
    /// </summary>
    /// <param name="ChildObjList"></param>
    private void MeshGrouping(List<Transform> ChildObjList)
    {
        // �e�I�u�W�F�N�g�̃��[���h�ϊ��s��i�t�s����擾�j
        Matrix4x4 parentTransform = transform.worldToLocalMatrix;

        List<MeshFilter> MeshMembers = new List<MeshFilter>();
        List<Material> materialList = new List<Material>();
        List<CombineInstance> CombineInstances = new List<CombineInstance>();

        // �O���[�v������I�u�W�F�N�g�̃��b�V�����擾
        foreach (Transform child in ChildObjList)
        {
            if (child.TryGetComponent(out MeshFilter filter) && child.TryGetComponent(out MeshRenderer renderer))
            {
                Mesh mesh = filter.sharedMesh;
                Material[] materials = renderer.sharedMaterials;

                // �T�u���b�V�����Ƃ�CombineInstance���쐬
                for (int subMeshIndex = 0; subMeshIndex < mesh.subMeshCount; subMeshIndex++)
                {
                    CombineInstance combineInstance = new CombineInstance
                    {
                        mesh = mesh,
                        subMeshIndex = subMeshIndex,
                        transform = parentTransform * filter.transform.localToWorldMatrix
                    };
                    CombineInstances.Add(combineInstance);

                    // �}�e���A�������W
                    materialList.Add(materials[subMeshIndex]);
                }

                // ���̎q�I�u�W�F�N�g���\��
                child.gameObject.SetActive(false);
            }
        }

        // �������ꂽ���b�V����V�����I�u�W�F�N�g�ɓK�p
        Mesh combinedMesh = new Mesh();
        //{
        //    indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 // �����65,535�ȏ�̒��_�ɑΉ�
        //};
        combinedMesh.CombineMeshes(CombineInstances.ToArray(), true); //�T�u���b�V���𓝍�

        // �V�������b�V�������݂̃I�u�W�F�N�g�ɓK�p
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = combinedMesh;
        meshRenderer.materials = materialList.ToArray(); // �T�u���b�V���Ή��̃}�e���A����ݒ�

        // �I�u�W�F�N�g���ĕ\��
        gameObject.SetActive(true);
    }

}
