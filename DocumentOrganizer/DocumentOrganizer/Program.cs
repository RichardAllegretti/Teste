using DocumentOrganizer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DocumentOrganizer
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<FileInfo> documents = new List<FileInfo>();

            Console.WriteLine("Bem vindo ao Organizador de Arquivos. ");

            Console.WriteLine("\nInforme o diretório de origem dos arquivos:");
            string sourcePath = Console.ReadLine();

            Console.WriteLine("\nInforme o diretório de destino dos arquivos, serão criados subdiretórios conforme seu tipo (Videos, Musicas, Textos, Imagens e Outros):");
            string destinationPath = Console.ReadLine();

            Console.WriteLine("\nInforme o diretório de backup dos arquivos: ");
            string directoryBackup = Console.ReadLine();

            Console.WriteLine("\nInforme o diretório onde será salvo o relatório dos arquivos ordenados:");
            string destinationReport = Console.ReadLine();            

            try
            {
                documents = FileHelper.LoadDocuments(sourcePath);

                foreach (var document in documents)
                {
                    TypeDocuments kind = VerifyTypeDocument(document);

                    ForwardsToDestination(document, kind, destinationPath, directoryBackup);
                }

                ReportGenerator(destinationPath, destinationReport);

                Console.WriteLine("Arquivos ordenados e relatório gerado. Flw Vlw xDDDD");
				Console.ReadLine();
            }
            catch (Exception)
            {
                throw;
            }

        }

        private static void ReportGenerator(string destinationPath, string destinationReport)
        {
            List<FileInfo> files = GetAllFiles(destinationPath);

            var csv = new StringBuilder();

            var firstLine = string.Format("{0};{1}", "Nome do arquivo", "Data de modificação");
            csv.AppendLine(firstLine);
            
            foreach (var file in files)
            {
                var firstColunm = file.Name;
                var secondColumn = file.LastWriteTime;
                var newLine = string.Format("{0};{1}", firstColunm, secondColumn);

                csv.AppendLine(newLine);

            }

            FileHelper.CreateDirectory(destinationReport);

            File.WriteAllText(Path.Combine(destinationReport + "\\" + Guid.NewGuid() + ".csv"), csv.ToString());
        }

        private static List<FileInfo> GetAllFiles(string destinationPath)
        {
            List<FileInfo> files = new List<FileInfo>();

            string[] documents = Directory.GetFiles(destinationPath, "*.*", SearchOption.AllDirectories);

            foreach (var document in documents)
            {
                FileInfo info = new FileInfo(document);
                files.Add(info);
            }

            files.Sort(delegate (FileInfo f1, FileInfo f2)
            {
                return f1.Name.CompareTo(f2.Name);
            });

            return files;
        }

        private static void ForwardsToDestination(FileInfo document, TypeDocuments kind, string destinationPath, string directoryBackup)
        {
            switch (kind)
            {
                case TypeDocuments.Video:

                    SaveDocument(document, destinationPath, directoryBackup, "Videos");
                    break;

                case TypeDocuments.Music:

                    SaveDocument(document, destinationPath, directoryBackup, "Musicas");
                    break;

                case TypeDocuments.Text:

                    string content;

                    content = ReplaceEspecificContent(document);

                    File.Delete(document.FullName);

                    SaveDocument(document, destinationPath, directoryBackup, "Textos", content);
                    break;

                case TypeDocuments.Image:

                    SaveDocument(document, destinationPath, directoryBackup, "Imagens");
                    break;

                case TypeDocuments.Any:

                    SaveDocument(document, destinationPath, directoryBackup, "Outros");
                    break;

                default:
                    break;
            }
        }

        private static void SaveDocument(FileInfo document, string destinationPath, string directoryBackup, string fileType, string content = null)
        {
            string letter = document.Name.ToUpper().Substring(0, 1);
            string path = Path.Combine(destinationPath + "\\" + fileType + "\\" + document.Extension.ToUpper().Replace(".", "") + "\\" + letter);
            string fullName = Path.Combine(path + "\\" + document.Name);

            FileHelper.CreateDirectory(path);

            if (!string.IsNullOrEmpty(content))
                WriteFile(document, directoryBackup, content, fullName);
            else
                MoveFile(document, directoryBackup, fullName);
        }

        private static void WriteFile(FileInfo document, string directoryBackup, string content, string fullName)
        {
            if (!File.Exists(fullName))
            {
                File.WriteAllText(fullName, content);
            }
            else
            {
                MoveDocumentToBackup(document, directoryBackup, fullName);

                File.WriteAllText(fullName, content);
            }
        }

        private static void MoveFile(FileInfo document, string directoryBackup, string fullName)
        {
            if (!File.Exists(fullName))
            {
                File.Move(document.FullName, fullName);
            }
            else
            {
                MoveDocumentToBackup(document, directoryBackup, fullName);

                File.Move(document.FullName, fullName);
            }
        }

        private static void MoveDocumentToBackup(FileInfo document, string directoryBackup, string fullName)
        {
            string name = document.Name.Split('.')[0];

            name = name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + document.Extension;

            File.Move(fullName, Path.Combine(directoryBackup + "\\" + name));
        }        

        private static string ReplaceEspecificContent(FileInfo document)
        {
            string content = File.ReadAllText(document.FullName);

            content = content.Replace("[nomeproprio]", "Johnny C.");

            return content;
        }

        private static TypeDocuments VerifyTypeDocument(FileInfo document)
        {
            if (document.Extension.ToLower().Equals(".mp4") || document.Extension.ToLower().Equals(".mkv"))
                return TypeDocuments.Video;

            if (document.Extension.ToLower().Equals(".mp3") || document.Extension.ToLower().Equals(".wav") || document.Extension.ToLower().Equals(".wma"))
                return TypeDocuments.Music;

            if (document.Extension.ToLower().Equals(".xml") || document.Extension.ToLower().Equals(".txt") || document.Extension.ToLower().Equals(".html") || document.Extension.ToLower().Equals(".csv"))
                return TypeDocuments.Text;

            if (document.Extension.ToLower().Equals(".pdf") || document.Extension.ToLower().Equals(".gif") || document.Extension.ToLower().Equals(".png"))
                return TypeDocuments.Image;

            return TypeDocuments.Any;
        }
    }
}