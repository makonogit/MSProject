//-------------------------------
// クラス名 :CS_GizmosDrawer
// 内容     :CS_Mechanicalを表示する
// 担当者   :中川 直登
//-------------------------------
#if UNITY_EDITOR
using Assets.C_Script.Electric.Mechanical;
using UnityEditor;
using UnityEngine;

namespace Assets.C_Script.Electric
{
    public class CS_GizmosDrawer :MonoBehaviour
    {
        [SerializeField]
        private CS_Mechanical mechanical;
        [SerializeField]
        private Mesh mesh;
        Vector3 position = new Vector3();
        public bool DrawFlag { get; set; }

        private void Start()
        {
            position = transform.position;
        }

        private void OnDrawGizmos()
        {
            if (DrawFlag) DrawGizmos();
            DrawFlag = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (mechanical == null) return;
            mechanical.DrawGizmos();
        }

        public void DrawGizmos() 
        {
            Gizmos.color = Color.green;
            Vector3 pos = position;
            if(!EditorApplication.isPlaying) pos =this.transform.position;
            Gizmos.DrawMesh(mesh, 0, pos, this.transform.rotation, this.transform.lossyScale);
        }
    }
}
#endif // UNITY_EDITOR
//===============================
// date : 2024/10/18
// programmed by Nakagawa Naoto
//===============================