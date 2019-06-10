using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class BoundsHelper
    {
        public static Bounds GetAverageRendererBounds(Renderer[] renderers)
        {
            float maxXOffset = 0f;
            float maxYOffset = 0f;
            float maxZOffset = 0f;

            float minXOffset = 0f;
            float minYOffset = 0f;
            float minZOffset = 0f;
            
            for (var i = 0; i < renderers.Length; i++)
            {
                if (i == 0)
                {
                    maxXOffset = renderers[0].bounds.max.x;
                    maxYOffset = renderers[0].bounds.max.y;
                    maxZOffset = renderers[0].bounds.max.z;

                    minXOffset = renderers[0].bounds.min.x;
                    minYOffset = renderers[0].bounds.min.y;
                    minZOffset = renderers[0].bounds.min.z;
                }
                else
                {
                    var currentX = renderers[i].bounds.max.x;
                    var currentY = renderers[i].bounds.max.y;
                    var currentZ = renderers[i].bounds.max.z;

                    if (currentX > maxXOffset)
                    {
                        maxXOffset = currentX;
                    }
                    if (currentY > maxYOffset)
                    {
                        maxYOffset = currentY;
                    }
                    if (currentZ > maxZOffset)
                    {
                        maxZOffset = currentZ;
                    }

                    var currentMinX = renderers[i].bounds.min.x;
                    var currentMinY = renderers[i].bounds.min.y;
                    var currentMinZ = renderers[i].bounds.min.z;

                    if (currentMinX < minXOffset)
                    {
                        maxXOffset = currentX;
                    }
                    if (currentMinY < minYOffset)
                    {
                        maxYOffset = currentY;
                    }
                    if (currentMinZ < minZOffset)
                    {
                        maxZOffset = currentZ;
                    }
                }
            }
            return new Bounds(new Vector3((maxXOffset - minXOffset) / 2f, (maxYOffset - minYOffset) / 2f, (maxZOffset - minYOffset) / 2f),
                                 new Vector3(maxXOffset - minXOffset, maxYOffset - minYOffset, maxZOffset - minYOffset));
        }
    }

}
