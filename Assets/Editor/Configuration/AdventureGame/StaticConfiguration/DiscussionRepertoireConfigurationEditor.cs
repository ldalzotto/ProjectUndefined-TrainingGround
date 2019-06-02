using AdventureGame;
using ConfigurationEditor;
using UnityEditor;

public class DiscussionRepertoireConfigurationEditor : IGenericConfigurationEditor
{
    private DiscussionTextRepertoire DiscussionTextRepertoire;


    private Editor CachedEditor;

    public void GUITick()
    {
        if (this.DiscussionTextRepertoire == null)
        {
            this.DiscussionTextRepertoire = AssetFinder.SafeSingleAssetFind<DiscussionTextRepertoire>("t:" + typeof(DiscussionTextRepertoire).Name);
        }
        if (this.CachedEditor == null)
        {
            this.CachedEditor = Editor.CreateEditor(this.DiscussionTextRepertoire);
        }
        if (this.CachedEditor != null)
        {
            this.CachedEditor.OnInspectorGUI();
        }
    }
}
