﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using RTPuzzle;
using UnityEditor;
using NodeGraph;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    public class PlayerTargetConditionEdge : NodeEdgeProfile
    {
        [CustomEnum]
        public TargetZoneID TargetZoneID;

        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() { };

#if UNITY_EDITOR
        private Editor cachedEditor;
        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            var so = new SerializedObject(this);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(this.TargetZoneID)));
            so.ApplyModifiedProperties();
        }
        protected override float DefaultGetEdgeHeight()
        {
            return 40f;
        }
#endif


    }
}