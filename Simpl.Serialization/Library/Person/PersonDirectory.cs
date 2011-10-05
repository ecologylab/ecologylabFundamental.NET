using System.Collections.Generic;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Person
{
    public class PersonDirectory
    {
        [SimplClasses(new[] {typeof (Person), typeof (Student), typeof (Faculty)})]
        [SimplCollection]
        private List<Person> persons;

        public PersonDirectory()
        {
            persons = new List<Person>();
        }

        public PersonDirectory(List<Person> pPersons)
        {
            persons = pPersons;
        }

    }
}
