using System;

namespace Coffee
{
    public class CoffeeGradeViewData
    {
        public CoffeeGrade Grade { get; set; }
        public bool Visible { get; set; }

        public CoffeeGradeViewData(CoffeeGrade grade)
        {
            if (grade == null)
                throw new ArgumentException();

            Grade = grade;
            Visible = true;
        }
    }
}