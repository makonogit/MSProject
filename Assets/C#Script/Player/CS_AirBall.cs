using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CS_AirBall : MonoBehaviour
{

    [SerializeField, Header("�U����")]
    private float AttackPower = 1.0f;
    [SerializeField, Header("����")]
    private float AttackSpeed = 1.0f;


    [SerializeField, Header("�Փ�Effect")]
    private GameObject HitEffect;

    private Vector3 TargetPos;

    private float TimeMesure = 0.0f;

    public Vector3 TargetPosition
    {
        set
        {
            TargetPos = value;
        }
    }

    /// <summary>
    /// Power
    /// </summary>
    public float Power
    {
        get
        {
            return AttackPower;
        }
    }

    private void FixedUpdate()
    {

        TimeMesure += Time.deltaTime;
        transform.position += transform.forward * AttackSpeed * Time.deltaTime;

        //�����ʒu����O�����ɔ���

        //transform.Translate(transform.forward * AttackSpeed * Time.deltaTime);

        //if (TargetPos != Vector3.zero)
        //{
        //    transform.position = Vector3.Lerp(transform.position, TargetPos, AttackSpeed * Time.deltaTime);
        //}
        //else
        //{
        //    //�����ʒu����O�����ɔ���
        //    transform.Translate(transform.forward * AttackSpeed * Time.fixedDeltaTime);
        //    //transform.position += transform.forward * AttackSpeed * Time.deltaTime;
        //}

        if (TimeMesure > 30.0f)
        {
            Destroy(this.gameObject);   //�Փ˂����玩�M��j��
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    bool GimmickHit = collision.gameObject.tag == "Burst";

    //    if (GimmickHit)
    //    {

    //        CS_Burst_of_object burst = collision.transform.GetComponent<CS_Burst_of_object>();
    //        if (burst == null) { Debug.LogWarning("null component"); return; }
    //        burst.HitDamage(Power);

    //        //---------------------------------------------------------------
    //        // �Փ˂����I�u�W�F�N�g�ɍU���͂����Z����H�I�u�W�F�N�g���ł��H
    //        //GetComponent<�Փ˂����I�u�W�F�N�g�̃R���|�[�l���g>.�ϋv�l;
    //        //�ϋv�l - AttackPower;
    //        //���Ȃ炱��Ȋ����H

    //        ContactPoint contact = collision.contacts[0]; // �ŏ��̐ڐG�_���擾
    //        Vector3 collisionPoint = contact.point; // �Փ˂����ʒu

    //        Instantiate(HitEffect,collisionPoint,Quaternion.identity);     //�Փ˂����ʒu�ɃG�t�F�N�g

    //        Destroy(this.gameObject);   //�Փ˂����玩�M��j��
    //    }

    //    if (TimeMesure > 0.2f)
    //    {
    //        ContactPoint contact = collision.contacts[0]; // �ŏ��̐ڐG�_���擾
    //        Vector3 collisionPoint = contact.point; // �Փ˂����ʒu
    //        Instantiate(HitEffect,collisionPoint, Quaternion.identity);     //�Փ˂����ʒu�ɃG�t�F�N�g
    //        Destroy(this.gameObject);   //�Փ˂����玩�M��j��
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        bool GimmickHit = other.gameObject.CompareTag("Burst");

        if (GimmickHit)
        {
            // �Փ˂����I�u�W�F�N�g���� CS_Burst_of_object �R���|�[�l���g���擾
            CS_Burst_of_object burst = other.transform.GetComponent<CS_Burst_of_object>();
            if (burst == null)
            {
                return;
            }

            // �_���[�W��^����
            burst.HitDamage(Power);

            //---------------------------------------------------------------
            // �����ϋv�l�����炷�Ȃǂ̏�����ǉ��������ꍇ�́A�R�����g�A�E�g�����̂悤�ɏ����܂�:
            // GetComponent<�Փ˂����I�u�W�F�N�g�̃R���|�[�l���g>.�ϋv�l -= AttackPower;

            // �Փ˂����ʒu�ɃG�t�F�N�g���C���X�^���X��
            Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position); // �Փ˓_�̋߂��ʒu���擾
            Instantiate(HitEffect, collisionPoint, Quaternion.identity);

            // �I�u�W�F�N�g��j��
            Destroy(gameObject); // ���̃I�u�W�F�N�g��j��
        }

        // TimeMesure �� 0.2f ���傫���ꍇ�̏���
        if (TimeMesure > 0.2f)
        {
            // �Փ˓_�ɃG�t�F�N�g���C���X�^���X��
            Vector3 collisionPoint = other.ClosestPointOnBounds(transform.position); // �Փ˓_�̋߂��ʒu���擾
            Instantiate(HitEffect, collisionPoint, Quaternion.identity);

            // �I�u�W�F�N�g��j��
            Destroy(gameObject);
        }
    }


}
