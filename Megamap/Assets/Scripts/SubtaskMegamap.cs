
using System;

using UnityEngine;

namespace Megamap {

    public class SubtaskMegamap : Subtask {

        public Megamap map;

        private readonly string description = "Finde den Raum mit den meisten Bällen.";
        private LaserPointer laser;
        private float startTime = 0f;

        private bool completed = false;

        private float transitionDuration = 0f;

        private void Awake()
        {
            if (map == null)
                map = FindObjectOfType<Megamap>();

            laser = FindObjectOfType<LaserPointer>();
        }

        private void OnEnable()
        {
            LogSubtask();

            FindObjectOfType<Task>().Description = description;

            laser.Show(true);

            // Update Megamap with values from condition.
            var condition = FindObjectOfType<ConditionSwitcher>().CurrentCondition;
            map.scale = condition.scale;
            map.heightOffset = condition.heightOffset;

            var rooms = map.GetComponentsInChildren<SelectRoom>(true);
            RecordData.CurrentRecord.numBallsPerRoom = new int[rooms.Length];
            RecordData.CurrentRecord.roomSelections = new int[rooms.Length];

            FindObjectOfType<SelectRoomConfiguration>().RandomizeBallNumbers();
            foreach (var room in rooms) {
                // Register handlers.
                room.OnTargetRoomSelected.AddListener(HandleTargetRoomSelected);
                room.OnWrongRoomSelected.AddListener(HandleWrongRoomSelected);
            }

            transitionDuration = map.transitionDuration;
            map.Show(true);

            startTime = Time.realtimeSinceStartup;
        }

        private void OnDisable()
        {
            if (laser != null)
                laser.Show(false);

            foreach (var room in map.GetComponentsInChildren<SelectRoom>(true)) {
                room.OnTargetRoomSelected.RemoveListener(HandleTargetRoomSelected);
                room.OnWrongRoomSelected.RemoveListener(HandleWrongRoomSelected);
            }

            completed = false;
        }

        private void Update()
        {
            if (completed && !map.IsShown())
                FindObjectOfType<Task>().NextSubtask();
        }

        private void HandleTargetRoomSelected(SelectRoom room)
        {
            // For data recording.
            float completionTime = Time.realtimeSinceStartup;
            // Would be cooler to instead of saving the transition duration calculate it as soon as map is shown, but this would require more implementation/events.
            RecordData.CurrentRecord.megamapTime = completionTime - startTime - transitionDuration;

            var rooms = map.GetComponentsInChildren<SelectRoom>(true);
            for (int i = 0; i < rooms.Length; ++i) {
                RecordData.CurrentRecord.numBallsPerRoom[i] = rooms[i].Balls.Count;
            }

            RecordData.CurrentRecord.correctRoomIndex = Array.IndexOf(rooms, room);
            RecordData.CurrentRecord.correctRoomName = room.name;

            map.Show(false);
            completed = true;
        }

        private void HandleWrongRoomSelected(SelectRoom room)
        {
            var task = FindObjectOfType<Task>();
            task.Description = "Raum hat nicht die meisten Bälle.\nVersuche es weiter.";

            ++RecordData.CurrentRecord.numErrors;
        }
    }

}
