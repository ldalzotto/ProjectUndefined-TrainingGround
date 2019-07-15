using ConfigurationEditor;
using System;
using UnityEngine;

namespace Editor_GameDesigner
{
    public abstract class ConfigurationModule<CONFIGURATION, ID, VALUE> : IGameDesignerModule where ID : Enum where VALUE : ScriptableObject where CONFIGURATION : IConfigurationSerialization
    {
        [SerializeField]
        private IGenericConfigurationEditor configurationEditor;

        public void GUITick(ref GameDesignerEditorProfile GameDesignerEditorProfile)
        {
            this.configurationEditor.GUITick();
        }

        public void OnDisabled()
        {
        }

        public void OnEnabled()
        {
            this.configurationEditor = new GenericConfigurationEditor<ID, VALUE>("t:" + typeof(CONFIGURATION).Name);
        }
    }
}