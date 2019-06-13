using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Editor_GameCustomEditors
{
    public abstract class GUIDrawModule<T, C>
    {
        private bool enabled = true;

        public bool Enabled { get => enabled; }

        public void SetEnabled(bool value)
        {
            this.enabled = value;
        }

        public void EditorGUI()
        {
            this.enabled = GUILayout.Toggle(enabled, this.GetType().Name, EditorStyles.miniButton);
        }

        public abstract void SceneGUI(C context, T target);
    }

}