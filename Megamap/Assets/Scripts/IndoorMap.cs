using System.Collections.Generic;

using UnityEngine;

namespace Megamap {

    public class IndoorMap : MonoBehaviour {

        public List<GameObject> Rooms
        {
            get {
                var rooms = new List<GameObject>();
                for (int i = 0; i < transform.childCount; ++i) {
                    var child = transform.GetChild(i);
                    if (child.name.ToLower().StartsWith("room"))
                        rooms.Add(child.gameObject);
                }
                return rooms;
            }
        }

        public List<SelectRoom> SelectableRooms
        {
            get {
                var rooms = Rooms;
                var selectableRooms = new List<SelectRoom>();
                foreach (var room in rooms) {
                    if (room.GetComponent<SelectRoom>() != null)
                        selectableRooms.Add(room.GetComponent<SelectRoom>());
                }
                return selectableRooms;
            }
        }

        public SelectRoom TargetRoom
        {
            get {
                return SelectableRooms.Find(room => room.IsTargetRoom);
            }
        }
    }

}
