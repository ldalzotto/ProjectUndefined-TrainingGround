using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System;

public class VisualElementsHelper : MonoBehaviour
{
    public static VisualElement VisualElementWithStyle(VisualElement ve, Action<IStyle> style)
    {
        style.Invoke(ve.style);
        return ve;
    }
}
