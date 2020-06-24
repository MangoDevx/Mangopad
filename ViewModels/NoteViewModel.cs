using Mangopad.Commands;
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

        private readonly NoteService _noteService;

        public RelayCommand SaveFileAsCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand NewFileCommand { get; }

        public NoteViewModel()
        {
            OpenNote = "Default";
            LastSaveTime = "0/0/0000 00:00";
            _noteService = new NoteService();
            SaveFileAsCommand = new RelayCommand(SaveAsFile, CanFireCommand);
            OpenFileCommand = new RelayCommand(OpenFile, CanFireCommand);
            SaveFileCommand = new RelayCommand(SaveFile, CanFireCommand);
            NewFileCommand = new RelayCommand(NewFile, CanFireCommand);

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
                _noteContext = value;
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
            _currentFilePath = path;
            var name = path.Split(@"\").LastOrDefault();
            OpenNote = name;
            LastSaveTime = DateTimeOffset.Now.ToString("g");
        }

        private void OpenFile(object input = null)
        {
            var strings = _noteService.OpenFile();
            if (strings.Length < 1) return;
            var name = strings[0].Split(@"\").LastOrDefault();
            _currentFilePath = strings[0];
            NoteContext = strings[1];
            OpenNote = name;
        }

        private void SaveFile(object input)
        {
            _noteService.SaveFile((string)input, _currentFilePath);
            LastSaveTime = DateTimeOffset.Now.ToString("g");
        }

        private void NewFile(object input)
        {
            NoteContext = string.Empty;
            _currentFilePath = string.Empty;
            OpenNote = "Default";
        }

        private bool CanFireCommand(object input)
        {
            return input is string || input is null;
        }
    }
}