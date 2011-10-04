using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Configuration;
using Simpl.Serialzation.Tests.TestHelper;

namespace Simpl.Serialzation.Tests
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void ConfigurationXml()
        {
            PrefInteger prefInteger = new PrefInteger("integer_pref", 2);
            PrefDouble prefDouble = new PrefDouble("integer_pref", 4);

            Pref pref = new Pref("only_pref");

            List<Pref> prefList = new List<Pref> {pref, prefInteger, prefDouble};

            Configuration c = new Configuration(prefInteger, prefList);
            TranslationScope translationScope = TranslationScope.Get("configuration", typeof(Configuration),
				typeof(PrefInteger), typeof(PrefDouble), typeof(Pref));

            TestMethods.TestSimplObject(c, translationScope, StringFormat.Xml);
        }
    }
}
