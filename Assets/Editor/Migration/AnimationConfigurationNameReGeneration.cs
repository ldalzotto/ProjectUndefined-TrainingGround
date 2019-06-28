using CoreGame;
using UnityEditor;
using UnityEngine;

public class AnimationConfigurationNameReGeneration : EditorWindow
{
    [MenuItem("Migration/AnimationConfigurationNameReGeneration")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AnimationConfigurationNameReGeneration));
    }

    private AnimationConfiguration AnimationConfiguration;

    private void OnEnable()
    {
        this.AnimationConfiguration = AssetFinder.SafeSingleAssetFind<AnimationConfiguration>("t:" + typeof(AnimationConfiguration));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("GO"))
        {
            foreach (var animationConf in this.AnimationConfiguration.ConfigurationInherentData)
            {
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(animationConf.Value), animationConf.Key.ToString() + "_AnimationData");
            }
        }
    }
}
