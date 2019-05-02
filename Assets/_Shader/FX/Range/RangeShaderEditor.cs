#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RangeShaderEditor : ShaderGUI
{
    public const string AURA_TEXTURE_ADDITIVE_KEYWORD = "AURA_ADDITVE";
    public const string AURA_TEXTURE_MULTIPLICATIVE_KEYWORD = "AURA_MULTIPLICATIVE";

    private AuraTextureMixType auraTextureMixType;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material targetMat = materialEditor.target as Material;
        this.auraTextureMixType = GetAuraTextureMixTypeFromMaterial(targetMat);
        this.auraTextureMixType = (AuraTextureMixType)EditorGUILayout.EnumPopup("Aura texture blend strategy : ", this.auraTextureMixType);
        base.OnGUI(materialEditor, properties);

        this.SetAuraTextureMixKeyword(targetMat);
    }

    private AuraTextureMixType GetAuraTextureMixTypeFromMaterial(Material targetMat)
    {
        if (Array.IndexOf(targetMat.shaderKeywords, AURA_TEXTURE_ADDITIVE_KEYWORD) != -1)
        {
            return AuraTextureMixType.ADDITIVE;
        }
        else if (Array.IndexOf(targetMat.shaderKeywords, AURA_TEXTURE_MULTIPLICATIVE_KEYWORD) != -1)
        {
            return AuraTextureMixType.MULTIPLICATIVE;
        }
        return default;
    }

    private void SetAuraTextureMixKeyword(Material targetMat)
    {
        targetMat.DisableKeyword(AURA_TEXTURE_ADDITIVE_KEYWORD);
        targetMat.DisableKeyword(AURA_TEXTURE_MULTIPLICATIVE_KEYWORD);

        switch (this.auraTextureMixType)
        {
            case AuraTextureMixType.ADDITIVE:
                targetMat.EnableKeyword(AURA_TEXTURE_ADDITIVE_KEYWORD);
                break;
            case AuraTextureMixType.MULTIPLICATIVE:
                targetMat.EnableKeyword(AURA_TEXTURE_MULTIPLICATIVE_KEYWORD);
                break;
        }

    }

    enum AuraTextureMixType
    {
        ADDITIVE, MULTIPLICATIVE
    }
}
#endif //UNITY_EDITOR