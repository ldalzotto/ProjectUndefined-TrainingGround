using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class CustomEnum : PropertyAttribute
{
    public bool IsSearchable;
    public bool IsCreateable;
    public bool ChoosedOpenRepertoire;

    public CustomEnum(bool isCreateable = false, bool isSearchable = true, bool choosedOpenRepertoire = false)
    {
        this.IsCreateable = isCreateable;
        this.IsSearchable = isSearchable;
        this.ChoosedOpenRepertoire = choosedOpenRepertoire;
    }
    
}
