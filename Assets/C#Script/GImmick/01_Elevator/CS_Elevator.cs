//-------------------------------
// クラス名 :CS_Elevator
// 内容     :エレベーターギミック
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
namespace Assets.C_Script.Gimmick
{
    public class CS_Elevator :CS_MoveObject
    {
        [SerializeField]
        private float CountDown = 9.0f;
        private float countDownTime = 9.0f;
        private int oldTime = 0;
        private bool firstTime = false;

        [SerializeField]
        private Material gauge;
        [Header("効果音")]
        // 効果音
        [SerializeField]
        private AudioClip countSound;
        [SerializeField]
        private AudioClip moveSound;
        [SerializeField]
        private AudioClip riseSound;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioSource countAudioSource;
        [SerializeField]
        private AudioSource moveAudioSource;



        protected override void Start()
        {
            base.Start();
            this.stick = true;
            countDownTime = CountDown;
            SetMaterialGauge();
        }
        protected override void PowerOn()
        {
            base.PowerOn();
            countDownTime = CountDown;
            firstTime = true;
        }
        protected override void Execute()
        {
            //base.Execute();
            countDownTime -= Time.deltaTime;
            if (countDownTime <= 0)
            {
                this.Movement(this.GetPosition());
                if (firstTime) SoundPlay(audioSource,riseSound,false);
                else SoundPlay(moveAudioSource,moveSound, true);
                firstTime = false;
            }
            else if (Mathf.FloorToInt(countDownTime) != oldTime) 
            {
                SoundPlay(countAudioSource,countSound, false);
            }
            oldTime = Mathf.FloorToInt(countDownTime);
            SetMaterialGauge();
        }
        protected override void PowerOff()
        {
            base.PowerOff();
            countDownTime = CountDown;
            if (moveAudioSource.isPlaying)moveAudioSource.Stop();
            if (countAudioSource.isPlaying)countAudioSource.Stop();
            if (audioSource.isPlaying)audioSource.Stop();
            SetMaterialGauge();
        }

        private void SoundPlay(AudioSource audio,AudioClip clip,bool loop) 
        {
            if (clip == null) return;
            if (audio.isPlaying) return;
            audio.clip = clip;
            audio.loop = loop;
            audio.Play();
        }

        private void SetMaterialGauge() 
        {
            float value = 1.0f - (countDownTime / CountDown);
            value = Mathf.Max(0f, Mathf.Min(value, 1f));
            if (gauge != null) gauge.SetFloat("_FillAmount", value);
        }
    }
}
//===============================
// date : 2024/10/24
// programmed by Nakagawa Naoto
//===============================