using AdventureGame;
using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace Tests
{
    public class PointOfInterestInitialization
    {
        private PointOfInterestDefinitionID PointOfInterestDefinitionID;
        public PointOfInterestDefinitionInherentData PointOfInterestDefinitionInherentData;
        public PointOfInterestInitializationObject PointOfInterestInitializationObject;

        public void InitializeTestConfigurations(PointOfInterestTestID PointOfInterestTestID)
        {
            var AdventureGameConfiguration = AssetFinder.SafeSingleAssetFind<AdventureGameConfiguration>("t:" + typeof(AdventureGameConfiguration));
            var pointOfInterestIDTree = PointOfInterestIDTree.PointOfInterestTestIDs[PointOfInterestTestID];

            if (PointOfInterestInitializationObject != null)
                PointOfInterestInitializationObject.PointOfInterestVisualMovementInherentData.IfNotNull(PointOfInterestVisualMovementInherentData =>
                    AdventureGameConfiguration.PointOfInterestVisualMovementConfiguration.ConfigurationInherentData[pointOfInterestIDTree.PointOfInterestVisualMovementID] = PointOfInterestVisualMovementInherentData);

            PointOfInterestDefinitionID = pointOfInterestIDTree.PointOfInterestDefinitionID;
            AdventureGameConfiguration.PointOfInterestDefinitionConfiguration.ConfigurationInherentData[pointOfInterestIDTree.PointOfInterestDefinitionID] = PointOfInterestDefinitionInherentData;
        }

        public PointOfInterestType Instanciate(Vector3 worldPosition)
        {
            var AdventureGameConfiguration = AssetFinder.SafeSingleAssetFind<AdventureGameConfiguration>("t:" + typeof(AdventureGameConfiguration));
            var AdventureStaticConfiguration = AssetFinder.SafeSingleAssetFind<AdventureStaticConfiguration>("t:" + typeof(AdventureStaticConfiguration));
            var PointOfInterestType = MonoBehaviour.Instantiate(AdventureStaticConfiguration.AdventurePrefabConfiguration.BasePointOfInterestType);
            PointOfInterestType.PointOfInterestDefinitionID = PointOfInterestDefinitionID;
            PointOfInterestType.transform.position = worldPosition;
            //PointOfInterestDefinitionInherentData.DefinePointOfInterest(PointOfInterestType, AdventureStaticConfiguration.AdventurePrefabConfiguration);
            return PointOfInterestType;
        }

        public PointOfInterestType InstanciateAndInit(Vector3 worldPosition)
        {
            var intstanciatedInteractiveObject = Instanciate(worldPosition);
            intstanciatedInteractiveObject.Init(PointOfInterestInitializationObject);
            return intstanciatedInteractiveObject;
        }
    }
}