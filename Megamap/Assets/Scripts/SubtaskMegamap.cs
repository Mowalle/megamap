using System.Collections;

using UnityEngine;

namespace Megamap {

    public class SubtaskMegamap : Subtask {

        public Megamap map;

        private readonly string description = "Finde den Raum mit den meisten Bällen.";
        private LaserPointer laser;
        private float startTime = 0f;

        private void Awake()
        {
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

            foreach (var room in map.GetComponentsInChildren<SelectRoom>()) {
                room.OnTargetRoomSelected.AddListener(HandleTargetRoomSelected);
                room.OnWrongRoomSelected.AddListener(HandleWrongRoomSelected);
            }

            // Map animation.
            StopCoroutine("StartSubtask");
            StartCoroutine("StartSubtask");
        }

        private void OnDisable()
        {
            if (laser != null)
                laser.Show(false);

            foreach (var room in map.GetComponentsInChildren<SelectRoom>()) {
                room.OnTargetRoomSelected.RemoveListener(HandleTargetRoomSelected);
                room.OnWrongRoomSelected.RemoveListener(HandleWrongRoomSelected);
            }
        }

        private void HandleTargetRoomSelected(SelectRoom room)
        {
            float completionTime = Time.realtimeSinceStartup;
            RecordData.CurrentRecord.megamapTime = completionTime - startTime;

            StopCoroutine("CompleteSubtask");
            StartCoroutine("CompleteSubtask");
        }

        private void HandleWrongRoomSelected(SelectRoom room)
        {
            var task = FindObjectOfType<Task>();
            task.Description = "Raum hat nicht die meisten Bälle.\nVersuche es weiter.";

            ++RecordData.CurrentRecord.numErrors;
        }

        private IEnumerator StartSubtask()
        {
            // For data recording.
            var selectableRooms = map.GetComponentsInChildren<SelectRoom>();
            RecordData.CurrentRecord.numBallsPerRoom = new int[selectableRooms.Length];
            RecordData.CurrentRecord.roomSelections = new int[selectableRooms.Length];
            for (int i = 0; i < selectableRooms.Length; ++i) {
                for (int child = 0; child < selectableRooms[i].transform.childCount; ++child) {
                    if (selectableRooms[i].transform.GetChild(child).name.StartsWith("Ball"))
                        ++RecordData.CurrentRecord.numBallsPerRoom[i];
                }

                if (selectableRooms[i].IsTargetRoom) {
                    RecordData.CurrentRecord.correctRoomIndex = i;
                    RecordData.CurrentRecord.correctRoomName = selectableRooms[i].name;
                }
            }

            yield return StartCoroutine(map.Show());

            startTime = Time.realtimeSinceStartup;
        }

        private IEnumerator CompleteSubtask()
        {
            yield return StartCoroutine(map.Hide());
            FindObjectOfType<Task>().NextSubtask();
        }
    }

}
