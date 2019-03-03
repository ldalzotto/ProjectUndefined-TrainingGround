using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILayoutHelper : MonoBehaviour
{

    public static void UpdateLayoutGroup(LayoutGroup layoutGroup) {
        layoutGroup.CalculateLayoutInputHorizontal();
        layoutGroup.CalculateLayoutInputVertical();
        layoutGroup.SetLayoutHorizontal();
        layoutGroup.SetLayoutVertical();
    }
  
}
