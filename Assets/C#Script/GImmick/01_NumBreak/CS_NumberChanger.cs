//-------------------------------
// クラス名 :CS_NumberChanger
// 内容     :数字ブロック等のUV移動による数字変更
// 担当者   :中川 直登
//-------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.Gimmick
{
    public class CS_NumberChanger : MonoBehaviour
    {
        private const int BlockNum = 19476;     // 数字ブロックの頂点番号
        private const int PlaneNum = 3554;    // 板型数字の頂点番号
        private Vector2[] defaultUVs;
        private Vector2[] offsetUV ={
            new Vector2(0.0f, 0.0f),// 1 4 8
            new Vector2(0.5f, 0.0f),// 2 5 9 
            new Vector2(0.0f, 0.5f),// 3 6 10
            new Vector2(0.5f, 0.5f),// 0 7 11
        };
        private MeshFilter meshFilter;
        private Mesh mesh;
        private MeshRenderer meshRenderer;
        [SerializeField]
        private bool isBlockNum = false;
        [SerializeField]
        private List<Material> materials = new List<Material>();

        private int saveNum = 0;
        private bool shouldSet = false;



        private void Start()
        {
            if (!TryGetComponent<MeshFilter>(out meshFilter)) Debug.LogError("null component");
            if (!TryGetComponent<MeshRenderer>(out meshRenderer)) Debug.LogError("null component");
            mesh = meshFilter.mesh;
            defaultUVs = mesh.uv;
        }
        private void Update()
        {
            if(shouldSet) SetNumber(saveNum);
        }
        /// <summary>
        /// 数字を変更
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool SetNumber(int number) 
        {
            saveNum = number;
            shouldSet = true;
            if(meshFilter == null)return false;
            if(meshRenderer == null)return false;
            if (mesh == null) mesh = meshFilter.mesh;
            if (defaultUVs.Length < mesh.uv.Length) defaultUVs = mesh.uv;
            // 0～11の間のみ
            int num = Mathf.Max(Mathf.Min(number, 11),0);
            meshFilter.mesh = GetNewMesh(num);
            meshRenderer.materials = GetMaterial(num);
            shouldSet = false;
            return true;
        }
        /// <summary>
        /// UV位置を変更
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private Mesh GetNewMesh(int number) 
        {
            // 配列番号を求める
            int num = number;
            if (number <= 0) num = 3;
            else if (number < 4) num--;
            num = num % 4;
            // 新しい変数を定義
            Mesh newMesh = mesh;
            Vector2[] uvs = new Vector2[defaultUVs.Length];
            int startNum = (isBlockNum ? BlockNum : PlaneNum);
            for (int i = 0; i < uvs.Length; i++) 
            {
                uvs[i] = defaultUVs[i];
                if (i >= startNum)uvs[i] += offsetUV[num]; 
            }
            newMesh.uv = uvs;
            return newMesh;
        }
        /// <summary>
        /// マテリアルを変更
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private Material[] GetMaterial(int number)
        {
            Material[] material = meshRenderer.materials;
            int num = Mathf.Min((number / 4), materials.Count - 1);
            material[1] = materials[num];
            return material;
        }

        /// <summary>
        /// メッシュレンダラーのオンオフ
        /// </summary>
        /// <param name="enable"></param>
        public void MeshRendererEnable(bool enable) 
        {
            meshRenderer.enabled = enable;
        }
    }
}
