#if UNITY_EDITOR
//-------------------------------
// クラス名 :CS_CheckTest
// 内容     :クラスのチェック用
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
namespace Assets.C_Script.Test
{
    internal class CS_CheckTest : MonoBehaviour
    {

        [SerializeField]
        private Transform otherTransform;
        private LineRenderer lineRenderer;
        
        private void Start() 
        {
            TryGetComponent(out lineRenderer);
            Vector3[] positions ={
                this.transform.position,
                otherTransform.position
            };
            lineRenderer.SetPositions(positions);
        }
        private void Update()
        {
            lineRenderer.SetPosition(0,this.transform.position);
            lineRenderer.SetPosition(1,otherTransform.position);
        }
        

    }
}
#endif //UNITY_EDITOR