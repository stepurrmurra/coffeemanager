using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Coffee
{
    class CoffeeGrade
    {
        private const int fieldCount = 6;

        public string Name { get; set; }
        public string FullName { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
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
            int minHeight, maxHeight, ripeDuration;

            if (!int.TryParse(elements[2], out minHeight) ||
                !int.TryParse(elements[3], out maxHeight) ||
                !int.TryParse(elements[4], out ripeDuration))
                return false;

            grade.MinHeight = minHeight;
            grade.MaxHeight = maxHeight;
            grade.RipeDuration = ripeDuration;

            grade.Description = elements[5];
            return true;
        }

        public override string ToString()
        {
            return $"{Name};{FullName};{MinHeight};{MaxHeight};{RipeDuration};{Description}";
        }
    }
}
