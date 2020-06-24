using Microsoft.Win32;
using System.IO;

namespace Mangopad.Commands
{
    public class NoteService
    {
        public string SaveAsFile(string text)
        {
            if (text is null) return string.Empty;

            var fileDialog = new SaveFileDialog
            {
                DefaultExt = "txt",
                AddExtension = true,
                CreatePrompt = true,
                OverwritePrompt = true
            };

            fileDialog.ShowDialog();

            var filePath = fileDialog.FileName;
            if (filePath == string.Empty) return string.Empty;
            File.WriteAllText(filePath, text);
            return filePath;
        }

        public string[] OpenFile()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            var filePath = fileDialog.FileName;
            if (filePath == string.Empty) return new string[] { };
            var fileContent = File.ReadAllText(filePath);
            return new[] { filePath, fileContent };
        }

        public void SaveFile(string content, string path)
        {
            if (path is null)
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.ShowDialog();
                path = fileDialog.FileName;
            }
            if (string.IsNullOrEmpty(path)) return;
            File.WriteAllText(path, content);
        }
    }
}
