//-------------------------------
// クラス名 :CS_Obstacle
// 内容     :体力のある障害物
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Other;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.Electric.notElectric
{
    public class CS_Obstacle :MonoBehaviour
    {
        [SerializeField]
        private int hp = 5;
        // 読み取り用
        public int HP { get { return hp; } }
        [SerializeField]
        private ParticleSystem DebrisParticle;
        [SerializeField]
        private List<CS_DrawNumber> drawNum = new List<CS_DrawNumber>();
        // スタート
        private void Start(){}
        // フィクスドアップデート
        private void FixedUpdate()
        {
            if (ShouldExplosion) Explosion();
        }
        // アップデート
        private void Update() {}

       
        /// <summary>
        /// 爆発するべきか
        /// </summary>
        private bool ShouldExplosion{ get { return hp <= 0; }}

        /// <summary>
        /// 爆破-消滅するまで
        /// </summary>
        private void Explosion()
        { 
            Destroy(this.gameObject);
        }

        /// <summary>
        /// ダメージを与える関数
        /// </summary>
        /// <param name="damage"></param>
        public void HitDamages(int  damage = 0) 
        {
            hp -= damage;
            foreach (CS_DrawNumber drawNumber in drawNum)drawNumber.SetNumber(hp);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (drawNum.Count <= 0) return;
            foreach (CS_DrawNumber drawNumber in drawNum) 
            {
                if (drawNumber == null) return;
                drawNumber.SetNumber(hp); 
            }
        }

#endif // UNITY_EDITOR
    }
}
//===============================
// date : 2024/10/20
// programmed by Nakagawa Naoto
//===============================