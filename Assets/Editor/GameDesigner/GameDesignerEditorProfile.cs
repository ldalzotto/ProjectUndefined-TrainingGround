using UnityEngine;
using System.Collections;
using OdinSerializer;
using GameConfigurationID;

namespace Editor_GameDesigner
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GameDesignerEditorProfile", menuName = "GameDesigner/GameDesignerEditorProfile", order = 1)]
    public class GameDesignerEditorProfile : SerializedScriptableObject
    {
        public int GameDesignerProfileInstanceIndex = 0;
        public GameDesignerTreePickerProfile GameDesignerTreePickerProfile;
        public IGameDesignerModule CurrentGameDesignerModule;
        public Vector2 ScrollPosition;
        public InteractiveObjectModuleWizardID InteractiveObjectModuleWizardID;

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

    [System.Serializable]
    public class InteractiveObjectModuleWizardID
    {
        [CustomEnum()]
        public ObjectRepelID ObjectRepelID;
        [CustomEnum()]
        public AttractiveObjectId AttractiveObjectId;
        [CustomEnum()]
        public LaunchProjectileID LaunchProjectileId;
        [CustomEnum()]
        public TargetZoneID TargetZoneID;
        [CustomEnum()]
        public DisarmObjectID DisarmObjectID;
        [CustomEnum()]
        public ActionInteractableObjectID ActionInteractableObjectID;
        
    }
}
