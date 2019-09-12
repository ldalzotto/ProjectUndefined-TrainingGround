namespace RTPuzzle
{
    public static class PlayerActionBuilder
    {
        public static RTPPlayerAction BuildAction(PlayerActionInherentData PlayerActionInherentData)
        {
            if (PlayerActionInherentData.GetType() == typeof(LaunchProjectileActionInherentData))
            {
                return new LaunchProjectileAction((LaunchProjectileActionInherentData)PlayerActionInherentData);
            }
            else if (PlayerActionInherentData.GetType() == typeof(AttractiveObjectActionInherentData))
            {
                return new AttractiveObjectAction((AttractiveObjectActionInherentData)PlayerActionInherentData);
            }
            return null;
        }
    }

}
