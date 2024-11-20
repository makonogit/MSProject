//-------------------------------
// クラス名 :CS_PillarPressMachine
// 内容     :プレス機の柱の処理
// 担当者   :中川 直登
//-------------------------------

using UnityEditor;
using UnityEngine;
namespace Assets.C_Script.Gimmick._02_PressMachine
{
    public class CS_PillarPressMachine : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField]
        private int hp = 5;
        [SerializeField]
        private CS_PressMachine pressMachine;
        [SerializeField]
        private Rigidbody rb;
        private const string attackTag = "Attack";
        private void Start()
        {
            if (!TryGetComponent(out audioSource)) Debug.LogError("null AudioSource component");
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("collisionEnter");
            if (collision.transform.tag == attackTag) HitDamage(1);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == attackTag) HitDamage(1);
        }

        private void HitDamage(int damage) 
        {
            hp -= damage;
            if (hp <=0 )
            {
                rb.isKinematic = false;
                pressMachine.Power = false;
                pressMachine.StopSounds();
                audioSource.Play();
            }
            
        }
    }
}

//===============================
// date : 2024/11/16
// programmed by Nakagawa Naoto
//===============================