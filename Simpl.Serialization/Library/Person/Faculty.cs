using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Person
{
    public class Faculty : Person
    {
        [SimplScalar] private string designation;

        public Faculty()
        {
            
        }

        public Faculty(string pName, string pDesignation)
            : base(pName)
        {
            designation = pDesignation;
        }
    }
}
