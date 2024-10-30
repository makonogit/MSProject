using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSP_UseItem : ActionBase
{
    [SerializeField, Header("Žg—pó‘Ô")]
    private bool isUse = false;
    public bool GetUse() => isUse;
    public void SetUse(bool flg) { isUse = flg; }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isUse)
        {
            HandleUseItem();
        }
    }

    void HandleUseItem()
    {
        GetSoundEffect().PlaySoundEffect(2, 5);
        GetAnimator().SetBool("Use Item", isUse);
    }
}
