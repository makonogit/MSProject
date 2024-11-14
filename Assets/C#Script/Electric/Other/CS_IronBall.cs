//-------------------------------
// クラス名 :CS_IronBall
// 内容     :大きな弾
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;
namespace Assets.C_Script.Electric.Other
{
    public class CS_IronBall : MonoBehaviour
    {
        private const float checkSpeed = 1.5f;
        [SerializeField]
        private bool isRolling = false;
        [SerializeField]
        private float knockbackPower = 50.0f;
        [SerializeField]
        private List<string> tags = new List<string>();
        private Rigidbody rigidbody;

        
        private void OnCollisionEnter(Collision collision)
        {
            if (!isRolling) return;
            foreach (string tag in tags)
            { 
                if (tag == collision.transform.tag) Knockback(collision.gameObject);
            }
        }
        private void Start() 
        {
            if (!TryGetComponent(out rigidbody)) Debug.LogError("null rigidbody component");
        }

        private void FixedUpdate()
        {
            // 転がっているかチェックする
            isRolling = checkSpeed <= rigidbody.velocity.magnitude;
        }

        private void Knockback(GameObject gameObject) 
        {
            Rigidbody rb;
            if (gameObject == null) return;
            if (!gameObject.TryGetComponent<Rigidbody>(out rb)) return;
            Vector3 vec = gameObject.transform.position - this.transform.position;
            vec.y *= -1;
            vec = vec.normalized;
            rb.AddForce(vec * knockbackPower ,ForceMode.Impulse);
            
        }

    }
}
