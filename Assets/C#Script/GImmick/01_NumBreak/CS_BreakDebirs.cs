//-------------------------------
// クラス名 :CS_BreakDebirs
// 内容     :壊れた破片
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
namespace Assets.C_Script.Gimmick
{
    public class CS_BreakDebirs:CS_TimeOff
    {
        protected AudioSource audioSource;
        protected Rigidbody rigidbody;

        [Header("サウンド関係")]
        [SerializeField, Tooltip("速度の範囲:\n X ＝ 下限　Y ＝ 上限\n速度によってピッチの値が変わる範囲。")]
        protected Vector2 VelocityRange = new Vector2(0.5f, 10.0f);
        [SerializeField, Tooltip("ピッチの範囲:\n X ＝ 下限　Y ＝ 上限\n")]
        protected Vector2 PitchRange = new Vector2(0.75f, 2.0f);
        [SerializeField]
        private const float soundVelocity = 1.8f;
        

        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            PlaySound();
            CountdownStartFlag = true;
        }

        protected override void Start()
        {
            base.Start();
            if (!TryGetComponent(out rigidbody)) Debug.LogError("null Rigidbody component");
            if (!TryGetComponent(out audioSource)) Debug.LogError("null AudioSource component");
        }

        protected void OnDisable()
        {
            if (rigidbody == null)return;
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected override void Init()
        {
            base.Init();
            CountdownStartFlag = false;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
            if (rigidbody == null) return;
            rigidbody.useGravity = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = false;
        }


        // 以下より効果音関係の関数のみ

        /// <summary>
        /// 音の再生
        /// </summary>
        protected void PlaySound()
        {
            if (rigidbody == null) return;
            if (audioSource == null) return;
            if (rigidbody.velocity.sqrMagnitude <= soundVelocity * soundVelocity) return;
            audioSource.pitch = GetPitch(PitchRange);
            audioSource.Play();
        }

        /// <summary>
        /// 速度に合わせったピッチを返す
        /// </summary>
        /// <param name="range">範囲</param>
        /// <returns>範囲内</returns>
        private float GetPitch(Vector2 range) => GetPitch(range.x, range.y);

        /// <summary>
        ///  値から指定した範囲の割合(パーセンテージ)を返す
        /// </summary>
        /// <param name="range">範囲</param>
        /// <param name="value">値</param>
        /// <returns> 0～1.0f </returns>
        private float GetProportion(Vector2 range, float value) => GetProportion(range.x, range.y, value);


        /// <summary>
        /// 速度に合わせったピッチを返す
        /// </summary>
        /// <param name="min">下限</param>
        /// <param name="max">上限</param>
        /// <returns>下限～上限</returns>
        private float GetPitch(float min, float max)
        {
            float velocityMag = rigidbody.velocity.magnitude;
            float value = GetProportion(VelocityRange, velocityMag);
            float ratio = max - min;
            value *= ratio;
            value += min;

            return value;
        }


        /// <summary>
        ///  値から指定した範囲の割合(パーセンテージ)を返す 
        /// </summary>
        /// <param name="min">下限</param>
        /// <param name="max">上限</param>
        /// <param name="value">値</param>
        /// <returns> 0～1.0f </returns>
        private float GetProportion(float min, float max, float value)
        {
            float subMax = max - min;
            float val = value - min;
            val /= subMax;

            val = Mathf.Min(1.0f, val);
            val = Mathf.Max(0, val);
            return val;
        }

    }
}
