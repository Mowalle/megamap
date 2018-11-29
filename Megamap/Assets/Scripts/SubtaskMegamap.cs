
using System;

using UnityEngine;

namespace Megamap {

    public class SubtaskMegamap : Subtask {

        public IndoorMap indoorMap = null;

        private Megamap map = null;
        private LaserPointer laser = null;

        private float startTime = 0f;

        private bool completed = false;

        public override void StartSubtask()
        {
            completed = false;

            LogSubtask();
            FindObjectOfType<TaskDisplay>().CurrentDescriptionID = "megamapNormal";

            laser.gameObject.SetActive(true);
            laser.Show(true);

            // Update Megamap with values from condition.
            var condition = FindObjectOfType<ConditionSwitcher>().CurrentCondition;
            map.scale = condition.scale;
            map.heightOffset = condition.heightOffset;

            map.SetMap(indoorMap);
            map.GetComponent<RoomGuides>().enabled = true;
            map.GetComponent<UserMarker>().enabled = true;

            var rooms = map.SelectableRooms;
            RecordData.CurrentRecord.numBallsPerRoom = new int[rooms.Count];
            RecordData.CurrentRecord.roomSelections = new int[rooms.Count];

            FindObjectOfType<SelectRoomConfiguration>().RandomizeBallNumbers();
            foreach (var room in rooms) {
                // Make it so that none of the rooms counts as 'clicked'.
                // Important if the same map is used across multiple tasks (e.g. in testing).
                room.ResetRoom();

                // Randomize balls.
                room.GenerateBalls();

                // Register handlers.
                room.OnTargetRoomSelected.AddListener(HandleTargetRoomSelected);
                room.OnWrongRoomSelected.AddListener(HandleWrongRoomSelected);
            }

            map.Show();

            startTime = Time.realtimeSinceStartup;
        }

        public override void StopSubtask()
        {
            laser.Show(false);

            foreach (var room in map.SelectableRooms) {
                room.OnTargetRoomSelected.RemoveListener(HandleTargetRoomSelected);
                room.OnWrongRoomSelected.RemoveListener(HandleWrongRoomSelected);
            }

            // No need to disable map here, as this is already handled
            // by map.Hide() in HandleTargetRoomSelected().
            // map.gameObject.SetActive(false); <-- No!
        }

        private void Awake()
        {
            map = FindObjectOfType<Megamap>();
            laser = FindObjectOfType<LaserPointer>();
        }

        private void Update()
        {
            // When the correct room was clicked and handled by HandleTargetRoomSelected(),
            // the hiding animation for the map is started. However, we don't want to switch
            // to the next subtask UNTIL the map is completely hidden. Thus, we wait.
            if (completed && !map.IsShown) {
                FindObjectOfType<TaskSwitcher>().CurrentTask.NextSubtask();
            }
        }

        private void HandleTargetRoomSelected(SelectRoom room)
        {
            // For data recording.
            float completionTime = Time.realtimeSinceStartup;
            // We have to subtract the animationDuration once since the start time is taken BEFORE the animation started (see StartSubtask()).
            RecordData.CurrentRecord.megamapTime = completionTime - startTime - map.animationDuration;

            var rooms = map.GetComponentsInChildren<SelectRoom>(true);
            for (int i = 0; i < rooms.Length; ++i) {
                RecordData.CurrentRecord.numBallsPerRoom[i] = rooms[i].Balls.Count;
            }

            RecordData.CurrentRecord.correctRoomIndex = Array.IndexOf(rooms, room);
            RecordData.CurrentRecord.correctRoomName = room.name;

            map.Hide();

            completed = true;
        }

        private void HandleWrongRoomSelected(SelectRoom room)
        {
            FindObjectOfType<TaskDisplay>().CurrentDescriptionID = "megamapError";

            ++RecordData.CurrentRecord.numErrors;
        }
    }

}
