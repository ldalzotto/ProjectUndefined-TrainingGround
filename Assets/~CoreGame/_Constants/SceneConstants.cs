using System.Collections.Generic;

public class SceneConstants
{
    public const string OneAIForcedTargetZone = "OneAIForcedTargetZone";
    public const string OneAIForcedHighDistanceTargetZone = "OneAIForcedHighDistanceTargetZone";
    public const string OneAINoTargetZone = "OneAINoTargetZone";

    public const string Sewers_LV1_Chunk = "Sewers_LV1_Chunk";

    public static Dictionary<LevelZonesID, string> LevelSceneChunkRetriever = new Dictionary<LevelZonesID, string>()
    {
        {LevelZonesID.SEWER_RTP,  Sewers_LV1_Chunk},
        {LevelZonesID.SEWER_ADVENTURE, Sewers_LV1_Chunk }
    };
}
