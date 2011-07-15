using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using ecologylabFundamental.ecologylab.serialization;
using System.Xml;
using System.IO;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    public class SerialisationTest
    {
        public static void Run()
        {
            ClassA classA = new ClassA(1, 2);
            classA.ClassAProp = classA;

            ClassB classB = new ClassB(4, 5, classA);
            classA.ClassBProp = classB;

            TranslationScope.graphSwitch = TranslationScope.GRAPH_SWITCH.ON;
            StringBuilder builder = new StringBuilder();
            classB.serialize(builder, Format.XML);
            Console.Write(builder);

            TextWriter tw = new StreamWriter("C:\\tmp\\graph_serialisation.xml");
            tw.WriteLine(builder);
            tw.Close();

            Console.Read();
        }
    }
}
