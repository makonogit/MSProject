using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSP_UseEnergyCore : ActionBase
{
    [SerializeField, Header("Žg—pó‘Ô")]
    private bool isUse = false;
    public bool GetUse() => isUse;
    public void SetUse(bool flg) { isUse = flg; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isUse)
        {
            HandleUseEnergyCore();
        }
    }

    void HandleUseEnergyCore()
    {
        GetSoundEffect().PlaySoundEffect(2, 6);
        GetAnimator().SetBool("Use EnergyCore", isUse);
    }
}
