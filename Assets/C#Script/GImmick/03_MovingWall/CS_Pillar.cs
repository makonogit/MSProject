//-------------------------------
// クラス名 :CS_PillarPressMachine
// 内容     :プレス機の柱の処理
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    internal class CS_Pillar : MonoBehaviour
    {
        private Vector3 firstPosition;
        private Transform Wall;

        private void Start()
        {
            Wall = transform.parent;
            firstPosition = Wall.localPosition;
        }

        private void FixedUpdate()
        {
            Vector3 scale = this.transform.localScale;
            scale.y = Wall.localPosition.y - firstPosition.y;
            scale.y *= -0.005f;
            this.transform.localScale = scale;
        }
    }
}
