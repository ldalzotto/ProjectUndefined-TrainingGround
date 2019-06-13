using Editor_GameDesigner;
using RTPuzzle;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_GameCustomEditors
{
    [CustomEditor(typeof(NPCAIManager))]
    public class NPCAIManagerCustomEditor : AbstractGameCustomEditorWithLiveSelection<NPCAIManager, NPCAIManagerCustomEditorContext, AIComponentsConfiguration, EditBehavior>
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
                    if (AIComponents != null)
                    {
                        this.context = new NPCAIManagerCustomEditorContext();
                        this.context.GenericPuzzleAIComponents = (GenericPuzzleAIComponents)AIComponents;

                        this.drawModules = new List<GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>>()
                        {
                            new AIProjectileEscapeComponent(),
                            new AIPatrolComponent(),
                            new AIPlayerEscapeComponent(),
                            new AITargetZoneComponent()
                        };
                    }
                }
            }
        }

    }

    public class NPCAIManagerCustomEditorContext
    {
        public GenericPuzzleAIComponents GenericPuzzleAIComponents;
    }

    public class AIProjectileEscapeComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
        {
            Handles.color = Color.blue;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Handles.color;
            Handles.Label(target.transform.position + Vector3.up * context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistance, this.GetType().Name, labelStyle);
            Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AIProjectileEscapeWithCollisionComponent.EscapeDistance);
        }
    }

    public class AIPatrolComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
        {
            Handles.color = Color.magenta;
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Color.magenta;
            Handles.Label(target.transform.position + (Vector3.up * context.GenericPuzzleAIComponents.AIRandomPatrolComponent.MaxDistance), "AI Patrol distance.", labelStyle);
            Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AIRandomPatrolComponent.MaxDistance);
        }
    }

    public class AIPlayerEscapeComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
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

    public class AITargetZoneComponent : GUIDrawModule<NPCAIManager, NPCAIManagerCustomEditorContext>
    {
        public override void SceneGUI(NPCAIManagerCustomEditorContext context, NPCAIManager target)
        {
            Handles.color = Color.green;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.normal.textColor = Handles.color;
            Handles.Label(target.transform.position + Vector3.up * context.GenericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance, nameof(context.GenericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance), labelStyle);
            Handles.DrawWireDisc(target.transform.position, Vector3.up, context.GenericPuzzleAIComponents.AITargetZoneComponent.TargetZoneEscapeDistance);
        }
    }
}

