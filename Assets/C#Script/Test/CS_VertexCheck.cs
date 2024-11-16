using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.C_Script.Test
{
    [RequireComponent(typeof(MeshFilter))]
    public class CS_VertexCheck : MonoBehaviour
    {
        private Mesh meshOld;
        private MeshFilter meshFilter;
        private Mesh meshNew;
        private int num;
        private int nums=10;
        [SerializeField]
        private float nowTime;
        [SerializeField]
        private float overTime = 0.5f;
        [SerializeField]
        private float over = 0.9985f;
        private Vector2[] uv;
        private void Start() 
        {
            this.gameObject.TryGetComponent<MeshFilter>(out meshFilter);
            meshOld = meshFilter.mesh;
            meshNew = meshFilter.mesh;
            num = 0;
            nowTime = 0;
            meshNew.name = "New Mesh";
            uv = meshNew.uv;
            Debug.Log(uv.Length * over);
            // 数字ブロックUV 19476～
            // 数字プレートUV 3553～

        }

        private void Update()
        {
            nowTime += Time.deltaTime;
            if (nowTime >= overTime) function();
        }

        private void function()
        {
            meshNew.uv = GetUV(3554,new Vector2(0.5f,0.0f));
            meshNew.name += "|";
            num++;
            num = Mathf.Min(num, meshNew.uv.Length - 1);
            nowTime = 0;
            meshFilter.mesh = meshNew;  
        }
        private Vector2[] GetUV(int startNum,Vector2 offset) 
        {
            Vector2[] newUv = uv;
            for (int i = startNum; i < newUv.Length; i++) newUv[i] += offset;
            return newUv;
        }


    }
}
