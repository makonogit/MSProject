//-------------------------------
// クラス名 :CS_DrawNumber 
// 内容     :数字表示
// 担当者   :中川 直登
//-------------------------------
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace Assets.C_Script.Electric.Other
{
    public class CS_DrawNumber :MonoBehaviour
    {
        [SerializeField,Tooltip("小数点表示")]
        private bool floatFlag;
        [SerializeField, Tooltip("マイナス表示")]
        private bool minusFlag;
        [SerializeField, Tooltip("中心表示")]
        private bool centerFlag;
        [SerializeField]
        private Gradient gradient = new Gradient();
        [SerializeField]
        private Vector2 minMax = new Vector2(0,100);
        private SpriteRenderer minusRenderers;
        [SerializeField,Range(0.0f,1.0f)]
        private float ratio = 1.0f;

        [SerializeField]
        private List<Sprite> numberSprite = new List<Sprite>();
        [SerializeField,Tooltip("マイナス スプライト")]
        private Sprite minusSprite;
        [SerializeField, Tooltip("小数点 スプライト")]
        private Sprite decimalPointSprite;

        [SerializeField]
        private List<SpriteRenderer> numberRenderers = new List<SpriteRenderer>();
        [SerializeField]
        private List<SpriteRenderer> floatRenderers = new List<SpriteRenderer>();
        
        private int oldNumber;

        //public void SetNumber(float num) 
        //{             
        //}

        public void SetNumber(int num)
        {
            bool isMinus = num < 0;
            bool shouldReturn = isMinus && !minusFlag;
            bool shouldDrawMinusBer = isMinus && minusFlag;
            bool isNoChange = num == oldNumber;
            
            if (shouldReturn) return;
            if (isNoChange) return;
            float time = (num - minMax.x)/minMax.y;
            Color  color = gradient.Evaluate(time);
            List<int>numbers = GetIntegerNumbers(num);
            DrawIntegers(numbers,color);
            oldNumber = num;
        }

        /// <summary>
        /// 各桁の整数をリストで取得
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private List<int>GetIntegerNumbers(int num) 
        {
            List<int> numbers = new List<int>();
            for (float i = num; i >= 1.0f; i *= 0.1f) numbers.Add(Mathf.FloorToInt(i) % 10);
            if (num == 0) numbers.Add(num);
            return numbers;
        }

        /// <summary>
        /// 整数の表示
        /// </summary>
        /// <param name="numbers"></param>
        private void DrawIntegers(List<int> numbers,Color color) 
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                // ない場合は作る
                if (i == numberRenderers.Count) CreateSpriteRenderer(i);
                numberRenderers[i].enabled = true;
                numberRenderers[i].sprite = GetSpriteNumber(numbers[i]);
                numberRenderers[i].color = color;
                numberRenderers[i].transform.localPosition = GetSpritePosition(i,numbers.Count);
            }
            for (int i = numbers.Count ; i < numberRenderers.Count; i++) numberRenderers[i].enabled = false;
            ShiftToCenter(numbers.Count);
        }

        /// <summary>
        /// スプライトレンダラーを追加生成する
        /// </summary>
        private void CreateSpriteRenderer(int num) 
        {
            int nameNumber = 1;
            for (int i = 0; i < num; i++) nameNumber *= 10;
            GameObject gameObject = new GameObject();
            SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.transform.parent = transform;
            renderer.transform.name = "Number:" + nameNumber.ToString();
            renderer.transform.position = this.transform.position;
            renderer.transform.rotation = this.transform.rotation;
            numberRenderers.Add(renderer);
        }

        /// <summary>
        /// スプライトの置く位置を取得
        /// </summary>
        /// <param name="num"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        private Vector3 GetSpritePosition(int num,int Max)
        {
            Vector3 position = new Vector3();
            if (num <=0) return position;
            position += numberRenderers[num - 1].transform.localPosition;
            position.x -= numberRenderers[num - 1].transform.lossyScale.x * ratio;
            // ずらす
            return position;
        }

        /// <summary>
        /// 数字から対応したスプライトを返す
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Sprite GetSpriteNumber(int value) 
        {
            int number = Mathf.Min(Mathf.Max(0, value),9);
            if (numberSprite.Count == 0) Debug.LogError("no sprites set yet");
            return numberSprite[number];
        }

        /// <summary>
        /// 真ん中にずらす
        /// </summary>
        /// <param name="Count"></param>
        private void ShiftToCenter(int Count) 
        {
            if (!centerFlag) return;
            if (Count == 1) return;
            for (int i = 0; i < Count; i++) numberRenderers[i].transform.localPosition += GetShiftVector(Count);
        }

        /// <summary>
        /// ずらす方向
        /// </summary>
        /// <param name="Count"></param>
        /// <returns></returns>
        private Vector3 GetShiftVector(int Count) 
        {
            Vector3 vec = new Vector3();
            vec.x = numberRenderers[0].transform.lossyScale.x * ratio * Count * 0.25f;
            return vec;
        }

//#if UNITY_EDITOR

//        [SerializeField,Range(0,100)]
//        private int number = 0;
        
//        private void OnValidate()
//        {
//            SetNumber(number);
//        }
//#endif // UNITY_EDITOR

    }
}
//===============================
// date : 2024/10/24
// programmed by Nakagawa Naoto
//===============================
