using UnityEngine;

namespace Megamap {

    public class TaskDescriptions : MonoBehaviour {

        private void Awake()
        {
            var display = FindObjectOfType<TaskDisplay>();

            var waitTutorial = new TaskDisplay.TaskDescription();
            waitTutorial.AddTranslation(TaskDisplay.Language.German, "WARTE auf Beginn des Tutorials...");
            waitTutorial.AddTranslation(TaskDisplay.Language.English, "WAIT for the start of the tutorial...");

            var waitExperiment = new TaskDisplay.TaskDescription();
            waitExperiment.AddTranslation(TaskDisplay.Language.German, "WARTE auf Beginn des Experiments...");
            waitExperiment.AddTranslation(TaskDisplay.Language.English, "WAIT for the start of the experiment...");

            var userSetupPosition = new TaskDisplay.TaskDescription();
            userSetupPosition.AddTranslation(TaskDisplay.Language.German, "STELLE dich auf das Ziel (X).");
            userSetupPosition.AddTranslation(TaskDisplay.Language.English, "STAND on the spot marked (X).");

            var userSetupGaze = new TaskDisplay.TaskDescription();
            userSetupGaze.AddTranslation(TaskDisplay.Language.German, "SCHAUE das Ziel (X) an der Wand an, um fortzufahren.");
            userSetupGaze.AddTranslation(TaskDisplay.Language.English, "LOOK AT the target on the wall marked (X).");

            var megamapNormal = new TaskDisplay.TaskDescription();
            megamapNormal.AddTranslation(TaskDisplay.Language.German, "1. FINDE den Raum mit den meisten Bällen.\n\n2. MERKE dir die Richtung zum Raum.\n\n3. WÄHLE den Raum mit dem TRIGGER aus.");
            megamapNormal.AddTranslation(TaskDisplay.Language.English, "1. FIND the room with highest number of balls.\n\n2. REMEMBER the direction towards the room.\n\n3. SELECT the room with the TRIGGER.");

            var megamapError = new TaskDisplay.TaskDescription();
            megamapError.AddTranslation(TaskDisplay.Language.German, "Raum hat nicht die meisten Bälle.\nVersuche es weiter.");
            megamapError.AddTranslation(TaskDisplay.Language.English, "Room does not have the most balls.\nTry again.");

            var pointingNormal = new TaskDisplay.TaskDescription();
            pointingNormal.AddTranslation(TaskDisplay.Language.German, "1. ZEIGE, wo sich der Raum IN DEINER UMGEBUNG befindet.\n(Ziele auf die Mitte des Raums).\n\n2. BESTÄTIGE die Richtung mit dem TRIGGER.");
            pointingNormal.AddTranslation(TaskDisplay.Language.English, "1. POINT towards the room IN YOUR ENVIRONMENT.\n(Point towards the room's center).\n\n2. CONFIRM the direction with the TRIGGER.");

            var pointingConfirm = new TaskDisplay.TaskDescription();
            pointingConfirm.AddTranslation(TaskDisplay.Language.German, "TRIGGER: Annehmen\n\nTRACKPAD: Korrigieren");
            pointingConfirm.AddTranslation(TaskDisplay.Language.English, "TRIGGER: Confirm\n\nTRACKPAD: Undo");

            var finished = new TaskDisplay.TaskDescription();
            finished.AddTranslation(TaskDisplay.Language.German, "Geschafft!\nDas Experiment ist vorbei.");
            finished.AddTranslation(TaskDisplay.Language.English, "Finished!\nThe experiment is over.");

            display.AddDescription("waitTutorial", waitTutorial);
            display.AddDescription("waitExperiment", waitExperiment);
            display.AddDescription("userSetupPosition", userSetupPosition);
            display.AddDescription("userSetupGaze", userSetupGaze);
            display.AddDescription("megamapNormal", megamapNormal);
            display.AddDescription("megamapError", megamapError);
            display.AddDescription("pointingNormal", pointingNormal);
            display.AddDescription("pointingConfirm", pointingConfirm);
            display.AddDescription("finished", finished);
        }
    }

}
