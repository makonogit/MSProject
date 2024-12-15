//-------------------------------
// クラス名 :CS_GameEvent 
// 内容     :ゲームイベントの基底クラス
// 担当者   :中川 直登
//-------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
namespace Assets.C_Script.GameEvent
{
    public class CSGE_GameOver : CS_GameEvent
    {
        // 静的メンバー
        private static CSGE_GameOver GameOverEvent;
        private static int floorNumber = 0;
        private static int groupNumber = 0;
        private static Vector3 position = Vector3.zero;
        private static Vector3 corePosition = Vector3.zero;
        private static Quaternion rotation = Quaternion.identity;
        public static void GameOver() 
        {
            if (GameOverEvent == null) return;
            GameOverEvent.enabled = true;
        }
        public static void SetFloorNumber(int num) => floorNumber = num;
        public static void SetGroupNumber(int num) => groupNumber = num;
        public static void SetPlayerPosition(Vector3 pos) => position = pos;
        public static void SetPlayerRotation(Quaternion rotate)=> rotation = rotate;
        public static void SetCorePosition(Vector3 pos) => corePosition = pos;
        /// <summary>
        /// リスポーン変数の初期化
        /// </summary>
        public static void InitRespawn() 
        {
            floorNumber = 0;
            groupNumber = 0;
            position = Vector3.zero;
            corePosition = Vector3.zero;
            rotation = Quaternion.identity;
        }


        [SerializeField]
        private float timeSpeed =1.0f;
        [SerializeField]
        private AudioMixer audioMixer;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private CS_InputSystem inputSystem;
        [SerializeField]
        private Transform player;
        [SerializeField]
        private Transform core;
        [SerializeField]
        private bool canContinue = false;
        [SerializeField]
        private GameObject stage;

        protected override void Awake()
        {
            base.Awake();
            GameOverEvent = this;
            RespawnSystem();
        }

        /// <summary>
        /// リスポーン処理
        /// </summary>
        private void RespawnSystem() 
        {
            if (player != null && floorNumber > 0)
            {
                player.position = position;
                player.rotation = rotation;
            }
            if (core != null && floorNumber > 0) core.position = corePosition;

            if (stage == null) Debug.LogError("CSGE_GameOverに stage を設定してください。");

            for (int i = 0; i <= floorNumber; i++)
            {
                if (i != floorNumber) FloorHidden(stage.transform, i);
                else if (groupNumber > 0) FloorGroupHidden(stage.transform, i);
            }
        }

        /// <summary>
        /// 階層非表示+無効化
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="index"></param>
        private void FloorHidden(Transform  trans,int index)
        {
            Transform child = trans.GetChild(index);
            // 非表示
            child.gameObject.SetActive(false);
            // BreakAreaを無効化
            List<Transform> objects = new List<Transform>();
            for (int i = 0; i < child.childCount; i++) objects.AddRange(SearchChildren(child, "BreakArea"));
            foreach (Transform obj in objects) obj.name = "----";
        }
        /// <summary>
        /// 階層内のグループを非表示+無効化
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="index"></param>
        private void FloorGroupHidden(Transform trans,int index) 
        {
            Transform child = trans.GetChild(index);
            List<Transform> objects = new List<Transform>();
            // BreakAreaの検索
            for (int i = 0; i < child.childCount; i++) objects.AddRange(SearchChildren(child, "BreakArea"));
            int max = Mathf.Min(groupNumber, objects.Count);
            // 非表示と無効化
            for (int i = 0; i < groupNumber; i++) 
            {
                objects[i].parent.gameObject.SetActive(false);
                objects[i].name = "---";
            }
        }
        /// <summary>
        /// 子オブジェクトの名前検索
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private List<Transform> SearchChildren(Transform trans,string name) 
        {
            List<Transform> objects = new List<Transform>();
            for (int i = 0; i < trans.childCount; i++) 
            {
                Transform child = trans.GetChild(i);
                if (child.name == name) objects.Add(child);
                objects.AddRange(SearchChildren(child, name));
            }
            return objects;
        }

        protected override void EventUpdate()
        {
            base.EventUpdate();
            animator.SetBool("GameOver", true);
            SetSlowly(timeSpeed);
            if (canContinue&&inputSystem.GetButtonATriggered()) Restart();
        }

        protected override void Init()
        {
            base.Init();
        }

        protected override void Uninit()
        {
            base.Uninit();
        }

        private void Restart()
        {
            canContinue = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            SetSlowly(1.0f);
        }

        /// <summary>
        /// 時間と音の速度設定
        /// </summary>
        /// <param name="value"></param>
        private void SetSlowly(float value) 
        {
            float val = Mathf.Min(Mathf.Max(0f, value), 1f);
            Time.timeScale = val;
            audioMixer.SetFloat("MasterPitch", val);
        }

    }
}
//===============================
// date : 2024/12/01
// programmed by Nakagawa Naoto
//===============================