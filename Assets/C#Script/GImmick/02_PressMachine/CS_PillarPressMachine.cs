//-------------------------------
// クラス名 :CS_PillarPressMachine
// 内容     :プレス機の柱の処理
// 担当者   :中川 直登
//-------------------------------
using UnityEditor;
using UnityEngine;
namespace Assets.C_Script.Gimmick
{
    public class CS_PillarPressMachine : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField]
        private bool noDamage = false;
        [SerializeField]
        private int hp = 5;
        [SerializeField]
        private CS_PressMachine pressMachine;
        private Transform Base;
        [SerializeField]
        private Rigidbody rb;
        private const string attackTag = "Attack";
        private void Start()
        {
            if (!TryGetComponent(out audioSource)) Debug.LogError("null AudioSource component");
            Base = this.transform.parent.parent;
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
        private void FixedUpdate()
        {
            Vector3 scale = this.transform.localScale;
            if(pressMachine != null) scale.y = Base.position.y - pressMachine.transform.position.y;
            scale.y *= 0.5f;
            this.transform.localScale = scale;
        }

        private void HitDamage(int damage) 
        {
            if (noDamage) return;
            hp -= damage;
            if (hp <=0 )
            {
                rb.isKinematic = false;
                if (pressMachine != null) 
                {
                    pressMachine.Power = false;
                    pressMachine.StopSounds();
                }
                audioSource.Play();
            }
            
        }
    }
}

//===============================
// date : 2024/11/16
// programmed by Nakagawa Naoto
//===============================