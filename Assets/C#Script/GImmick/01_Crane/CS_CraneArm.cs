//-------------------------------
// クラス名 :CS_CraneArm
// 内容     :クレーンのアーム
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;


namespace Assets.C_Script.Gimmick
{
    class CS_CraneArm : CS_Mechanical
    {
        [SerializeField]
        private CS_CrabTrolley crabTrolley;
        [SerializeField]
        private string itemTag = "Core";
        [SerializeField]
        private bool caught = false;
        [SerializeField]
        private bool Drop = false;
        private bool Dropped = false;
        private GameObject itemObject;
        private Rigidbody itemRb;
        [SerializeField]
        private GameObject Cable;
        [SerializeField]
        private Animator animaArm;
        [SerializeField]
        private float armMoveSpeed = 60.0f;
        private float speedOffset = 0.034f;
        [SerializeField]
        private bool downFlag;
        [SerializeField]
        private bool stopFlag;
        [SerializeField]
        private CS_CoreUnit endPointCoreUnit;

        // 効果音
        [SerializeField, Tooltip("起動音")]
        private AudioClip powerOnSound;
        [SerializeField,Tooltip("クレーンアームの移動音")]
        private AudioClip craneSound;
        [SerializeField, Tooltip("つかむ音")]
        private AudioClip garbSound;
        // オーディオソース
        private AudioSource audioSource;


        protected override void Start()
        {
            base.Start();
            if (endPointCoreUnit == null) Debug.LogError("Error : null component : CS_CoreUnit");
            if (crabTrolley == null) Debug.LogError("Error : null component : CS_CrabTrolley");
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) Debug.LogError("not found : null component : AudioSource");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == itemTag) SetItem(other.gameObject);
        }
        private void OnTriggerExit(Collider other)
        {
            if (itemObject == other.gameObject) ClearItem();
        }
        // ギミック起動時
        protected override void PowerOn()
        {
            base.PowerOn();
            animaArm.SetFloat("speed", -1);
            animaArm.Play("AM_ArmClosing", 0, 1);
            audioSource.clip = powerOnSound;
            audioSource.loop = false;
            audioSource.Play();
        }
        // ギミック実行中
        protected override void Execute()
        {
            base.Execute();

            if (shouldDown) ArmDown();
            if (shouldUp) ArmUp();

            if (Drop)DropItem();
            else if (!caught)HoldItem();
        }
        // ギミックシャットダウン時
        protected override void PowerOff()
        {
            base.PowerOff();
        }
        

        /// <summary>
        /// つかむアイテムの情報をセット
        /// </summary>
        /// <param name="gameObject"></param>
        private void SetItem(GameObject gameObject) 
        {
            if(Dropped)return;
            itemObject = gameObject;
            itemRb = gameObject.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// つかむアイテムの情報をクリア
        /// </summary>
        private void ClearItem() 
        {
            if (!Dropped) DropItem();
            itemObject = null;
            itemRb = null;
        }

        /// <summary>
        /// アイテムをつかむ
        /// </summary>
        private void HoldItem() 
        {
            if (itemObject == null) return;
            itemObject.transform.SetParent(this.transform, true);
            caught = true;
            downFlag = false;
            if (itemRb != null) StopRigidbody(true);
            // 閉じる
            animaArm.SetFloat("speed", 1);
            animaArm.Play("AM_ArmClosing", 0, 0);
            
            audioSource.clip = garbSound;
            audioSource.loop = false;
            audioSource.Play();
        }

        /// <summary>
        /// アイテムを落とす
        /// </summary>
        private void DropItem() 
        {
            if (itemRb != null) StopRigidbody(false);
            if (itemObject == null) return;
            
            itemObject.transform.SetParent(null,true);
            // 開く speed が0なのでPlayは必要ない
            animaArm.SetFloat("speed", -1);
            // コアを台に置く
            endPointCoreUnit.SetCore(itemObject);
            caught = false;     // 放した
            downFlag = false;   // 上がる
            Drop = false;       // 落とした
            Dropped = true;
        }

        /// <summary>
        /// 剛体処理を無効化するかしないか
        /// </summary>
        /// <param name="flag"></param>
        void StopRigidbody(bool flag) 
        {
            itemRb.isKinematic = flag;
            itemRb.freezeRotation = flag;
            itemRb.useGravity = !flag;
            if(flag) itemRb.constraints = RigidbodyConstraints.FreezeAll;
            if (!flag) itemRb.constraints = RigidbodyConstraints.None;
        }

        /// <summary>
        /// アームが下がる時
        /// </summary>
        private bool shouldDown
        { 
            get
            {
                bool isMove = crabTrolley.Power;
                if (isMove) return false;
                if (stopFlag) return false; 
                return downFlag;
            }
        }
        /// <summary>
        /// アームが上がる時
        /// </summary>
        private bool shouldUp
        {
            get
            {
                bool isMove = crabTrolley.Power;
                if (isMove) return false;
                return !downFlag;
            }
        }

        private bool shouldStopDown 
        {
            get 
            {
                Vector3 Distance = endPointCoreUnit.transform.position - this.transform.position;
                float radius = 1.5f;
                float sqrLength = Distance.sqrMagnitude;
                return sqrLength < radius * radius;
            }
        }

        /// <summary>
        /// アームが下がる処理
        /// </summary>
        private void ArmDown() 
        {
            this.transform.localPosition += Vector3.down * Time.deltaTime * armMoveSpeed;
            Cable.transform.localScale += Vector3.up * Time.deltaTime * armMoveSpeed * speedOffset;
            CraneSoundPlay();
            if (audioSource.isPlaying)audioSource.Play();
            if (shouldStopDown) 
            {
                Drop = true;
            }
        }
        /// <summary>
        /// アームが上がる処理
        /// </summary>
        private void ArmUp()
        {
            float Top = 0.01f;
            this.transform.localPosition += Vector3.up * Time.deltaTime * armMoveSpeed;
            Cable.transform.localScale += Vector3.down * Time.deltaTime * armMoveSpeed * speedOffset;
            CraneSoundPlay();
            // 上がり切ったら
            if (Cable.transform.lossyScale.y <= Top) 
            { 
                downFlag = true;
                crabTrolley.Power = caught;
                audioSource.Stop();
            }
            if (crabTrolley.endPoint) stopFlag = true;
        }

        /// <summary>
        /// 効果音を鳴らす
        /// </summary>
        private void CraneSoundPlay() 
        {
            if (audioSource.isPlaying) return;
            audioSource.clip = craneSound;
            audioSource.loop = true;
            audioSource.Play(); 
        }

#if UNITY_EDITOR
        [SerializeField]
        private Mesh mesh;
        [SerializeField]
        private bool ShowGizmos = false;
        private void OnDrawGizmos()
        {
            if (ShowGizmos) DrawGizmos();
        }
        private void OnDrawGizmosSelected()
        {
            if (!ShowGizmos)DrawGizmos();
        }

        public override void DrawGizmos()
        {
            base.DrawGizmos();
            
            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(this.transform.position + Vector3.down, Vector3.down);
            bool isHit = Physics.Raycast(ray, out hit);
            if (isHit)
            {
                Vector3 EndPoint = hit.point;
                Vector3 scale = new Vector3(0.01f, 0.01f, 0.01f);
                // 線描画
                Gizmos.color = Color.green;
                Gizmos.DrawLine(this.transform.position, EndPoint);
                Gizmos.DrawMesh(mesh, 0, EndPoint,Quaternion.identity,scale);
            }
        }
#endif // UNITY_EDITOR
    }
}

//===============================
// date : 2024/10/29
// programmed by Nakagawa Naoto
//===============================