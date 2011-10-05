using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Person
{
    public class Student : Person
    {
        [SimplScalar] private string stuNum;

        public Student()
        {
            
        }

        public Student(string pName, string pStuNum)
            : base(pName)
        {
            stuNum = pStuNum;
        }
    }
}
