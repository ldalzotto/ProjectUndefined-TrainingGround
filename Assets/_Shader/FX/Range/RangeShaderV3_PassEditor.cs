
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class RangeShaderV3_PassEditor : ShaderGUI
{
    private const string AuraTextureName = "_AuraTexture";
    private const string AuraTextureShaderFeature = "AURA_TEXTURED";

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUI.BeginChangeCheck();
        base.OnGUI(materialEditor, properties);
        if (EditorGUI.EndChangeCheck())
        {
            Material targetMat = materialEditor.target as Material;
            var auraTexture = targetMat.GetTexture(AuraTextureName);
            if (auraTexture != null)
            {
                targetMat.EnableKeyword(AuraTextureShaderFeature);
            }
            else
            {
                targetMat.DisableKeyword(AuraTextureShaderFeature);
            }
        }
    }
}
#endif