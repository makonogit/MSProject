//-------------------------------
// クラス名 :CS_Elevator
// 内容     :エレベーターギミック
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Electric.Other;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
namespace Assets.C_Script.Electric.Mechanical
{
    public class CS_Elevator :CS_MoveObject
    {
        [SerializeField]
        private float CountDown = 9.0f;
        private float countDownTime = 9.0f;
        [SerializeField]
        private CS_DrawNumber drawNumber;
        protected override void Start()
        {
            base.Start();
            this.stick = true;
            countDownTime = CountDown;
        }
        protected override void PowerOn()
        {
            base.PowerOn();
            countDownTime = CountDown;
        }
        protected override void Execute()
        {
            //base.Execute();
            countDownTime -= Time.deltaTime;
            if (drawNumber != null) drawNumber.SetNumber(Mathf.FloorToInt(countDownTime));
            if (countDownTime <= 0) this.Movement(this.GetPosition());
            
        }

        protected override void PowerOff()
        {
            base.PowerOff();
            countDownTime = CountDown;
            if (drawNumber != null) drawNumber.SetNumber(Mathf.FloorToInt(countDownTime));
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (EditorApplication.isPlaying) return;
            if (drawNumber != null) drawNumber.SetNumber(Mathf.FloorToInt(CountDown));
        }

#endif // UNITY_EDITOR

    }
}
//===============================
// date : 2024/10/24
// programmed by Nakagawa Naoto
//===============================