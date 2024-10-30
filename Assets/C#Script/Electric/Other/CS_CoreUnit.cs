//-------------------------------
// クラス名 :CS_CoreUnit
// 内容     :コアの置き場
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Mechanical;
using UnityEngine;
using static UnityEditor.Progress;

namespace Assets.C_Script.Electric.Other
{
    public class CS_CoreUnit :MonoBehaviour
    {
        [SerializeField]
        private GameObject coreObject;
        private Rigidbody coreRb;
        [SerializeField]
        private CS_Mechanical receiver;
        [SerializeField]
        private Transform unitTransform;
        [SerializeField]
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) Debug.LogError("null component");
        }

        /// <summary>
        /// コアをセットする
        /// </summary>
        /// <param name="core"></param>
        public void SetCore(GameObject core) 
        {
            coreObject = core;
            coreObject.transform.localPosition = unitTransform.position;
            coreRb = coreObject.GetComponent<Rigidbody>();
            if (coreRb != null ) StopRigidbody(coreRb,true);
            if (receiver != null) receiver.Power = true;
            if (!audioSource.isPlaying) audioSource.Play();
        }

        /// <summary>
        /// 剛体処理を無効化するかしないか
        /// </summary>
        /// <param name="flag"></param>
        private void StopRigidbody(Rigidbody rb,bool flag)
        {
            rb.isKinematic = flag;
            rb.freezeRotation = flag;
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