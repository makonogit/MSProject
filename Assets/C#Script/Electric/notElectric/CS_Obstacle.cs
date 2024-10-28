//-------------------------------
// クラス名 :CS_Obstacle
// 内容     :体力のある障害物
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Other;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.C_Script.Electric.notElectric
{
    public class CS_Obstacle :MonoBehaviour
    {
        [SerializeField,Range(0,11)]
        private int hp = 5;
        // 読み取り用
        public int HP { get { return hp; } }
        [SerializeField]
        private ParticleSystem DebrisParticle;

        [SerializeField]
        private List<CS_ChangeMaterial> changeMaterials = new List<CS_ChangeMaterial>();

        [SerializeField]
        private AudioClip explosion;
        [SerializeField]
        private List<AudioClip>audioClips = new List<AudioClip>();
        private AudioSource audioSource;

        // スタート
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) Debug.LogError("null AudioSource component");
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
            
            if(!audioSource.isPlaying)Destroy(this.gameObject);
        }

        /// <summary>
        /// ダメージを与える関数
        /// </summary>
        /// <param name="damage"></param>
        public void HitDamages(int  damage = 0) 
        {
            hp -= damage;
            ChangeNumber();
            PlaySounds();
        }

        /// <summary>
        /// 数字の変更
        /// </summary>
        private void ChangeNumber() 
        {
            int[] index = {3,7,11};
            int IndexNumber = 0;
            for (int i = 0; i < index.Length; i++) 
            { 
                if (hp <= index[i]) 
                { 
                    IndexNumber = i; 
                    break; 
                }
            }
            foreach (CS_ChangeMaterial changeMaterial in changeMaterials)changeMaterial.gameObject.SetActive(false);

            if (hp < 4) // 0～3
            {
                changeMaterials[hp % 4].ChangeMaterial(IndexNumber);
                changeMaterials[hp % 4].gameObject.SetActive(true);
            }
            else // 4～11
            {
                changeMaterials[(hp + 1) % 4].ChangeMaterial(IndexNumber);
                changeMaterials[(hp + 1) % 4].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 音を鳴らす
        /// </summary>
        private void PlaySounds() 
        {
            if (audioSource == null) return;
            int audioNum = Mathf.Max(Mathf.Min(hp, audioClips.Count), 0);
            if (audioNum == 0) audioSource.clip = explosion;
            else audioSource.clip = audioClips[audioNum - 1];
            audioSource.Play();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            HitDamages();
        }

#endif // UNITY_EDITOR
    }
}
//===============================
// date : 2024/10/20
// programmed by Nakagawa Naoto
//===============================