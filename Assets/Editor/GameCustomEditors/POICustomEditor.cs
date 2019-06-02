using UnityEngine;
using System.Collections;
using AdventureGame;
using UnityEditor;
using Editor_GameDesigner;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(PointOfInterestType))]
    public class POICustomEditor : AbstractGameCustomEditorWithLiveSelection<PointOfInterestType, POICustomEditorContext, PointOfInterestConfiguration, EditPOI>
    {

    }

    public class POICustomEditorContext
    {

    }
}