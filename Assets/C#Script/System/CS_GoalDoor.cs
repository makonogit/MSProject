using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ドアを開閉する
 * 
 * 担当：藤原昂祐
 */
public class CS_GoalDoor : MonoBehaviour
{
    [SerializeField, Header("開閉状態")]
    private bool isOpen = false;
    private bool isEnd = false;
    public void Open() { isOpen = true; animSpeed = 1f; isEnd = false; }
    public void Open(float delay) { isOpen = true; animSpeed = 1f; countdown.Initialize(delay); isEnd = false; }
    public void Close() { isOpen = false; animSpeed = -1f; isEnd = false; }
    public void Close(float delay) { isOpen = false; animSpeed = -1f; countdown.Initialize(delay); isEnd = false; }
    public bool GetEnd() => isEnd;

    [Header("アニメーター")]
    [SerializeField, Header("R")]
    private Animator animatorDoorR;
    [SerializeField, Header("L")]
    private Animator animatorDoorL;
    private float animSpeed = 0;

    private CS_Countdown countdown;

    // Start is called before the first frame update
    void Start()
    {
        countdown = gameObject.AddComponent<CS_Countdown>();

        // アニメーションを停止
        animatorDoorR.speed = 0f;
        animatorDoorL.speed = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (countdown.IsCountdownFinished())
        {
            if (isOpen)
            {
                animatorDoorR.speed = animSpeed;
                animatorDoorL.speed = animSpeed;
            }
        }

        if(IsAnimationFinished(animatorDoorR)
            && IsAnimationFinished(animatorDoorL))
        {
            isEnd = true;

            CS_CameraManager.SetNextCamera(-1);
        }
    }

    bool IsAnimationFinished(Animator animator)
    {
        // 現在のアニメーションステートを取得
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // normalizedTimeが1.0以上になったらアニメーションは終了している
        return stateInfo.normalizedTime >= 1f;
    }
}
