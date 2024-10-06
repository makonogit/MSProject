using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_Core : MonoBehaviour
{
    [SerializeField, Header("‘JˆÚScene–¼")]
    private string Phase2SceneName;

    private void OnCollisionEnter(Collision collision)
    {
        //ƒvƒŒƒCƒ„[‚ÆÕ“Ë‚µ‚½‚©
        bool PlayerHit = collision.transform.tag == "Player";

        if (PlayerHit)
        {
            SceneManager.LoadScene(Phase2SceneName);
        }
    }
}
