using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard_file_splitter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string InputFilesFolderPath = ConfigurationManager.AppSettings["InputFilesFolderPath"];
            string WithContractsFolder = ConfigurationManager.AppSettings["WithContractsFolder"];
            string WithoutContractsFolder = ConfigurationManager.AppSettings["WithoutContractsFolder"];
            string ProcessedFileFolder = ConfigurationManager.AppSettings["ProcessedFileFolder"];
            string ContractNos = ConfigurationManager.AppSettings["ContractNos"];

            DirectoryValidations(new string[] { InputFilesFolderPath, WithContractsFolder, ProcessedFileFolder, WithoutContractsFolder, ConfigurationManager.AppSettings["ErrorFileFolder"] });
            FileSplitter fileSplitter = new FileSplitter(WithContractsFolder, WithoutContractsFolder, ProcessedFileFolder);
            Console.WriteLine("Splitting Process Started.\n");
            fileSplitter.SplitFile(InputFilesFolderPath, ContractNos.Split(','));
            Console.WriteLine("Splitting Process is Completed.");
        }

        private static void DirectoryValidations(string[] pathList)
        {
            foreach (string path in pathList)
            {
                FileHandling.ValidateDirectory(path);
            }
        }
    }
}
