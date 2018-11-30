using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Megamap {

    public class TaskDisplay : MonoBehaviour {

        public enum Language {
            German, English
        }

        public class TaskDescription {
            Dictionary<Language, string> translations = new Dictionary<Language, string>();

            public void AddTranslation(Language lang, string translation)
            {
                translations.Add(lang, translation);
            }

            public string GetTranslation(Language lang)
            {
                return translations[lang];
            }
        }


        [SerializeField]
        private Text taskDisplay = null;

        public string CurrentDescriptionID { get; set; }
        
        private Language language = Language.German;
        private Dictionary<string, TaskDescription> descriptions = new Dictionary<string, TaskDescription>();

        public void AddDescription(string id, TaskDescription description)
        {
            descriptions.Add(id, description);
        }

        public TaskDescription GetDescription(string id)
        {
            return descriptions[id];
        }

        public void SetLanguage(Language newLanguage)
        {
            language = newLanguage;
        }

        private void Awake()
        {
            CurrentDescriptionID = "";
        }

        private void Update()
        {
            if (CurrentDescriptionID == "")
                return;

            taskDisplay.text = GetDescription(CurrentDescriptionID).GetTranslation(language);
        }
    }

}
