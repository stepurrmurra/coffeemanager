using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Coffee
{
    /// <summary>
    /// Обеспечивает упрощенный способ чтения объектов типа CoffeeGrade из файла
    /// </summary>
	internal class CoffeeFileReader : IDisposable
    {
        /// <summary>
        /// Символ разделения полей в файле CSV
        /// </summary>
        public char CsvSeparator { get; }

        /// <summary>
        /// Максимальная длина одной строки в файле, 
        /// по достижению этой длины чтение файла прерывается
        /// </summary>
        private const int maxLineLength = 1024;

        /// <summary>
        /// Байт на символ (UTF-8)
        /// </summary>
		private const int bytesPerSymbol = 2;

        /// <summary>
        /// Путь до читаемого файла
        /// </summary>
		private readonly string filePath;
		private readonly StreamReader fileStreamReader;

        /// <summary>
        /// "Ленивая" коллекция со строками файла
        /// </summary>
		private readonly IEnumerable<string> fileLines;

        /// <summary>
        /// Проверяет, является ли последний считанный символ переносом строки
        /// </summary>
		private bool IsLineEndingReached(char lastRead, StreamReader stream)
		{
			if (lastRead == '\n')
				return true;
			if (lastRead == '\r')
			{
				int next = stream.Read();
				if (next < 0)
					return true;

				char nextCharacter = (char)next;

				if (nextCharacter != '\n')
				{
					// Seek back in case of line ending = '\r'
					stream.BaseStream.Seek(bytesPerSymbol, SeekOrigin.Current);
				}
				return true;
			}
			return false;
		}

        /// <summary>
        /// Считывает построчно файл, ограничивая длину строки
        /// </summary>
        /// <returns></returns>
		private IEnumerable<string> ReadLinesBounded()
		{
			var builder = new StringBuilder(maxLineLength);
			int nextValue;

			while ((nextValue = fileStreamReader.Read()) > 0)
			{
				char character = (char)nextValue;
				if (!IsLineEndingReached(character, fileStreamReader) &&
					builder.Length < maxLineLength)
				{
					builder.Append(character);
				}
				else
				{
					if (builder.Length >= maxLineLength)
						throw new IOException("Ошибка: Входной файл имел некорректный формат. " +
						                      "Слишком длинная строка. ");
					yield return builder.ToString();
					builder.Clear();
				}

			}
		}

        /// <summary>
        /// Инициализирует CoffeeFileReader файлом
        /// </summary>
        /// <param name="filePath">Путь до читаемого файла</param>
        /// <param name="csvSeparator">Разделитель в формате CSV</param>
		public CoffeeFileReader(string filePath, char csvSeparator = ';')
		{
		    this.CsvSeparator = csvSeparator;
			if (!File.Exists(filePath))
				throw new ArgumentException($"Файл '{filePath}' не существует");
			
			this.filePath = filePath;
			fileStreamReader = new StreamReader(this.filePath);

			fileLines = ReadLinesBounded();

            // Пропускаем заголовок CSV
			var enumerator = fileLines.GetEnumerator();
			if (!enumerator.MoveNext())
				throw new IOException("Ошибка: Достигнут конец файла");
		}

        /// <summary>
        /// Получает построчно объекты типа CoffeeGrade
        /// </summary>
		public IEnumerable<CoffeeGrade> ReadGrades()
		{
			var enumerator = fileLines.GetEnumerator();
			CoffeeGrade grade;

			while (enumerator.MoveNext())
			{
				string line = enumerator.Current;
				bool success = CoffeeGrade.TryParse(out grade, line, CsvSeparator);
				yield return success ? grade : null;
			}
		}

        /// <summary>
        /// Освобождает задействованные ресурсы
        /// </summary>
		public void Dispose()
		{
			fileStreamReader.Close();
			fileStreamReader.Dispose();
		}
	}
}

