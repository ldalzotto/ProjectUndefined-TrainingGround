﻿#if UNITY_EDITOR

using ConfigurationEditor;
using RTPuzzle;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FXColorConfigurationEditor : EditorWindow
{
    [MenuItem("Color/FX/FXColorConfigurationEditor")]
    static void Init()
    {
        var window = EditorWindow.GetWindow<FXColorConfigurationEditor>();
        window.Show();
    }


    #region Scroll Position
    [SerializeField] private Vector2 windowScrollPosition;
    #endregion

    #region Foldables
    [SerializeField] private bool launchProjectileRangeMaterialFold;
    [SerializeField] private bool launchProjectileCursorAreaMaterialFold;
    [SerializeField] private bool launchProjectileRangeAreaMaterialFold;
    [SerializeField] private bool attractiveObjectAreaMaterialFold;
    [SerializeField] private bool throwProjectilePathPrefabFold;
    [SerializeField] private bool throwProjectileCursorPrefabFold;
    #endregion

    private Material launchProjectileRangeMaterial;

    private Material launchProjectileCursorAreaMaterial;
    private PrefabEditorInjection launchProjectileRangeAreaEffectsManager;
    private Material attractiveObjectAreaMaterial;
    private ThrowProjectilePath throwProjectilePathPrefab;
    private PrefabEditorInjection throwProjectileCursorPrefab;

    private void OnGUI()
    {

        this.windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

        launchProjectileRangeMaterialFold = EditorGUILayout.Foldout(launchProjectileRangeAreaMaterialFold, new GUIContent("Projectile throw range material : "), true);
        if (launchProjectileRangeAreaMaterialFold)
        {
            this.launchProjectileRangeMaterial = EditorGUILayout.ObjectField(launchProjectileRangeMaterial, typeof(Material), false) as Material;
            if (this.launchProjectileRangeMaterial != null)
            {
                this.launchProjectileRangeMaterial.SetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY, EditorGUILayout.ColorField(new GUIContent("Aura color : "), this.launchProjectileRangeMaterial.GetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY), true, true, false));
            }
        }

        launchProjectileCursorAreaMaterialFold = EditorGUILayout.Foldout(launchProjectileCursorAreaMaterialFold, new GUIContent("Launch projectile cursor area material : "), true);
        if (launchProjectileCursorAreaMaterialFold)
        {
            this.launchProjectileCursorAreaMaterial = EditorGUILayout.ObjectField(launchProjectileCursorAreaMaterial, typeof(Material), false) as Material;
            if (this.launchProjectileCursorAreaMaterial != null)
            {
                this.launchProjectileCursorAreaMaterial.SetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY, EditorGUILayout.ColorField(new GUIContent("Aura color : "), this.launchProjectileCursorAreaMaterial.GetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY), true, true, false));
            }
        }

        EditorGUILayout.Separator();

        launchProjectileRangeAreaMaterialFold = EditorGUILayout.Foldout(launchProjectileRangeAreaMaterialFold, new GUIContent("Launch projectile range area material : "), true);
        if (launchProjectileRangeAreaMaterialFold)
        {
            this.launchProjectileRangeAreaEffectsManager.SelectedPrefab = EditorGUILayout.ObjectField(launchProjectileRangeAreaEffectsManager.SelectedPrefab, typeof(GameObject), false) as GameObject;
            if (this.launchProjectileRangeAreaEffectsManager.SelectedPrefab != null)
            {
                /*
                this.launchProjectileRangeAreaEffectsManager.DisplayComponentEditorUI<GroundEffectsManager>((GroundEffectsManager groundEffectsManager) => {
                    groundEffectsManager.ThrowCursorRangeEffectManagerComponent.CursorOnRangeAuraColor = EditorGUILayout.ColorField(new GUIContent("Cursor on range aura color : "), groundEffectsManager.ThrowCursorRangeEffectManagerComponent.CursorOnRangeAuraColor, true, true, false);
                    groundEffectsManager.ThrowCursorRangeEffectManagerComponent.CursorOutOfRangeAuraColor = EditorGUILayout.ColorField(new GUIContent("Cursor out of range aura color : "), groundEffectsManager.ThrowCursorRangeEffectManagerComponent.CursorOutOfRangeAuraColor, true, true, false);
                });
                */
            }
        }

        EditorGUILayout.Separator();

        attractiveObjectAreaMaterialFold = EditorGUILayout.Foldout(attractiveObjectAreaMaterialFold, new GUIContent("Attractive object area material : "), true);
        if (attractiveObjectAreaMaterialFold)
        {
            this.attractiveObjectAreaMaterial = EditorGUILayout.ObjectField(attractiveObjectAreaMaterial, typeof(Material), false) as Material;
            if (this.attractiveObjectAreaMaterial != null)
            {
                this.attractiveObjectAreaMaterial.SetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY, EditorGUILayout.ColorField(new GUIContent("Aura color : "), this.attractiveObjectAreaMaterial.GetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY), true, true, false));
            }
        }

        EditorGUILayout.Separator();

        throwProjectilePathPrefabFold = EditorGUILayout.Foldout(throwProjectilePathPrefabFold, new GUIContent("Throw projectile path prefab : "), true);
        if (throwProjectilePathPrefabFold)
        {
            this.throwProjectilePathPrefab = EditorGUILayout.ObjectField(this.throwProjectilePathPrefab, typeof(ThrowProjectilePath), false) as ThrowProjectilePath;
            if (this.throwProjectilePathPrefab != null)
            {
                var trails = this.throwProjectilePathPrefab.GetComponent<ParticleSystem>().trails;
                var colorOverTrail = trails.colorOverTrail;
                colorOverTrail.gradient = EditorGUILayout.GradientField(new GUIContent("Color over trail : "), colorOverTrail.gradient);
                trails.colorOverTrail = colorOverTrail;
            }
        }

        EditorGUILayout.Separator();

        throwProjectileCursorPrefabFold = EditorGUILayout.Foldout(throwProjectileCursorPrefabFold, new GUIContent("Launch projectile cursor : "), true);
        if (throwProjectileCursorPrefabFold)
        {

            EditorGUI.BeginChangeCheck();
            this.throwProjectileCursorPrefab.SelectedPrefab = EditorGUILayout.ObjectField(this.throwProjectileCursorPrefab.SelectedPrefab, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                this.throwProjectileCursorPrefab.OnObjectFieldSelectionChange();
            }
            this.throwProjectileCursorPrefab.DisplayComponentEditorUI<Image>();
        }

        EditorGUILayout.EndScrollView();
    }

    private void OnEnable()
    {
        var data = EditorPrefs.GetString(typeof(FXColorConfigurationEditor).ToString(), JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);

        if (this.throwProjectileCursorPrefab == null)
        {
            this.throwProjectileCursorPrefab = new PrefabEditorInjection();
        }

        if(this.launchProjectileRangeAreaEffectsManager == null)
        {
            this.launchProjectileRangeAreaEffectsManager = new PrefabEditorInjection();
        }

        this.launchProjectileRangeMaterial = AssetFinder.SafeSingleAssetFind<Material>("LaunchProjectileRangeMaterial t:Material");
        this.launchProjectileCursorAreaMaterial = AssetFinder.SafeSingleAssetFind<Material>("LaunchProjectileCursorMaterial t:Material");
        this.launchProjectileRangeAreaEffectsManager.SelectedPrefab = AssetFinder.SafeSingleAssetFind<GameObject>("PuzzleEnvironmentObject t:Prefab");
        this.attractiveObjectAreaMaterial = AssetFinder.SafeSingleAssetFind<Material>("AttractiveObjectRangeMaterial t:Material");
        this.throwProjectilePathPrefab = AssetFinder.SafeSingleAssetFind<ThrowProjectilePath>("ThrowProjectilePath t:Prefab");
        this.throwProjectileCursorPrefab.SelectedPrefab = AssetFinder.SafeSingleAssetFind<GameObject>("ThrowProjectileCursor t:Prefab");

    }


    private void OnDisable()
    {
        this.throwProjectileCursorPrefab.OnDisable();
        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString(typeof(FXColorConfigurationEditor).ToString(), data);
    }

}

#endif