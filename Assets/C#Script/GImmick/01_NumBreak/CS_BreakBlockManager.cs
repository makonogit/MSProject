//-------------------------------
// クラス名 :CS_BreakBlockManager
// 内容     :壊れるオブジェクト管理
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_BreakBlockManager : MonoBehaviour
    {
        private static CS_BreakBlockManager manager;
        private int indexNumber = 0;
        private int MaxIndex = 0;

        private void Start()
        {
            if (manager != null) { Destroy(manager); return; }
            manager = this;
            MaxIndex = transform.childCount;
            indexNumber = 0;
        }
        /// <summary>
        /// ブレイクブロックを呼ぶ
        /// </summary>
        /// <param name="index">インデックス番号</param>
        /// <param name="position">ワールド位置</param>
        /// <param name="quaternion">ローカル角度</param>
        /// <param name="scale">ローカルサイズ</param>
        private void callBreakBlock(int index , Vector3 position, Quaternion quaternion,Vector3 scale)
        {
            if (MaxIndex <= 0) return;
            int num = Mathf.Max(Mathf.Min(index, MaxIndex - 1), 0);
            Transform childTrans= transform.GetChild(index);
            childTrans.gameObject.SetActive(false);
            childTrans.position = position;
            childTrans.rotation = quaternion;
            childTrans.localScale = scale;
            childTrans.gameObject.SetActive(true);
        }
        /// <summary>
        /// 次のインデックスに進む
        /// </summary>
        private void NextIndex() 
        {
            indexNumber++;
            if (indexNumber >= MaxIndex) indexNumber = 0;
        }
        /// <summary>
        /// 前のインデックスに戻る
        /// </summary>
        private void PrevIndex() 
        {
            indexNumber--;
            if (indexNumber < 0) indexNumber = MaxIndex - 1;
        }

        /// <summary>
        /// 外部からブレイクブロックを呼ぶ
        /// </summary>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <param name="scale"></param>
        public static void CallBreakBlock(Vector3 position, Quaternion quaternion, Vector3 scale)
        {
            if (manager == null) { Debug.LogWarning("PR_BreakBlockManagerを追加してください。"); return; }
            manager.callBreakBlock(manager.indexNumber, position, quaternion, scale);
            manager.NextIndex();
        }



    }
}
