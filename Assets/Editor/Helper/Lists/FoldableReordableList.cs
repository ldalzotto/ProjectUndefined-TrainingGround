using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class FoldableReordableList<T> : ReorderableList
{

    [SerializeField]
    protected bool displayed;

    public FoldableReordableList(List<T> list, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemovebutton, string listTitle, float elementHeightFactor, ElementCallbackDelegate elementCallbackDelegate)
             : base(list, typeof(T), draggable, displayHeader, displayAddButton, displayRemovebutton)
    {
        this.drawHeaderCallback = (Rect rect) =>
        {
            rect.x += 15;
            displayed = EditorGUI.Foldout(rect, displayed, listTitle, true, EditorStyles.foldout);
            if (displayed)
            {
                this.draggable = true;
                this.displayAdd = true;
                this.displayRemove = true;
            }
            else
            {
                this.draggable = false;
                this.displayAdd = false;
                this.displayRemove = false;
            }
        };
        this.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            if (displayed)
            {
                rect.y += 2;
                rect.height -= 5;
                elementCallbackDelegate(rect, index, isActive, isFocused);
            }
        };
        elementHeightCallback = (int index) =>
        {
            if (displayed)
            {
                return elementHeight * elementHeightFactor;
            }
            else
            {
                return 0;
            }
        };
    }
}


[System.Serializable]
public class FilterFoldableReordableList<T> : FoldableReordableList<T>
{

    [SerializeField]
    private bool isFilterEnabled;

    public FilterFoldableReordableList(List<T> list, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemovebutton, string listTitle, float elementHeightFactor, ElementCallbackDelegate elementCallbackDelegate)
         : base(list, draggable, displayHeader, displayAddButton, displayRemovebutton, listTitle, elementHeightFactor, elementCallbackDelegate)
    {
        this.drawHeaderCallback = (Rect rect) =>
        {

            Rect isFilterToggleRect = new Rect(rect);
            isFilterToggleRect.width = 10f;
            isFilterToggleRect.height = 10f;
            isFilterToggleRect.y += 3f;
            this.isFilterEnabled = EditorGUI.Toggle(isFilterToggleRect, this.isFilterEnabled, EditorStyles.miniButton);
            // isFilterEnabled = EditorGUI.Toggle()
            Rect foldoutRect = new Rect(rect);
            foldoutRect.x += 20;
            displayed = EditorGUI.Foldout(foldoutRect, displayed, listTitle, true);
            if (displayed)
            {
                this.draggable = true;
                this.displayAdd = true;
                this.displayRemove = true;
            }
            else
            {
                this.draggable = false;
                this.displayAdd = false;
                this.displayRemove = false;
            }
        };
    }

    public bool IsFilterEnabled { get => isFilterEnabled; }
}