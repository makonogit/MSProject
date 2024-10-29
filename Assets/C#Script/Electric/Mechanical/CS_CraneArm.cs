//-------------------------------
// クラス名 :CS_CraneArm
// 内容     :クレーンのアーム
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Basic;
using System.Collections;
using System.Diagnostics;
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
        private bool caugthed = false;
        [SerializeField]
        private bool Drop = false;
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
        private bool stopFlag;

        
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
        }
        // ギミック実行中
        protected override void Execute()
        {
            base.Execute();

            if (shouldDown) ArmDown();
            if (shouldUp) ArmUp();

            if (Drop)DropItem();
            else if (!caugthed)HoldItem();
            if (!crabTrolley.Power && animaArm.GetFloat("speed") == 0)stopFlag = false;
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
            itemObject = gameObject;
            itemRb = gameObject.GetComponent<Rigidbody>();
            stopFlag = true;
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
            itemObject.transform.SetParent(this.transform, true);
            caugthed = true;
            downFlag = false;
            if (itemRb != null) StopRigidbody(true);
            // 閉じる
            animaArm.SetFloat("speed", 1);
            animaArm.Play("AM_ArmClosing", 0, 0);
        }

        /// <summary>
        /// アイテムを落とす
        /// </summary>
        private void DropItem() 
        {
            if (itemRb != null) StopRigidbody(false);
            if (itemObject == null) return;
            
            itemObject.transform.SetParent(null,true);
            caugthed = false;
            stopFlag = false;
            // 開く speed が0なのでPlayは必要ない
            animaArm.SetFloat("speed", -1);
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
                if (stopFlag) return false;
                return !downFlag;
            }
        }

        /// <summary>
        /// アームが下がる処理
        /// </summary>
        private void ArmDown() 
        {
            this.transform.localPosition += Vector3.down * Time.deltaTime * armMoveSpeed;
            Cable.transform.localScale += Vector3.up * Time.deltaTime * armMoveSpeed * speedOffset;
        }
        /// <summary>
        /// アームが上がる処理
        /// </summary>
        private void ArmUp()
        {
            float Top = 0.01f;
            this.transform.localPosition += Vector3.up * Time.deltaTime * armMoveSpeed;
            Cable.transform.localScale += Vector3.down * Time.deltaTime * armMoveSpeed * speedOffset;
            // 上がり切ったら
            if (Cable.transform.lossyScale.y <= Top) 
            { 
                stopFlag = true; 
                downFlag = true;
                crabTrolley.Power = true;
            }
        }

        
        
    }
}

//===============================
// date : 2024/10/23
// programmed by Nakagawa Naoto
//===============================