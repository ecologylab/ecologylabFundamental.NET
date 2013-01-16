using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Person;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class PersonDirectoryTest
    {
        [TestMethod]
        public void PersonDirectoryTestXml()
        {
            List<Person> persons = new List<Person>
                                       {
                                           new Student("nabeel", "234342"),
                                           new Student("yin", "3423423"),
                                           new Faculty("andruid", "professor"),
                                           new Student("bill", "4234"),
                                           new Student("sashi", "545454"),
                                           new Student("jon", "53453453")
                                       };
            PersonDirectory p = new PersonDirectory(persons);

            SimplTypesScope simplTypesScope = SimplTypesScope.Get("personDir", typeof(Person),
				typeof(Faculty), typeof(Student), typeof(PersonDirectory));

             TestMethods.TestSimplObject(p, simplTypesScope, Format.Xml);
        }


        [TestMethod]
        public void PersonDirectoryTestJson()
        {
            List<Person> persons = new List<Person>
                                       {
                                           new Student("nabeel", "234342"),
                                           new Student("yin", "3423423"),
                                           new Faculty("andruid", "professor"),
                                           new Student("bill", "4234"),
                                           new Student("sashi", "545454"),
                                           new Student("jon", "53453453")
                                       };
            PersonDirectory p = new PersonDirectory(persons);

            SimplTypesScope simplTypesScope = SimplTypesScope.Get("personDir", typeof(Person),
                typeof(Faculty), typeof(Student), typeof(PersonDirectory));

            TestMethods.TestSimplObject(p, simplTypesScope, Format.Json);
        }
    }
}
