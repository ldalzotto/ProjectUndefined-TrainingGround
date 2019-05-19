﻿using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ICreationWizardEditor<out T> where T : AbstractCreationWizardEditorProfile
{
    void OnGUI();
}

public abstract class AbstractCreationWizardEditor<T> : ICreationWizardEditor<T> where T : AbstractCreationWizardEditorProfile
{
    protected T editorProfile;

    public void OnGUI()
    {
        this.editorProfile = (T)EditorGUILayout.ObjectField(this.editorProfile, typeof(T), false);

        if (this.editorProfile == null)
        {
            this.editorProfile = AssetFinder.SafeSingleAssetFind<T>("t:" + typeof(T).Name);
        }

        if (this.editorProfile != null)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("R", "Reset creation wzard."), EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                this.editorProfile.ResetEditor();
            }
            EditorGUILayout.EndHorizontal();

            editorProfile.WizardScrollPosition = EditorGUILayout.BeginScrollView(editorProfile.WizardScrollPosition);
            this.OnWizardGUI();

            if (GUILayout.Button("GENERATE"))
            {
                if (this.editorProfile.ContainsError())
                {
                    if (EditorUtility.DisplayDialog("Proceed generation ?", "There are errors. Do you want to proceed generation ?", "YES", "NO"))
                    {
                        DoGeneration();
                    }
                }
                else if (this.editorProfile.ContainsWarn())
                {
                    if (EditorUtility.DisplayDialog("Proceed generation ?", "There are warnings. Do you want to proceed generation ?", "YES", "NO"))
                    {
                        DoGeneration();
                    }
                }
                else
                {
                    DoGeneration();
                }



            }

            this.DoGenereatedObject();
            EditorGUILayout.EndScrollView();

        }
    }

    private void DoGeneration()
    {
        this.editorProfile.CreationWizardFeedLines.Clear();

        var tmpScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        tmpScene.name = UnityEngine.Random.Range(0, 999999).ToString();

        try
        {
            this.OnGenerationClicked(tmpScene);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e.StackTrace);
        }
        finally
        {
            this.editorProfile.OnGenerationEnd();
            EditorSceneManager.CloseScene(tmpScene, true);
        }
    }

    protected abstract void OnWizardGUI();
    protected abstract void OnGenerationClicked(Scene tmpScene);

    private void DoGenereatedObject()
    {
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Generated objects : ");
        if (GUILayout.Button(new GUIContent("D", "Delete all generations."), EditorStyles.miniButton, GUILayout.Width(20)))
        {

            foreach (var creationWizardFeedLines in this.editorProfile.CreationWizardFeedLines)
            {
                if (creationWizardFeedLines.GetType() == typeof(CreatedObjectFeedLine))
                {
                    var feedLine = (CreatedObjectFeedLine)creationWizardFeedLines;
                    AssetDatabase.DeleteAsset(feedLine.FilePath);
                }
                else if (creationWizardFeedLines.GetType() == typeof(ConfigurationModifiedFeedLine))
                {
                    ((ConfigurationModifiedFeedLine)creationWizardFeedLines).RemoveEntry();
                }
            }
            this.editorProfile.CreationWizardFeedLines.Clear();
        }
        EditorGUILayout.EndHorizontal();
        foreach (var creationWizardFeedLines in this.editorProfile.CreationWizardFeedLines)
        {
            creationWizardFeedLines.GUITick();
        }
        EditorGUILayout.EndVertical();
    }

    protected M GetModule<M>() where M : CreationModuleComponent
    {
        return (M)this.editorProfile.Modules[typeof(M).Name];
    }
}
