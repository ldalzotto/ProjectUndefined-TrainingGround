using System.Collections.Generic;
using System.Reflection;
using AIObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIPatrolGraphV2), editorForChildClasses: true)]
public class AIPatrolGraphV2CustomEditor : Editor
{
    private IEnumerable<FieldInfo> targetFields;

    private void OnEnable()
    {
        this.targetFields = ReflectionHelper.GetAllFields(target.GetType());
        SceneView.duringSceneGui += this.SceneTick;
    }

    private void OnDisable()
    {
        Debug.Log("AIPatrolGraphV2CustomEditor : OnDisable");
        SceneView.duringSceneGui -= this.SceneTick;
    }

    private void SceneTick(SceneView sceneView)
    {
        foreach (var targetField in this.targetFields)
        {
            var oldColor = Handles.color;

            var CustomAttributes = targetField.GetCustomAttributes();
            if (CustomAttributes != null)
            {
                foreach (var CustomAttribute in CustomAttributes)
                {
                    switch (CustomAttribute)
                    {
                        case GraphPatrolLineAttribute WireTargetLineDrawAttribute:
                            var sourcePosition = ((AIMoveToActionInputData) targetField.GetValue(target)).GetWorldPosition();
                            var targetPosition = ((AIMoveToActionInputData) target.GetType().GetField(WireTargetLineDrawAttribute.FieldTargetWorldPosition).GetValue(target)).GetWorldPosition();
                            HandlesHelper.DrawArrow(sourcePosition, targetPosition, Color.white);
                            break;
                        case GraphPatrolPointAttribute WireCircleAttributePositionned:
                            var position = ((AIMoveToActionInputData) targetField.GetValue(target)).GetWorldPosition();
                            var oldC = Handles.color;
                            Handles.color = WireCircleAttributePositionned.GetColor();
                            Handles.DrawWireDisc(position, Vector3.up, 1f);
                            Handles.Label(position + (Vector3.up * 2f), targetField.Name, MyEditorStyles.BuildLabelStyle(Handles.color));
                            Handles.color = oldC;
                            break;
                    }
                }
            }

            Handles.color = oldColor;
        }
    }
}