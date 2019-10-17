using GameConfigurationID;
using UnityEngine;

namespace InteractiveObjectTest
{
    [SceneHandleDraw]
    [System.Serializable]
    public class InteractiveObjectInitializer : MonoBehaviour
    {
        [CustomEnum(ConfigurationType = typeof(InteractiveObjectV2Configuration))]
        [DrawConfiguration(ConfigurationType = typeof(InteractiveObjectV2Configuration))]
        public InteractiveObjectV2DefinitionID InteractiveObjectV2DefinitionID;

        public virtual void Init()
        {
            //TODO
            /*
            PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.PuzzleGameConfiguration.InteractiveObjectV2Configuration.ConfigurationInherentData[this.InteractiveObjectV2DefinitionID]
                .BuildInteractiveObject(this.gameObject);
                */
        }

    }

}
