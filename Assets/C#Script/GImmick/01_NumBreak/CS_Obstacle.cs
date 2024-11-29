//-------------------------------
// クラス名 :CS_Obstacle
// 内容     :体力のある障害物
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_Obstacle :MonoBehaviour
    {
        [SerializeField,Range(0,11)]
        private int hp = 5;
        private int hitCount = 0;
        [SerializeField]
        private float PitchUp = 0.5f;
        // 読み取り用
        public int HP { get { return hp; } }
        [SerializeField]
        private GameObject DebrisParticle;

        [SerializeField]
        private CS_NumberChanger numberChanger;

        [SerializeField]
        private AudioClip explosion;
        [SerializeField]
        private AudioClip hitSound;
        private AudioSource audioSource;

        private Collider collider;

        private bool firstTime = false;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == "Attack") HitDamages(1);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Attack") HitDamages(1);
        }

        // スタート
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) Debug.LogError("null AudioSource component");
            hitCount = 0;
            if (numberChanger != null) numberChanger.SetNumber(hp);
            if (TryGetComponent(out collider)) Debug.LogWarning("null collider component");
            firstTime = false;
        }
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
            if (!firstTime)
            {
                // メッシュ非表示
                numberChanger.MeshRendererEnable(false);
                // あたり判定非アクティブ
                collider.enabled = false;
                // 壊れる演出
                CS_BreakBlockManager.CallBreakBlock(transform.position, transform.rotation, transform.localScale);
            }
            // 効果音が終わったら消す
            if(!audioSource.isPlaying)Destroy(this.gameObject);
            firstTime = true;
        }

        /// <summary>
        /// ダメージを与える関数
        /// </summary>
        /// <param name="damage"></param>
        public void HitDamages(int  damage = 0) 
        {
            hp -= damage;
            hitCount++;
            numberChanger.SetNumber(hp);
            PlaySounds(hitCount);
        }

        /// <summary>
        /// 音を鳴らす
        /// </summary>
        private void PlaySounds(int num) 
        {
            int maxNum = 5;
            if (audioSource == null) return;
            audioSource.pitch = 1.0f;
            if (hp == 0) audioSource.clip = explosion;
            else 
            { 
                audioSource.clip = hitSound;
                audioSource.pitch =1.0f + PitchUp * Mathf.Min(num,maxNum);
            }
            audioSource.Play();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            //HitDamages();
        }

#endif // UNITY_EDITOR
    }
}
//===============================
// date : 2024/10/20
// programmed by Nakagawa Naoto
//===============================