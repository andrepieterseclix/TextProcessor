using Framework.Wpf.Mvvm;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.Processors.Data;

namespace TextProcessor.Processors.ViewModels
{
    class BaseSettingsViewModel : ViewModelBase
    {
        string separator;

        string selectedOrientation;

        string customFrontEnclosingString;

        string customBackEnclosingString;

        EnclosingCharacters selectedEnclosingCharacters;

        bool enableCustomEnclosingCharacters;

        public BaseSettingsViewModel()
        {
            Separator = ", ";
            Orientations = new string[] { "Horizontal", "Vertical" };

            SelectedOrientation = Orientations.FirstOrDefault();
            EnclosingCharacters = new List<EnclosingCharacters>();

            initializeEnclosingCharacters();
        }

        public string Separator
        {
            get { return separator; }
            set { SetProperty(ref separator, value); }
        }

        public string[] Orientations { get; private set; }

        public List<EnclosingCharacters> EnclosingCharacters { get; private set; }

        public bool EnableCustomEnclosingCharacters
        {
            get { return enableCustomEnclosingCharacters; }
            set { SetProperty(ref enableCustomEnclosingCharacters, value); }
        }

        public EnclosingCharacters SelectedEnclosingCharacters
        {
            get { return selectedEnclosingCharacters; }
            set
            {
                if(SetProperty(ref selectedEnclosingCharacters, value))
                    EnableCustomEnclosingCharacters = (SelectedEnclosingCharacters == CustomEnclosingCharacters);
            }
        }

        public EnclosingCharacters CustomEnclosingCharacters { get; private set; }

        public string SelectedOrientation
        {
            get { return selectedOrientation; }
            set { SetProperty(ref selectedOrientation, value); }
        }

        public string CustomFrontEnclosingString
        {
            get { return customFrontEnclosingString; }
            set { SetProperty(ref customFrontEnclosingString, value); }
        }

        public string CustomBackEnclosingString
        {
            get { return customBackEnclosingString; }
            set { SetProperty(ref customBackEnclosingString, value); }
        }

        public string GetFrontEnclosingString()
        {
            return EnableCustomEnclosingCharacters ? CustomFrontEnclosingString : SelectedEnclosingCharacters.Front;
        }

        public string GetBackEnclosingString()
        {
            return EnableCustomEnclosingCharacters ? customBackEnclosingString : SelectedEnclosingCharacters.Back;
        }

        void initializeEnclosingCharacters()
        {
            CustomEnclosingCharacters = new EnclosingCharacters("Custom", null);
            var noEnclosingCharacters = new EnclosingCharacters("(none)", "");
            EnclosingCharacters.Add(noEnclosingCharacters);
            EnclosingCharacters.Add(CustomEnclosingCharacters);
            EnclosingCharacters.Add(new EnclosingCharacters("Braces", "{", "}"));
            EnclosingCharacters.Add(new EnclosingCharacters("Brackets", "[", "]"));
            EnclosingCharacters.Add(new EnclosingCharacters("Parentheses", "(", ")"));
            EnclosingCharacters.Add(new EnclosingCharacters("Double Quotes", "\""));
            EnclosingCharacters.Add(new EnclosingCharacters("Single Quotes", "'"));
            SelectedEnclosingCharacters = noEnclosingCharacters;
        }
    }
}
