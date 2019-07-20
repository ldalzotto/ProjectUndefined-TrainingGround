using Editor_GameDesigner;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [CustomEditor(typeof(NPCAIManager))]
    public class NPCAIManagerCustomEditor : AbstractGameCustomEditorWithLiveSelection<NPCAIManager, NPCAIManagerCustomEditorContext, AIComponentsConfigurationModule, EditBehavior>
    {

        public AIComponentsConfiguration AIComponentsConfiguration;

        private void OnEnable()
        {
            if (target != null)
            {
                AIComponentsConfiguration = AssetFinder.SafeSingleAssetFind<AIComponentsConfiguration>("t:" + typeof(AIComponentsConfiguration).Name);
                if (AIComponentsConfiguration != null)
                {
                    AbstractAIComponents AIComponents = AIComponentsConfiguration.ConfigurationInherentData[(target as NPCAIManager).AiID].AIComponents;
                    this.context = new NPCAIManagerCustomEditorContext();
                    this.context.AISightVision = (target as NPCAIManager).GetComponentInChildren<AISightVision>();

                    if (AIComponents != null)
                    {
                        this.context.GenericPuzzleAIComponents = (GenericPuzzleAIComponents)AIComponents;

                        this.drawModules = new List<GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>>()
                        {
                            new AIProjectileEscapeComponent(),
                            new AIPatrolComponent(),
                            new AIPlayerEscapeComponent(),
                            new AITargetZoneComponent(),
                            new AISightComponent()
                        };
                    }
                }
            }
        }

    }

    public class NPCAIManagerCustomEditorContext
    {
        public GenericPuzzleAIComponents GenericPuzzleAIComponents;
        public AISightVision AISightVision;
    }

    public class AIProjectileEscapeComponent : IDPickGUIModule<NPCAIManager, NPCAIManagerCustomEditorContext, LaunchProjectileId, float>
    {
        public override Func<NPCAIManagerCustomEditorContext, ByEnumProperty<LaunchProjectileId, float>> GetByEnumProperty
        {
            get
            {
                return (NPCAIManagerCustomEditorContext NPCAIManagerCustomEditorContext) =>
                {
                    if(NPCAIManagerCustomEditorContext.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent == null) { return null; }
                    return NPCAIManagerCustomEditorContext.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2;
                };
            }
        }

        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target, LaunchProjectileId selectedKey)
        {
            if (context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent != null && context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2 != null)
            {
                Handles.color = Color.blue;
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Handles.color;
                Handles.Label(target.transform.position + Vector3.up * context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[selectedKey],
                    this.GetType().Name + "_" + selectedKey.ToString(), labelStyle);
                Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[selectedKey]);

                Handles.color = Color.yellow;
                Handles.Label(target.transform.position + Vector3.up * 5f, nameof(context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2) + "_" + selectedKey.ToString(), MyEditorStyles.LabelYellow);
                Handles.DrawWireArc(target.transform.position, Vector3.up, target.transform.forward, context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[selectedKey], 5f);
                Handles.DrawWireArc(target.transform.position, Vector3.up, target.transform.forward, -context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistanceV2.Values[selectedKey], 5f);
            }
        }
    }

    public class AIPatrolComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
        {
            if (context.GenericPuzzleAIComponents.AIRandomPatrolComponent != null)
            {
                Handles.color = Color.magenta;
                var labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Color.magenta;
                Handles.Label(target.transform.position + (Vector3.up * context.GenericPuzzleAIComponents.AIRandomPatrolComponent.MaxDistance), "AI Patrol distance.", labelStyle);
                Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AIRandomPatrolComponent.MaxDistance);
            }
        }
    }

    public class AIPlayerEscapeComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
        {
            if (context.GenericPuzzleAIComponents.AIPlayerEscapeComponent != null)
            {
                Handles.color = Color.yellow;

                var labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Handles.color;

                Handles.Label(target.transform.position + Vector3.up * context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeDistance, nameof(context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeDistance), labelStyle);
                Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeDistance);

                Handles.Label(target.transform.position + Vector3.up * context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius, nameof(context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius), labelStyle);
                Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.PlayerDetectionRadius);

                Handles.Label(target.transform.position + target.transform.forward * 4, "Escape angle.", labelStyle);
                Handles.DrawWireArc(target.transform.position, Vector3.up, target.transform.forward, context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeSemiAngle, 5f);
                Handles.DrawWireArc(target.transform.position, Vector3.up, target.transform.forward, -context.GenericPuzzleAIComponents.AIPlayerEscapeComponent.EscapeSemiAngle, 5f);
            }
        }
    }

    public class AITargetZoneComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
        {
            if (context.GenericPuzzleAIComponents.AITargetZoneComponent != null)
            {
                Handles.color = Color.green;
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Handles.color;
                Handles.Label(target.transform.position + Vector3.up * context.GenericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance, nameof(context.GenericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance), labelStyle);
                Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance);
            }
        }
    }

    public class AISightComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
        {
            if(context.AISightVision != null)
            {
                Handles.color = MyColors.HotPink;
                context.AISightVision.HandlesTick();
            }
        }
    }
}

