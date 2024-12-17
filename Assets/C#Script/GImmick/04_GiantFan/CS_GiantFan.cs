//-------------------------------
// クラス名 :CS_GiantFan 
// 内容     :巨大扇風機
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Assets.C_Script.Gimmick
{
    public class CS_GiantFan :MonoBehaviour
    {
        private enum Type 
        {
            Infinity,   // 永続的
            Regularly   // 定期的
        }
        [SerializeField]
        private Type type = Type.Regularly;
        [SerializeField,Tooltip("停止時間")]
        private float stopTime = 0.0f;
        [SerializeField, Tooltip("持続時間")]
        private float runTime = 1.0f;
        [SerializeField, Tooltip("起動時間")]
        private float startupTime = 1.0f;
        private float nowTime = 0.0f;
        private bool isRun = false;
        [SerializeField]
        private float windPower = 1.0f;
        [SerializeField,Tooltip("起動時の風力がMaxになるまでのカーブ")]
        private AnimationCurve startupForceCurve;
        [SerializeField]
        private List<Rigidbody> rigidbodies = new List<Rigidbody> ();
        [Header("振動の設定")]
        [SerializeField] private float duration = 1.0f;
        [SerializeField] private int powerType = 0;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private int repetition = 1;

        private AudioSource myAudioSource;

        private void Start()
        {
            nowTime = stopTime;
            if (!TryGetComponent(out myAudioSource)) Debug.LogError("null audioSource component");
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") CS_ControllerVibration.StartVibrationWithCurve(duration, powerType, curve, repetition);
            if (!other.gameObject.TryGetComponent(out Rigidbody rb)) return;
            rigidbodies.Add(rb);
        }
        private void OnTriggerExit(Collider other)
        {
            
            if (rigidbodies.Count == 0) return;
            for (int i = 0; i < rigidbodies.Count; i++) 
            {
                if (rigidbodies[i] == null) continue;
                if (rigidbodies[i].transform.name == other.transform.name) rigidbodies.Remove(rigidbodies[i]);
            }
        }

        private void FixedUpdate()
        {
            if (type == Type.Infinity || IsRun) 
            {
                for (int i = 0; i < rigidbodies.Count; i++) 
                {
                    if (rigidbodies[i] != null) ForceWind(rigidbodies[i]);
                }
            }
            nowTime -= Time.deltaTime;
        }

        private bool IsRun 
        {
            get 
            {
                bool timeOver = nowTime <= 0;
                // 止まる
                if (isRun && timeOver) 
                {
                    nowTime = stopTime;
                    isRun = false;
                    myAudioSource.Stop();
                    return false;
                }
                // 動く
                if (!isRun && timeOver) 
                { 
                    nowTime = runTime + startupTime;
                    isRun = true;
                    myAudioSource.loop = true;
                    myAudioSource.Play();
                    return true; 
                }

                return isRun;
            }
        }
        private void ForceWind(Rigidbody rb)
        {
            float time = (startupTime + runTime) - nowTime;
            time /= startupTime;
            float force = startupForceCurve.Evaluate(time) * windPower;
            Vector3 forceVec = this.transform.forward * force;
            if (rb != null && rb.tag == "Player") rb.velocity += forceVec;
            else rb.AddForce(forceVec, ForceMode.Impulse);

        }
    }
}
//===============================
// date : 2024/11/15
// programmed by Nakagawa Naoto
//===============================