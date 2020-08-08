using Mangopad.Commands;
using Mangopad.Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private SolidColorBrush _autoSaveBrush = Brushes.LawnGreen;
        private Timer autoSaveTimer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);

        private readonly NoteService _noteService;

        public RelayCommand SaveFileAsCommand { get; private set; }
        public RelayCommand OpenFileCommand { get; private set; }
        public RelayCommand SaveFileCommand { get; private set; }
        public RelayCommand NewFileCommand { get; private set; }
        public RelayCommand ExitFileCommand { get; private set; }
        public RelayCommand PrintFileCommand { get; private set; }
        public RelayCommand SelectAllCommand { get; private set; }
        public RelayCommand AutoSaveCommand { get; private set; }

        public NoteViewModel()
        {
            OpenNote = "Default";
            LastSaveTime = "";
            _noteService = new NoteService();
            autoSaveTimer.Start();
            autoSaveTimer.Elapsed += AutoSaveTimerElapsed;
            InitializeRelayCommands();
        }

        private void InitializeRelayCommands()
        {
            SaveFileAsCommand = new RelayCommand(SaveAsFile, CanFireCommand);
            OpenFileCommand = new RelayCommand(OpenFile, CanFireCommand);
            SaveFileCommand = new RelayCommand(SaveFile, CanFireCommand);
            NewFileCommand = new RelayCommand(NewFile, CanFireCommand);
            ExitFileCommand = new RelayCommand(ExitFile, CanFireCommand);
            PrintFileCommand = new RelayCommand(PrintFile, CanFireCommand);
            SelectAllCommand = new RelayCommand(SelectAll, CanFireCommand);
            AutoSaveCommand = new RelayCommand(AutoSave, CanFireCommand);
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

        public SolidColorBrush AutoSaveBrush
        {
            get => _autoSaveBrush;
            set
            {
                if (_autoSaveBrush == value) return;
                _autoSaveBrush = value;
                RaisePropertyChanged("AutoSaveBrush");
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
            if (_unsavedChanges) FireUnsavedChangesPopup();
            Environment.Exit(-1);
        }

        private void PrintFile(object input)
        {
            _noteService.PrintFile((string)input);
        }

        private void FireUnsavedChangesPopup()
        {
            if (MessageBox.Show("Would you like to save before exiting?", "Save before exiting", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SaveFile(_noteContext);
            }
        }

        private void SelectAll(object input)
        {
            var textBox = (TextBox)input;
            textBox.Dispatcher.Invoke(() => textBox.SelectAll());
            textBox.Dispatcher.Invoke(() => textBox.Focus());
        }

        private void AutoSave(object input)
        {
            if (AutoSaveBrush == Brushes.White)
            {
                AutoSaveBrush = Brushes.LawnGreen;
                autoSaveTimer.Start();
            }
            else
            {
                AutoSaveBrush = Brushes.White;
                autoSaveTimer.Stop();
            }
        }

        private void AutoSaveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(LastSaveTime)) return;
            SaveFile(NoteContext);
        }

        private bool CanFireCommand(object input)
        {
            return true;
        }
    }
}