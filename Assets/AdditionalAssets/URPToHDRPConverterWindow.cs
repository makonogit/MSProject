#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class URPToHDRPConverterWindow : EditorWindow
{
    [MenuItem("Tools/URP to HDRP Material Converter")]
    public static void ShowWindow()
    {
        // ウィンドウを表示
        GetWindow<URPToHDRPConverterWindow>("URP to HDRP Material Converter");
    }

    private bool overwriteOriginal = false; // オリジナルマテリアルを上書きするかどうかのフラグ

    void OnGUI()
    {
        GUILayout.Label("URP to HDRP Material Converter", EditorStyles.boldLabel);
        overwriteOriginal = EditorGUILayout.Toggle("Overwrite Original", overwriteOriginal); // 上書きオプションのトグル

        // ボタンがクリックされたらマテリアルを変換
        if (GUILayout.Button("Convert Selected Materials"))
        {
            ConvertSelectedMaterials();
        }
    }

    void ConvertSelectedMaterials()
    {
        // 選択したすべてのマテリアルを取得
        Material[] selectedMaterials = Selection.GetFiltered<Material>(SelectionMode.DeepAssets);
        if (selectedMaterials.Length == 0)
        {
            Debug.LogWarning("Please select at least one URP material."); // マテリアルが選択されていない場合の警告
            return;
        }

        foreach (var urpMaterial in selectedMaterials)
        {
            if (urpMaterial == null)
                continue; // 空のマテリアルをスキップ

            // HDRPマテリアルを作成または上書き
            Material hdrpMaterial = CreateOrOverwriteMaterial(urpMaterial, "HDRP/Lit");

            // プロパティを変換
            ConvertProperties(urpMaterial, hdrpMaterial);
        }

        Debug.Log("Materials converted successfully!"); // 変換成功のメッセージ
    }

    Material CreateOrOverwriteMaterial(Material urpMaterial, string hdrpShaderName)
    {
        Material hdrpMaterial;
        if (overwriteOriginal)
        {
            // オリジナルを上書きする場合
            hdrpMaterial = urpMaterial;
            hdrpMaterial.shader = Shader.Find(hdrpShaderName); // シェーダを変更
        }
        else
        {
            // 新しいHDRPマテリアルを作成する場合
            hdrpMaterial = new Material(Shader.Find(hdrpShaderName));
            string hdrpMaterialPath = AssetDatabase.GetAssetPath(urpMaterial);
            hdrpMaterialPath = hdrpMaterialPath.Replace(".mat", "_HDRP.mat"); // 新しいパスを設定
            AssetDatabase.CreateAsset(hdrpMaterial, hdrpMaterialPath); // アセットを作成
        }

        if (hdrpMaterial.HasProperty("_MaterialType"))
        {
            int materialType = urpMaterial.GetInt("_MaterialType");
            hdrpMaterial.SetFloat("_MaterialType", materialType == 0 ? 0 : 1); // 0: Standard, 1: Unlit
        }
      
        return hdrpMaterial; // 変換したマテリアルを返す
    }

    void ConvertProperties(Material urpMaterial, Material hdrpMaterial)
    {
        // SurfaceOptions
        //if (urpMaterial.HasProperty("_Surface"))
        //{
        //    int surfaceType = urpMaterial.GetInt("_Surface");
        //    hdrpMaterial.SetFloat("_SurfaceType", surfaceType); // SurfaceTypeを設定
        //}

        if (urpMaterial.HasProperty("_Surface"))
        {
            int surfaceType = urpMaterial.GetInt("_Surface");
            hdrpMaterial.SetFloat("_SurfaceType", surfaceType);

            // SurfaceTypeがTransparentの場合、BlendingModeとPreserveSpecularをコピー
            if (surfaceType == 1) // Transparentの数値
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
            hdrpMaterial.SetFloat("_DoubleSidedEnable", renderFace == (int)UnityEngine.Rendering.CullMode.Off ? 1.0f : 0.0f); // RenderFaceを設定
        }

        if (urpMaterial.HasProperty("_AlphaClip"))
        {
            bool alphaClipping = urpMaterial.GetFloat("_AlphaClip") == 1;
            hdrpMaterial.SetFloat("_AlphaCutoffEnable", alphaClipping ? 1.0f : 0.0f); // AlphaClippingを設定

            // AlphaClippingが有効な場合、Thresholdの値もコピー
            if (alphaClipping)
            {
                // URPのThreshold値を取得
                float thresholdValue = urpMaterial.GetFloat("_Cutoff"); // URPのカットオフ値を取得
                hdrpMaterial.SetFloat("_AlphaCutoff", thresholdValue); // HDRPにカットオフ値を設定
            }
        }

        if (urpMaterial.HasProperty("_ReceiveShadows"))
        {
            // URPマテリアルのReceiveShadowsの値を取得
            bool receiveShadows = urpMaterial.GetFloat("_ReceiveShadows") == 1;

            // HDRPマテリアルのReceiveShadowsの設定
            hdrpMaterial.SetFloat("_ReceiveShadows", receiveShadows ? 1.0f : 0.0f);
        }


        if (urpMaterial.HasProperty("_ColorMode"))
        {
            int colorMode = urpMaterial.GetInt("_ColorMode");
            hdrpMaterial.SetFloat("_MaterialID", colorMode); // ColorModeを設定
        }

        // SurfaceInputs
        if (urpMaterial.HasProperty("_BaseMap"))
        {
            hdrpMaterial.SetTexture("_BaseColorMap", urpMaterial.GetTexture("_BaseMap")); // BaseMapを設定
            hdrpMaterial.SetColor("_BaseColor", urpMaterial.GetColor("_BaseColor")); // BaseColorを設定
        }

        if (urpMaterial.HasProperty("_BumpMap"))
        {
            hdrpMaterial.SetTexture("_NormalMap", urpMaterial.GetTexture("_BumpMap")); // NormalMapを設定
        }

        if (urpMaterial.HasProperty("_EmissionColor"))
        {
            Color emissionColor = urpMaterial.GetColor("_EmissionColor");
            bool emissionEnabled = emissionColor.maxColorComponent > 0.0f; // Emissionが有効かどうかを確認
            //hdrpMaterial.SetFloat("_EmissiveColorMode", emissionEnabled ? 1.0f : 0.0f); // Emissionを有効/無効に設定

            if (urpMaterial.HasProperty("_EmissionMap"))
            {
                hdrpMaterial.SetTexture("_EmissiveColorMap", urpMaterial.GetTexture("_EmissionMap")); // EmissionMapを設定
            }

            hdrpMaterial.SetColor("_EmissiveColor", emissionColor); // EmissionColorを設定
            //hdrpMaterial.SetFloat("_EmissiveIntensity", emissionEnabled ? 1.0f : 0.0f); // Emissionの強度を設定
            //hdrpMaterial.SetFloat("_UseEmissiveIntensity", emissionEnabled ? 1.0f : 0.0f); // EmissionIntensityの使用を設定
        }

       

        //// AdvancedOptions
        //if (urpMaterial.HasProperty("_FlipBookMode"))
        //{
        //    int flipBookMode = urpMaterial.GetInt("_FlipBookMode");
        //    hdrpMaterial.SetFloat("_FlipbookBlending", flipBookMode); // Flip-BookBlendingを設定
        //}

        //if (urpMaterial.HasProperty("_SoftParticlesEnabled"))
        //{
        //    bool softParticles = urpMaterial.GetFloat("_SoftParticlesEnabled") == 1;
        //    hdrpMaterial.SetFloat("_EnableFogOnTransparent", softParticles ? 1.0f : 0.0f); // SoftParticlesを設定
        //}

        //if (urpMaterial.HasProperty("_CameraFading"))
        //{
        //    hdrpMaterial.SetFloat("_TransparentZWrite", urpMaterial.GetFloat("_CameraFading")); // CameraFadingを設定
        //}

        //if (urpMaterial.HasProperty("_DistortionEnabled"))
        //{
        //    hdrpMaterial.SetFloat("_DistortionEnable", urpMaterial.GetFloat("_DistortionEnabled")); // Distortionを設定
        //}

        //if (urpMaterial.HasProperty("_QueueOffset"))
        //{
        //    hdrpMaterial.renderQueue = urpMaterial.renderQueue; // RenderQueueを設定
        //}

        // 他のプロパティも必要に応じて追加できます
    }
}
#endif // UNITY_EDITOR