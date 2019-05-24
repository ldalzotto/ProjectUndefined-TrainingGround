﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using Editor_PuzzleGameCreationWizard;
using Editor_AICreationObjectCreationWizard;
using RTPuzzle;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class CreateAI : CreateInEditorModule<AIObjectCreationWizard>
    {
    }
}