using Editor_DiscussionTreeNodeEditor;
using Editor_MainGameCreationWizard;
using NodeGraph;
using UnityEditor;

namespace Editor_DiscussionTreeCreationWizard
{
    public class DiscussionTreeDataCreation : CreateableScriptableObjectComponent<DiscussionTreeNodeEditorProfile>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var createdDiscussionTreeNodeEditorProfile = this.CreateAsset(editorInformationsData.CommonGameConfigurations.InstancePath.DiscussionTreePath,
                    editorInformationsData.DiscussionTreeId.ToString() + NameConstants.DiscussionNodeEditorObject, editorProfile);
            createdDiscussionTreeNodeEditorProfile.DiscussionTreeID = editorInformationsData.DiscussionTreeId;
            EditorUtility.SetDirty(createdDiscussionTreeNodeEditorProfile);

            var castedNodeEditorProfile = (NodeEditorProfile)createdDiscussionTreeNodeEditorProfile;
            DiscussionTreeNodeEditorProfile.Init(castedNodeEditorProfile);
        }
    }
}