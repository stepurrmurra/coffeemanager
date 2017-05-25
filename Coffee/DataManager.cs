using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coffee
{
    class DataManager
    {
        /// <summary>
        /// Символ разделения полей в файле CSV
        /// </summary>
        public const char CsvSeparator = ';';


        private const string samplePath = "../../../data_sample.csv";

        /// <summary>
        /// Путь до файла с информацией для таблицы
        /// </summary>
        private const string filePath = "../../../data.csv";

        /// <summary>
        /// Заголовок для файла 
        /// </summary>
        private const string csvHeader = "Сорт;Название;Регион;Высота;Длительность созревания(нед.);Описание";

        /// <summary>
        /// Информация о последней неудавшейся операции
        /// </summary>
        public string ErrorInfo { get; private set; }

        /// <summary>
        /// Пытается загрузить данные из файла
        /// </summary>
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
                            return grades.Count != 0;
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

        /// <summary>
        /// Воссоздает файл таблицы для программы из примера
        /// </summary>
        public void LoadSample(out IList<CoffeeGrade> grades)
        {
            File.Copy(samplePath, filePath);
            if (!TryLoad(out grades))
                throw new Exception("Не могу восстановить данные из cемпла");
        }

        /// <summary>
        /// Сохраняет коллекцию в файл таблицы
        /// </summary>
        public bool Save(IEnumerable<CoffeeGrade> grades)
        {
            try
            {
                using (var stream = File.OpenWrite(filePath))
                {
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
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
