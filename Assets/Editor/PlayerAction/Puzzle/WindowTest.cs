using UnityEngine;
using System.Collections;
using UnityEditor;
using Editor_AttractiveObjectVariantWizardEditor;

public class WindowTest : EditorWindow
{

    [MenuItem("Test/Test")]
    public static void Init()
    {
        var window = EditorWindow.GetWindow<WindowTest>();
        window.Show();
    }

    private AttractiveObjectVariantCreationWizardV2 AttractiveObjectVariantCreationWizardV2;

    private void OnGUI()
    {
        if(this.AttractiveObjectVariantCreationWizardV2 == null) { this.AttractiveObjectVariantCreationWizardV2 = new AttractiveObjectVariantCreationWizardV2(); }
        this.AttractiveObjectVariantCreationWizardV2.OnGUI();
    }
}
