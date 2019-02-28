using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(LayoutGroup))]
public class AnimatedLayout : MonoBehaviour
{
    public AnimatedLayoutComponent AnimatedLayoutComponent;

    private LayoutGroup controlledLayout;
    private bool isAnimating;

    private List<AnimatedLayoutCell> layoutElements;
    private List<AnimatedLayoutCell> destroyedElements;

    public void Init()
    {
        controlledLayout = GetComponent<LayoutGroup>();
        layoutElements = GetComponentsInChildren<AnimatedLayoutCell>().ToList();
        destroyedElements = new List<AnimatedLayoutCell>();
        isAnimating = false;
    }

    #region External Events
    public void AddLayoutElement(AnimatedLayoutCell animatedLayoutCell, int siblingIndex)
    {
        animatedLayoutCell.transform.SetParent(transform);
        animatedLayoutCell.transform.SetSiblingIndex(siblingIndex);
        layoutElements.Add(animatedLayoutCell);
        UpdateLayoutForAnimation();
        animatedLayoutCell.SetCurrentPositionAsTarget();
        animatedLayoutCell.UpdateOldPosition();
        animatedLayoutCell.AdjustCurrentPositionForInAniamtion();
        isAnimating = true;
    }

    public void DeleteLayoutElement(AnimatedLayoutCell deletedElement)
    {
        deletedElement.transform.SetParent(transform.parent);
        UpdateLayoutForAnimation();
        deletedElement.AdjustTargetPositionForOutAnimation();
        layoutElements.Remove(deletedElement);
        destroyedElements.Add(deletedElement);
        isAnimating = true;
    }

    /*
    public void DeleteRandomLayoutElement()
    {
        var deletedElement = layoutElements[Random.Range(0, layoutElements.Count)];
        deletedElement.transform.SetParent(transform.parent);
        UpdateLayoutForAnimation();
        deletedElement.AdjustTargetPositionForOutAnimation();
        layoutElements.Remove(deletedElement);
        destroyedElements.Add(deletedElement);
        isAnimating = true;
    }
    */
    #endregion

    public void Tick(float d)
    {
        if (isAnimating)
        {
            controlledLayout.enabled = false;

            bool stopAnimation = true;
            foreach (var animatedLayoutCell in layoutElements)
            {
                if (!animatedLayoutCell.AnimationTick(d, AnimatedLayoutComponent.AnimationSpeed))
                {
                    stopAnimation = false;
                }
            }

            List<AnimatedLayoutCell> elementsToDestroy = null;
            foreach (var cellToDelete in destroyedElements)
            {
                if (cellToDelete.AnimationTick(d, AnimatedLayoutComponent.AnimationSpeed))
                {
                    if (elementsToDestroy == null)
                    {
                        elementsToDestroy = new List<AnimatedLayoutCell>();
                    }
                    elementsToDestroy.Add(cellToDelete);
                }
                else
                {
                    stopAnimation = false;
                }
            }
            if (elementsToDestroy != null)
            {
                foreach (var cellToDelete in elementsToDestroy)
                {
                    destroyedElements.Remove(cellToDelete);
                    MonoBehaviour.Destroy(cellToDelete.gameObject);
                }
            }

            if (stopAnimation)
            {
                isAnimating = false;
                controlledLayout.enabled = true;
            }
        }
    }

    private void UpdateLayoutForAnimation()
    {
        foreach (var animatedLayoutCell in layoutElements)
        {
            animatedLayoutCell.UpdateOldPosition();
        }

        controlledLayout.enabled = true;
        controlledLayout.CalculateLayoutInputHorizontal();
        controlledLayout.CalculateLayoutInputVertical();
        controlledLayout.SetLayoutHorizontal();
        controlledLayout.SetLayoutVertical();
        controlledLayout.enabled = false;

        foreach (var animatedLayoutCell in layoutElements)
        {
            animatedLayoutCell.SetTargetPosition(animatedLayoutCell.transform.position);
            animatedLayoutCell.SetCurrentPositionAsOld();
        }
    }

}

[System.Serializable]
public class AnimatedLayoutComponent
{
    public float AnimationSpeed;
}