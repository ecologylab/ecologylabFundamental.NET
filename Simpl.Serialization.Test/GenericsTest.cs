using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simpl.Serialization;
using Simpl.Serialization.Library.Graph;
using Simpl.Serialzation.Tests.TestHelper;
using Simpl.Serialization.Library.Generics;
using System.Reflection;

namespace Simpl.Serialzation.Tests
{
        
    [TestClass]
    public class GenericsTest
    {

        [TestMethod]
        public void TestBasicGenerics()//ported from simplTranslators/test ecologylab.translators.java.generics.JavaTranslatorGenericTest.java
        {
            SimplTypesScope.EnableGraphSerialization();
  
            //My
            SimplTypesScope s = SimplTypesScope.Get("test_basic_generics", typeof(My<>));
		    
            FieldInfo f = typeof(My<>).GetField("v");
		    Type t1 = f.FieldType;
		    Console.Out.WriteLine(t1);
		    
            f = typeof(My<>).GetField("n");
		    Type t2 = f.FieldType;
		    Console.Out.WriteLine(t2);

            ClassDescriptor cd; 
            s.EntriesByClassSimpleName.TryGetValue("My", out cd);

		    FieldDescriptor fd = cd.GetFieldDescriptorByFieldName("v");

            //Search
		    SimplTypesScope scope = SimplTypesScope.Get("test-basic-generics", typeof(SearchResult), typeof(Search<>), typeof(SocialSearchResult), typeof(SocialSearch));
		
		    ClassDescriptor cdSearch;
            scope.EntriesByClassSimpleName.TryGetValue("Search", out cdSearch);

		    List<GenericTypeVar> classGenericTypeVars = cdSearch.GetGenericTypeVars();
		    foreach (GenericTypeVar genericTypeVar in classGenericTypeVars)
		    {
			    Console.Out.WriteLine(genericTypeVar.ToString());
		    }
		
		    FieldDescriptor fdSearchResults = cdSearch.GetFieldDescriptorByFieldName("searchResults");
            List<GenericTypeVar> fieldGenericTypeVars = fdSearchResults.GetGenericTypeVars();
		    foreach (GenericTypeVar genericTypeVar in fieldGenericTypeVars)
		    {
			    Console.Out.WriteLine(genericTypeVar.ToString());
		    }
		
		    scope.EntriesByClassSimpleName.TryGetValue("SocialSearch", out cdSearch);
		    classGenericTypeVars = cdSearch.GetGenericTypeVars();
		    foreach (GenericTypeVar genericTypeVar in classGenericTypeVars)
		    {
			    Console.Out.WriteLine(genericTypeVar.ToString());
		    }
		
		    fdSearchResults = cdSearch.GetFieldDescriptorByFieldName("searchResults");
		    fieldGenericTypeVars = fdSearchResults.GetGenericTypeVars();
		    foreach (GenericTypeVar genericTypeVar in fieldGenericTypeVars)
		    {
			    Console.Out.WriteLine(genericTypeVar.ToString());
		    }
		
            SimplTypesScope.DisableGraphSerialization();
        }

//	    [TestMethod]
        public void TestAdvGenerics1()//ported from simplTranslators/test ecologylab.translators.java.generics.JavaTranslatorGenericTest.java
	    {
            SimplTypesScope.EnableGraphSerialization();

		    SimplTypesScope scope = SimplTypesScope.Get("test-adv-generics-1", typeof(SearchResult), typeof(Search<>), typeof(SocialSearchResult), typeof(SocialSearch), typeof(TypedSocialSearch<>));

            SimplTypesScope.DisableGraphSerialization();
	    }

        [TestMethod]
        public void TestClassGenerics()//ported from simplTestCases tests.generics.TestSimplGenerics.java
	    {
		    // case 1: constraint is a concrete class
		    ClassDescriptor c = ClassDescriptor.GetClassDescriptor(typeof(Search<>));
		    List<GenericTypeVar> vars = c.GetGenericTypeVars();
		    Assert.AreEqual(vars.Count, 1);
		    GenericTypeVar var1 = vars[0];
		    Assert.AreEqual(var1.Name, "T");
		    Assert.AreSame(var1.ConstraintClassDescriptor, ClassDescriptor.GetClassDescriptor(typeof(SearchResult)));
		
		    // case 2: constraint is parameterized
		    c = ClassDescriptor.GetClassDescriptor(typeof(MediaSearch<,>));
		    vars = c.GetGenericTypeVars();
		    Assert.AreEqual(vars.Count, 2);
		    var1 = vars[0];
		    GenericTypeVar var2 = vars[1];
		    Assert.AreEqual(var2.Name, "T");
		    Assert.AreSame(var2.ConstraintClassDescriptor, ClassDescriptor.GetClassDescriptor(typeof(MediaSearchResult<>)));
		    List<GenericTypeVar> var2args = var2.ConstraintGenericTypeVarArgs;
		    Assert.AreEqual(var2args.Count, 1);
		    GenericTypeVar var2arg1 = var2args[0];
		    Assert.AreEqual(var2arg1.Name, "M");
		    Assert.AreSame(var2arg1.ReferredGenericTypeVar, var1);
		
		    // case 3: constraint is parameterized
		    c = ClassDescriptor.GetClassDescriptor(typeof(ImageSearch<,,>));
		    vars = c.GetGenericTypeVars();
		    Assert.AreEqual(vars.Count, 3);
		    var1 = vars[0];
		    var2 = vars[1];
		    GenericTypeVar var3 = vars[2];
		    Assert.AreEqual(var2.Name, "X");
		    Assert.AreSame(var2.ConstraintGenericTypeVar, var1);
            Assert.AreEqual(var3.Name, "T");
		    Assert.AreSame(var3.ConstraintClassDescriptor, ClassDescriptor.GetClassDescriptor(typeof(MediaSearchResult<>)));
		    List<GenericTypeVar> var3args = var3.ConstraintGenericTypeVarArgs;
		    Assert.AreEqual(var3args.Count, 1);
		    GenericTypeVar var3arg1 = var3args[0];
		    Assert.AreEqual(var3arg1.Name, "X");
		    Assert.AreSame(var3arg1.ReferredGenericTypeVar, var2);
	    }

	    [TestMethod]
        public void TestSuperClassGenerics()//ported from simplTestCases tests.generics.TestSimplGenerics.java
	    {
		    // public class FlickrSearchResult extends MediaSearchResult<Image>
		    ClassDescriptor c = ClassDescriptor.GetClassDescriptor(typeof(FlickrSearchResult));
            List<GenericTypeVar> scvars = c.GetSuperClassGenericTypeVars();
		    Assert.AreEqual(scvars.Count, 1);
		    GenericTypeVar var1 = scvars[0];
		    Assert.AreSame(var1.ClassDescriptor, ClassDescriptor.GetClassDescriptor(typeof(Image)));
		
		    // public class MediaSearch<M extends Media, T extends MediaSearchResult<M>> extends Search<T>
		    c = ClassDescriptor.GetClassDescriptor(typeof(MediaSearch<,>));
		    List<GenericTypeVar> vars = c.GetGenericTypeVars();
		    scvars = c.GetSuperClassGenericTypeVars();
		    Assert.AreEqual(scvars.Count, 1);
		    var1 = scvars[0];
		    Assert.AreEqual(var1.Name, "T");
		    Assert.AreSame(var1.ReferredGenericTypeVar, vars[1]);
		
		    // public class ImageSearch<I extends Image, X extends I, T extends MediaSearchResult<X>> extends MediaSearch<X, T>
		    c = ClassDescriptor.GetClassDescriptor(typeof(ImageSearch<,,>));
		    vars = c.GetGenericTypeVars();
		    scvars = c.GetSuperClassGenericTypeVars();
		    Assert.AreEqual(scvars.Count, 2);
		    var1 = scvars[0];
		    Assert.AreEqual(var1.Name, "X");
		    Assert.AreSame(var1.ReferredGenericTypeVar, vars[1]);
		    GenericTypeVar var2 = scvars[1];
            Assert.AreSame(var2.ClassDescriptor, ClassDescriptor.GetClassDescriptor(typeof(MediaSearchResult<>)));
            Assert.AreEqual(var2.GenericTypeVarArgs.Count, 1);
            Assert.AreEqual(var2.GenericTypeVarArgs[0].Name, "X");
            Assert.AreSame(var2.GenericTypeVarArgs[0].ReferredGenericTypeVar, vars[1]);
	    }

	    [TestMethod]
        public void TestGenericField()//ported from simplTestCases tests.generics.TestSimplGenerics.java
	    {
		    // Search.results
		    ClassDescriptor c = ClassDescriptor.GetClassDescriptor(typeof(Search<>));
		    List<GenericTypeVar> cvars = c.GetGenericTypeVars();
            FieldDescriptor f = c.GetFieldDescriptorByFieldName("searchResults");
		    List<GenericTypeVar> vars = f.GetGenericTypeVars();
		    GenericTypeVar var1 = vars[0];
		    Assert.AreEqual(var1.Name, "T");
		    Assert.AreSame(var1.ReferredGenericTypeVar, cvars[0]);
		
		    // MediaSearch.results
		    c = ClassDescriptor.GetClassDescriptor(typeof(MediaSearch<,>));
		    cvars = c.GetGenericTypeVars();
            f = c.GetFieldDescriptorByFieldName("searchResults");
		    vars = f.GetGenericTypeVars();
		    var1 = vars[0];
		    Assert.AreEqual(var1.Name, "T");
		    Assert.AreSame(var1.ReferredGenericTypeVar, cvars[1]);
		
		    // MediaSearch.firstResult
		    f = c.GetFieldDescriptorByFieldName("firstResult");
		    vars = f.GetGenericTypeVars();
		    var1 = vars[0];
		    Assert.AreSame(var1.ClassDescriptor, ClassDescriptor.GetClassDescriptor(typeof(Media)));
		
		    // MediaSearchResult.media
		    c = ClassDescriptor.GetClassDescriptor(typeof(MediaSearchResult<>));
		    cvars = c.GetGenericTypeVars();
		    f = c.GetFieldDescriptorByFieldName("media");
		    vars = f.GetGenericTypeVars();
		    var1 = vars[0];
		    Assert.AreEqual(var1.Name, "M");
		    Assert.AreSame(var1.ReferredGenericTypeVar, cvars[0]);
		
		    // MeidaSearchResult.ms
		    f = c.GetFieldDescriptorByFieldName("ms");
		    vars = f.GetGenericTypeVars();
		    var1 = vars[0];
		    Assert.AreEqual(var1.Name, "M");
		    Assert.AreSame(var1.ReferredGenericTypeVar, cvars[0]);
		    GenericTypeVar var2 = vars[1];
		    Assert.AreSame(var2.ClassDescriptor, ClassDescriptor.GetClassDescriptor(typeof(MediaSearchResult<>)));
		    List<GenericTypeVar> var2args = var2.GenericTypeVarArgs;
		    Assert.AreEqual(var2args.Count, 1);
		    GenericTypeVar var2arg1 = var2args[0]; 
		    Assert.AreEqual(var2arg1.Name, "M");
		    Assert.AreSame(var2arg1.ReferredGenericTypeVar, cvars[0]);
	    }

    }
}
