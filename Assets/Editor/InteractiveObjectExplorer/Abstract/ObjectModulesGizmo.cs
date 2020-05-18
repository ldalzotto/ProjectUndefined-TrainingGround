﻿using Editor_MainGameCreationWizard;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ObjectModulesGizmo
{
    protected CommonGameConfigurations CommonGameConfigurations;

    protected abstract Dictionary<Type, ScriptableObject> GetDefinitionModules();
    protected virtual List<AbstractObjectGizmoDisplay> GetAdditionalDrawAreas() { return new List<AbstractObjectGizmoDisplay>(); }

    protected virtual void Init()
    {
        if (this.DrawDisplay == null)
        {
            this.DrawDisplay = new Dictionary<Type, IObjectGizmoDisplayEnableArea>();
            foreach (var rangeDefinitionModule in this.GetDefinitionModules())
            {
                this.DrawDisplay[rangeDefinitionModule.Key] =
                     this.HandleGizmoDisplayAreaCreation(rangeDefinitionModule.Key.Name, rangeDefinitionModule.Value);
            }
        }
    }

    protected ObjectModulesGizmo(CommonGameConfigurations commonGameConfigurations)
    {
        CommonGameConfigurations = commonGameConfigurations;
    }

    private Dictionary<Type, IObjectGizmoDisplayEnableArea> DrawDisplay;

    public virtual void OnGUI()
    {
        this.Init();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(50f));
        if (GUILayout.Button(new GUIContent("U", "Unselect ALL"), GUILayout.Width(20f)))
        {
            foreach (var drawDisplay in this.DrawDisplay)
            {
                drawDisplay.Value.Disable();
            }
            foreach (var additionalDrawAreas in this.GetAdditionalDrawAreas())
            {
                additionalDrawAreas.Disable();
            }
        }
        if (GUILayout.Button(new GUIContent("S", "Select ALL"), GUILayout.Width(20f)))
        {
            foreach (var drawDisplay in this.DrawDisplay)
            {
                drawDisplay.Value.Enable();
            }
            foreach (var additionalDrawAreas in this.GetAdditionalDrawAreas())
            {
                additionalDrawAreas.Enable();
            }
        }
        EditorGUILayout.EndHorizontal();

        foreach (var drawDisplay in this.DrawDisplay)
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
        Handles.color = oldHandlesColor;
    }

    protected abstract void DrawGizmo(Type moduleDefinitionType, Transform objectTransform);
    protected abstract IObjectGizmoDisplayEnableArea HandleGizmoDisplayAreaCreation(string moduleDefinitionType, ScriptableObject definitionSO);

    protected IObjectGizmoDisplayEnableArea GetDrawDisplay(Type definitionType)
    {
        return this.DrawDisplay[definitionType];
    }
}
