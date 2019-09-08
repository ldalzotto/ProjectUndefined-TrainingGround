using UnityEngine;
using UnityEngine.Rendering;

namespace CoreGame
{
    public class ObjectSelectionRendererManager
    {
        private SelectableObjectIconAnimation SelectableObjectIconAnimation;
        private CoreMaterialConfiguration CoreMaterialConfiguration;
        private CommandBuffer commandBufer;

        public ObjectSelectionRendererManager(CoreMaterialConfiguration CoreMaterialConfiguration)
        {
            this.CoreMaterialConfiguration = CoreMaterialConfiguration;
            this.commandBufer = new CommandBuffer();
            this.commandBufer.name = this.GetType().Name;
            Camera.main.AddCommandBuffer(CameraEvent.AfterForwardAlpha, this.commandBufer);

            this.SelectableObjectIconAnimation = new SelectableObjectIconAnimation();
        }

        public void Tick(float d, AbstractSelectableObject currentSelectedObject)
        {
            this.commandBufer.Clear();

            if (currentSelectedObject != null)
            {
                this.SelectableObjectIconAnimation.Tick(d);
                var modelOutlined = currentSelectedObject.ModelObjectModule;

                if (modelOutlined != null)
                {
                    var targetTransform = IRenderBoundRetrievableStatic.FromIRenderBoundRetrievable(modelOutlined).transform;

                    //icon
                    this.commandBufer.DrawMesh(this.CoreMaterialConfiguration.ForwardPlane,
                        Matrix4x4.TRS(targetTransform.position + Vector3.Project(new Vector3(0, modelOutlined.GetAverageModelBoundLocalSpace().SideDistances.y * 0.5f, 0), targetTransform.up),
                         Quaternion.LookRotation(Camera.main.transform.position - targetTransform.position), Vector3.one * this.SelectableObjectIconAnimation.GetIconScale()), this.CoreMaterialConfiguration.SelectionDoticonMaterial);
                }
            }
        }
    }
}
