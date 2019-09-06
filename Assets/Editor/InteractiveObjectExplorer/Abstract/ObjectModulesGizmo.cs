using Editor_MainGameCreationWizard;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ObjectModulesGizmo
{
    protected CommonGameConfigurations CommonGameConfigurations;

    protected abstract Dictionary<Type, ScriptableObject> GetDefinitionModules();

    protected virtual void Init()
    {
        if (this.DrawDisplay == null)
        {
            this.DrawDisplay = new Dictionary<string, IObjectGizmoDisplayEnableArea>();
            foreach (var rangeDefinitionModule in this.GetDefinitionModules())
            {
                this.DrawDisplay[rangeDefinitionModule.Key.Name] =
                     this.HandleGizmoDisplayAreaCreation(rangeDefinitionModule.Key.Name, rangeDefinitionModule.Value);
            }
        }
    }

    protected ObjectModulesGizmo(CommonGameConfigurations commonGameConfigurations)
    {
        CommonGameConfigurations = commonGameConfigurations;
    }

    private Dictionary<string, IObjectGizmoDisplayEnableArea> DrawDisplay;

    public virtual void OnGUI()
    {
        this.Init();
        foreach(var drawDisplay in this.DrawDisplay)
        {
            drawDisplay.Value.OnGUI(null);
        }
    }

    public virtual void OnSceneGUI(SceneView sceneView, Transform objectTransform)
    {
        this.Init();
        var oldHandlesColor = Handles.color;
        foreach (var drawDisplay in this.DrawDisplay)
        {
            this.DrawGizmo(drawDisplay.Key, objectTransform);
        }
    }

    protected abstract void DrawGizmo(string moduleDefinitionType, Transform objectTransform);
    protected abstract IObjectGizmoDisplayEnableArea HandleGizmoDisplayAreaCreation(string moduleDefinitionType, ScriptableObject definitionSO);

    protected IObjectGizmoDisplayEnableArea GetDrawDisplay(string key)
    {
        return this.DrawDisplay[key];
    }
}
