﻿using AdventureGame;
using GameConfigurationID;
using NodeGraph;
using NodeGraph_Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class AdventureDiscussionTextOnlyNodeEdge : AbstractTextOnlyNodeEdge
    {
        
        public PointOfInterestId Talker;

        protected override void AdditionalGUI()
        {
            this.Talker = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.Talker);
        }
    }
}