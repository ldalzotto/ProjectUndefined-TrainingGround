using UnityEngine;

namespace RTPuzzle
{
    public class DisarmObjectProgressBarType : MonoBehaviour
    {
        private RectTransform emptyBarTransform;

        public void Init()
        {
            this.emptyBarTransform = (RectTransform)this.gameObject.FindChildObjectWithLevelLimit("EmptyBar", 0).transform;
        }

        public void SetDisarmPercentage(float disarmPercentage)
        {
            var ls = this.emptyBarTransform.localScale;
            ls.x = 1 - disarmPercentage;
            this.emptyBarTransform.localScale = ls;
        }

        public void SetPosition(Vector3 worldPosition)
        {
            this.transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        }
    }

}
