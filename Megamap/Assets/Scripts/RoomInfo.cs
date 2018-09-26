using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap { 

    public class RoomInfo : MonoBehaviour {

        [Serializable]
        private class BuildingData {
            public string building;
            public Level[] levels;
        }

        [Serializable]
        private class Level {
            public int level;
            public string name;
            public Room[] rooms;
        }

        [Serializable]
        private class Room {
            public string id;
            public string name;
            public string type;
            public Contact[] contacts;
        }

        [Serializable]
        private class Contact {
            public string name;
            public string telephone;
            public string telephone_2;
            public string email;
            public string email_2;
        }

        public TextAsset JsonFile;

        public int level;
        public string roomID;

        private void Start()
        {
            if (JsonFile == null)
                return;

            var json = JsonFile.text;
            var buildingData = JsonUtility.FromJson<BuildingData>(json);

            // TODO: Better error handling.
            var levelData = Array.Find(buildingData.levels, l => l.level == this.level);
            if (levelData == null) {
                Debug.Log("Data for building " + buildingData.building + " does not contain level with index " + this.level + ".");
                gameObject.SetActive(false);
                return;
            }

            var roomData = Array.Find(levelData.rooms, room => room.id.Equals(roomID));
            if (roomData == null) {
                Debug.Log("Data for level " + level + " does not contain data for room " + roomID + ".");
                gameObject.SetActive(false);
                return;
            }

            string infoText = "Room " + roomData.name + '\n';
            // infoText += //ToString(roomData.type)
            if (roomData.contacts != null && roomData.contacts.Length > 0) {
                foreach (Contact c in roomData.contacts) {
                    infoText += c.name + '\n';
                    if (!c.telephone.Equals(""))
                        infoText += c.telephone + '\n';
                    if (c.telephone_2 != null)
                        infoText += c.telephone_2 + '\n';
                    if (c.email != null)
                        infoText += c.email + '\n';
                    if (c.email_2 != null)
                        infoText += c.email_2 + '\n';
                }
            }

            infoText = infoText.TrimEnd('\n');

            var textField = GetComponentInChildren<Text>();
            textField.text = infoText;
        }
    }
}