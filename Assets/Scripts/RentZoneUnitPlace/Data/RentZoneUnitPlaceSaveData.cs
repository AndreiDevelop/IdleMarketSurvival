namespace RentTycoon
{
    [System.Serializable]
    public struct RentZoneUnitPlaceSaveData
    {
        public string RentZoneId;
        public string Id;
        public string AttachedUnitId;
        public bool IsUnlocked;
        public bool IsUnitAttached;
    }
}