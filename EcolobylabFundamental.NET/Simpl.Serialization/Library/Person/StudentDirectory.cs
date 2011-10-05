using System.Collections.Generic;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Person
{
    public class StudentDirectory
    {
        [SimplNoWrap]
        [SimplCollection("student")]
        private List<Student> students;

        public StudentDirectory()
        {
            students = new List<Student>();
        }

        public StudentDirectory(List<Student> pStudents)
        {
            students = pStudents;
        }
    }
}
