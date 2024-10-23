//-------------------------------
// クラス名 :CS_Obstacle
// 内容     :体力のある障害物
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Electric.notElectric
{
    public class CS_Obstacle :MonoBehaviour
    {
        [SerializeField,Range(0,10)]
        private int hp = 5;
        // 読み取り用
        public int HP { get { return hp; } }
        [SerializeField]
        private ParticleSystem DebrisParticle;

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
        }
    }
}
//===============================
// date : 2024/10/20
// programmed by Nakagawa Naoto
//===============================