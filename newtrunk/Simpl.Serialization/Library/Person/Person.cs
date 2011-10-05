using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Person
{
    public class Person
    {
        [SimplScalar] private string name;

        public Person()
        {
            
        }

        public Person(string pName)
        {
            name = pName;
        }
    }
}
