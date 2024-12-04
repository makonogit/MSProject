using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.C_Script.GameEvent;

public class CS_DropGameOver : MonoBehaviour
{
    private CS_CameraManager cameraManager;     // �J�����}�l�[�W���[
    private GameObject tpsCamera;               // TPS�J����
    private GameObject fallingCamera;           // �������J����


    // Start is called before the first frame update
    void Start()
    {
        cameraManager = GameObject.Find("CameraManager")?.GetComponent<CS_CameraManager>();
        tpsCamera = GameObject.Find("TpsCamera");
        fallingCamera = GameObject.Find("FallingCamera");
        if (fallingCamera == null)
        {
            UnityEngine.Debug.LogError("FallingCamera��Hierarchy�ɒǉ����Ă��������B");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // ���C���J�����̈ʒu�����g�ɐݒ�
            fallingCamera.transform.position = tpsCamera.transform.position;
            fallingCamera.transform.rotation = tpsCamera.transform.rotation;

            // �J�����̐؂�ւ�
            cameraManager.SwitchingCamera(1);

            CSGE_GameOver.GameOver();
        }
    }
}
