//-------------------------------
// クラス名 :CS_CanUiDrop
// 内容     :缶が落ちてくる
// 担当者   :中川 直登
//-------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.C_Script.UI.Result
{
    public class CS_CanUiDrop :MonoBehaviour
    {
        [SerializeField]
        private int Max = 10;
        private int num = 0;
        [SerializeField]
        private float time =1.0f;
        [SerializeField]
        private float up = 400f;
        [SerializeField]
        private AnimationCurve animCurve = new AnimationCurve();
        private Vector3 tmpPos;
        private float oneTime = 0.0f;
        private float rate = 0.0f;
        private float nowTime = 0f;
        private int Index = 0;
        private bool isEnd =false;
        public bool isStartDrop = false;
        private List<Transform> transforms = new List<Transform>();

        private void Start() 
        {
            for (int i = 0; i < transform.childCount; i++) 
            { 
                Transform t = transform.GetChild(i);
                t.position += Vector3.up * up;
                transforms.Add(t); 
            }
            oneTime = time / transform.childCount;
            rate = 1 / oneTime;
            Index = 0;
            tmpPos = transforms[Index].position;
            nowTime = 0;
            if (transforms.Count <= 0) isEnd = true;
        }

        private void OnEnable()=> Init();
        
        private void Init() 
        {
            if (transforms.Count <=0) return;
            if (isEnd)
            { 
                for (int i = 0; i < transforms.Count; i++) transforms[i].position += Vector3.up * up;
            }

            Index = 0;
            tmpPos = transforms[Index].position;
            nowTime = 0;
            isEnd = false;
        }

        private void Update() 
        {
            if (!isEnd && isStartDrop) Drop();
        }
        
        private void Drop() 
        {
            float height = animCurve.Evaluate(nowTime * rate) * up;
            transforms[Index].position = tmpPos + Vector3.down * height;
            if (nowTime >= oneTime) DropEndTime();
            nowTime += Time.unscaledDeltaTime;
        }

        private void DropEndTime()
        {
            Index++;
            if (EndDrop) 
            { 
                isEnd = true; 
                return;
            }
            tmpPos = transforms[Index].position;
            nowTime = 0.0f;
        }

        public void SetValue(int can) 
        {
            float value = can / Max;
            num = Mathf.FloorToInt(value) * transforms.Count;
        }

        private bool EndDrop 
        {
            get 
            {
                bool over = Index >= transforms.Count;
                bool Num = num > 0;
                bool numOver = Index >= num;
                if (over) return true;
                if ( Num && numOver )return true;
                return false;
            }
        }
        public bool End 
        {
            get 
            {
                return isEnd;
            }
        }
    }
}
