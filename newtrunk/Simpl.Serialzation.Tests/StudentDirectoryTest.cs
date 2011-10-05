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
    public class StudentDirectoryTest
    {
        [TestMethod]
        public void StudentDirectoryTestXml()
        {
            List<Student> students = new List<Student>
                                       {
                                           new Student("nabeel", "234342"),
                                           new Student("yin", "3423423"),
                                           new Student("bill", "4234"),
                                           new Student("sashi", "545454"),
                                           new Student("jon", "53453453")
                                       };
            StudentDirectory p = new StudentDirectory(students);

            TranslationScope translationScope = TranslationScope.Get("studentDir", typeof(Person),
                                                                     typeof(Student), typeof(StudentDirectory));

            TestMethods.TestSimplObject(p, translationScope, StringFormat.Xml);
        }


        [TestMethod]
        public void StudentDirectoryTestJson()
        {
            List<Student> students = new List<Student>
                                         {
                                             new Student("nabeel", "234342"),
                                             new Student("yin", "3423423"),
                                             new Student("bill", "4234"),
                                             new Student("sashi", "545454"),
                                             new Student("jon", "53453453")
                                         };
            StudentDirectory p = new StudentDirectory(students);

            TranslationScope translationScope = TranslationScope.Get("studentDir", typeof (Person),
                                                                     typeof (Student), typeof (StudentDirectory));

            TestMethods.TestSimplObject(p, translationScope, StringFormat.Json);
        }
    }
}
