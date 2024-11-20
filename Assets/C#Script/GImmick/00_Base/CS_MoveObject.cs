//-------------------------------
// クラス名 :CS_MoveObject
// 内容     :ポイントに移動するギミック
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.C_Script.Gimmick._00_Base
{
    public class CS_MoveObject : CS_Mechanical
    {
        private Vector3 startPoint = new Vector3();
        [SerializeField]
        protected Vector3 endPoint = new Vector3();
        [SerializeField, Tooltip("到達時間：\nポイントまでの移動時間")]
        protected float arrivalTime = 1.0f;
        [SerializeField, Tooltip("くっつき：\n")]
        protected bool stick = true;
        [SerializeField, Tooltip("止まるか：\n")]
        protected bool Stop = true;
        protected float nowTime = 0.0f;
        protected bool GoEndPoint = true;
        [SerializeField, Tooltip("イージング：\n")]
        private AnimationCurve animCurve = new AnimationCurve();
        [SerializeField,Tooltip("子オブジェクトにするタグ")]
        private List<string>tags = new List<string>();
        

        virtual protected void Start()
        {
            //割り算を毎ループしないためにここで割る
            arrivalTime = 1.0f / arrivalTime;
            startPoint = transform.position;
        }

        protected override void Execute()
        {
            base.Execute();
            Movement(GetPosition());
        }

        // コリジョン
        private void OnCollisionEnter(Collision collision)
        {
            bool shouldSet = false;
            foreach (string tag in tags) if( tag == collision.transform.tag)shouldSet = true;
            if (!shouldSet)return;
            if (stick) collision.transform.SetParent(transform, true);
        }
        private void OnCollisionExit(Collision collision)
        {
            if (stick) collision.transform.SetParent(null, true);
        }

        // トリガー
        private void OnTriggerEnter(Collider other)
        {
            bool shouldSet = false;
            foreach (string tag in tags) if (tag == other.tag) shouldSet = true;
            if (!shouldSet) return;
            if (stick) other.transform.SetParent(transform, true);
        }
        private void OnTriggerExit(Collider other)
        {
            if (stick) other.transform.SetParent(null, true);
        }



        /// <summary>
        /// 移動処理
        /// </summary>
        protected bool Movement(Vector3 point)
        {
            if (ShouldStop) return false;
            nowTime += Time.deltaTime;
            this.transform.position = point;
            MoveSound();
            return true;
        }

        /// <summary>
        /// を取得する
        /// </summary>
        protected Vector3 GetPosition()
        {
            Vector3 vec = Vector3.zero;
            if (GoEndPoint)
            {
                vec = endPoint - startPoint;
                vec *= animCurve.Evaluate(nowTime * arrivalTime);
                vec += startPoint;
            }
            else
            {
                vec = startPoint - endPoint;
                vec *= animCurve.Evaluate(nowTime * arrivalTime);
                vec += endPoint;
            }

            return vec;

        }

        private Vector3 GetVector()
        {
            Vector3 vec = Vector3.zero;
            if (GoEndPoint)
            {
                vec = endPoint - startPoint;
                vec *= nowTime * arrivalTime;
            }
            else
            {
                vec = startPoint - endPoint;
                vec *= nowTime * arrivalTime;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 止まるか
        /// </summary>
        private bool ShouldStop
        {
            get
            {
                bool IsTimeOver = nowTime * arrivalTime >= 1.0f;
                if (!IsTimeOver) return false;
                // 反転
                GoEndPoint = !GoEndPoint;
                nowTime = 0.0f;
                Power = !Stop;
                PressSound();
                
                return true;
            }
        }

        virtual protected void PressSound() { }
        virtual protected void MoveSound() { }

#if UNITY_EDITOR

        [SerializeField]
        private CS_GizmosDrawer drawer;
        
        private void OnDrawGizmos() => ResetPositions(); 
        
        private void OnDrawGizmosSelected()=> DrawGizmos();

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            // 実行中は呼ばない
            ResetPositions();
            drawer.DrawFlag = true;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(endPoint, startPoint);
        }

        private  void ResetPositions() 
        {
            if(!EditorApplication.isPlaying)startPoint = this.transform.position;
            if (drawer == null) return;
            endPoint = drawer.GetPosition();
        }
#endif //  UNITY_EDITOR
    }
}

//===============================
// date : 2024/10/18
// programmed by Nakagawa Naoto
//===============================