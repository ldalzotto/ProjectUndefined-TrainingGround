using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;
using System;
using static AnimationConstants;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneItemHoldAction : SequencedAction
    {
        [CustomEnum()]
        [SerializeField]
        public BipedBone Bone;
        [CustomEnum()]
        [SerializeField]
        public ItemID HoldedItem;

        [NonSerialized]
        private Transform BoneTransformResolved;
        [NonSerialized]
        private GameObject InstanciatedObject;

        public CutsceneItemHoldAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            var cutsceneActionInput = (CutsceneActionInput)ContextActionInput;

            #region External Dependencies
            var PlayerManager = GameObject.FindObjectOfType<PlayerManager>();
            #endregion

            var playerAnimator = PlayerManager.GetPlayerAnimator();
            var boneObj = BipedBoneRetriever.GetPlayerBone(Bone, playerAnimator);
            if (boneObj != null)
            {
                BoneTransformResolved = boneObj.transform;
            }

            var scaleFactor = Vector3.one;
            ComponentSearchHelper.ComputeScaleFactorRecursively(BoneTransformResolved, playerAnimator.transform, ref scaleFactor);

            InstanciatedObject = MonoBehaviour.Instantiate(cutsceneActionInput.AdventureGameConfigurationManager.ItemConf()[HoldedItem].ItemModel, BoneTransformResolved.transform);
            InstanciatedObject.transform.localScale = new Vector3(
                    InstanciatedObject.transform.localScale.x / scaleFactor.x,
                    InstanciatedObject.transform.localScale.y / scaleFactor.y,
                    InstanciatedObject.transform.localScale.z / scaleFactor.z
                );
        }

        public override void Tick(float d)
        {

        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.Bone = (BipedBone)NodeEditorGUILayout.EnumField("Bone : ", string.Empty, this.Bone);
            this.HoldedItem = (ItemID)NodeEditorGUILayout.EnumField("Holded item : ", string.Empty, this.HoldedItem);
        }
#endif
    }

}
