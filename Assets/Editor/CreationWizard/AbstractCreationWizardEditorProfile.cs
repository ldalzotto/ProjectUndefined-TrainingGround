using OdinSerializer;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCreationWizardEditorProfile : SerializedScriptableObject
{
    [HideInInspector]
    public Vector2 WizardScrollPosition { get; set; }

    [HideInInspector]
    public List<Object> GeneratedObjects { get; set; }

    public virtual void OnEnable()
    {
        this.GeneratedObjects = new List<Object>();
    }

    public virtual void ResetEditor()
    {
        this.GeneratedObjects.Clear();
    }

    public abstract void OnGenerationEnd();
}
