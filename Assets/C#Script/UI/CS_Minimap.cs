using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S���F���@Minimap�V�X�e��
/// </summary>
public class CS_Minimap : MonoBehaviour
{
    
    [SerializeField, Header("�v���C���[�ʒu")]
    private Transform PlayerTrans;

    [Header("���m�͈�")]
    [SerializeField,Tooltip("���a")]
    private float DetectionRadius = 1.0f;
    //[SerializeField, Tooltip("����")]
    //private float DetectionHeight = 1.0f;

    [SerializeField, Header("���m�Ώ�Layer")]
    private LayerMask DetectionLayer;

    [Space(10)]
    [Header("=========�T�����i�L�P��===========")]
    [SerializeField,Header("�G�A�C�R����Prefab")]
    private GameObject EnemyIconPrefab;
    [SerializeField, Header("�A�C�e���A�C�R����Prefab")]
    private GameObject ItemIconPrefab;
    [SerializeField, Header("MinmapUITrans")]
    private RectTransform MinimapRect;
    [SerializeField, Header("�A�C�R��MaskObject")]
    private RectTransform IconMaskObj;

    private List<GameObject> activeIcons = new List<GameObject>(); // �A�C�R�����X�g

    [SerializeField, Header("�A�C�R���̃v�[����")]
    private int IconPoolNum = 30;

    private Queue<GameObject> IconPool = new Queue<GameObject>(); // �A�C�R���p�̃v�[��

    // Start is called before the first frame update
    void Start()
    {
        //// �v���C���[�̌��݂�Y���W�������ɕϊ�
        //float progress = Mathf.InverseLerp(StartPosition.y, EndPosition.y, PlayerTrans.position.y);

        //// Minimap��̈ʒu��ݒ�
        //Vector2 markerPosition = MinimapSize.rect.min + new Vector2(0, MinimapSize.rect.height * progress);
        //MiniPlayerPin.anchoredPosition = markerPosition;

        ////����UI�̃T�C�Y��ݒ�
        //BreakUImaxsize = BreakNowPos.sizeDelta.y;
        //BreakNowPos.sizeDelta = new Vector2(BreakNowPos.sizeDelta.x, 0f);
        //BreakedPos.sizeDelta = new Vector2(BreakedPos.sizeDelta.x, 0f);

        // ������: �A�C�R�����v�[���Ɏ��O�ɒǉ����Ă���
        for (int i = 0; i < IconPoolNum; i++) // 50�̃A�C�R�����v�[���ɒǉ������
        {
            GameObject icon = Instantiate(EnemyIconPrefab);
            icon.SetActive(false); // �ŏ��͔�\��
            icon.transform.SetParent(IconMaskObj);
            icon.GetComponent<RectTransform>().localScale = new Vector2(1.5f,1.5f);
            IconPool.Enqueue(icon);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(PlayerTrans.position, DetectionRadius, DetectionLayer);

        // �A�C�R���̕\��
        List<GameObject> currentIcons = new List<GameObject>(); // ���ݕ\������A�C�R����ێ�

        foreach (var hit in colliders)
        {
            Vector3 relativePos = hit.transform.position - PlayerTrans.position; // �v���C���[����̑��Έʒu
            float distance = relativePos.magnitude;

            // ���Έʒu���~�j�}�b�v�̍��W�ɕϊ�
            Vector2 minimapPosition = new Vector2(-relativePos.x, -relativePos.z) / DetectionRadius * (MinimapRect.sizeDelta.x / 3);

            // �A�C�R���̐����܂��͍ė��p
            GameObject icon = GetIconFromPool();
            icon.SetActive(true);
            icon.GetComponent<RectTransform>().anchoredPosition = minimapPosition;

            currentIcons.Add(icon);
        }

        // �v�[���ɖ߂��A�C�R�����\���ɂ���
        foreach (var icon in activeIcons)
        {
            if (!currentIcons.Contains(icon))
            {
                icon.SetActive(false);
                ReturnIconToPool(icon);
            }
        }

        // ���݂̃A�C�R�����X�g���X�V
        activeIcons = currentIcons;

    }

    /// <summary>
    /// �v�[������A�C�R�����擾
    /// </summary>
    /// <returns></returns>

    GameObject GetIconFromPool()
    {
        if (IconPool.Count > 0)
        {
            return IconPool.Dequeue(); // �v�[������A�C�R�����擾
        }
        else
        {
            // �v�[������Ȃ�V�����A�C�R���𐶐�
            GameObject icon = Instantiate(EnemyIconPrefab);
            icon.transform.SetParent(MinimapRect);
            return icon;
        }
    }

    /// <summary>
    /// �����A�C�R�����v�[���ɖ߂�
    /// </summary>
    /// <param �A�C�R��Object="icon"></param>
    void ReturnIconToPool(GameObject icon)
    {
        IconPool.Enqueue(icon); // �v�[���ɖ߂�
    }

    /// <summary>
    /// Ray��\��
    /// </summary>
    private void OnDrawGizmos()
    {
        //�v���C���[���m�͈�
        Gizmos.color = Color.yellow;
        //Gizmos.matrix = Matrix4x4.TRS(, transform.rotation, Vector3.one);
        Gizmos.DrawWireSphere(PlayerTrans.position,DetectionRadius);

        //DrawCylinderGizmo(PlayerTrans.position, DetectionRadius, DetectionHeight);
    }


    void DrawCylinderGizmo(Vector3 position, float radius, float height)
    {
        // �~���̒�ʂƏ�ʂ�`��
        Gizmos.DrawWireSphere(position, radius);  // ���
        Gizmos.DrawWireSphere(position + Vector3.up * height, radius);  // ���

        // ��ʂƒ�ʂ��Ȃ�����`��
        int segmentCount = 36; // �~���𕪊����鐔�i�����قǊ��炩�j
        for (int i = 0; i < segmentCount; i++)
        {
            float angleA = i * Mathf.PI * 2 / segmentCount;
            float angleB = (i + 1) * Mathf.PI * 2 / segmentCount;
            Vector3 pointA = position + new Vector3(Mathf.Cos(angleA) * radius, 0, Mathf.Sin(angleA) * radius);
            Vector3 pointB = position + new Vector3(Mathf.Cos(angleB) * radius, 0, Mathf.Sin(angleB) * radius);
            Gizmos.DrawLine(pointA, pointB);  // ��ʂ̉~����`��

            pointA = position + new Vector3(Mathf.Cos(angleA) * radius, height, Mathf.Sin(angleA) * radius);
            pointB = position + new Vector3(Mathf.Cos(angleB) * radius, height, Mathf.Sin(angleB) * radius);
            Gizmos.DrawLine(pointA, pointB);  // ��ʂ̉~����`��

            Gizmos.DrawLine(position + new Vector3(Mathf.Cos(angleA) * radius, 0, Mathf.Sin(angleA) * radius),
                            position + new Vector3(Mathf.Cos(angleA) * radius, height, Mathf.Sin(angleA) * radius)); // �㉺���q�����C��
        }
    }

}
