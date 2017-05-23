using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Coffee
{
	internal class CoffeeFileReader : IDisposable
    {
		private const int maxLineLength = 1024;
		private const int bytesPerSymbol = 2;
	    private const char csvSeparator = ';';

		private readonly string m_filePath;
		private readonly StreamReader m_fileStreamReader;

		private readonly IEnumerable<string> m_fileLines;

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

		private IEnumerable<string> ReadLinesBounded()
		{
			var builder = new StringBuilder(maxLineLength);
			int nextValue;

			while ((nextValue = m_fileStreamReader.Read()) > 0)
			{
				char character = (char)nextValue;
				if (!IsLineEndingReached(character, m_fileStreamReader) &&
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

		public CoffeeFileReader(string filePath)
		{
			if (!File.Exists(filePath))
				throw new ArgumentException($"Файл '{filePath}' не существует");
			
			m_filePath = filePath;
			m_fileStreamReader = new StreamReader(m_filePath);

			m_fileLines = ReadLinesBounded();

            // Пропускаем заголовок CSV
			var enumerator = m_fileLines.GetEnumerator();
			if (!enumerator.MoveNext())
				throw new IOException("Ошибка: Достигнут конец файла");
		}

		public IEnumerable<CoffeeGrade> ReadGrades()
		{
			var enumerator = m_fileLines.GetEnumerator();
			CoffeeGrade grade;
			while (enumerator.MoveNext())
			{
				string line = enumerator.Current;
				bool success = CoffeeGrade.TryParse(out grade, line, csvSeparator);
				yield return success ? grade : null;
			}
		}

		public void Dispose()
		{
			m_fileStreamReader.Close();
			m_fileStreamReader.Dispose();
		}
	}
}

