﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard_file_splitter
{
    internal class FileSplitter
    {
        private readonly string WithContractsFolder;
        private readonly string WithoutContractsFolder;
        private readonly string ProcessedFileFolder;
        private readonly string ErrorFileFolder;
        private readonly string QUOTE = "\"";

        public FileSplitter(string WithContractsFolder, string WithoutContractsFolder, string ProcessedFileFolder)
        {
            FileHandling.ValidateDirectory(WithContractsFolder);
            this.WithContractsFolder = WithContractsFolder;
            FileHandling.ValidateDirectory(WithoutContractsFolder);
            this.WithoutContractsFolder = WithoutContractsFolder;
            FileHandling.ValidateDirectory(ProcessedFileFolder);
            this.ProcessedFileFolder = ProcessedFileFolder;
            FileHandling.ValidateDirectory(ConfigurationManager.AppSettings["ErrorFileFolder"]);
            this.ErrorFileFolder = ConfigurationManager.AppSettings["ErrorFileFolder"];
        }

        public void SplitFile(string InputFilesFolderPath, string[] contractNos)
        {
            var withContract = new List<string>();
            var withoutContract = new List<string>();
            var errorFileList = new List<string>();

            foreach (var file in Directory.GetFiles(InputFilesFolderPath))
            {
                Console.WriteLine("Splitting Process Started.\n");
                var fileName = Path.GetFileName(file);
                if (!fileName.ToLower().Contains("jd"))
                {
                    FileHandling.CopyFile(file, Path.Combine(WithContractsFolder, fileName));
                    FileHandling.CopyFile(file, Path.Combine(WithoutContractsFolder, fileName));
                    FileHandling.MoveFile(file, Path.Combine(ProcessedFileFolder, fileName));
                    continue;
                }

                Console.WriteLine($"{File.ReadAllLines(file).Count()} line were found in the {fileName} file.\n");

                int index = 0;
                foreach (var line in File.ReadAllLines(file))
                {
                    try
                    {
                        var row = line.Split(new char[] { ' ' });
                        row = RefineColumns(row);
                        if (contractNos.Any(contactNo => contactNo.Equals(row[34])))
                        {
                            withContract.Add(line);
                            Console.WriteLine($"1 line has been identified as with Contract\n");
                        }
                        else
                        {
                            withoutContract.Add(line);
                            Console.WriteLine($"1 line has been identified as without Contract\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"1 line has been identified as Error\n");
                        Console.WriteLine(ex.Message);
                        errorFileList.Add($"File Name: {fileName} \t Error: {ex.Message} \t Line No: {index} \t Time Occured: {DateTime.Now}");
                    }
                    index++;
                }

                foreach(var contract in new List<string>[] { withContract, withoutContract })
                {
                    try
                    {
                        WriteToDirectory(Path.Combine(WithContractsFolder, fileName), withContract, "with contract");
                        WriteToDirectory(Path.Combine(WithoutContractsFolder, fileName), withoutContract, "without contract");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                FileHandling.MoveFile(file, Path.Combine(ProcessedFileFolder, fileName));
            }
            WriteToDirectory(Path.Combine(ErrorFileFolder, "Error File List"), errorFileList, "error");
            Console.WriteLine("Splitting Process is Completed.");
        }

        private void WriteToDirectory(string destinationDirectoryPath, List<string> fileList, string fileType)
        {
            if (fileList.Any())
            {
                Console.WriteLine($"{fileList.Count} number of line(s) were found as {fileType}.\n");
                FileHandling.WriteFile(destinationDirectoryPath, fileList);
            }
        }

        private string[] RefineColumns(string[] rows)
        {
            var refinedRow = new List<string>();
            var data = new StringBuilder();
            var activeRowSplit = false;

            foreach (var value in rows)
            {
                if(!activeRowSplit)
                {
                    
                    if (value.Equals(QUOTE) || (value.StartsWith(QUOTE) && !value.EndsWith(QUOTE)))
                    {
                        data.Append(value);
                        activeRowSplit = true;
                        continue;
                    }

                    
                    if ((!value.Contains(QUOTE)) || (value.StartsWith(QUOTE) && value.EndsWith(QUOTE)))
                    {
                        refinedRow.Add(value);
                        continue;
                    }
                }
                else if(activeRowSplit)
                {
                    if (!value.Contains(QUOTE))
                    {
                        data.Append($" {value}");
                        continue;
                    }

                    if (value.Equals(QUOTE) || value.EndsWith(QUOTE))
                    {
                        data.Append($" {value}");
                        refinedRow.Add(data.ToString());
                        data.Clear();
                        activeRowSplit = false;
                        continue;
                    }
                }
            }

            return refinedRow.ToArray();
        }
    }
}