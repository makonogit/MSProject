//-------------------------------
// クラス名 :CS_CraneArm
// 内容     :クレーンのアーム
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Basic;
using UnityEngine;
using UnityEngine.SearchService;

namespace Assets.C_Script.Electric.Mechanical
{
    class CS_CraneArm : CS_Mechanical
    {
        [SerializeField]
        private CS_CrabTrolley crabTrolley;
        [SerializeField]
        private string itemTag = "Core";
        [SerializeField]
        private bool holded = false;
        [SerializeField]
        private bool Drop = false;
        private GameObject itemObject;
        private Rigidbody itemRb;


        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == itemTag) SetItem(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (itemObject == other.gameObject) ClearItem();
        }

        protected override void PowerOn()
        {
            base.PowerOn();
        }
        protected override void Execute()
        {
            base.Execute();
            if (Drop)DropItem();
            else if (!holded)HoldItem();
            
        }
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
            itemObject = gameObject;
            itemRb = gameObject.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// つかむアイテムの情報をクリア
        /// </summary>
        private void ClearItem() 
        {
            itemObject = null;
            itemRb = null;
        }


        /// <summary>
        /// アイテムをつかむ
        /// </summary>
        private void HoldItem() 
        {
            if (itemObject == null) return;
            itemObject.transform.parent = this.transform;
            holded = true;
            crabTrolley.Power = true;
            if (itemRb != null) StopRigidbody(true);            
        }

        /// <summary>
        /// アイテムを落とす
        /// </summary>
        private void DropItem() 
        {
            if (itemRb != null) StopRigidbody(false);
            if (itemObject == null) return;
            itemObject.transform.parent = null;
            holded = false;
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


    }
}

//===============================
// date : 2024/10/23
// programmed by Nakagawa Naoto
//===============================