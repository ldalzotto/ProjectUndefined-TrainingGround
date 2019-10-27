using System.Collections.Generic;
using System.Reflection;
using AIObjects;
using SequencedAction;
using UnityEditor;

[CustomEditor(typeof(ASequencedActionGraph), editorForChildClasses: true)]
public class ASequencedActionCustomEditor : Editor
{
    private IEnumerable<FieldInfo> targetFields;

    private void OnEnable()
    {
        this.targetFields = ReflectionHelper.GetAllFields(target.GetType());
        SceneView.duringSceneGui += this.SceneTick;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= this.SceneTick;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.SceneTick;
    }

    private void SceneTick(SceneView sceneView)
    {
        SceneHandlerDrawer.Draw(target, null, null);
    }
}