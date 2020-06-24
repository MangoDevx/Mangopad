using Mangopad.Commands;
using Mangopad.Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mangopad.ViewModels
{

    public class NoteViewModel : INotifyPropertyChanged
    {
        private string _openNote;
        private string _noteContext;
        private string _currentFilePath;
        private string _lastSaveTime;
        private string _savedText;
        private int _wordCount;
        private int _charCount;
        private bool _unsavedChanges;

        private readonly NoteService _noteService;

        public RelayCommand SaveFileAsCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand NewFileCommand { get; }
        public RelayCommand ExitFileCommand { get; }
        public RelayCommand PrintFileCommand { get; }

        public NoteViewModel()
        {
            OpenNote = "Default";
            LastSaveTime = "";
            _noteService = new NoteService();
            SaveFileAsCommand = new RelayCommand(SaveAsFile, CanFireCommand);
            OpenFileCommand = new RelayCommand(OpenFile, CanFireCommand);
            SaveFileCommand = new RelayCommand(SaveFile, CanFireCommand);
            NewFileCommand = new RelayCommand(NewFile, CanFireCommand);
            ExitFileCommand = new RelayCommand(ExitFile, CanFireCommand);
            PrintFileCommand = new RelayCommand(PrintFile, CanFireCommand);
        }

        public string OpenNote
        {
            get => _openNote;
            set
            {
                if (_openNote == value) return;
                _openNote = value;
                RaisePropertyChanged("OpenNote");
            }

        }

        public string NoteContext
        {
            get => _noteContext;
            set
            {
                if (_noteContext == value) return;
                if (_noteContext == null)
                {
                    WordCount = 0;
                }

                WordCount = value.CountWords();
                _noteContext = value;
                CharCount = value.Length;

                if (value != _savedText) _unsavedChanges = true;

                RaisePropertyChanged("NoteContext");
            }
        }

        public string LastSaveTime
        {
            get => _lastSaveTime;
            set
            {
                if (_lastSaveTime == value) return;
                _lastSaveTime = value;
                RaisePropertyChanged("LastSaveTime");
            }
        }


        public int WordCount
        {
            get => _wordCount;
            set
            {
                if (_wordCount == value) return;
                _wordCount = value;
                RaisePropertyChanged("WordCount");
            }
        }

        public int CharCount
        {
            get => _charCount;
            set
            {
                if (_charCount == value) return;
                _charCount = value;
                RaisePropertyChanged("CharCount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string property = null)
        {
            if (property is null) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void SaveAsFile(object input)
        {
            var path = _noteService.SaveAsFile((string)input);
            if (path.Length < 1) return;

            var name = path.Split(@"\").LastOrDefault();
            LastSaveTime = DateTimeOffset.Now.ToString("G");

            _currentFilePath = path;
            _unsavedChanges = false;
            _savedText = (string)input;
            OpenNote = name;
        }

        private void OpenFile(object input = null)
        {
            var strings = _noteService.OpenFile();
            if (strings.Length < 1) return;
            var name = strings[0].Split(@"\").LastOrDefault();
            WordCount = strings[1].CountWords();
            _currentFilePath = strings[0];
            NoteContext = strings[1];
            _savedText = strings[1];
            OpenNote = name;
            LastSaveTime = string.Empty;
        }

        private void SaveFile(object input)
        {
            var content = _noteService.SaveFile((string)input, _currentFilePath);
            if (!content) return;
            LastSaveTime = DateTimeOffset.Now.ToString("G");
            _unsavedChanges = false;
            _savedText = (string)input;
        }

        private void NewFile(object input)
        {
            NoteContext = string.Empty;
            _currentFilePath = string.Empty;
            OpenNote = "Default";
        }

        private void ExitFile(object input)
        {
            Environment.Exit(-1);
        }

        private void PrintFile(object input)
        {
            _noteService.PrintFile((string)input);
        }

        private bool CanFireCommand(object input)
        {
            return input is string || input is null;
        }
    }
}