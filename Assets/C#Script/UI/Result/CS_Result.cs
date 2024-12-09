//-------------------------------
// クラス名 :CS_Result
// 内容     :缶が落ちてくる
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.GameEvent;
using Assets.C_Script.UI.Gage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.C_Script.UI.Result
{
    public class CS_Result : CS_GameEvent
    {
        [SerializeField]
        private List<float> timeReviews = new List<float>();
        [SerializeField]
        private List<int> CountReviews = new List<int>();
        [SerializeField]
        private List<float> EnergyReviews= new List<float>();
        [SerializeField]
        private List<int> RankReviews = new List<int>();
        private bool MoveSlider = false;
        [SerializeField]
        private float arrivalTime = 0.0f;
        [SerializeField]
        private float rankArrivalTime = 0.0f;
        [SerializeField]
        private AnimationCurve animationCurve = new AnimationCurve();

        [SerializeField]
        private bool RankAnimationSlider = false;

        private float startTime = 0.0f;
        private int rank = 0;

        [SerializeField]
        private CS_CanUiDrop cans;
        
        private float clearRate = 0.0f;
        private float bigCanRate = 0.0f;
        private float EnergyRate = 0.0f;
        private float rankRate = 0.0f;

        [SerializeField]
        private Text clearTime;
        [SerializeField]
        private CS_Slider clearSlider;
        

        [SerializeField]
        private Text BigCanCount;
        [SerializeField]
        private CS_Slider BigCanSlider;

        [SerializeField]
        private Text CoreEnergy;
        [SerializeField]
        private CS_Slider CoreEnergySlider;
        

        [SerializeField]
        private Text Rank;
        [SerializeField]
        private CS_Slider RankSlider;
        
        [SerializeField]
        private List<float> rate = new List<float>();
        private float nowTime=0.0f;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private CS_EnergyCure core;
        [SerializeField]
        private CS_InputSystem inputSystem;
        [SerializeField]
        private GameObject decideButton;

        protected override void Awake()
        {
            base.Awake();
            startTime = Time.time;
            arrivalTime = 1 / arrivalTime;
            rankArrivalTime = 1 / rankArrivalTime;
        }
        public void Set() 
        {
            SetClearTime(Time.time - startTime);
            SetBigCanCount(10);
            SetEnergyValue(core.GetEnergy() * 0.1f);
            MoveSlider = true;
        }

        protected override void EventUpdate()
        {
            Time.timeScale = 0.0f;
            animator.SetBool("Start", true);
            if (MoveSlider) AnimationSlider();
            if (RankAnimationSlider) RankAnimation();
            if (cans.End) decideButton.SetActive(true);
            if (cans.End && inputSystem.GetButtonATriggered()) 
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("SC_StageSelect");
            }
        }

        private void AnimationSlider() 
        {
            nowTime += Time.unscaledDeltaTime;
            float value = animationCurve.Evaluate(nowTime * arrivalTime);
            bool isClearTime = clearRate > clearSlider.GetValue() && !clearTime.gameObject.activeSelf;
            bool isEndClear = clearRate <= clearSlider.GetValue() && !clearTime.gameObject.activeSelf;
            if (isClearTime) 
            { 
                clearSlider.SetValue(value * clearRate); 
                return;
            }
            if (isEndClear) 
            {
                clearTime.gameObject.SetActive(true);
                nowTime = 0.0f;
                return;
            }

            bool isBigCan = bigCanRate > BigCanSlider.GetValue() && !BigCanCount.gameObject.activeSelf;
            bool isEndBigCan = bigCanRate <= BigCanSlider.GetValue() && !BigCanCount.gameObject.activeSelf;
            if (isBigCan) 
            { 
                BigCanSlider.SetValue(value * bigCanRate); 
                return;
            }
            if (isEndBigCan)
            {
                BigCanCount.gameObject.SetActive(true);
                nowTime = 0.0f;
                return ;
            }

            bool isEnergy = EnergyRate > CoreEnergySlider.GetValue() && !CoreEnergy.gameObject.activeSelf;
            bool isEndEnergy = EnergyRate <= CoreEnergySlider.GetValue() && !CoreEnergy.gameObject.activeSelf;
            if (isEnergy) 
            { 
                CoreEnergySlider.SetValue(value * EnergyRate); 
                return;
            }
            if (isEndEnergy)
            {
                CoreEnergy.gameObject.SetActive(true);
                nowTime = 0.0f;
                CheckRank(rank);
                animator.SetBool("EnergyMove", true);
                MoveSlider = false;
                return;
            }

        }
        private void RankAnimation() 
        {
            nowTime += Time.unscaledDeltaTime;
            float value = animationCurve.Evaluate(nowTime * rankArrivalTime);
            bool isClearTime = rankRate > RankSlider.GetValue() && !Rank.gameObject.activeSelf;
            bool isEndClear = rankRate <= RankSlider.GetValue() && !Rank.gameObject.activeSelf;
            if (isClearTime) RankSlider.SetValue(value * rankRate);
            if (isEndClear)
            {
                Rank.gameObject.SetActive(true);
                nowTime = 0.0f;
                cans.SetValue(10);
                cans.isStartDrop = true;
                return;
            }
        }

        public void SetClearTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time - minutes * 60f);
            clearTime.text =minutes.ToString("00")+"：" + seconds.ToString("00");
            rank++;
            int num = 0;
            for (int i = 0; i < timeReviews.Count; i++) 
            {
                if (time <= timeReviews[i]) 
                {
                    rank++;
                    num = i + 1;
                }
            }
            if (num > 0 && num < timeReviews.Count)
            {
                clearRate = (time - timeReviews[num]);
                clearRate /= timeReviews[num];
                clearRate = 1 - clearRate;
                clearRate *= rate[num] - rate[num - 1];
                clearRate += rate[num-1];
            }
            else if (num == 0)
            {
                clearRate = time / timeReviews[num];
                clearRate = 1 - clearRate;
                clearRate *= rate[num];
            }
            else clearRate = 1f;
        }

        public void SetBigCanCount(int count) 
        {
            BigCanCount.text = count.ToString()+"個";
            rank++;
            int num = 0;
            for (int i = 0; i < CountReviews.Count; i++) 
            {
                if (count >= CountReviews[i]) 
                {
                    rank++;
                    num = i + 1;
                }
            }
            if (num > 0 && num < CountReviews.Count)
            {
                bigCanRate = (count - CountReviews[num]);
                bigCanRate /= CountReviews[num] - CountReviews[num - 1];
                bigCanRate *= rate[num] - rate[num - 1];
                bigCanRate += rate[num];
            }
            else if (num == 0)
            {
                bigCanRate = (float)count / (float)CountReviews[num];
                bigCanRate *= rate[num];
            }
            else bigCanRate = 1f;
        }

        public void SetEnergyValue(float value) 
        {
            float percent = value * 100f;
            CoreEnergy.text = percent.ToString() + "%";
            rank++;
            for (int i = 0; i < EnergyReviews.Count; i++) if (percent >= EnergyReviews[i]) rank++;
            EnergyRate = value;
        }

        private void CheckRank(int rank) 
        {
            const int B = 5;
            const int A = 8;
            const int S = 12;
            int num = 0;
            if (rank < B)
            {
                Rank.text = "C";
            }
            else if (rank < A)
            {
                Rank.text = "B";
                num = 1;
            }
            else if (rank < S)
            {
                Rank.text = "A";
                num = 2;
            }
            else 
            { 
                Rank.text = "S"; 
                num = 3;
            }
            if (num > 0 && num < RankReviews.Count) 
            {
                rankRate = (rank - RankReviews[num]);
                rankRate /= (float)(RankReviews[num] - RankReviews[num - 1]);
                rankRate *= rate[num] - rate[num - 1];
                rankRate += rate[num];
            }
            else if ( num == 0)
            {
                rankRate = ((float)rank / (float)RankReviews[num]);
                rankRate *= rate[num];
            }
            else
            {
                rankRate = 1f;
            }

        }
    }
}
