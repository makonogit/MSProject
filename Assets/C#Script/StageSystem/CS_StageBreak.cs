using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// �S��;���@�X�e�[�W����V�X�e��
/// </summary>
public class CS_StageBreak : MonoBehaviour
{
    [SerializeField, Header("����I�u�W�F�N�g���X�g")]
    private List<GameObject> BreakObjList;

    private int CurrentBreakNum = 0;    //���݉��Ă���I�u�W�F�N�g�ԍ�
    private GameObject AreaBlock;       //���݉��Ă���G���A�̍��W

    [SerializeField, Header("����G�t�F�N�g")]
    private GameObject BreakEffect;

    private int poolsize = 50; //�G�t�F�N�g���v�[�����鐔
    private Queue<GameObject> EffectPool;   //�G�t�F�N�g�v�[��

    [Header("���󌟒m�֘A")]
    [SerializeField,Tooltip("����T�C�Y")]
    private Vector3 BoxSize;
    [SerializeField, Tooltip("���肷��Arealayer")]
    private LayerMask AreaLayer;
    [SerializeField, Tooltip("���肷��layer")]
    private LayerMask layer;
    
    private int BreakObjCount = 0;     //��ꂽ�I�u�W�F�N�g�̐�
    private int MaxBreakObjCount = 0; //����I�u�W�F�N�g�̍ő吔
    private float BreakRate = 0;  //����x

    [Header("�ړ��֌W")]
    [SerializeField, Tooltip("���󃋁[�g")]
    private SplineAnimate splineanim;
    //[SerializeField, Tooltip("�ړ����x")]
    //private float MoveSpeed = 1f;
    

    // Start is called before the first frame update
    void Start()
    {
        //����I�u�W�F�N�g�̐���ۑ�(�S�Ďq�I�u�W�F�N�g)
        for (int i= 0; i < BreakObjList.Count; i++)
        {
            for(int j = 0; j < BreakObjList[i].transform.childCount; j++)
            {
                MaxBreakObjCount += BreakObjList[i].transform.GetChild(j).transform.childCount;
            }
        }
        
        AreaBlock = BreakObjList[CurrentBreakNum];
        //�J�n���̃R���C�_�[��S�ăA�N�e�B�u�ɂ���

        //�G�t�F�N�g�v�[���̏�����
        //EffectPool = new Queue<GameObject>();

        // �v�[���T�C�Y���̃G�t�F�N�g�𐶐����A��\���ɂ��ăL���[�ɒǉ�
        //for (int i = 0; i < poolsize; i++)
        //{
        //    GameObject effect = Instantiate(BreakEffect);
        //    effect.SetActive(false);
        //    EffectPool.Enqueue(effect);
        //}


        SetBreakArea();

        TemporaryStorage.DataRegistration("Stage", 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        BreakJudgment();
        ChangeArea();

        //�f�[�^���Z�[�u
        TemporaryStorage.DataSave("Stage",Mathf.FloorToInt(BreakObjCount / MaxBreakObjCount * 100f));
    }

    /// <summary>
    /// ���󌟒m(Ray)
    /// </summary>
    private void BreakJudgment()
    {
        //�����蔻��
        // �{�b�N�X���ɓ����Ă���R���C�_�[���擾
        Collider[] Hits = Physics.OverlapBox(transform.position, BoxSize / 2, transform.rotation, layer);

        bool hitflg = Hits.Length > 0;

        if (!hitflg) { return; }

        if (Hits[0].gameObject != null)
        {
            //�G�t�F�N�g����
            //Vector3 Hitpos = Hits[0].ClosestPoint(transform.position);
            //GetEffect(Hitpos, Quaternion.identity);
        }
        //�K�i�������}���u
        if (Hits[0].gameObject.name == "Collider") { Destroy(Hits[0].transform.parent.gameObject); }
        //layer��ύX
        //hit.transform.gameObject.layer = Breakedlayer;
        //�Փ˂����I�u�W�F�N�g������Ԃɂ���

        GameObject objectToDestroy = Hits[0].gameObject;

        Destroy(objectToDestroy);   //����
        BreakObjCount++;                    //��ꂽ�I�u�W�F�N�g�̐����J�E���g
    }

    /// <summary>
    /// �G���A�ύX
    /// </summary>
    private void ChangeArea()
    {
        // RaycastHit AreaHit;
        // �{�b�N�X���ɓ����Ă���R���C�_�[���擾
        Collider[] AreaHits = Physics.OverlapBox(transform.position, BoxSize / 2, transform.rotation, AreaLayer);

        //�����蔻��
        bool Areahitflg = AreaHits.Length > 0;
        if (!Areahitflg) { return; }

       
        GameObject oldarea = AreaBlock; //�O�񕪋L�^���Ă���
        AreaBlock = AreaHits[0].gameObject;

        bool ChangeAreaFlg = oldarea != AreaBlock;

        if (ChangeAreaFlg)
        {
            oldarea.GetComponent<BoxCollider>().isTrigger = true;
            if (BreakObjList.Count - 1 > CurrentBreakNum) { CurrentBreakNum++; }
            SetBreakArea(); //�G���A�ύX
        }

    }


    /// <summary>
    /// ����Ώۂ̃G���A���X�V
    /// </summary>
    private void SetBreakArea()
    {
        
        int child = BreakObjList[CurrentBreakNum].transform.childCount;

        for(int i = 0;i<child;i++)
        {
            GameObject breakobj = BreakObjList[CurrentBreakNum].transform.GetChild(i).gameObject;

            //�R���C�_�[��L����
            BoxCollider[] coll = breakobj.GetComponentsInChildren<BoxCollider>();
            foreach (var collider in coll)
            {
                //�K�i�������}���u
                if(collider.gameObject.name == "Stairs_TypeA") { collider.transform.GetChild(0).transform.GetComponent<BoxCollider>().enabled = true; }
                collider.enabled = true;
            }

        }

        //�G���A�R���C�_�[�𖳌���
        BoxCollider areacoll;
        BreakObjList[CurrentBreakNum].transform.TryGetComponent<BoxCollider>(out areacoll);
        areacoll.enabled = false;
    }


    /// <summary>
    /// Ray�̕\��
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, BoxSize);
    }

    /// <summary>
    /// ����x��Ԃ�
    /// </summary>
    /// <returns></returns>
    private int GetBreakRate()
    {
       return (int)(MaxBreakObjCount / BreakObjCount);
    }


    /// <summary>
    /// �v�[������G�t�F�N�g���擾
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public GameObject GetEffect(Vector3 position, Quaternion rotation)
    {
        if (EffectPool.Count > 0)
        {
            GameObject effect = EffectPool.Dequeue();
            effect.transform.position = position;
            effect.transform.rotation = rotation;
            effect.SetActive(true);

            // �p�[�e�B�N���V�X�e�����Đ�
            ParticleSystem particleSystem;
            effect.TryGetComponent<ParticleSystem>(out particleSystem);
            if (particleSystem != null)
            {
                particleSystem.Clear(); // �O��̍Đ��f�[�^���N���A
                particleSystem.Play();  // �Đ����J�n
            }

            // �g�p��ɍēx�v�[���ɖ߂��i���b��ɔ�\���j
            StartCoroutine(ReturnToPool(effect, 0.5f)); // 2�b�Ŗ߂���
            return effect;
        }
        else
        {
            Debug.LogWarning("�v�[������ł��B�V�����G�t�F�N�g�𐶐����܂��B");
            return Instantiate(BreakEffect, position, rotation);
        }
    }

    /// <summary>
    /// �G�t�F�N�g���\���ɂ��ăv�[���ɖ߂�
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private System.Collections.IEnumerator ReturnToPool(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effect.SetActive(false);
        EffectPool.Enqueue(effect);
    }

}
