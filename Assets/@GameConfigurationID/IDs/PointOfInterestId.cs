﻿namespace GameConfigurationID
{
    [System.Serializable]
    public enum PointOfInterestId
    {
        NONE = 0,
        BOUNCER = 1,
        ID_CARD = 2,
        ID_CARD_INVENTORY = 11,
        ID_CARD_V2 = 3,
        PLAYER = 4,
        DUMBSTER = 5,
        CROWBAR = 6,
        CROWBAR_INVENTORY = 10,
        SEWER_ENTRANCE = 7,
        SEWER_EXIT = 8,
        SEWER_TO_PUZZLE = 9,
        SEWER_TO_PUZZLE_2 = 12,

        #region SEWER_LV1
        SEWER_RTP_1_DOOR = 13,
        #endregion

        RTP_PUZZLE_CREATION_TEST = 9999
    }
}