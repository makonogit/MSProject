using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    
    [Header("�G�̃Z���T�[���x����")]
    [Header("�����x���ő勗��(���m�͈�)�̉��p�[�Z���g�̊�������Ƃ���̂���")]

    [Tooltip("������")]
    [Range(0, 100)] public float EnemySencorCautionRatio = 75f; // Safe �̊����i��F�ő勗����75%�j
    [Tooltip("��")]
    [Range(0, 100)] public float EnemySencorWarningRatio = 50f; // Caution �̊����i��F�ő勗����50%�j
    [Tooltip("�Z����")]
    [Range(0, 100)] public float EnemySencorDangerRatio = 25f; // Warning �̊����i��F�ő勗����25%�j

   
    [SerializeField, Header("���m�Ώ�Layer")]
    private LayerMask DetectionLayer;

    [Space(10)]
    [Header("=========�T�����i�L�P��===========")]
    [SerializeField,Header("�G�Z���T�[�̊Ǘ��I�u�W�F�N�g")]
    private GameObject EnemySencorGroup;
    [SerializeField, Header("�G�Z���T�[�̒��S")]
    private GameObject EnemySencorCenter;
    [SerializeField, Header("�ʋl�A�C�R����Prefab")]
    private GameObject CanIconPrefab;
    [SerializeField, Header("�R�A�A�C�R����UI")]
    private GameObject CoreIconImage;
    [SerializeField, Header("MinmapUITrans")]
    private RectTransform MinimapRect;
    [SerializeField, Header("�A�C�R��MaskObject")]
    private RectTransform IconMaskObj;

    [SerializeField,Header("�~�j�}�b�v�̘g")]
    private RectTransform MinimapFrame;

    [Header("���m�Ώۂ�Tag��")]
    [SerializeField,Tooltip("�G")]
    private string EnemyTagName;
    [SerializeField, Tooltip("�ʋl")]
    private string CanTagName;
    [SerializeField, Tooltip("�R�A")]
    private string CoreTagName;

    [SerializeField, Header("�A�C�R���̃v�[����")]
    private int IconPoolNum = 20;

    private List<GameObject> activeIcons = new List<GameObject>(); // �A�C�R�����X�g
    private List<int> activeEnemySencor = new List<int>(); //�G�l�~�[�Z���T�[���X�g

    private Queue<GameObject> CanIconPool = new Queue<GameObject>();    // �ʋl�A�C�R���p�̃v�[��
   
    //16�����̃Z���T�[UI
    private GameObject[] EnemySencorSection = new GameObject[16];

    // Start is called before the first frame update
    void Start()
    {
        //�G�Z���T�[�pUI���擾���Ĕ�A�N�e�B�u�ɂ��Ă���
        for(int i = 0; i< EnemySencorSection.Length;i++)
        {
            EnemySencorSection[i] = EnemySencorGroup.transform.GetChild(i).gameObject;
            EnemySencorSection[i].SetActive(false);
        }

        //�Z���T�[�̊p�x

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
        for (int i = 0; i < IconPoolNum; i++) 
        {
            GameObject icon = Instantiate(CanIconPrefab);
            icon.SetActive(false); // �ŏ��͔�\��
            icon.transform.SetParent(IconMaskObj);
            icon.GetComponent<RectTransform>().localScale = new Vector2(1.5f,1.5f);
            CanIconPool.Enqueue(icon);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //�~�j�}�b�v�̘g�񂵂�������肵��
        MinimapFrame.localRotation = Quaternion.Euler(0, 0, PlayerTrans.eulerAngles.y);


        Collider[] colliders = Physics.OverlapSphere(PlayerTrans.position, DetectionRadius, DetectionLayer);

        // �A�C�R���̕\��
        List<GameObject> currentIcons = new List<GameObject>(); // ���ݕ\������A�C�R����ێ�
        List<int> currentEnemyIndex = new List<int>();          //���ݕ\�����Ă���G�̃Z���T�[�ԍ�

        //�R�A�̔�\��
        if (CoreIconImage.activeSelf) { CoreIconImage.SetActive(false); }
        //�����Z���T�[�̔�\��
        if (EnemySencorCenter.activeSelf) { EnemySencorCenter.SetActive(false); }

        foreach (var hit in colliders)
        {
            //���m�Ώۂ�����
            bool Enemyhit = hit.tag == EnemyTagName;
            bool CanHit = hit.tag == CanTagName;
            bool CoreHit = hit.tag == CoreTagName;


            Vector3 relativePos = hit.transform.position - PlayerTrans.position; // �v���C���[����̑��Έʒu
            float distance = relativePos.magnitude;


            // �I�u�W�F�N�g�̊p�x���v�Z
            Vector3 forward = PlayerTrans.forward; // �v���C���[�̑O����
            forward.y = 0; // �����ʂł̌v�Z�Ɍ���
            relativePos.y = 0; // �����ʂł̌v�Z�Ɍ���
            float angle = Vector3.SignedAngle(forward, relativePos, Vector3.up);

            // ���Έʒu���~�j�}�b�v�̍��W�ɕϊ�
            Vector2 minimapPosition = new Vector2(-relativePos.x, -relativePos.z) / DetectionRadius * (MinimapRect.sizeDelta.x / 3);
            // �p�x�𗘗p���Ĉʒu��␳����
            minimapPosition = new Vector2(-minimapPosition.x, -minimapPosition.y * Mathf.Cos(angle * Mathf.Deg2Rad));

            // �G�����m�����ꍇ�̓Z���T�[��\��
            if (Enemyhit)
            {

                // �p�x��0���`360���ɕϊ�
                if (angle < 0)
                {
                    angle += 360;
                }

                // �\������Z���T�[�ԍ����v�Z�i�Z�N�V������22.5�����݁j
                int index = Mathf.FloorToInt(angle / 22.5f);
                
                
                // �Ώۂ̃Z���T�[��\��
                EnemySencorSection[index].SetActive(true);
                currentEnemyIndex.Add(index);

                // -------------�����ɂ���ē����x��ύX����--------------

                // �ő匟�m�����ɑ΂��銄�����v�Z (0�`1�̒l)
                float distanceRatio = Mathf.Clamp01(distance / DetectionRadius);
                // �p�[�Z���g�\�L�ɕϊ�
                float distancePercent = distanceRatio * 100;


                Image SencorImage = EnemySencorSection[index].GetComponent<Image>();

                if (distancePercent >= EnemySencorCautionRatio) //������
                {
                    SencorImage.color = new Color(1, 1, 1, 0.25f);
                }
                else if (distancePercent >= EnemySencorWarningRatio)   //�ԁH
                {
                    SencorImage.color = new Color(1, 1, 1, 0.5f);
                }
                else if (distancePercent >= EnemySencorDangerRatio) //�Z����
                {
                    SencorImage.color = new Color(1, 1, 1, 0.9f);
                }
                else
                {
                    EnemySencorSection[index].SetActive(false);
                    currentEnemyIndex.Remove(index);
                    EnemySencorCenter.SetActive(true);          //���S
                }

                    
                
            }

            //�ʋl�̏ꍇ�̓v�[������
            if (CanHit)
            {
                // �A�C�R���̐����܂��͍ė��p
                GameObject icon = GetIconFromPool();
                icon.SetActive(true);
                icon.GetComponent<RectTransform>().anchoredPosition = minimapPosition;

                currentIcons.Add(icon);
            }

            //�R�A�̏ꍇ�͈ʒu��ύX
            if (CoreHit)
            {
                CoreIconImage.SetActive(true);
                CoreIconImage.GetComponent<RectTransform>().anchoredPosition = minimapPosition;
            }


        }

        //�͈͊O�ɏo�Ă���ꍇ�ɂ͔�\���ɂ���
        {
            // �v�[���ɖ߂��A�C�R�����\���ɂ���
            foreach (var icon in activeIcons)
            {
                if (!currentIcons.Contains(icon))
                {
                    icon.SetActive(false);
                    ReturnIconToPool(icon);
                }
            }

            //�G�l�~�[���\��
            foreach(int index in activeEnemySencor)
            {
                if(!currentEnemyIndex.Contains(index))
                {
                    //EnemySencorSection[index].TryGetComponent<Image>(out Image image);
                    
                    EnemySencorSection[index].SetActive(false);
                }
            }

            // ���݂̃A�C�R�����X�g���X�V
            activeIcons = currentIcons;
            activeEnemySencor = currentEnemyIndex;

            
            
        }
    }

    /// <summary>
    /// �v�[������A�C�R�����擾
    /// </summary>
    /// <returns></returns>

    GameObject GetIconFromPool()
    {
        if (CanIconPool.Count > 0)
        {
            return CanIconPool.Dequeue(); // �v�[������A�C�R�����擾
        }
        else
        {
            // �v�[������Ȃ�V�����A�C�R���𐶐�
            GameObject icon = Instantiate(CanIconPrefab);
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
        CanIconPool.Enqueue(icon); // �v�[���ɖ߂�
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
