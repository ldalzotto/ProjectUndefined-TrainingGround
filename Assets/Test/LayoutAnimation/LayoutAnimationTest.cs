using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LayoutAnimationTest : MonoBehaviour
{

    public VerticalLayoutGroup VerticalLayoutGroup;

    public float speed;

    private Vector2 targetPosition;

    private void Start()
    {
        VerticalLayoutGroup.enabled = true;
        VerticalLayoutGroup.CalculateLayoutInputHorizontal();
        VerticalLayoutGroup.CalculateLayoutInputVertical();
        VerticalLayoutGroup.SetLayoutHorizontal();
        VerticalLayoutGroup.SetLayoutVertical();
        targetPosition = ((RectTransform)transform).position;
        ((RectTransform)transform).position = targetPosition + new Vector2(-200, 0);
        VerticalLayoutGroup.enabled = false;
        StartCoroutine(OnDestinationReached());
    }

    private void Update()
    {
        var d = Time.deltaTime;
        ((RectTransform)transform).position = Vector2.Lerp(((RectTransform)transform).position, targetPosition, d * speed);
    }

    private IEnumerator OnDestinationReached()
    {
        yield return new WaitForTransformPositionReached(transform, targetPosition, 1f);
        transform.position = targetPosition;
        VerticalLayoutGroup.enabled = true;
    }

}
