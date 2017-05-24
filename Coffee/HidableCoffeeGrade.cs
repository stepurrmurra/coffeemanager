using System;

namespace Coffee
{
    public class HidableCoffeeGrade
    {
        public CoffeeGrade Grade { get; set; }
        public bool Visibility { get; set; }

        public HidableCoffeeGrade(CoffeeGrade grade)
        {
            if (grade == null)
                throw new ArgumentException();

            Grade = grade;
            Visibility = true;
        }
    }
}