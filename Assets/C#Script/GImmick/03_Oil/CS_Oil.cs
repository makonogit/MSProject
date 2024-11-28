//-------------------------------
// クラス名 :CS_Oil
// 内容     :オイル
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_Oil : MonoBehaviour
    {
        [SerializeField,Tooltip("滑るスピード")]
        private float slipSpeed = 2.0f;
        [SerializeField, Tooltip("減速倍率:\n元のスピード×減速倍率\n1.0  ＝ 1/1\n0.5  ＝ 1/2\n0.25 ＝ 1/4"),Range(0.0f, 1.0f)]
        private float slowlySpeed = 0.5f;

        [SerializeField]
        private bool isBurning = false;
        [SerializeField]
        private List<string> tags = new List<string>();
        private Collider myCollider;
        [SerializeField]
        private PhysicMaterial burningMaterial;

        private Rigidbody myRigidbody;
        private AudioSource myAudioSource;


        private static GameObject playerObj;
        private static Rigidbody playerRb;
        private static CSP_ParallelMove playerMove;
        private static float orignalSpeed = 0.0f;
        private static Vector3 oldPlayerPosition;
        private const string playerTag = "Player";
        private const string burningTag = "BurningObject";
        private const float MaxSpeed = 5.0f;


        private void OnCollisionEnter(Collision collision) => IntoArea(collision.gameObject);
        private void OnTriggerEnter(Collider other) =>IntoArea(other.gameObject);

        private void OnCollisionExit(Collision collision)
        {
            if (playerObj == collision.gameObject) 
            { 
                playerObj = null; 
                playerMove.SetSpeed(orignalSpeed);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (playerObj == other.gameObject) 
            { 
                playerObj = null;
                playerMove.SetSpeed(orignalSpeed);
            }

        }

        private void Start()
        {
            if (!TryGetComponent(out myCollider)) Debug.LogError("null collider component");
            if (!TryGetComponent(out myRigidbody)) Debug.LogError("null rigidbody component");
            if (!TryGetComponent(out myAudioSource)) Debug.LogError("null audioSource component");
        }

        private void FixedUpdate()
        {
            if (!IsBurning)PlayerSlip();
            if (playerObj == null)oldPlayerPosition = Vector3.zero;
        }


        /// <summary>
        /// エリアに入った時の処理
        /// </summary>
        /// <param name="gameObject"></param>
        private void IntoArea(GameObject gameObject)
        {
            if (!myRigidbody.isKinematic) myRigidbody.isKinematic = true;
            // Playerが当たったか
            if (playerTag == gameObject.tag) HitPlayer(gameObject);
            // 燃えていたら抜ける
            if (isBurning) return;

            // 燃えるか
            foreach (string tag in tags) if (tag == gameObject.tag) IsBurning = true;

        }

        /// <summary>
        /// プレイヤーに当たった時
        /// </summary>
        /// <param name="gameObject"></param>
        private void HitPlayer(GameObject gameObject) 
        {
            if (gameObject == null) return;
            playerObj = gameObject;
            if (playerRb == null) playerObj.TryGetComponent(out playerRb);
            if (playerMove == null) playerObj.TryGetComponent(out playerMove);
            if (playerMove != null && orignalSpeed == 0.0f) orignalSpeed = playerMove.GetSpeed();
            if (oldPlayerPosition.magnitude <= 0) oldPlayerPosition = gameObject.transform.position;
            if(IsBurning)PlayerSlowly();
        }

        /// <summary>
        /// プレイヤーが遅くなる処理
        /// </summary>
        private void PlayerSlowly() 
        {
            if(playerMove == null) return;
            float speed = orignalSpeed * slowlySpeed;
            playerMove.SetSpeed(speed);
        }

        /// <summary>
        /// プレイヤーが滑る処理
        /// </summary>
        private void PlayerSlip() => PlayerAddSpeed(slipSpeed);
        
        /// <summary>
        /// プレイヤの移動スピードに速度を足す
        /// </summary>
        /// <param name="speed"></param>
        private void PlayerAddSpeed(float speed) 
        {
            if (playerObj == null) return;
            if (!IsPlayerRigidbody) return;
            Vector3 vec = playerRb.position - oldPlayerPosition;
            // 過去の位置がない時
            if (oldPlayerPosition == Vector3.zero) vec = playerRb.velocity.normalized;
            // 現速度が最大速度を超えていないか
            if (playerRb.velocity.magnitude <= MaxSpeed) playerRb.AddForce(vec * speed, ForceMode.Impulse);
            oldPlayerPosition = playerRb.position;
        }

        /// <summary>
        /// 燃えているか
        /// </summary>
        private bool IsBurning 
        {
            set 
            {
                isBurning = value;
                // 滑る処理を無効にする
                if (value == true) 
                { 
                    myCollider.material = burningMaterial;
                    this.tag = burningTag;
                    myRigidbody.isKinematic = false;
                    myAudioSource.loop = true;
                    myAudioSource.Play();
                }
                
            }
            get
            {
                return isBurning;
            }
        }

        /// <summary>
        /// プレイヤーのリギッドボディがあるかどうか
        /// </summary>
        private bool IsPlayerRigidbody 
        {
            get 
            {
                if (playerRb != null) return true;
                if (playerObj == null) return false;
                if (!playerObj.TryGetComponent(out playerRb)) return false;
                return false;
            }
        }
    }
}
//===============================
// date : 2024/11/15
// programmed by Nakagawa Naoto
//===============================