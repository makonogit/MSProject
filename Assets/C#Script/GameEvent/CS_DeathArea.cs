//-------------------------------
// �N���X�� :CS_DeathArea
// ���e     :���������玀��
// �S����   :���� ���o
//-------------------------------
using Assets.C_Script.GameEvent;
using UnityEngine;
public class CS_DeathArea : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    { if(collision.transform.tag == "Player")CSGE_GameOver.GameOver(); }
    private void OnTriggerEnter(Collider other)
    { if (other.tag == "Player") CSGE_GameOver.GameOver(); }
}