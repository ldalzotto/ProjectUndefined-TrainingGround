using UnityEngine;
using System.Collections;
using CoreGame;
using RTPuzzle;
using Editor_GameDesigner;
using UnityEditor;

namespace Editor_GameCustomEditors
{
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerCustomEditor : AbstractGameCustomEditorWithLiveSelection<LevelManager, LevelManagerCustomEditorContext, LevelConfiguration, EditPuzzleLevel>
    {

    }

    public class LevelManagerCustomEditorContext
    {

    }
}