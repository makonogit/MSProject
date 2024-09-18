using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_AirBall : MonoBehaviour
{

    [SerializeField, Header("�U����")]
    private float AttackPower = 1.0f;
    
    private void FixedUpdate()
    {
        //�����ʒu����O�����ɔ���
        transform.position += Vector3.forward * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool GimmickHit = collision.gameObject.tag == "Gimmick";
        
        if (GimmickHit)
        {
            //---------------------------------------------------------------
            // �Փ˂����I�u�W�F�N�g�ɍU���͂����Z����H�I�u�W�F�N�g���ł��H
            //GetComponent<�Փ˂����I�u�W�F�N�g�̃R���|�[�l���g>.�ϋv�l;
            //�ϋv�l - AttackPower;
            //���Ȃ炱��Ȋ����H
            
            Destroy(this.gameObject);   //�Փ˂����玩�M��j��
        }
        
    }

}
