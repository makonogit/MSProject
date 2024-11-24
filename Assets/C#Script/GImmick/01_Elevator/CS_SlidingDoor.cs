//-------------------------------
// クラス名 :CS_SlidingDoor
// 内容     :スライドするドア
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.Gimmick;

namespace Assets.C_Script.Gimmick
{
    public class CS_SlidingDoor : CS_MoveObject
    {
        protected override void Execute()
        {
            //base.Execute();
            if(GoEndPoint)Movement(GetPosition());
        }

        protected override void Stopped()
        {
            //base.Stopped();
            if (!GoEndPoint) Movement(GetPosition());
        }
    }
}
