using UnityEngine;

namespace RTPuzzle
{
    public class NpcInteractionRingManager
    {

        private const string INTERACTION_RING_OBJECT_NAME = "InteractionRing";

        #region External Renferences
        private NPCAIManager npcAiManagerRef;
        #endregion

        private NpcInteractionRingType NpcInteractionRingType;
        private Vector3 ringPositionOffset;

        #region Data Retrieval
        public Vector3 RingPositionOffset { get => ringPositionOffset; }
        #endregion

        public NpcInteractionRingManager(NPCAIManager npcAiManagerRef)
        {
            ComputePositionOffset(npcAiManagerRef);

            this.npcAiManagerRef = npcAiManagerRef;
            var npcInteractionRingContainer = GameObject.FindObjectOfType<NpcInteractionRingContainer>();

            NpcInteractionRingType = GameObject.Instantiate(PrefabContainer.Instance.NpcInteractionRingPrefab, npcInteractionRingContainer.transform);
            NpcInteractionRingType.transform.rotation = Quaternion.Euler(Vector3.forward);
            NpcInteractionRingType.Init();
        }

        public void Tick(float d)
        {
            NpcInteractionRingType.transform.position = this.npcAiManagerRef.transform.position + ringPositionOffset;
        }

        public void OnFOVChanged(FOV newFOV)
        {

            var colors = NpcInteractionRingType.RingTexture.GetPixels();
            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.red;
            }

            foreach (var fovSlice in newFOV.FovSlices)
            {
                ComputeColorsPixel(fovSlice.BeginAngleIncluded, fovSlice.EndAngleExcluded, ref colors);
  
            }
            NpcInteractionRingType.RingTexture.SetPixels(colors);
            NpcInteractionRingType.RingTexture.Apply(false);
           // Debug.Log("It has changed : " + newFOV.ToString());
        }

        private void ComputeColorsPixel(float beginAngle, float endAngle, ref Color[] colors)
        {
            var beginAngleInt = Mathf.RoundToInt(beginAngle);
            var endAngleInt = Mathf.RoundToInt(endAngle);

            for (var i = beginAngleInt; i < endAngleInt; i++)
            {
                colors[i] = Color.green;
            }
        }

        private void ComputePositionOffset(NPCAIManager npcAiManagerRef)
        {
            float maxYOffset = 0f;

            var renderers = npcAiManagerRef.GetRenderers();
            for (var i = 0; i < renderers.Length; i++)
            {
                if (i == 0)
                {
                    maxYOffset = renderers[0].bounds.max.y;
                }
                else
                {
                    var currentY = renderers[i].bounds.max.y;
                    if (currentY > maxYOffset)
                    {
                        maxYOffset = currentY;
                    }
                }
            }
            ringPositionOffset = new Vector3(0, maxYOffset, 0);
        }
    }
}
