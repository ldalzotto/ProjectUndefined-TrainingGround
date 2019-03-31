﻿using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Editor_AttractiveObjectVariantWizardEditor
{
    [System.Serializable]
    public class ModelCreation : CreationModuleComponent
    {

        public GameObject ModelAsset;

        public ModelCreation(bool moduleFoldout, bool moduleEnabled, bool moduleDisableAble) : base(moduleFoldout, moduleEnabled, moduleDisableAble)
        {
        }

        protected override string foldoutLabel => "Model creation : ";

        public override void ResetEditor()
        {
            this.ModelAsset = null;
        }

        protected override void OnInspectorGUIImpl()
        {
            this.ModelAsset = EditorGUILayout.ObjectField("Model asset : ", this.ModelAsset, typeof(GameObject), false) as GameObject;
        }
    }
}
