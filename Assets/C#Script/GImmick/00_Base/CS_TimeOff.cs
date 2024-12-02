//-------------------------------
// クラス名 :CS_TimeDelete 
// 内容     :時間で自己破棄するオブジェクト
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_TimeOff :MonoBehaviour
    {
        [SerializeField,Tooltip("X : Min   Y : Max")]
        private Vector2 randomDeleteTimeConstans = new Vector2();
        [SerializeField]
        private bool isFixedUpdate = true;
        [SerializeField]
        protected bool CountdownStartFlag = true;
        private float deleteTime = 1.0f;
        private float nowTime = 0.0f;

        protected void OnEnable() =>Init();
        

        virtual protected void Start() =>Init();
        

        private void FixedUpdate()
        {
            if (isFixedUpdate && CountdownStartFlag) TimeCalculations();
        }
        private void Update()
        {
            if (!isFixedUpdate && CountdownStartFlag) TimeCalculations();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        virtual protected void Init()
        {
            if (randomDeleteTimeConstans.sqrMagnitude > 0)
            {
                float min = randomDeleteTimeConstans.x;
                float max = randomDeleteTimeConstans.y;
                deleteTime = Random.Range(min, max);
            }
            nowTime = 0.0f;
        }
        /// <summary>
        /// 時間計算
        /// </summary>
        private void TimeCalculations() 
        {
            nowTime += Time.deltaTime;
            if (nowTime >= deleteTime) gameObject.SetActive(false);
        }
    }
}
