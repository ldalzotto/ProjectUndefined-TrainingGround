#if UNITY_EDITOR

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
    [SerializeField] private bool launchProjectileRangeAreaMaterialFold;
    [SerializeField] private bool attractiveObjectAreaMaterialFold;
    [SerializeField] private bool throwProjectilePathPrefabFold;
    [SerializeField] private bool throwProjectileCursorPrefabFold;
    #endregion

    private Material launchProjectileRangeMaterial;

    private Material attractiveObjectAreaMaterial;
    private ThrowProjectilePath throwProjectilePathPrefab;
    private PrefabEditorInjection throwProjectileCursorPrefab;
    private RangeColorConfiguration RangeColorConfiguration;

    private void OnGUI()
    {

        this.windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

        launchProjectileRangeAreaMaterialFold = EditorGUILayout.Foldout(launchProjectileRangeAreaMaterialFold, new GUIContent("Projectile throw range material : "), true);
        if (launchProjectileRangeAreaMaterialFold)
        {
            this.launchProjectileRangeMaterial = EditorGUILayout.ObjectField(launchProjectileRangeMaterial, typeof(Material), false) as Material;
            if (this.launchProjectileRangeMaterial != null)
            {
                this.launchProjectileRangeMaterial.SetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY, EditorGUILayout.ColorField(new GUIContent("Aura color : "), this.launchProjectileRangeMaterial.GetColor(GroundEffectsManagerV2.AURA_COLOR_MATERIAL_PROPERTY), true, true, false));
            }
        }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField(new GUIContent("Range color configuration : "));
        Editor.CreateEditor(this.RangeColorConfiguration).OnInspectorGUI();

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

        this.launchProjectileRangeMaterial = AssetFinder.SafeSingleAssetFind<Material>("LaunchProjectileRangeMaterial t:Material");
        this.attractiveObjectAreaMaterial = AssetFinder.SafeSingleAssetFind<Material>("AttractiveObjectRangeMaterial t:Material");
        this.throwProjectilePathPrefab = AssetFinder.SafeSingleAssetFind<ThrowProjectilePath>("ThrowProjectilePath t:Prefab");
        this.throwProjectileCursorPrefab.SelectedPrefab = AssetFinder.SafeSingleAssetFind<GameObject>("ThrowProjectileCursor t:Prefab");
        this.RangeColorConfiguration = AssetFinder.SafeSingleAssetFind<RangeColorConfiguration>("t:" + typeof(RangeColorConfiguration).Name);
    }


    private void OnDisable()
    {
        this.throwProjectileCursorPrefab.OnDisable();
        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString(typeof(FXColorConfigurationEditor).ToString(), data);
    }

}

#endif