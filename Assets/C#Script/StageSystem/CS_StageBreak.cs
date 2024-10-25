using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// �S��;���@�X�e�[�W����V�X�e��
/// </summary>
public class CS_StageBreak : MonoBehaviour
{
    [SerializeField, Header("Effect")]
    private ParticleSystem BreakEffect;

    [Header("���󌟒m�֘A")]
    [SerializeField,Tooltip("����T�C�Y")]
    private Vector3 BoxSize;
    [SerializeField, Tooltip("���肷��layer")]
    private LayerMask layer;
    [SerializeField, Tooltip("������Layer")]
    private LayerMask Breakedlayer;

    private int BreakObjCount = 0;  //����I�u�W�F�N�g�̐�
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
        BreakObjCount = transform.childCount;

        //�ړ����x��ݒ�
        splineanim.Duration = MoveSpeed;

    }

    // Update is called once per frame
    void FixedUpdate()
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
        bool hitflg = Physics.BoxCast(transform.position, BoxSize, Vector3.forward, out hit,Quaternion.identity, 0f, layer);

        if (!hitflg) { return; }

        //layer��ύX
        hit.transform.gameObject.layer = Breakedlayer;
        Debug.Log(hit);
        //�Փ˂����I�u�W�F�N�g������Ԃɂ���
        hit.collider.GetComponent<Rigidbody>().useGravity = true;   
        BreakObjCount--;    //�S�̂̔j��I�u�W�F�N�g�������炷
    }


    /// <summary>
    /// Ray�̕\��
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + Vector3.forward, BoxSize);
    }


}
