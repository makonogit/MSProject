using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_HPGage : MonoBehaviour
{

    [SerializeField, Tooltip("HPCanvas")]
    private GameObject HPCanvas;
    [SerializeField, Tooltip("HPGage")]
    private Image HPGage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator EndViewHP()
    {
        yield return new WaitForSeconds(3f);

        //çƒÇ—îÒï\é¶Ç…
        HPCanvas.SetActive(false);

    }
}
