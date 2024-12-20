using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * �h�A���J����
 * 
 * �S���F�����V�S
 */
public class CS_GoalDoor : MonoBehaviour
{
    [SerializeField, Header("�J���")]
    private bool isOpen = false;
    private bool isEnd = false;
    public void Open() { isOpen = true; animSpeed = 1f; isEnd = false; }
    public void Open(float delay) { isOpen = true; animSpeed = 1f; countdown.Initialize(delay); isEnd = false; }
    public void Close() { isOpen = false; animSpeed = -1f; isEnd = false; }
    public void Close(float delay) { isOpen = false; animSpeed = -1f; countdown.Initialize(delay); isEnd = false; }
    public bool GetEnd() => isEnd;

    [Header("�A�j���[�^�[")]
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

        // �A�j���[�V�������~
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
        // ���݂̃A�j���[�V�����X�e�[�g���擾
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // normalizedTime��1.0�ȏ�ɂȂ�����A�j���[�V�����͏I�����Ă���
        return stateInfo.normalizedTime >= 1f;
    }
}
