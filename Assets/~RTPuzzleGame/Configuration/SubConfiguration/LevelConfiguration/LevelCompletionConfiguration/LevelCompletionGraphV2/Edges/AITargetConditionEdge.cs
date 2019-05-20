using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using RTPuzzle;
using UnityEditor;
using NodeGraph;

namespace RTPuzzle
{
    [System.Serializable]
    public class AITargetConditionEdge : NodeEdgeProfile
    {
        [SearchableEnum]
        public AiID AiID;
        [SearchableEnum]
        public TargetZoneID TargetZoneID;

        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() { };

#if UNITY_EDITOR
        protected override void GUI_Impl(Rect rect)
        {
            var so = new SerializedObject(this);
            var currentRect = new Rect(rect);
            currentRect.height = rect.height / 2;
            EditorGUI.PropertyField(currentRect, so.FindProperty(nameof(this.AiID)), GUIContent.none);
            currentRect.position = new Vector2(currentRect.position.x, currentRect.position.y + currentRect.height);
            EditorGUI.PropertyField(currentRect, so.FindProperty(nameof(this.TargetZoneID)), GUIContent.none);
            so.ApplyModifiedProperties();
        }
        protected override float DefaultGetEdgeHeight()
        {
            return 40f;
        }
#endif


    }
}