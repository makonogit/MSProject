﻿//-------------------------------
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
        [SerializeField]
        private float speed = 2.0f;
        [SerializeField]
        private bool isBurning = false;
        [SerializeField]
        private List<string> tags = new List<string>();
        private Collider myCollider;
        
        private Rigidbody myRigidbody;
        private float nowTime=0.0f;
        private AudioSource myAudioSource;


        private static GameObject playerObj;
        private static Rigidbody playerRb;
        private static Vector3 oldPlayerPosition;
        private const string playerTag = "Player";
        private const string burningTag = "Burning";
        private const float overTime = 1.0f;
        private const float MaxSpeed = 10.0f;


        private void OnCollisionEnter(Collision collision) => IntoArea(collision.gameObject);
        private void OnTriggerEnter(Collider other) =>IntoArea(other.gameObject);

        private void OnCollisionExit(Collision collision)
        {
            if (playerObj == collision.gameObject) playerObj = null;
        }
        private void OnTriggerExit(Collider other)
        { 
            if (playerObj == other.gameObject) playerObj = null;
        }

        private void Start()
        {
            if (!TryGetComponent(out myCollider)) Debug.LogError("null collider component");
            if (!TryGetComponent(out myRigidbody)) Debug.LogError("null rigidbody component");
            if (!TryGetComponent(out myAudioSource)) Debug.LogError("null audioSource component");
        }

        private void FixedUpdate()
        {
            if (IsBurning) Burning();
            else Slip();
        }


        /// <summary>
        /// エリアに入った時の処理
        /// </summary>
        /// <param name="gameObject"></param>
        private void IntoArea(GameObject gameObject)
        {
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
            playerObj.TryGetComponent(out playerRb);
            if (oldPlayerPosition.magnitude <= 0) oldPlayerPosition = gameObject.transform.position;
        }

        /// <summary>
        /// プレイヤーが滑る処理
        /// </summary>
        private void Slip() 
        {
            if (playerObj == null) return;
            if (!IsPlayerRigidbody) return;
            Vector3 vec = playerRb.position - oldPlayerPosition;
            if (playerRb.velocity.magnitude <= MaxSpeed) playerRb.AddForce(vec * speed, ForceMode.Impulse);
            oldPlayerPosition = playerRb.position;
        }

        /// <summary>
        /// 燃えている時
        /// </summary>
        private void Burning() 
        {
            if (playerObj == null) 
            {
                nowTime = 0.0f;
                return; 
            }
            
            if (nowTime >= overTime)
            {
                // ダメージ処理

                // 時間のリセット
                nowTime = 0.0f;
                myRigidbody.isKinematic = true;
            }


            nowTime += Time.deltaTime;
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
                    myCollider.material = null;
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