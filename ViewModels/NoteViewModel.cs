using Mangopad.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Mangopad.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        private Note _activeNote;

        public NoteViewModel()
        {
            LoadActiveNote();
        }

        public Note ActiveNote
        {
            get => _activeNote;
            set
            {
                if (_activeNote == value) return;
                _activeNote = value;
                RaisePropertyChanged("ActiveNote");
            }

        }

        public async void LoadActiveNote()
        {
            await using var fs = File.OpenRead(@$"{Environment.CurrentDirectory}\NoteMemory.json");
            var notes = await JsonSerializer.DeserializeAsync<NotesModel>(fs);
            var note = notes.Notes.FirstOrDefault(x => x.Default);
            if (note == null) return;
            ActiveNote = new Note { Default = note.Default, Path = note.Path };
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
    }
}