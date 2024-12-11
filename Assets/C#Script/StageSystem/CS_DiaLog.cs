//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class CS_DiaLog : MonoBehaviour
//{
//    [SerializeField, Header("�\���������_�C�A���OImege")]
//    private List<Sprite> DialogSprite;

//    [SerializeField, Tooltip("�_�C�A���O")]
//    private Image DiaLogImage;
//    [SerializeField, Tooltip("�_�C�A���OTrans")]
//    private RectTransform DiaLogTrans;

//    //�_�C�A���O�̃T�C�Y�{��(�J�n���Ɏ擾)
//    private Vector3 DialogScale;
//    [SerializeField, Tooltip("�v���C���[�̌��m�͈�")]
//    private float DitictionRadius = 5f;
//    [SerializeField, Tooltip("�v���C���[�̌��mlayer")]
//    private LayerMask PlayerLayer;
//    [SerializeField, Tooltip("�_�C�A���O���J���X�s�[�h")]
//    private float OpenSpeed = 1f;

//    //�\���t���O
//    [SerializeField] private bool ViewFlg = false;

//    //���ݕ\�����Ă���X�v���C�g
//    private int CurrentDialogSprite = 0;

//    [SerializeField, Header("���͊֌W")]
//    private CS_InputSystem Input;

//    // Start is called before the first frame update
//    void Start()
//    {
//        //�ŏ��̃_�C�A���O�摜��ݒ�
//        DiaLogImage.sprite = DialogSprite[CurrentDialogSprite];
//        DialogScale = DiaLogTrans.localScale;
//        //�_�C�A���O���\���ɂ��Ă���
//        DiaLogTrans.localScale = Vector3.zero;
//    }

//    // Update is called once per frame
//    void Update()
//    {

//        //�v���C���[�����m���ĕ\���A��\���̐؂�ւ�
//        Collider[] hits = Physics.OverlapSphere(transform.position, DitictionRadius, PlayerLayer);

//        foreach (Collider hit in hits)
//        {
//            Debug.Log(hit);
//            bool PlayerHit = hit.CompareTag("Player");
//            if (PlayerHit && !ViewFlg)
//            {
//                ViewFlg = true;
//                Time.timeScale = 0f;    //�ꎞ��~����
//            }
//            if (!PlayerHit && ViewFlg)
//            {
//                ViewFlg = false;
//            }
//        }

//        //�\������ĂȂ������珈�����ȁ[��
//        if (!ViewFlg) { return; }

//        //�_�C�A���O�̕\��(�T�C�Y��傫�����Ă���)
//        DiaLogTrans.localScale = Vector3.Lerp(DiaLogTrans.localScale, DialogScale, OpenSpeed * 0.01f);

//        //�_�C�A���O�̕\��
//        DialogInfo();

//    }

//    /// <summary>
//    /// �_�C�A���O�̕\���X�V
//    /// </summary>
//    private void DialogInfo()
//    {
     
//        bool BButton = Input.GetButtonBTriggered();

//        if (!BButton) { return; }

//        //�S�ĕ\��������_�C�A���O�\���I��
//        if (CurrentDialogSprite >= DialogSprite.Count)
//        {
//            Time.timeScale = 1f;
//            DiaLogTrans.localScale = Vector3.zero;
//        }

//        //�X�v���C�g�̍X�V
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
//    //    //�v���C���[����������\��
//    //    bool PlayerHit = other.tag == "Player";
//    //    if(PlayerHit)
//    //    {
//    //        ViewFlg = true;
//    //        Time.timeScale = 0f;    //�ꎞ��~����
//    //    }
//    //}

//    //private void OnCollisionEnter(Collision collision)
//    //{
//    //    //�v���C���[����������\��
//    //    bool PlayerHit = collision.transform.tag == "Player";
//    //    if (PlayerHit)
//    //    {
//    //        ViewFlg = true;
//    //        Time.timeScale = 0f;    //�ꎞ��~����
//    //    }
//    //}
//}
