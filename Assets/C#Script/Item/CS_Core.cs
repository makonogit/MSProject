using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_Core : MonoBehaviour
{
    [SerializeField, Header("遷移Scene名")]
    private string Phase2SceneName;

    private void OnCollisionEnter(Collision collision)
    {
        //プレイヤーと衝突したか
        bool PlayerHit = collision.transform.tag == "Player";

        if (PlayerHit)
        {
            SceneManager.LoadScene(Phase2SceneName);
        }
    }
}
