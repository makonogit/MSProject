//------------------------------------------
// ��C�C/���h���̊֐���`
// ���ƂŃv���C���[�ɍ���
//------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_AirGun : MonoBehaviour
{
    [SerializeField, Header("��C�C�̍U����")]
    private float AirAttackPowar = 1.0f;

    [SerializeField, Header("��C�C�̒e�I�u�W�F�N�g")]
    private GameObject AirBall;

    [SerializeField, Header("���h���̒����Ԋu")]
    [Header("���U����/�����Ԋu")]
    private const float Injection_Interval = 0.5f;

    private const float MaxPressure = 3.0f; //�ő刳��

    //�����Ԋu�v�Z�p
    private float Injection_IntarvalTime = 0.0f;

    private void FixedUpdate()
    {
        //����Ă݂�
        AirGun(KeyCode.E, false);
    }

    //----------------------------
    // ��C�C�֐�
    // ����:���̓L�[,�I�u�W�F�N�g�ɋ߂Â��Ă��邩
    // �߂�l�F�Ȃ�
    //----------------------------
    void AirGun(KeyCode key, bool ObjDistance)
    {
        //���ˉ\��(�L�[�������ꂽ�u��&�I�u�W�F�N�g�ɋ߂Â��Ă��Ȃ�)
        bool StartShooting = Input.GetKeyDown(key) && !ObjDistance;
        
        if (!StartShooting) { return; }

        //���͂�����Βe�𐶐�
        //�|�C���^�̈ʒu����@Instantiate(AirBall,transform.pointa);
        GameObject ballobj = Instantiate(AirBall);

    }

    //----------------------------
    // ���h��(��C����)�֐�
    // ����:���̓L�[,�߂Â��Ă��邩,�߂Â��Ă���I�u�W�F�N�g�̈���,�߂Â��Ă���I�u�W�F�N�g�̑ϋv�l
    // �߂�l�F�Ȃ�
    //----------------------------
    void AirInjection(KeyCode key,bool ObjDistance,float ObjPressure,float ObjDurability)
    {
        //�����\��(�L�[�����͂���Ă��ăI�u�W�F�N�g�ɋ߂Â��Ă���)
        bool Injection = Input.GetKey(key) && ObjDistance;
        
        if (!Injection) { return; }

        //���Ԍv��
        Injection_IntarvalTime += Time.deltaTime;
        bool TimeProgress = Injection_IntarvalTime > Injection_Interval;   //�����Ԋu�����Ԍo�߂��Ă��邩
        if (!TimeProgress) { return; }

        Injection_IntarvalTime = 0.0f;  //���Ԃ����Z�b�g

        bool StartInjection = ObjPressure > MaxPressure;                   //�U���J�n��(���͂��ő傩)

        //���Ԍo�߂�����U���͂�ǉ�
        if (!StartInjection) { ObjPressure += AirAttackPowar * Time.deltaTime; }

        //���͂��ő�ɂȂ�Αϋv�l������������
        ObjDurability -= AirAttackPowar * Time.deltaTime; 

    }


}
