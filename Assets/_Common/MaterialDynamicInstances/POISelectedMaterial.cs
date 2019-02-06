using UnityEngine;

public class POISelectedMaterial
{

    public static Material Build(Material parameterCopyMaterial)
    {
        var resourceMaterial = MaterialContainer.Instance.POISelectedMaterial;
        var selectedMaterial = new Material(MaterialContainer.Instance.POISelectedMaterial);
        selectedMaterial.CopyPropertiesFromMaterial(parameterCopyMaterial);
        selectedMaterial.SetFloat("_RimMaxInfluenceFactor", resourceMaterial.GetFloat("_RimMaxInfluenceFactor"));
        selectedMaterial.SetFloat("_RimInfluenceSpeed", resourceMaterial.GetFloat("_RimInfluenceSpeed"));
        selectedMaterial.SetColor("_RimColor", resourceMaterial.GetColor("_RimColor"));
        return selectedMaterial;
    }

}
