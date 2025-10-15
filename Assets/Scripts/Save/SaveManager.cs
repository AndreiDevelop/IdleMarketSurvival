using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RentTycoon
{
    public class SaveManager
    {
        private string rentZoneUpdatesFilePath;
        private string playerDataFilePath;
        
        public SaveManager()
        {
            rentZoneUpdatesFilePath = Path.Combine(Application.persistentDataPath, "RentZoneSaveData.json");
            playerDataFilePath = Path.Combine(Application.persistentDataPath, "PlayerData.json");
        }

        #region RentZoneData

        public void SaveRentZoneData(RentZone rentZoneData)
        {
            //get all saved data
            List<RentZoneSaveData> rentZoneSaveDataList = LoadRentZoneData();
            
            if(rentZoneSaveDataList != null && rentZoneSaveDataList.Count > 0)
            {
                //try to findd index to replace data
                int replacableIndex = rentZoneSaveDataList.FindIndex(x => x.Id.Equals(rentZoneData.Id));

                //if index not finded
                if (replacableIndex == -1)
                {
                    rentZoneSaveDataList.Add(rentZoneData.SaveData);
                }
                //replace the data
                else
                {
                    rentZoneSaveDataList[replacableIndex] = rentZoneData.SaveData;
                }
            }
            else
            {
                rentZoneSaveDataList = new List<RentZoneSaveData>()
                {
                    rentZoneData.SaveData
                };
            }
            
            
            //save to file
            SaveRentZoneData(rentZoneSaveDataList);
        }
        
        public void SaveRentZoneData(List<RentZone> rentZoneDataList)
        {
            //convert from RentZone to RentZoneSaveData
            List<RentZoneSaveData> rentZoneSaveDataList = new List<RentZoneSaveData>();
            foreach (var rentZone in rentZoneDataList)
            {
                rentZoneSaveDataList.Add(rentZone.SaveData);
            }

            SaveRentZoneData(rentZoneSaveDataList);
        }

        public void SaveRentZoneData(List<RentZoneSaveData> rentZoneSaveDataList)
        {
            // Create a new list to avoid modifying the original list while iterating
            List<RentZoneSaveData> newRentZoneSaveDataList = new List<RentZoneSaveData>(rentZoneSaveDataList);

            string json = JsonUtility.ToJson(new RentZoneSaveDataListWrapper { RentZoneSaveDataList = newRentZoneSaveDataList }, true);
            File.WriteAllText(rentZoneUpdatesFilePath, json);
        }
        
        //TODO fix rent load data
        public List<RentZoneSaveData> LoadRentZoneData()
        {
            //if (!File.Exists(rentZoneUpdatesFilePath))
            {
                return new List<RentZoneSaveData>()
                {
                    new RentZoneSaveData()
                    {
                        Id = "0",
                        Level = 1,
                        RewardCount = 2,
                        IsUnlocked = false
                    },
                    new RentZoneSaveData()
                    {
                        Id = "1",
                        Level = 1,
                        RewardCount = 5,
                        IsUnlocked = false
                    }
                };
            }
            
            //string json = File.ReadAllText(rentZoneUpdatesFilePath);

            //RentZoneSaveDataListWrapper wrapper = JsonUtility.FromJson<RentZoneSaveDataListWrapper>(json);
            //return wrapper.RentZoneSaveDataList;
        }

        [System.Serializable]
        private class RentZoneSaveDataListWrapper
        {
            public List<RentZoneSaveData> RentZoneSaveDataList;
        }

        #endregion
        
        #region PlayerData
        public void SavePlayerData(PlayerData playerData)
        {
            string json = JsonUtility.ToJson(playerData, true);
            File.WriteAllText(playerDataFilePath, json);
        }

        //TODO fix player load data
        public PlayerData LoadPlayerData()
        {
            //if (!File.Exists(playerDataFilePath))
            {
                return new PlayerData()
                {
                    CoinsCount = 1000,
                    CrystalsCount = 50
                }; // Return a new instance if no data is found
            }

            //string json = File.ReadAllText(playerDataFilePath);
            //return JsonUtility.FromJson<PlayerData>(json);
        }
        
        #endregion

        #region Rental
        
        public List<RentalSaveData> LoadRentalData()
        {
            return new List<RentalSaveData>();
        }

        public void SaveRentalData(Rental value)
        {
            
        }

        #endregion

        #region RentalPlace

        public List<RentZoneUnitPlaceSaveData> LoadRentZoneUnitPlaceData()
        {
            return new List<RentZoneUnitPlaceSaveData>()
            {
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "0",
                    Id = "0",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "0",
                    Id = "1",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "0",
                    Id = "2",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "0",
                    Id = "3",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "1",
                    Id = "0",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "1",
                    Id = "1",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "1",
                    Id = "2",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
                new RentZoneUnitPlaceSaveData()
                {
                    RentZoneId = "1",
                    Id = "3",
                    AttachedUnitId = null,
                    IsUnlocked = false,
                    IsUnitAttached = false
                },
            };
        }
        public void SaveRentZoneUnitPlaceData(RentZoneUnitPlaceSaveData value)
        {
            
        }
        
        #endregion
    }
}