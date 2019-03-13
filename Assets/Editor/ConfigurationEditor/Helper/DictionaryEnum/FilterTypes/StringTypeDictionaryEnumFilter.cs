#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Reflection;

namespace ConfigurationEditor
{
    [System.Serializable]
    public class StringTypeDictionaryEnumFilter : AbstractDictionaryEnumGUIFilter
    {
        [SerializeField]
        private RegexFilterOperation RegexFilterOperation;

        public StringTypeDictionaryEnumFilter(PropertyInfo propInfo) : base(propInfo)
        {
            this.RegexFilterOperation = new RegexFilterOperation();
        }

        protected override bool ComputeFieldFilters(object fieldValue)
        {
            return this.RegexFilterOperation.ComputeOperation(fieldValue.ToString());
        }

        protected override void GUITick()
        {
            this.RegexFilterOperation.GUiTick();
        }

        protected override void OnFilterDisabled()
        {
            this.RegexFilterOperation.ClearGUIComponent();
        }
    }

}

#endif //UNITY_EDITOR