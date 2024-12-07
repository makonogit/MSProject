using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �S���F���@�X�e�[�W����A���[�g
/// </summary>
public class CS_BreakAlart : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    private float TimeMesure = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(anim.GetBool("Alarm") == false) { return; }

        if(anim.speed <= 0) { return; }

        TimeMesure += Time.deltaTime;

        float time = anim.GetFloat("AlartTime");

        if (TimeMesure > time && anim.GetBool("Alart"))
        {
            anim.SetBool("AlartEnd", true);
        }
        
    }


    /// <summary>
    /// �A���[�g�I��(AnimatorEvent)
    /// </summary>
    void EndAlart()
    {
        Destroy(this.gameObject);
    }
}
