//-------------------------------
// クラス名 :CS_IronBall
// 内容     :大きな弾
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;
namespace Assets.C_Script.Gimmick
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
        [SerializeField]
        private AudioSource rollingSource;
        [SerializeField]
        private AudioSource hitSource;

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
            rollingSource.loop = true;
        }

        private void FixedUpdate()
        {
            // 転がっているかチェックする
            isRolling = checkSpeed <= rigidbody.velocity.magnitude;

            if (rollingSource == null) return;

            bool isPlaying = rollingSource.isPlaying;
            bool shouldPlay = isRolling && !isPlaying;
            bool shouldStop = !isRolling && isPlaying;
            if (shouldPlay) rollingSource.Play();
            if (shouldStop) rollingSource.Stop();
            
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
            if (hitSource != null)hitSource.Play();
        }

    }
}
