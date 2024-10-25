using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// �S��;���@�X�e�[�W����V�X�e��
/// </summary>
public class CS_StageBreak : MonoBehaviour
{
    [SerializeField, Header("�X�e�[�W�Ǘ��e�I�u�W�F�N�g")]
    private GameObject StageObj;

    [Header("���󌟒m�֘A")]
    [SerializeField,Tooltip("����T�C�Y")]
    private Vector3 BoxSize;
    [SerializeField, Tooltip("���肷��layer")]
    private LayerMask layer;
    [SerializeField, Tooltip("������Layer�ԍ�")]
    private int Breakedlayer;
    
    private int BreakObjCount = 0;     //��ꂽ�I�u�W�F�N�g�̐�
    private int MaxBreakObjCount = 0; //����I�u�W�F�N�g�̍ő吔
    private float BreakRate = 0;  //����x

    [Header("�ړ��֌W")]
    [SerializeField, Tooltip("���󃋁[�g")]
    private SplineAnimate splineanim;
    [SerializeField, Tooltip("�ړ����x")]
    private float MoveSpeed = 1f;
    

    // Start is called before the first frame update
    void Start()
    {
        //����I�u�W�F�N�g�̐���ۑ�(�S�Ďq�I�u�W�F�N�g)
        MaxBreakObjCount = StageObj.transform.childCount;

        //�ړ����x��ݒ�
        splineanim.Duration = MoveSpeed;

        Debug.Log(MaxBreakObjCount);

    }

    // Update is called once per frame
    void Update()
    {
        BreakJudgment();
    }

    /// <summary>
    /// ���󌟒m(Ray)
    /// </summary>
    private void BreakJudgment()
    {
        RaycastHit hit;

        //�����蔻��
        bool hitflg = Physics.BoxCast(transform.position, BoxSize, Vector3.one, out hit,Quaternion.identity, 5f, layer);

        if (!hitflg) { return; }

        //layer��ύX
        hit.transform.gameObject.layer = Breakedlayer;
        //�Փ˂����I�u�W�F�N�g������Ԃɂ���
        hit.collider.GetComponent<Rigidbody>().useGravity = true;   
        BreakObjCount++;    //��ꂽ�I�u�W�F�N�g�̐����J�E���g
    }


    /// <summary>
    /// Ray�̕\��
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + Vector3.forward, BoxSize);
    }

    /// <summary>
    /// ����x��Ԃ�
    /// </summary>
    /// <returns></returns>
    private int GetBreakRate()
    {
       return (int)(MaxBreakObjCount / BreakObjCount);
    }

}
