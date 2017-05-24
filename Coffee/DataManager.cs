using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Coffee
{
    class DataManager
    {
        public const char CsvSeparator = ';';

        private const string samplePath = "../data_sample.csv";
        private const string filePath = "../data.csv";

        private const string csvHeader = "Сорт;Название;Минимальная высота;Максимальная высота;Длительность созревания(нед.);Описание";

        public string ErrorInfo { get; private set; }

        public bool TryLoad(out IList<CoffeeGrade> grades)
        {
            grades = new List<CoffeeGrade>();
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using (var reader = new CoffeeFileReader(filePath, CsvSeparator))
                {
                    foreach (var grade in reader.ReadGrades())
                    {
                        if (grade == null)
                        {
                            ErrorInfo = $"Получена некорректная строка из файла {filePath}";
                            return false;
                        }
                        grades.Add(grade);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorInfo = ex.Message;
                return false;
            }
        }

        public void LoadSample(out IList<CoffeeGrade> grades)
        {
            File.Copy(samplePath, filePath);
            if (!TryLoad(out grades))
                throw new Exception("Не могу восстановить данные из cемпла");
        }

        public bool Save(IEnumerable<CoffeeGrade> grades)
        {
            try
            {
                using (var stream = File.OpenWrite(filePath))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(csvHeader);
                        foreach (var grade in grades)
                        {
                            writer.WriteLine(grade.ToString());
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorInfo = ex.Message;
                return false;
            }
        }
    }
}
