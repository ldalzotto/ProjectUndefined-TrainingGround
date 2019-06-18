using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class CustomEnum : PropertyAttribute
{
    private bool isSearchable;
    private bool isCreateable;

    public CustomEnum(bool isCreateable = false, bool isSearchable = true)
    {
        this.isCreateable = isCreateable;
        this.isSearchable = isSearchable;
    }

    public bool IsCreateable { get => isCreateable; }
    public bool IsSearchable { get => isSearchable; }
}
