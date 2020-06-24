using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;

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

        public bool SaveFile(string content, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var fileDialog = new OpenFileDialog();
                fileDialog.ShowDialog();
                path = fileDialog.FileName;
            }
            if (string.IsNullOrEmpty(path)) return false;
            File.WriteAllText(path, content);
            return true;
        }

        public void PrintFile(string content)
        {
            var pd = new PrintDialog();
            pd.ShowDialog();
            var document = CreateFlowDocument(content);
            IDocumentPaginatorSource idpSource = document;
            pd.PrintDocument(idpSource.DocumentPaginator, "");
        }

        private FlowDocument CreateFlowDocument(string input)
        {
            FlowDocument doc = new FlowDocument();
            Section sec = new Section();
            Paragraph p1 = new Paragraph();
            p1.Inlines.Add(input);
            sec.Blocks.Add(p1);
            doc.Blocks.Add(sec);
            return doc;
        }
    }
}
