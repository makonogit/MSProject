//-------------------------------
// クラス名 :CS_ElevatorLamp
// 内容     :エレベータ―ランプのオンオフ
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Gimmick;
using UnityEngine;

namespace Assets.C_Script.Gimmick 
{ 

    public class CS_ElevatorLamp : CS_Mechanical
    {
        private MeshFilter meshFilter;
        private Mesh mesh;
        private Vector2[] uv;
        private const float offsetU = 0.0f;
        private const float offsetV = 0.5f;
        private const int uvNum = 980;

        protected override void Start()
        {
            if (!TryGetComponent(out meshFilter)) Debug.LogError("null MeshFilter");
            mesh = meshFilter.mesh;
            uv = meshFilter.mesh.uv;
            Change();
        }
        protected override void PowerOn()
        {
            base.PowerOn();
            Change();
        }
        protected override void PowerOff()
        {
            base.PowerOff();
            Change();
        }


        private void Change()
        {
            Vector2[] newUV = uv;
            Vector2 offsetUV = new Vector2(offsetU, offsetV);
            Mesh newMesh = mesh;
            for (int i = uvNum; i < newUV.Length; i++) newUV[i] += offsetUV;
            newMesh.uv = newUV;
            meshFilter.mesh = newMesh;
        }

    }
}
//===============================
// date : 2024/11/20
// programmed by Nakagawa Naoto
//===============================