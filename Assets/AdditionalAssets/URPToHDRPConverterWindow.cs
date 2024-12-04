#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class URPToHDRPConverterWindow : EditorWindow
{
    [MenuItem("Tools/URP to HDRP Material Converter")]
    public static void ShowWindow()
    {
        // �E�B���h�E��\��
        GetWindow<URPToHDRPConverterWindow>("URP to HDRP Material Converter");
    }

    private bool overwriteOriginal = false; // �I���W�i���}�e���A�����㏑�����邩�ǂ����̃t���O

    void OnGUI()
    {
        GUILayout.Label("URP to HDRP Material Converter", EditorStyles.boldLabel);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal); // �㏑���I�v�V�����̃g�O��

        // �{�^�����N���b�N���ꂽ��}�e���A����ϊ�
        if (GUILayout.Button("Convert Selected Materials"))
        {
            ConvertSelectedMaterials();
        }
    }

    void ConvertSelectedMaterials()
    {
        // �I���������ׂẴ}�e���A�����擾
        Material[] selectedMaterials = Selection.GetFiltered<Material>(SelectionMode.DeepAssets);
        if (selectedMaterials.Length == 0)
        {
            Debug.LogWarning("Please select at least one URP material."); // �}�e���A�����I������Ă��Ȃ��ꍇ�̌x��
            return;
        }

        foreach (var urpMaterial in selectedMaterials)
        {
            if (urpMaterial == null)
                continue; // ��̃}�e���A�����X�L�b�v

            // HDRP�}�e���A�����쐬�܂��͏㏑��
            Material hdrpMaterial = CreateOrOverwriteMaterial(urpMaterial, "HDRP/Lit");

            // �v���p�e�B��ϊ�
            ConvertProperties(urpMaterial, hdrpMaterial);
        }

        Debug.Log("Materials converted successfully!"); // �ϊ������̃��b�Z�[�W
    }

    Material CreateOrOverwriteMaterial(Material urpMaterial, string hdrpShaderName)
    {
        Material hdrpMaterial;
        if (overwriteOriginal)
        {
            // �I���W�i�����㏑������ꍇ
            hdrpMaterial = urpMaterial;
            hdrpMaterial.shader = Shader.Find(hdrpShaderName); // �V�F�[�_��ύX
        }
        else
        {
            // �V����HDRP�}�e���A�����쐬����ꍇ
            hdrpMaterial = new Material(Shader.Find(hdrpShaderName));
            string hdrpMaterialPath = AssetDatabase.GetAssetPath(urpMaterial);
            hdrpMaterialPath = hdrpMaterialPath.Replace(".mat", "_HDRP.mat"); // �V�����p�X��ݒ�
            AssetDatabase.CreateAsset(hdrpMaterial, hdrpMaterialPath); // �A�Z�b�g���쐬
        }

        if (hdrpMaterial.HasProperty("_MaterialType"))
        {
            int materialType = urpMaterial.GetInt("_MaterialType");
            hdrpMaterial.SetFloat("_MaterialType", materialType == 0 ? 0 : 1); // 0: Standard, 1: Unlit
        }
      
        return hdrpMaterial; // �ϊ������}�e���A����Ԃ�
    }

    void ConvertProperties(Material urpMaterial, Material hdrpMaterial)
    {
        // SurfaceOptions
        //if (urpMaterial.HasProperty("_Surface"))
        //{
        //    int surfaceType = urpMaterial.GetInt("_Surface");
        //    hdrpMaterial.SetFloat("_SurfaceType", surfaceType); // SurfaceType��ݒ�
        //}

        if (urpMaterial.HasProperty("_Surface"))
        {
            int surfaceType = urpMaterial.GetInt("_Surface");
            hdrpMaterial.SetFloat("_SurfaceType", surfaceType);

            // SurfaceType��Transparent�̏ꍇ�ABlendingMode��PreserveSpecular���R�s�[
            if (surfaceType == 1) // Transparent�̐��l
            {
                if (urpMaterial.HasProperty("_Blend"))
                {
                    int blendMode = urpMaterial.GetInt("_Blend");
                    hdrpMaterial.SetFloat("_BlendMode", blendMode);
                }

                if (urpMaterial.HasProperty("_PreserveSpecular"))
                {
                    hdrpMaterial.SetFloat("_PreserveSpecular", urpMaterial.GetFloat("_PreserveSpecular"));
                }

            }
        }



        if (urpMaterial.HasProperty("_Cull"))
        {
            int renderFace = urpMaterial.GetInt("_Cull");
            hdrpMaterial.SetFloat("_DoubleSidedEnable", renderFace == (int)UnityEngine.Rendering.CullMode.Off ? 1.0f : 0.0f); // RenderFace��ݒ�
        }

        if (urpMaterial.HasProperty("_AlphaClip"))
        {
            bool alphaClipping = urpMaterial.GetFloat("_AlphaClip") == 1;
            hdrpMaterial.SetFloat("_AlphaCutoffEnable", alphaClipping ? 1.0f : 0.0f); // AlphaClipping��ݒ�

            // AlphaClipping���L���ȏꍇ�AThreshold�̒l���R�s�[
            if (alphaClipping)
            {
                // URP��Threshold�l���擾
                float thresholdValue = urpMaterial.GetFloat("_Cutoff"); // URP�̃J�b�g�I�t�l���擾
                hdrpMaterial.SetFloat("_AlphaCutoff", thresholdValue); // HDRP�ɃJ�b�g�I�t�l��ݒ�
            }
        }

        if (urpMaterial.HasProperty("_ReceiveShadows"))
        {
            // URP�}�e���A����ReceiveShadows�̒l���擾
            bool receiveShadows = urpMaterial.GetFloat("_ReceiveShadows") == 1;

            // HDRP�}�e���A����ReceiveShadows�̐ݒ�
            hdrpMaterial.SetFloat("_ReceiveShadows", receiveShadows ? 1.0f : 0.0f);
        }


        if (urpMaterial.HasProperty("_ColorMode"))
        {
            int colorMode = urpMaterial.GetInt("_ColorMode");
            hdrpMaterial.SetFloat("_MaterialID", colorMode); // ColorMode��ݒ�
        }

        // SurfaceInputs
        if (urpMaterial.HasProperty("_BaseMap"))
        {
            hdrpMaterial.SetTexture("_BaseColorMap", urpMaterial.GetTexture("_BaseMap")); // BaseMap��ݒ�
            hdrpMaterial.SetColor("_BaseColor", urpMaterial.GetColor("_BaseColor")); // BaseColor��ݒ�
        }

        if (urpMaterial.HasProperty("_BumpMap"))
        {
            hdrpMaterial.SetTexture("_NormalMap", urpMaterial.GetTexture("_BumpMap")); // NormalMap��ݒ�
        }

        if (urpMaterial.HasProperty("_EmissionColor"))
        {
            Color emissionColor = urpMaterial.GetColor("_EmissionColor");
            bool emissionEnabled = emissionColor.maxColorComponent > 0.0f; // Emission���L�����ǂ������m�F
            //hdrpMaterial.SetFloat("_EmissiveColorMode", emissionEnabled ? 1.0f : 0.0f); // Emission��L��/�����ɐݒ�

            if (urpMaterial.HasProperty("_EmissionMap"))
            {
                hdrpMaterial.SetTexture("_EmissiveColorMap", urpMaterial.GetTexture("_EmissionMap")); // EmissionMap��ݒ�
            }

            hdrpMaterial.SetColor("_EmissiveColor", emissionColor); // EmissionColor��ݒ�
            //hdrpMaterial.SetFloat("_EmissiveIntensity", emissionEnabled ? 1.0f : 0.0f); // Emission�̋��x��ݒ�
            //hdrpMaterial.SetFloat("_UseEmissiveIntensity", emissionEnabled ? 1.0f : 0.0f); // EmissionIntensity�̎g�p��ݒ�
        }

       

        //// AdvancedOptions
        //if (urpMaterial.HasProperty("_FlipBookMode"))
        //{
        //    int flipBookMode = urpMaterial.GetInt("_FlipBookMode");
        //    hdrpMaterial.SetFloat("_FlipbookBlending", flipBookMode); // Flip-BookBlending��ݒ�
        //}

        //if (urpMaterial.HasProperty("_SoftParticlesEnabled"))
        //{
        //    bool softParticles = urpMaterial.GetFloat("_SoftParticlesEnabled") == 1;
        //    hdrpMaterial.SetFloat("_EnableFogOnTransparent", softParticles ? 1.0f : 0.0f); // SoftParticles��ݒ�
        //}

        //if (urpMaterial.HasProperty("_CameraFading"))
        //{
        //    hdrpMaterial.SetFloat("_TransparentZWrite", urpMaterial.GetFloat("_CameraFading")); // CameraFading��ݒ�
        //}

        //if (urpMaterial.HasProperty("_DistortionEnabled"))
        //{
        //    hdrpMaterial.SetFloat("_DistortionEnable", urpMaterial.GetFloat("_DistortionEnabled")); // Distortion��ݒ�
        //}

        //if (urpMaterial.HasProperty("_QueueOffset"))
        //{
        //    hdrpMaterial.renderQueue = urpMaterial.renderQueue; // RenderQueue��ݒ�
        //}

        // ���̃v���p�e�B���K�v�ɉ����Ēǉ��ł��܂�
    }
}
#endif // UNITY_EDITOR