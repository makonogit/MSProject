using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����I�u�W�F�N�g
/// </summary>
public class CS_FallingObj : MonoBehaviour
{
    /// <summary>
    /// ���ƐڐG��������ł���
    /// </summary>
    /// <param �Փ˕�="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        //�����ƐڐG���������
        Destroy(this.gameObject);

        //�v���C���[�ƐڐG������
        bool HitFloor = collision.gameObject.tag == "Player";
        if (HitFloor)
        {
            //Destroy(this.gameObject);
        }
    }
}
