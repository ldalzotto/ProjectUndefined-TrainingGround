using AdventureGame;
using CoreGame;
using GameConfigurationID;
using NodeGraph;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTreeNodeEditor", menuName = "Configuration/AdventureGame/DiscussionConfiguration/DiscussionTreeNodeEditor", order = 1)]
    public class DiscussionTreeNodeEditorProfile : NodeEditorProfile
    {
        [SerializeField]
        public DiscussionTreeId DiscussionTreeID;

        public static void Init(NodeEditorProfile nodeEditorProfile)
        {
            DiscussionTreeNodeEditor window = (DiscussionTreeNodeEditor)EditorWindow.GetWindow(typeof(DiscussionTreeNodeEditor));
            nodeEditorProfile.Init();
            window.NodeEditorProfile = nodeEditorProfile;
            window.Show();
        }

        [SerializeField]
        public DiscussionTextRepertoire DiscussionTextRepertoire;

        public override void Init()
        {
            base.Init();
            this.DiscussionTextRepertoire = AssetFinder.SafeSingleAssetFind<DiscussionTextRepertoire>("t:" + typeof(DiscussionTextRepertoire).Name);
        }


    }
}