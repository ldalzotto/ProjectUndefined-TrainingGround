public class NamingConventionHelper
{

    public static string BuildName(string baseName, LevelZonesID levelZoneID, PrefixType prefixType, SufixType sufixType)
    {
        var prefixTypeString = string.Empty;
        switch (prefixType)
        {
            case PrefixType.ATTRACTIVE_OBJECT:
                prefixTypeString = "AttractiveObject";
                break;
        }

        var sufix = string.Empty;
        switch (sufixType)
        {
            case SufixType.MODEL:
                sufix = "Model";
                break;
            case SufixType.ATTRACTIVE_OBJECT:
                sufix = "AttractiveObject";
                break;
            case SufixType.ATTRACTIVE_OBJECT_INHERENT_DATA:
                sufix = "AttractiveObject_Conf";
                break;
        }

        return levelZoneID.ToString() + "_" + prefixTypeString + "_" + baseName + "_" + sufix;
    }

}

public enum PrefixType
{
    ATTRACTIVE_OBJECT = 0
}
public enum SufixType
{
    MODEL = 0,
    ATTRACTIVE_OBJECT = 1,
    ATTRACTIVE_OBJECT_INHERENT_DATA = 2
}