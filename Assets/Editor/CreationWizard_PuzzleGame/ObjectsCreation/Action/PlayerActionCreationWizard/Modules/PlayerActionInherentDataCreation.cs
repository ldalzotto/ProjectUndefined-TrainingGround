using UnityEngine;
using System.Collections;
using RTPuzzle;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Editor_MainGameCreationWizard;

namespace Editor_PlayerActionCreationWizard
{
    public class PlayerActionInherentDataCreation : CreateableScriptableObjectComponent<PlayerActionInherentData>
    {
        public Type PlayerActionType;
        private List<Type> PlayerActionTypesAvailable;
        private int PlayerActionTypeSelectedIndex;

        public override void InstanciateInEditor(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.CreatedObject = (PlayerActionInherentData)ScriptableObject.CreateInstance(this.PlayerActionType);
            this.isNew = true;
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(InstancePath.PlayerActionInherentDataPath, editorInformationsData.PlayerActionId.ToString() + "_" + this.PlayerActionType.Name + NameConstants.PlayerActionsInherentData, editorProfile); ;
            this.AddToGameConfiguration(editorInformationsData.PlayerActionId, editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.PlayerActionConfiguration, editorProfile);

            var levelZoneConfiguration = editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData[editorInformationsData.LevelZonesID];
            levelZoneConfiguration.AddPlayerActionId(new PlayerActionIdWrapper(editorInformationsData.PlayerActionId));
            editorProfile.AddedPlayerAction(editorInformationsData.PlayerActionId, editorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration, editorInformationsData.LevelZonesID);
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            if (this.PlayerActionTypesAvailable == null)
            {
                this.PlayerActionTypesAvailable = TypeHelper.GetAllTypeAssignableFrom(typeof(PlayerActionInherentData)).ToList();
            }

            EditorGUILayout.LabelField("Select player action type : ");
            EditorGUI.BeginChangeCheck();
            this.PlayerActionTypeSelectedIndex = EditorGUILayout.Popup(this.PlayerActionTypeSelectedIndex, this.PlayerActionTypesAvailable.ConvertAll(t => t.Name).ToArray());
            this.PlayerActionType = this.PlayerActionTypesAvailable[this.PlayerActionTypeSelectedIndex];
            if (EditorGUI.EndChangeCheck())
            {
                this.InstanciateInEditor(editorProfile);
            }
            base.OnInspectorGUIImpl(serializedObject, editorProfile);
        }

    }
}