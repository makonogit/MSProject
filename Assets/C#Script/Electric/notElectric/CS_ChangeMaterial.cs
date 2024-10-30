//-------------------------------
// クラス名 :CS_ChangeMaterial
// 内容     :マテリアルを変更するクラス
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.Electric.notElectric
{
    public class CS_ChangeMaterial :MonoBehaviour
    {
        [SerializeField]
        private List<MeshRenderer> renderers = new List<MeshRenderer>();
        [SerializeField]
        private List<Material> materials = new List<Material>();

        /// <summary>
        /// マテリアル変更
        /// </summary>
        /// <param name="index">マテリアルの配列番号を指定</param>
        /// <returns> 変更した True　出来なかった False </returns>
        public bool ChangeMaterial(int index) 
        {
            if (materials.Count <= index) return false;
            if (index < 0) return false;
            if (materials[index] == null)return false;
            foreach (var renderer in renderers)renderer.material = materials[index];
            return true;
        }
    }
}
//===============================
// date : 2024/10/28
// programmed by Nakagawa Naoto
//===============================