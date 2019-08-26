using RTPuzzle;
using UnityEngine;

namespace Tests
{
    public class RangeObjectInitialization
    {
        public RangeTypeObjectDefinitionInherentData RangeTypeObjectDefinitionInherentData;
        
        public RangeTypeObject Instanciate(Vector3 position, Quaternion rotation)
        {
            var PuzzleStaticConfiguration = AssetFinder.SafeSingleAssetFind<PuzzleStaticConfiguration>("t:" + typeof(PuzzleStaticConfiguration));
            RangeTypeObject instanciatedRangeObject = MonoBehaviour.Instantiate(PuzzleStaticConfiguration.PuzzlePrefabConfiguration.BaseRangeTypeObject);
            instanciatedRangeObject.transform.position = position;
            instanciatedRangeObject.transform.rotation = rotation;
            RangeTypeObjectDefinitionInherentData.DefineRangeTypeObject(instanciatedRangeObject, PuzzleStaticConfiguration.PuzzlePrefabConfiguration);
            return instanciatedRangeObject;
        }

        public RangeTypeObject Instanciate(Transform transform)
        {
            return Instanciate(transform.position, transform.rotation);
        }
    }
}
