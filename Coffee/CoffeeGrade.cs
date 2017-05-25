using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Coffee
{
    public class CoffeeGrade
    {
        private const int fieldCount = 6;

        public string Name { get; set; }
        public string FullName { get; set; }
        public string Region { get; set; }
        public int Height { get; set; }
        public int RipeDuration { get; set; }
        public string Description { get; set; }

        public static bool TryParse(out CoffeeGrade grade, string str, char separator)
        {
            grade = new CoffeeGrade();
            var elements = str.Split(separator);
            if (elements.Length != fieldCount)
                return false;

            grade.Name = elements[0];
            grade.FullName = elements[1];
            grade.Region = elements[2];

            int height, ripeDuration;

            if (!int.TryParse(elements[3], out height) ||
                !int.TryParse(elements[4], out ripeDuration))
                return false;

            grade.Height = height;
            grade.RipeDuration = ripeDuration;

            grade.Description = elements[5];
            return true;
        }

        public override string ToString()
        {
            return $"{Name};{FullName};{Region};{Height};{RipeDuration};{Description}";
        }
    }
}
