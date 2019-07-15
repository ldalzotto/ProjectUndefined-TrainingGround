#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ConfigurationEditor
{
    public class FieldFilerResult
    {
        private bool isFiltered;
        private bool hasAtLeastOneFilterEnabled;

        public FieldFilerResult(bool isFiltered, bool hasAtLeastOneFilterEnabled)
        {
            this.isFiltered = isFiltered;
            this.hasAtLeastOneFilterEnabled = hasAtLeastOneFilterEnabled;
        }

        public bool IsFiltered { get => isFiltered; }
        public bool HasAtLeastOneFilterEnabled { get => hasAtLeastOneFilterEnabled; }
    }

    public interface IDictionaryEnumGUIFilterContainer<out T> where T : ScriptableObject
    {
        void GUITick();
        void DeactivateAllFilters();
        void ColapseAllFilters();
        bool IsAtLeastOneFilterEnabledExpectExcluded();
    }

    [System.Serializable]
    public class DictionaryEnumGUIFilterContainer<T> : IDictionaryEnumGUIFilterContainer<T> where T : ScriptableObject
    {

        [SerializeField]
        private bool typeFolded;

        [SerializeField]
        private List<FilterElement> containsValues;

        private FilterFoldableReordableList<FilterElement> containsValuesReorderable;

        [SerializeField]
        private List<FilterElement> excludedValues;

        private FilterFoldableReordableList<FilterElement> excludedValuesReorderable;

        [SerializeField]
        private Dictionary<string, AbstractDictionaryEnumGUIFilter> retrievedDictionaryEnumSearchFilters;
        [SerializeField]
        private Dictionary<string, IDictionaryEnumGUIFilterContainer<ScriptableObject>> nestedObjectFilters;

        public DictionaryEnumGUIFilterContainer()
        { }

        private void Init()
        {
            if (this.nestedObjectFilters == null || this.retrievedDictionaryEnumSearchFilters == null)
            {
                this.nestedObjectFilters = new Dictionary<string, IDictionaryEnumGUIFilterContainer<ScriptableObject>>();
                this.retrievedDictionaryEnumSearchFilters = new Dictionary<string, AbstractDictionaryEnumGUIFilter>();
                var objProperties = typeof(T).GetProperties();
                foreach (var propInfo in objProperties)
                {
                    var dictionaryFilterAttributes = propInfo.GetCustomAttributes(typeof(DictionaryEnumSearch), true);
                    if (dictionaryFilterAttributes != null && dictionaryFilterAttributes.Length > 0)
                    {
                        var propertyType = propInfo.PropertyType;

                        var dictionaryEnumSearch = (DictionaryEnumSearch)dictionaryFilterAttributes[0];
                        if (dictionaryEnumSearch.IsObject)
                        {
                            if (propertyType.BaseType == typeof(ScriptableObject))
                            {
                                var generatedType = typeof(DictionaryEnumGUIFilterContainer<>).MakeGenericType(new Type[] { propertyType });
                                var test = Activator.CreateInstance(generatedType);
                                this.nestedObjectFilters.Add(propInfo.Name, (IDictionaryEnumGUIFilterContainer<ScriptableObject>)test);
                            }
                            this.retrievedDictionaryEnumSearchFilters.Add(propInfo.Name, new ObjectTypeDictionaryEnumFilter(propInfo));
                        }
                        else if (propertyType == typeof(String) || propertyType == typeof(string) || propertyType.IsEnum)
                        {
                            this.retrievedDictionaryEnumSearchFilters.Add(propInfo.Name, new StringTypeDictionaryEnumFilter(propInfo));
                        }
                        else if (propertyType.IsSubclassOf(typeof(UnityEngine.Object)))
                        {
                            this.retrievedDictionaryEnumSearchFilters.Add(propInfo.Name, new UnityObjectTypeDictionaryEnumFilter(propInfo));
                        }
                        else
                        {
                            this.retrievedDictionaryEnumSearchFilters.Add(propInfo.Name, new NativeTypeDictionaryEnumFilter(propInfo));
                        }
                    }
                }
            }

            if (containsValues == null)
            {
                containsValues = new List<FilterElement>();

            }
            if (containsValuesReorderable == null)
            {
                containsValuesReorderable = new FilterFoldableReordableList<FilterElement>(this.containsValues, true, true, true, true, "Contains : ", 1,
                            (Rect rect, int index, bool isActive, bool isFocused) => { FilterElement.DrawElement(rect, this.containsValues[index]); },
                            null);
            }
            if (excludedValues == null)
            {
                excludedValues = new List<FilterElement>();
            }
            if (excludedValuesReorderable == null)
            {
                excludedValuesReorderable = new FilterFoldableReordableList<FilterElement>(this.excludedValues, true, true, true, true, "Exclude : ", 1,
                  (Rect rect, int index, bool isActive, bool isFocused) => { FilterElement.DrawElement(rect, this.excludedValues[index]); },
                  null);
            }

        }

        public void GUITick()
        {
            this.Init();

            EditorGUILayout.BeginVertical(EditorStyles.textField);
            EditorGUI.BeginChangeCheck();
            var typeFolded = EditorGUILayout.Foldout(this.typeFolded, typeof(T).ToString(), true, EditorStyles.foldout);
            if (EditorGUI.EndChangeCheck())
            {
                this.typeFolded = typeFolded;
            }
            if (this.typeFolded)
            {
                this.containsValuesReorderable.DoLayoutList();
                this.excludedValuesReorderable.DoLayoutList();

                foreach (var searchFilter in retrievedDictionaryEnumSearchFilters)
                {
                    if (searchFilter.Value != null)
                    {
                        searchFilter.Value.AbstractGUITick();
                    }

                }

                foreach (var nestedObjectFilter in this.nestedObjectFilters)
                {
                    if (nestedObjectFilter.Value != null)
                    {
                        nestedObjectFilter.Value.GUITick();
                    }

                }
            }
            EditorGUILayout.EndVertical();

        }

        public void DeactivateAllFilters()
        {
            this.containsValuesReorderable.DisableFilter();
            this.excludedValuesReorderable.DisableFilter();
            foreach (var enumSearchFilter in this.retrievedDictionaryEnumSearchFilters)
            {
                enumSearchFilter.Value.DeactivateFilter();
            }
            foreach (var nestedObjectFilter in this.nestedObjectFilters)
            {
                nestedObjectFilter.Value.DeactivateAllFilters();
            }
        }


        public void ColapseAllFilters()
        {
            this.typeFolded = false;
            this.containsValuesReorderable.ColapseFilter();
            this.excludedValuesReorderable.ColapseFilter();
            foreach (var enumSearchFilter in this.retrievedDictionaryEnumSearchFilters)
            {
                enumSearchFilter.Value.ColapseFilter();
            }
            foreach (var nestedObjectFilter in this.nestedObjectFilters)
            {
                nestedObjectFilter.Value.ColapseAllFilters();
            }
        }

        public bool ComputeFieldFilters(T value)
        {

            this.Init();
            bool isFiltered = false;

            if (value != null)
            {
                //nested objects
                foreach (var nestedObjectFilter in this.nestedObjectFilters)
                {
                    var nestedVal = value.GetType().GetProperty(nestedObjectFilter.Key).GetValue(value, null);

                    if (!(bool)nestedObjectFilter.Value.GetType().GetMethod(nameof(DictionaryEnumGUIFilterContainer<T>.ComputeFieldFilters)).Invoke(nestedObjectFilter.Value, new object[] { nestedVal }))
                    {
                        return false;
                    }
                }

                isFiltered = !(this.containsValues == null || this.containsValues.Count == 0 || !this.containsValuesReorderable.IsFilterEnabled || FilterElement.ToElements(this.containsValues).Contains(value));
                if (isFiltered)
                {
                    return false;
                }

                isFiltered = !(this.excludedValues == null || this.excludedValues.Count == 0 || !this.excludedValuesReorderable.IsFilterEnabled || !FilterElement.ToElements(this.excludedValues).Contains(value));
                if (isFiltered)
                {
                    return false;
                }
            }

            foreach (var enumSearchFilter in this.retrievedDictionaryEnumSearchFilters)
            {
                if (enumSearchFilter.Value != null)
                {
                    if (enumSearchFilter.Value.IsFilterEnabled())
                    {
                        if (value == null)
                        {
                            return false;
                        }
                        if (!enumSearchFilter.Value.ComputeObjectFilters(value))
                        {
                            return false;
                        }
                    }
                }

            }

            //passed through all filters
            if (value == null)
            {
                if (this.IsAtLeastOneFilterEnabledExpectExcluded())
                {
                    return false;
                }
            }

            return true;
            //return !isFiltered;
        }


        public bool IsAtLeastOneFilterEnabledExpectExcluded()
        {

            bool containsValuesEnabled = (containsValues != null && containsValues.Count > 0 && this.containsValuesReorderable.IsFilterEnabled);

            bool atLeastOneEnumSearchFilter = false;
            if (this.retrievedDictionaryEnumSearchFilters != null)
            {
                foreach (var retrievedDictionaryEnumSearchFilter in this.retrievedDictionaryEnumSearchFilters)
                {
                    if (retrievedDictionaryEnumSearchFilter.Value != null)
                    {
                        if (retrievedDictionaryEnumSearchFilter.Value.IsFilterEnabled())
                        {
                            atLeastOneEnumSearchFilter = true;
                        }
                    }

                }
            }

            bool nestedFilterEnabled = false;
            if (this.nestedObjectFilters != null)
            {
                foreach (var nestedFilter in this.nestedObjectFilters)
                {
                    nestedFilterEnabled = nestedFilter.Value.IsAtLeastOneFilterEnabledExpectExcluded();
                }
            }

            return (containsValuesEnabled || atLeastOneEnumSearchFilter || nestedFilterEnabled);
        }

        #region custom filter data 
        [System.Serializable]
        class FilterElement
        {
            [SerializeField]
            private T element;

            public T Element { get => element; set => element = value; }

            public static void DrawElement(Rect rect, FilterElement element)
            {
                element.Element = EditorGUI.ObjectField(rect, element.Element, typeof(T), false) as T; ;
            }

            public static List<T> ToElements(List<FilterElement> filterContainsElements)
            {
                return filterContainsElements.ConvertAll<T>((e) => e.element);
            }

        }
        #endregion
    }

    [System.Serializable]
    public class AbstractDictionaryEnumGUIFilterWithFieldName
    {
        [SerializeField]
        private AbstractDictionaryEnumGUIFilter abstractDictionaryEnumGUI;

        [SerializeField]
        private string fieldName;

        public AbstractDictionaryEnumGUIFilterWithFieldName(AbstractDictionaryEnumGUIFilter abstractDictionaryEnumGUI, string fieldName)
        {
            this.abstractDictionaryEnumGUI = abstractDictionaryEnumGUI;
            this.fieldName = fieldName;
        }

        public string FieldName { get => fieldName; }
        public AbstractDictionaryEnumGUIFilter AbstractDictionaryEnumGUI { get => abstractDictionaryEnumGUI; }

    }

    [System.Serializable]
    public class IDictionaryEnumGUIFilterContainerWithFieldName
    {
        [SerializeField]
        private IDictionaryEnumGUIFilterContainer<ScriptableObject> dictionaryEnumGUIFilterContainer;

        [SerializeField]
        private string fieldName;

        public string FieldName { get => fieldName; }
        public IDictionaryEnumGUIFilterContainer<ScriptableObject> DictionaryEnumGUIFilterContainer { get => dictionaryEnumGUIFilterContainer; }

        public IDictionaryEnumGUIFilterContainerWithFieldName(IDictionaryEnumGUIFilterContainer<ScriptableObject> dictionaryEnumGUIFilterContainer, string fieldName)
        {
            this.dictionaryEnumGUIFilterContainer = dictionaryEnumGUIFilterContainer;
            this.fieldName = fieldName;
        }
    }
}


#endif //UNITY_EDITOR