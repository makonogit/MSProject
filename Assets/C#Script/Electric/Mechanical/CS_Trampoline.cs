//-------------------------------
// クラス名 :CS_Trampoline
// 内容     :物を吹っ飛ばすオブジェクト
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine ;

namespace Assets.C_Script.Electric.Mechanical
{
    public class CS_Trampoline :CS_Mechanical
    {
        [SerializeField]
        private float arrivalTime = 200f;
        [SerializeField]
        private List<string> tags = new List<string>();
        [SerializeField,Tooltip("パワーオフまでの時間")]
        private float PowerOffTime = 1.0f;
        private float nowTime =0.0f;
        [SerializeField]
        protected Vector3 forceVector = new Vector3();
        [SerializeField]
        private List<GameObject> hitObjects = new List<GameObject>();
        [SerializeField]
        private bool isRandom = true;
        [SerializeField]
        private float radius = 1.0f;
        [SerializeField]
        private Animator animator;
        // コリジョン
        private void OnCollisionEnter(Collision collision)=> hitObjects.Add(collision.gameObject);
        private void OnCollisionExit(Collision collision) => hitObjects.Remove(collision.gameObject);
        // トリガー
        private void OnTriggerEnter(Collider other) 
        { 
            hitObjects.Add(other.gameObject);
            foreach (string tag in tags) if (other.tag == tag) Power = true;
        }
        private void OnTriggerExit(Collider other) => hitObjects.Remove(other.gameObject);

        protected override void Start()
        {
            base.Start();
            animator.speed = 0.0f;
            animator.Play("AM_CloseTrampoline", 0, 1);
        }
        // 起動した瞬間
        protected override void PowerOn()
        {
            base.PowerOn();
            
            animator.SetBool("Power",true);
            animator.speed = 1.0f;
            nowTime = 0;
        }
        // 起動時
        protected override void Execute()
        {
            base.Execute();
            nowTime += Time.deltaTime;
            if (nowTime > PowerOffTime) Power = false;
        }

        protected override void PowerOff()
        {
            base.PowerOff();
            animator.SetBool("Power", false);
        }
        /// <summary>
        /// ジャンプイベント
        /// </summary>
        protected void Jumping()
        {
            foreach (GameObject gameObject in hitObjects) Jump(gameObject);
        }

        /// <summary>
        /// 飛ばす処理
        /// </summary>
        /// <param name="gameObject">飛ばしたいもの</param>
        /// <param name="force">力の倍率</param>
        private void Jump(GameObject gameObject,float force = 1.0f) 
        {
            if (!gameObject.TryGetComponent<Rigidbody>(out var rb)) return;

            Vector3 vector = Vector3.zero;
            if (isRandom) vector = RandomVector() * radius;
            rb.AddForce(forceVector * force + vector, ForceMode.Force);
        }
        private void Jump(Collider other,float force = 1.0f) =>Jump(other.gameObject,force);
        private void Jump(Collision collision, float force = 1.0f) =>Jump(collision.collider,force);

        private Vector3 RandomVector() 
        {
            float x = Random.Range(-1.0f, 1.0f);
            float y = Random.Range(-1.0f, 1.0f);
            float z = Random.Range(-1.0f, 1.0f);
            Vector3 vector = new Vector3(x,y,z);
            return vector;
        }

#if UNITY_EDITOR
        [SerializeField,Tooltip("予測線の色")]
        private Color color = new Color(0, 1, 0, 0.7f);
        [SerializeField]
        private CS_GizmosDrawer drawer = new CS_GizmosDrawer();

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            forceVector = GetAcceleration(drawer.transform.position,this.transform.position,Vector3.zero);
            Gizmos.color = color;
            Gizmos.DrawLineStrip(ExpectedDebrisPoints(forceVector, this.transform.position,Vector3.zero).ToArray(),false);
            drawer.DrawGizmos();
        }
        
        /// <summary>
        /// 予測の位置リストを返す
        /// 落ちる位置の表示
        /// </summary>
        /// <param name="ForceVector"></param>
        /// <param name="objPosition"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private List<Vector3> ExpectedDebrisPoints(Vector3 ForceVector, Vector3 objPosition, Vector3 offset)
        {
            float deltaTime = 0.04167f;

            List<Vector3> Points = new List<Vector3>();

            // 初期位置設定
            Vector3 position = objPosition + offset;
            Points.Add(position);
            // 初速度の設定
            Vector3 Velocity = (ForceVector * deltaTime * deltaTime * 0.5f);
            RaycastHit hit = new RaycastHit();
            // ぶつかるまでの軌道の線を引く
            for (float time = 0.0f; time < 50; time += deltaTime)
            {
                Velocity += Vector3.down * (9.81f * deltaTime * deltaTime);
                Ray ray = new Ray(position, Velocity.normalized);
                bool IsHit = Physics.Raycast(ray, out hit, Velocity.magnitude);
                bool IsGround = false;
                if (IsHit) IsGround = hit.transform.tag == "Ground";
                position += Velocity;
                if (IsHit && IsGround)
                {
                    Points.Add(hit.point);
                    break;
                }
                Points.Add(position);
            }
            // 着地点表示
            Gizmos.DrawWireSphere(hit.point, radius * 0.1f);
            Gizmos.DrawSphere(hit.point, 0.5f);

            return Points;
        }

        /// <summary>
        /// 加速度の方向の計算
        /// </summary>
        /// <param name="num"></param>
        /// <param name="power"></param>
        /// <param name="position"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Vector3 GetAcceleration(Vector3 endPosition, Vector3 nowPosition, Vector3 offset)
        {
            float deltaTime = 0.033334f;
            float gravityA = 9.81f;

            Vector3 firstPos = nowPosition + offset;
            Vector3 accelerationVec = endPosition - firstPos;
            accelerationVec.y += (gravityA * arrivalTime * arrivalTime);
            accelerationVec /= arrivalTime * deltaTime;
            return accelerationVec;
        }
#endif // UNITY_EDITOR

    }
}
//===============================
// date : 2024/10/19
// programmed by Nakagawa Naoto
//===============================