//-------------------------------
// クラス名 :CS_IronBall
// 内容     :大きな弾
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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

        private float hitAngle = -0.125f;
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
        /// <summary>
        /// 弾き飛ばされる処理
        /// </summary>
        /// <param name="gameObject"></param>
        private void Knockback(GameObject gameObject) 
        {
            Rigidbody rb;
            // 吹き飛ぶか？
            if (!ShouldKnockBack(gameObject,out rb)) return;
            Vector3 vec = gameObject.transform.position - this.transform.position;
            vec.y *= -1;
            vec = vec.normalized;
            rb.AddForce(vec * knockbackPower ,ForceMode.Impulse);
            if (hitSource != null)hitSource.Play();
        }
        /// <summary>
        /// 弾き飛ばすべきか
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rb"></param>
        /// <returns>true 弾け飛ぶ　false 飛ばない</returns>
        private bool ShouldKnockBack(GameObject gameObject,out Rigidbody rb) 
        {
            rb = null;
            if (gameObject == null) return false;
            if (!gameObject.TryGetComponent<Rigidbody>(out rb)) return false;
            Vector3 distance = gameObject.transform.position - this.transform.position;
            float dot = Vector3.Dot(rigidbody.velocity.normalized, distance.normalized);
            if (dot <= hitAngle) return false;
            return true;
        }

    }
}
