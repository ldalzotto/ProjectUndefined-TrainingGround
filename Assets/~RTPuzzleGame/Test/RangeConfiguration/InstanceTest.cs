using UnityEngine;
using System.Collections;
using RTPuzzle;

public class InstanceTest : MonoBehaviour
{
    public PuzzlePrefabConfiguration puzzlePrefabConfiguration;
    public RangeTypeObject RangeTypeObject;
    public RangeTypeObjectDefinitionConfigurationInherentData RangeDefinition;

    private void Awake()
    {
        RangeDefinition.DefineRangeTypeObject(this.RangeTypeObject, this.puzzlePrefabConfiguration);
    }
}
