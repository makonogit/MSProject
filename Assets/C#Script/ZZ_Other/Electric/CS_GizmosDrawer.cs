//-------------------------------
// クラス名 :CS_GizmosDrawer
// 内容     :CS_Mechanicalを表示する
// 担当者   :中川 直登
//-------------------------------
#if UNITY_EDITOR
using Assets.C_Script.Electric.Mechanical;
using Assets.C_Script.Gimmick;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Assets.C_Script.Electric
{
    public class CS_GizmosDrawer :MonoBehaviour
    {
        [SerializeField]
        private CS_Mechanical mechanical;
        
        [SerializeField]
        private Mesh mesh;
        [SerializeField]
        private Color color;
        [SerializeField]
        private bool lossyScale = false;
        [SerializeField]
        private bool show = false;
        Vector3 position = new Vector3();
        public bool DrawFlag { get; set; }

        private void Start()
        {
            position = transform.position;
        }
        private void Update()
        {
            transform.position = position;
        }

        private void OnDrawGizmos()
        {
            if (show) DrawGizmos();
            if (DrawFlag) DrawGizmos();
            DrawFlag = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (!show) DrawGizmos();
            if (mechanical == null) return;
            mechanical.DrawGizmos();
        }

        public void DrawGizmos() 
        {
            Gizmos.color = color;
            Vector3 pos = GetPosition();
            Vector3 scale = transform.localScale;
            
            if (lossyScale)scale = transform.lossyScale;
            Gizmos.DrawMesh(mesh, 0, pos, this.transform.rotation, scale);
        }

        public Vector3 GetPosition() 
        {
            Vector3 pos = position;
            if (!EditorApplication.isPlaying) pos = this.transform.position;
            return pos;
        }
    }
}
#endif // UNITY_EDITOR
//===============================
// date : 2024/10/20
// programmed by Nakagawa Naoto
//===============================