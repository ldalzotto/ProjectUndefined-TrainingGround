using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace Editor_GameDesigner
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GameDesignerEditorProfile", menuName = "GameDesigner/GameDesignerEditorProfile", order = 1)]
    public class GameDesignerEditorProfile : SerializedScriptableObject
    {
        public GameDesignerTreePickerProfile GameDesignerTreePickerProfile;
        public IGameDesignerModule CurrentGameDesignerModule;

        public void ChangeCurrentModule(IGameDesignerModule nextModule)
        {
            if (this.CurrentGameDesignerModule != null)
            {
                this.CurrentGameDesignerModule.OnDisabled();
            }
            this.CurrentGameDesignerModule = nextModule;
            this.CurrentGameDesignerModule.OnEnabled();
        }
    }

    [System.Serializable]
    public class GameDesignerTreePickerProfile
    {
        public string SelectedKey;
    }

}
