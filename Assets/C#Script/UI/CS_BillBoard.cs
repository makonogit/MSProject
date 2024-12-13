//-------------------------------
// クラス名 :CS_BillBoard 
// 内容     :ビルボード
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.C_Script.UI
{
    public class CS_BillBoard : MonoBehaviour
    {
        [Header("位置固定")]
        [SerializeField] private bool lockPos = true;
        [SerializeField] private Vector3 offset;
        [Header("回転固定")]
        [SerializeField] private bool lockX = false;
        [SerializeField] private bool lockY = false;
        [SerializeField] private bool lockZ = false;
        [Header("サイズ固定")]
        [SerializeField] private bool lockSize = true;
        [SerializeField] private float size = 0.1f;

        private Transform parentTransform;

        private void Start()
        {
            if (transform.parent != null) parentTransform = transform.parent;
        }
        private void Update()
        {
            Vector3 vec = Camera.main.transform.position;
            Vector3 up = Camera.main.transform.up;
            // 回転固定
            if (lockX) vec.y = transform.position.y;
            if (lockY) vec.x = transform.position.x;
            if (lockZ) up = Vector3.up;
            // 位置固定
            if (lockPos)transform.position = parentTransform.position + offset;
            // 設定
            transform.LookAt(vec,up);
            // 親子関係を固定
            if (parentTransform != null) transform.parent = parentTransform;
            // ビルボードのサイズを距離に依存せず固定する
            if (!lockSize) return;
            float distance = Vector3.Distance(transform.position, Camera.main.transform.position); 
            transform.localScale = Vector3.one * distance * size;
        }
    }
}
//===============================
// date : 2024/12/11
// programmed by Nakagawa Naoto
//===============================