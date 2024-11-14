//-------------------------------
// クラス名 :CS_TimeDelete 
// 内容     :時間で自己破棄するオブジェクト
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Electric.Other
{
    public class CS_TimeDelete :MonoBehaviour
    {
        [SerializeField,Tooltip("X : Min   Y : Max")]
        private Vector2 randomDeleteTimeConstans = new Vector2();
        [SerializeField]
        private bool isFixedUpdate = true;
        private float deleteTime = 1.0f;
        private float nowTime = 0.0f;
        private void Start() 
        {
            if (randomDeleteTimeConstans.magnitude > 0)
            {
                float min = randomDeleteTimeConstans.x;
                float max = randomDeleteTimeConstans.y;
                deleteTime = Random.Range(min, max);
            }
            nowTime = 0.0f;
        }
        private void FixedUpdate()
        {
            if (isFixedUpdate) TimeCalculations();
        }
        private void Update()
        {
            if (!isFixedUpdate) TimeCalculations();
        }
        /// <summary>
        /// 時間計算
        /// </summary>
        private void TimeCalculations() 
        {
            nowTime += Time.deltaTime;
            if (nowTime >= deleteTime) Destroy(gameObject);
        }
    }
}
