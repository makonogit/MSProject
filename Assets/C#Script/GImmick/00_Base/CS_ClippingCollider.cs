//-------------------------------
// クラス名 :CS_ClippingCollider 
// 内容     :コライダーをクリッピングする
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SearchService;
namespace Assets.C_Script.Gimmick
{
    public class CS_ClippingCollider :MonoBehaviour
    {
        [SerializeField,Tooltip("すり抜けたいオブジェクトのタグ")]
        private List<string> tags = new List<string>();
        [SerializeField,Tooltip("すり抜けられるレイヤー")]
        private List<string> Layers = new List<string>();

        private void OnTriggerEnter(Collider other) 
        {
            foreach (string tag in tags) 
            {
                if (other.transform.CompareTag(tag)) ObjectEnter(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other) 
        {
            foreach (string tag in tags) 
            {
                if (other.transform.CompareTag(tag)) ObjectExit(other.gameObject); 
            }
        }
        /// <summary>
        /// オブジェクトが入った時
        /// </summary>
        /// <param name="gameObject"></param>
        private void ObjectEnter(GameObject gameObject) 
        {
            if (!gameObject.TryGetComponent(out Rigidbody rb)) return;
            foreach (string layer in Layers) rb.excludeLayers += LayerMask.GetMask(layer);
        }

        /// <summary>
        /// オブジェクトが出たとき
        /// </summary>
        /// <param name="gameObject"></param>
        private void ObjectExit(GameObject gameObject) 
        {
            if (!gameObject.TryGetComponent(out Rigidbody rb)) return;
            foreach (string layer in Layers) rb.excludeLayers -= LayerMask.GetMask(layer);
        }

        /// <summary>
        /// crossSectionの設定
        /// </summary>
        /// <param name="value"></param>
        //private void SetCrossSection(bool value) 
        //{
        //    crossSection.enabled = value;
        //    isCrossingWithCollidedObj = value;
        //}
    }
}
//===============================
// date : 2024/11/20
// programmed by Nakagawa Naoto
//===============================