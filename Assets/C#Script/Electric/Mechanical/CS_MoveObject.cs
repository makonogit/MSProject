//-------------------------------
// クラス名 :CS_Elevator
// 内容     :ポイントに移動するギミック
// 担当者   :中川 直登
//-------------------------------
using UnityEditor;
using UnityEngine;

namespace Assets.C_Script.Electric.Mechanical
{
    public class CS_MoveObject:CS_Mechanical
    {
        private Vector3 startPoint = new Vector3();
        [SerializeField]
        private Vector3 endPoint = new Vector3();
        [SerializeField,Tooltip("到達時間：\nポイントまでの移動時間")]
        private float arrivalTime = 1.0f;
        [SerializeField, Tooltip("くっつき：\n")]
        private bool stick = true;
        private float nowTime = 0.0f;
        private bool GoEndPoint = true;

        private void Start()
        {
            //割り算を毎ループしないためにここで割る
            arrivalTime = 1.0f/arrivalTime;
            startPoint = transform.position;
        }

        protected override void Execute() 
        {
            base.Execute();
            Movement(GetPosition());
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (stick) collision.transform.SetParent(transform, true);
        }
        private void OnCollisionStay(Collision collision)
        {
            if (!stick) collision.transform.SetParent(transform, true);
        }
        private void OnCollisionExit(Collision collision)
        {
            if (stick) collision.transform.SetParent(null, true);
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        private void Movement(Vector3 point) 
        {
            if (ShouldStop) return;
            nowTime += Time.deltaTime;
            this.transform.position = point;
        }

        /// <summary>
        /// を取得する
        /// </summary>
        private Vector3 GetPosition ()
        {
            Vector3 vec = Vector3.zero;
            if (GoEndPoint)
            {
                vec = endPoint - startPoint;
                vec *= nowTime * arrivalTime;
                vec += startPoint;
            }
            else 
            { 
                vec = startPoint - endPoint;
                vec *= nowTime * arrivalTime;
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
                Power = false;
                return true;
            }
        }


#if UNITY_EDITOR
 
        [SerializeField]
        private CS_GizmosDrawer drawer;

        private void OnValidate()
        {
            if (!EditorApplication.isPlaying) ResetPositions();
        }
        private void OnDrawGizmos()
        {
            if (!EditorApplication.isPlaying) ResetPositions();
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            // 実行中は呼ばない
            if (!EditorApplication.isPlaying) ResetPositions();
            drawer.DrawFlag = true;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(endPoint, startPoint);
        }

        private  void ResetPositions() 
        {
            startPoint = this.transform.position;
            if (drawer == null) return;
            endPoint = drawer.transform.position;
        }
#endif //  UNITY_EDITOR
    }
}

//===============================
// date : 2024/10/18
// programmed by Nakagawa Naoto
//===============================