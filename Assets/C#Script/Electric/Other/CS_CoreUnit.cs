//-------------------------------
// クラス名 :CS_CoreUnit
// 内容     :コアの置き場
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Basic;
using Assets.C_Script.Electric.Mechanical;
using Assets.C_Script.Electric.Sensor;
using UnityEngine;
using static UnityEditor.Progress;

namespace Assets.C_Script.Electric.Other
{
    public class CS_CoreUnit : CS_Switch
    {
        [SerializeField]
        private GameObject coreObject;
        private Rigidbody coreRb;
        private CS_Core core;
        [SerializeField]
        private Transform unitTransform;
        [SerializeField]
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) Debug.LogError("null component");
        
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (core == null) return;
            if (Signal && core.STATE == CS_Core.CORE_STATE.HAVEPLAYER) 
            {
                Signal = false;
            }

        }

        /// <summary>
        /// コアをセットする
        /// </summary>
        /// <param name="targetObject"></param>
        public void SetCore(GameObject targetObject) 
        {
            coreObject = targetObject;
            coreRb = coreObject.GetComponent<Rigidbody>();
            coreObject.GetComponent<Collider>().enabled = true;
            core = coreObject.GetComponentInChildren<CS_Core>();
            if (coreRb != null) StopRigidbody(coreRb, true);
            if (!audioSource.isPlaying) audioSource.Play();
            coreObject.transform.position = unitTransform.position;
            coreObject.transform.rotation = unitTransform.rotation;
            core.STATE = CS_Core.CORE_STATE.DROP;
            Signal = true;
        }

        /// <summary>
        /// 剛体処理を無効化するかしないか
        /// </summary>
        /// <param name="flag"></param>
        private void StopRigidbody(Rigidbody rb,bool flag)
        {
            rb.isKinematic = flag;
            rb.useGravity = !flag;
            if (flag) rb.constraints = RigidbodyConstraints.FreezeAll;
            if (!flag) rb.constraints = RigidbodyConstraints.None;
        }

        
        
    }
}
//===============================
// date : 2024/10/29
// programmed by Nakagawa Naoto
//===============================