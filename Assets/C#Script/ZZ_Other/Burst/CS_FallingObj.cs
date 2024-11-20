using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����I�u�W�F�N�g
/// </summary>
public class CS_FallingObj : MonoBehaviour
{
    [SerializeField, Header("����Effect")]
    private GameObject BreakEffect;
    [SerializeField]
    private GameObject BreakDust;

    [SerializeField,Header("Player�����m����͈�(���a)")]
    private float DetectionRadius = 10.0f;
    [SerializeField,Header("���m���郌�C���[")]
    private LayerMask DetectionLayer;

    [Header("��C��R")]
    [SerializeField,Tooltip("�傫���قǗ�����̒x����[")]
    private float FallingDrag = 5.0f;

    [SerializeField, Header("RigidBody")]
    private Rigidbody thisrd;

    private void FixedUpdate()
    {
        DetectObjectsInRange();

    }


    /// <summary>
    /// �͈͓���Player�������������m���ďd�͂𔭓�������֐�
    /// </summary>

    void DetectObjectsInRange()
    {
        // Sphere�Ŕ͈͓��̃I�u�W�F�N�g�����m
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRadius, DetectionLayer);

        foreach (var hitCollider in hitColliders)
        {
            // ���m�����I�u�W�F�N�g�ɑ΂��ĉ����������s��
            bool PlayerHit = hitCollider.CompareTag("Player");
            if (PlayerHit)
            {
                thisrd.drag = FallingDrag;
                thisrd.useGravity = true;
                
            }
        }
    }

    // �͈͂��������邽�߂̃f�o�b�O�p
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, DetectionRadius);
    }

    /// <summary>
    /// ���ƐڐG��������ł���
    /// </summary>
    /// <param �Փ˕�="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //�����ƐڐG���������
        Destroy(this.gameObject);

        Vector3 pos = transform.position;
        Instantiate(BreakEffect,pos,Quaternion.identity);
        //Instantiate(BreakDust,pos,Quaternion.identity);

        //�v���C���[�ƐڐG������
        bool HitFloor = collision.gameObject.tag == "Player";
        if (HitFloor)
        {
            //Destroy(this.gameObject);
        }
    }
}
