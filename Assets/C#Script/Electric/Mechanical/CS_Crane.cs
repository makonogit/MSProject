//-------------------------------
// クラス名 :CS_Crane
// 内容     :物を運ぶクレーン
// 担当者   :中川 直登
//-------------------------------
using UnityEngine;
using UnityEngine.Splines;

namespace Assets.C_Script.Electric.Mechanical
{
    public class CS_Crane :CS_Mechanical
    {
        private SplineAnimate splineAnimate;
        private float duration;
        private bool Return = false;
        private bool StopCrane = false;
        

        protected void Start()
        {
            splineAnimate = GetComponent<SplineAnimate>();
            if (splineAnimate == null) Debug.LogError("null component");    
        }


        protected override void PowerOn()
        {
            base.PowerOn();
            splineAnimate.Play();
        }

        protected override void PowerOff()
        {
            base.PowerOff();
            splineAnimate.Pause();
        }
        protected override void Execute()
        {
            base.Execute();
            
            if (ShouldStop) 
            {
                Return = !Return;
                Power = false;
            }
        }

        private bool ShouldStop 
        {
            get 
            {
                bool endPoint = splineAnimate.Duration <= splineAnimate.ElapsedTime && !Return;
                bool startPoint = splineAnimate.Duration * 2 <= splineAnimate.ElapsedTime && Return;
                if (endPoint) return true;
                if (startPoint) 
                {
                    splineAnimate.ElapsedTime = 0;
                    return true; 
                }
                return false;
            }
        }
    }
}
//===============================
// date : 2024/10/18
// programmed by Nakagawa Naoto
//===============================