using UnityEngine;
using UnityEngine.Rendering;

namespace CoreGame
{
    public class ObjectSelectionRendererManager
    {
        private SelectableObjectIconAnimation SelectableObjectIconAnimation;
        private CoreMaterialConfiguration CoreMaterialConfiguration;
        public CommandBuffer CommandBufer { get; private set; }

        private MaterialPropertyBlock SelectionDoticonMaterialProperty;

        public ObjectSelectionRendererManager(CoreMaterialConfiguration CoreMaterialConfiguration)
        {
            this.CoreMaterialConfiguration = CoreMaterialConfiguration;
            this.CommandBufer = new CommandBuffer();
            this.CommandBufer.name = this.GetType().Name;

            //Camera.main.AddCommandBuffer(CameraEvent.AfterForwardAlpha, this.commandBufer);

            this.SelectableObjectIconAnimation = new SelectableObjectIconAnimation();
            this.SelectionDoticonMaterialProperty = new MaterialPropertyBlock();
        }


        public void Tick(float d, ISelectable currentSelectedObject, bool hasMultipleAvailableSelectionObjects)
        {
            this.CommandBufer.Clear();

            if (currentSelectedObject != null)
            {
                this.SelectableObjectIconAnimation.Tick(d, hasMultipleAvailableSelectionObjects);
                var averageBoundsLocalSpace = currentSelectedObject.GetAverageModelBoundLocalSpace();

                if (!averageBoundsLocalSpace.IsNull())
                {
                    if (hasMultipleAvailableSelectionObjects)
                    {
                        this.SelectionDoticonMaterialProperty.SetTexture("_MainTex", this.CoreMaterialConfiguration.SelectionDotSwitchIconTexture);
                    }
                    else
                    {
                        this.SelectionDoticonMaterialProperty.SetTexture("_MainTex", this.CoreMaterialConfiguration.SelectionDotIconTexture);
                    }

                    var targetTransform = currentSelectedObject.GetTransform();

                    //icon
                    this.CommandBufer.DrawMesh(this.CoreMaterialConfiguration.ForwardPlane,
                        Matrix4x4.TRS(targetTransform.position + Vector3.Project(new Vector3(0, averageBoundsLocalSpace.SideDistances.y * 0.5f, 0), targetTransform.up),
                         Quaternion.LookRotation(Camera.main.transform.position - targetTransform.position) * Quaternion.Euler(0, 0, this.SelectableObjectIconAnimation.GetRotationAngleDeg()), Vector3.one * this.SelectableObjectIconAnimation.GetIconScale()),
                                this.CoreMaterialConfiguration.SelectionDoticonMaterial,
                                0, 0, this.SelectionDoticonMaterialProperty);
                }
            }
        }
    }
}
