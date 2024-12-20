//-------------------------------
// クラス名 :CS_Result
// 内容     :缶が落ちてくる
// 担当者   :中川 直登
//-------------------------------
using Assets.C_Script.GameEvent;
using Assets.C_Script.UI.Gage;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.C_Script.UI.Result
{
    public class CSGE_Result : CS_GameEvent
    {
        private static int bigCanNum = 0;
        [SerializeField]
        private float MaxTime = 400f;  
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
        [SerializeField]
        private AudioMixer audioMixer;

        public static void GettingBigCan() 
        { 
            // CSGE_GameOver.AddBigCanCount(); 
        }


        private static float preTime = 0.0f;
        public static void SetGameOverTime(float time) 
        {
            preTime = time;
        }

        protected override void Awake()
        {
            base.Awake();
            startTime = Time.time;
            arrivalTime = 1 / arrivalTime;
            rankArrivalTime = 1 / rankArrivalTime;
            bigCanNum = 0;
        }
        public void Set()
        {
            bigCanNum = CSGE_GameOver.GetBigCanCount();
            SetClearTime(Time.time - startTime + preTime);
            SetBigCanCount(bigCanNum);
            SetEnergyValue(core.GetEnergy() * 0.1f);
            MoveSlider = true;
        }

        protected override void EventUpdate()
        {
            Time.timeScale = 0.0f;
            audioMixer.SetFloat("MainGameVolume", -80.0f);
            animator.SetBool("Start", true);
            if (MoveSlider) AnimationSlider();
            if (RankAnimationSlider) RankSliderAnimation();
            if (cans.End) decideButton.SetActive(true);
            if (cans.End && inputSystem.GetButtonATriggered()) 
            {
                Time.timeScale = 1f;
                audioMixer.SetFloat("MainGameVolume", 0.0f);
                SceneManager.LoadScene("SC_StageSelect");
            }
        }

        /// <summary>
        /// スライダーのアニメーション
        /// </summary>
        private void AnimationSlider() 
        {
            nowTime += Time.unscaledDeltaTime;
            float value = animationCurve.Evaluate(nowTime * arrivalTime);
            // クリアタイムスライダーのアニメーション
            if (ClearTimeSliderAnimation(value)) return;
            // デカ缶詰スライダーのアニメーション
            if (BigCanSliderAnimation(value)) return;
            // コアエネルギースライダーのアニメーション
            if (CoreEnergySliderAnimation(value)) return;
        }
        /// <summary>
        /// クリアタイムスライダーのアニメーション
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool ClearTimeSliderAnimation(float value) 
        {
            bool isClearTime = clearRate > clearSlider.GetValue() && !clearTime.gameObject.activeSelf;
            bool isEndClear = clearRate <= clearSlider.GetValue() && !clearTime.gameObject.activeSelf;
            
            if (isClearTime)
            {
                clearSlider.SetValue(value * clearRate);
                return true;
            }
            if (isEndClear)
            {
                clearTime.gameObject.SetActive(true);
                nowTime = 0.0f;
                return true;
            }
            return false;
        }

        /// <summary>
        /// デカ缶詰スライダーのアニメーション   
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool BigCanSliderAnimation(float value) 
        {
            bool isBigCan = bigCanRate > BigCanSlider.GetValue() && !BigCanCount.gameObject.activeSelf;
            bool isEndBigCan = bigCanRate <= BigCanSlider.GetValue() && !BigCanCount.gameObject.activeSelf;
            if (isBigCan)
            {
                BigCanSlider.SetValue(value * bigCanRate);
                return true;
            }
            if (isEndBigCan)
            {
                BigCanCount.gameObject.SetActive(true);
                nowTime = 0.0f;
                return true;
            }
            return false;
        }

        
        /// <summary>
        /// コアエネルギースライダーのアニメーション
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CoreEnergySliderAnimation(float value) 
        {
            bool isEnergy = EnergyRate > CoreEnergySlider.GetValue() && !CoreEnergy.gameObject.activeSelf;
            bool isEndEnergy = EnergyRate <= CoreEnergySlider.GetValue() && !CoreEnergy.gameObject.activeSelf;
            if (isEnergy)
            {
                CoreEnergySlider.SetValue(value * EnergyRate);
                return true;
            }
            if (isEndEnergy)
            {
                CoreEnergy.gameObject.SetActive(true);
                nowTime = 0.0f;
                CheckRank(rank);
                animator.SetBool("EnergyMove", true);
                MoveSlider = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// ランクスライダーのアニメーション
        /// </summary>
        private void RankSliderAnimation() 
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
                cans.SetValue(rank);
                cans.isStartDrop = true;
                return;
            }
        }

        /// <summary>
        /// クリアタイムのパーセントを求める
        /// </summary>
        /// <param name="time"></param>
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
                clearRate = (time - timeReviews[num - 1]);
                clearRate /= timeReviews[num] - timeReviews[num - 1];
                clearRate *= rate[num] - rate[num - 1];
                clearRate += rate[num-1];
            }
            else if (num == 0)
            {
                clearRate = (time - MaxTime) / (timeReviews[num] - MaxTime);
                clearRate *= rate[num];
            }
            else clearRate = 1f;
        }

        /// <summary>
        /// デカ缶詰のパーセントを求める
        /// </summary>
        /// <param name="count"></param>
        public float SetBigCanCount(int count) 
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
            return bigCanRate;
        }

        /// <summary>
        /// コアエネルギーのパーセントを設定
        /// </summary>
        /// <param name="value"></param>
        public void SetEnergyValue(float value) 
        {
            float percent = value * 100f;
            CoreEnergy.text = percent.ToString("0.0") + "%";
            rank++;
            for (int i = 0; i < EnergyReviews.Count; i++) if (percent >= EnergyReviews[i]) rank++;
            EnergyRate = value;
        }

        /// <summary>
        /// ランクを求める
        /// </summary>
        /// <param name="rank"></param>
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
