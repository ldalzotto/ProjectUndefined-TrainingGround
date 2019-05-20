using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Experimental.Editor_NodeEditor
{
    public class IntNodeEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>()
        {
            typeof(IntNodeEdge)
        };

        protected override void GUI_Impl(Rect rect)
        {
            if (this.Value != null)
            {
                EditorGUI.IntField(rect, (int)this.Value);
            }
        }
    }
}