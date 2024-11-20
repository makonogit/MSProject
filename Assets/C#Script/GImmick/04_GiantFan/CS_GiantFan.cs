//-------------------------------
// クラス名 :CS_GiantFan 
// 内容     :巨大扇風機
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Assets.C_Script.Gimmick._04_GiantFan
{
    public class CS_GiantFan :MonoBehaviour
    {
        [SerializeField]
        private float windPower = 1.0f;
        [SerializeField]
        private List<Rigidbody> rigidbodies = new List<Rigidbody> ();
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out Rigidbody rb)) return;
            rigidbodies.Add(rb);
        }
        private void OnTriggerExit(Collider other)
        {
            foreach (var rb in rigidbodies) 
            {
               if (rb.transform ==  other.transform) rigidbodies.Remove(rb);
            }
        }

        private void FixedUpdate()
        {
            foreach(var rigidbody in rigidbodies)ForceWind(rigidbody);
        }
        private void ForceWind(Rigidbody rb)
        {
            Vector3 forceVec = this.transform.forward * windPower;
            rb.AddForce(forceVec,ForceMode.Impulse);
        }
    }
}
//===============================
// date : 2024/11/15
// programmed by Nakagawa Naoto
//===============================