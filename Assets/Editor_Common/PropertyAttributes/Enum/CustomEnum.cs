using System;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class CustomEnum : PropertyAttribute
{
    public bool IsSearchable;
    public bool IsCreateable;
    public bool ChoosedOpenRepertoire;
    public Type ConfigurationType;
    public bool OpenToConfiguration;

    public CustomEnum(bool isCreateable = false, bool isSearchable = true, bool choosedOpenRepertoire = false, Type configurationType = null, bool openToConfiguration = false)
    {
        this.IsCreateable = isCreateable;
        this.IsSearchable = isSearchable;
        this.ChoosedOpenRepertoire = choosedOpenRepertoire;
        this.ConfigurationType = configurationType;
        this.OpenToConfiguration = openToConfiguration;
    }
    
}
