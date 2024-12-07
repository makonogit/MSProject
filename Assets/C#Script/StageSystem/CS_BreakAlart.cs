using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 担当：菅　ステージ崩壊アラート
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
    /// アラート終了(AnimatorEvent)
    /// </summary>
    void EndAlart()
    {
        Destroy(this.gameObject);
    }
}
