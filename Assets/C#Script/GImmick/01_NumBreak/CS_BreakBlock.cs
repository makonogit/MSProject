//-------------------------------
// クラス名 :CS_BreakBlock
// 内容     :壊れるオブジェクト
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
namespace Assets.C_Script.Gimmick
{
    public class CS_BreakBlock :MonoBehaviour
    {
        private void OnEnable()
        {
            int max = transform.childCount;
            for (int i = 0; i <max ; i++) transform.GetChild(i).gameObject.SetActive(true);
        }
        private void OnDisable()
        {
            int max = transform.childCount;
            for (int i = 0; i < max; i++) transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}
