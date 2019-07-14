using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class CustomEnum : PropertyAttribute
{
    private bool isSearchable;
    private bool isCreateable;
    private bool choosedOpenRepertoire;

    public CustomEnum(bool isCreateable = false, bool isSearchable = true, bool choosedOpenRepertoire = false)
    {
        this.isCreateable = isCreateable;
        this.isSearchable = isSearchable;
        this.choosedOpenRepertoire = choosedOpenRepertoire;
    }

    public bool IsCreateable { get => isCreateable; }
    public bool IsSearchable { get => isSearchable; }
    public bool ChoosedOpenRepertoire { get => choosedOpenRepertoire; }
}
