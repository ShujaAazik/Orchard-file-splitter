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

            try
            {
                FileSplitter fileSplitter = new FileSplitter(WithContractsFolder, WithoutContractsFolder, ProcessedFileFolder);
                fileSplitter.SplitFile(InputFilesFolderPath, ContractNos.Split(','));
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
