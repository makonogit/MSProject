using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Reticle : MonoBehaviour
{
    private void FixedUpdate()
    {

        // カメラ正面からレイを作成
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        RaycastHit[] hits = Physics.SphereCastAll(ray, 1f, 100f);


        // レティクル内に敵がいたらゲージを表示
        foreach (RaycastHit hit in hits)
        {
            //----- コフィンのHPゲージ表示 ------------
            if (hit.collider.tag == "Enemy")
            {
                hit.transform.TryGetComponent<CS_CofineAI>(out CS_CofineAI cofine);
                hit.transform.TryGetComponent<CS_DrawnAI>(out CS_DrawnAI drawn);

                if (cofine) { cofine.ViewHPGage(); }
                if (drawn) { drawn.ViewHPGage(); }

            }

        }
    }


}
