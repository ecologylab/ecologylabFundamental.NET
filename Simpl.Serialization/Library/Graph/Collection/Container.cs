using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.Library.Graph.Collection
{
    public class Container
    {
        [SimplCollection("objectsA")]
        private List<ClassA> aObjects = new List<ClassA>();

        public Container()
        {

        }

        public Container InitializeInstance()
        {
            ClassA objA = new ClassA();

            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);
            aObjects.Add(objA);

            aObjects.Add(new ClassA());
            aObjects.Add(new ClassA());

            return this;
        }
    }
}
