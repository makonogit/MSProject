//-------------------------------
// クラス名 :CS_HitTrigger
// 内容     :コリジョンに当たったり
// 入ったりしたら送信する機能
// 
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Sensor;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.Electric.Switch
{
    [RequireComponent(typeof(Collision))]
    public class CS_HitTrigger :CS_Switch
    {
        [SerializeField, Tooltip("タグリスト")]
        private List<string> HitTags = new List<string>();
        

        // コリジョン
        private void OnCollisionEnter(Collision collision) => Transmission(collision.gameObject, true);
        private void OnCollisionExit(Collision collision) => Transmission(collision.gameObject, false);
        // トリガー
        private void OnTriggerEnter(Collider other) => Transmission(other.gameObject, true);
        private void OnTriggerExit(Collider other) => Transmission(other.gameObject,false);

        /// <summary>
        /// 設定されたタグに当たったならシグナルを変更
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="value"></param>
        private void Transmission(GameObject gameObject,bool value) 
        {
            bool isHit = HitTags.Count <= 0;
            foreach (var hit in HitTags) if (hit == gameObject.transform.tag) isHit = true;
            if (!isHit) return;
            Signal = value;
        }

    }
}
//===============================
// date : 2024/10/20
// programmed by Nakagawa Naoto
//===============================