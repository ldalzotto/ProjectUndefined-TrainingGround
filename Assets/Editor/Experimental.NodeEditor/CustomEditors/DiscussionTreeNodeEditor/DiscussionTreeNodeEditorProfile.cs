using AdventureGame;
using NodeGraph;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTreeNodeEditor", menuName = "Configuration/AdventureGame/DiscussionConfiguration/DiscussionTreeNodeEditor", order = 1)]
    public class DiscussionTreeNodeEditorProfile : NodeEditorProfile
    {

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            DiscussionTreeNodeEditor window = (DiscussionTreeNodeEditor)EditorWindow.GetWindow(typeof(DiscussionTreeNodeEditor));
            nodeEditorProfile.Init();
            window.NodeEditorProfile = nodeEditorProfile;
            window.Show();
        }

        public static DiscussionTextRepertoire DiscussionTextRepertoire;

        public override void Init()
        {
            base.Init();
            DiscussionTreeNodeEditorProfile.DiscussionTextRepertoire = AssetFinder.SafeSingleAssetFind<DiscussionTextRepertoire>("t:" + typeof(DiscussionTextRepertoire).Name);
        }
        
        
    }
}