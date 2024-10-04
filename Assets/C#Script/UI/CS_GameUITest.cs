using Assets.C_Script.C_GameUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_GameUITest : MonoBehaviour
{
    CS_GameUIClass testUI;

    [SerializeField]
    private GameObject GameOverPanal;

    [SerializeField]
    private GameObject GameClearPanal;

    [SerializeField]
    private RectTransform GageTrans;

    [SerializeField]
    private float MAXHP = 100;

    [SerializeField]
    private float nowHP;

    [SerializeField]
    private CS_InputSystem csinput;

    // Start is called before the first frame update
    void Start()
    {
        testUI = new CS_GameUIClass(MAXHP,GageTrans);
        nowHP = MAXHP;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(csinput.GetButtonAPressed())
        {
            testUI.ViewResultUI(GameOverPanal);
        }


        if (csinput.GetButtonBPressed())
        {
            testUI.ViewResultUI(GameClearPanal);
        }

        //ÉQÅ[ÉW
        if (csinput.GetButtonXPressed())
        {
            testUI.ResourceChange(ref nowHP,-1,GageTrans);
        }

    }
}
