using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization;
using System.Reflection;
using ecologylabFundamental.ecologylab.serialization;

namespace ecologylabFundamentalTestCases.ObjectGraph
{
    public class ObjectGraphTest
    {
	    public static bool	passed_;
	    public static String	test_;

	    public static void start(String t)
	    {
		    passed_ = true;
		    test_ = t;
		    Console.WriteLine("Beginning test: " + test_);
	    }

	    public static void end()
	    {
		    Console.WriteLine("Test: " + test_);
		    if (passed_)
		    {
			    Console.WriteLine(" passed.");
		    }
		    else
		    {
			    Console.WriteLine(" failed.");
		    }
	    }

	    /**
	     * Fails the test with an error message.
	     * 
	     * @param msg
	     */
	    public static void fail(String msg)
	    {
		    Console.WriteLine(test_ + " failed: " + msg);
		    passed_ = false;
	    }

	    public static void RunTests()
	    {
		    TranslationScope.graphSwitch = TranslationScope.GRAPH_SWITCH.ON;
                        
		    foreach (MethodInfo m in typeof(ObjectGraphTest).GetMethods())
		    {
			    if (m.Name.StartsWith("Test"))
			    {
				    start(m.Name);
				    try
				    {
                        m.Invoke(new ObjectGraphTest(),null);                       
				    }
				    catch (Exception e)
				    {
					    fail("could not run test.");
                        Console.WriteLine(e.StackTrace);					    
				    }				    
				    finally
				    {
					    end();
				    }
			    }
		    }
            Console.Read();
	    }

	    /**
	     * Tests the new @simpl_use_equals_equals annotation. Objects are only equal if they actually
	     * point to the same instance.
	     */
	    public void TestEqualsEquals()
	    {
		    ListEqEq list = new ListEqEq();
		    list.points.Add(new PointEqEq(4, 5));
		    list.points.Add(new PointEqEq(5, 4)); // same hash
		    list.points.Add(new PointEqEq(1, 2)); // totally different
		    list.points.Add(new PointEqEq(4, 5)); // same hash and .equals
		    list.points.Add(list.points[0]); // same reference
		    try
		    {
                StringBuilder output = new StringBuilder();
                list.serialize(output,Format.XML);

                TranslationScope ts = new TranslationScope("testEqualsEquals", typeof(ListEqEq),
                        typeof(PointEqEq));
                ListEqEq deserialized = (ListEqEq) ts.deserializeString(output.ToString(),new TranslationContext(),Format.XML);
			    PointEqEq first = deserialized.points[0];
			    PointEqEq last = deserialized.points[deserialized.points.Count - 1];
			    if (first != last)
			    {
				    fail("first--last reference was not maintained.");
			    }
			    for (int i = 1; i < deserialized.points.Count - 1; ++i)
			    {
				    if (first == deserialized.points[i])
					    fail("extra reference was created between items 0 and " + i);
			    }
		    }
		    catch (Exception e)
		    {
			    fail("exception.");
                Console.WriteLine(e.StackTrace);			    
		    }		
	    }

	    /**
	     * Tests the default behavior of object graph serialization. Objects are equal if they satisfy
	     * .equals().
	     */
	    public void TestDotEquals()
	    {
		    ListDotEquals list = new ListDotEquals();
		    list.points.Add(new PointDotEquals(4, 5));
		    list.points.Add(new PointDotEquals(5, 4)); // same hash
		    list.points.Add(new PointDotEquals(1, 2)); // totally different
		    list.points.Add(new PointDotEquals(4, 5)); // same hash and .equals
		    list.points.Add(new PointDotEquals(4, 5)); // same hash and .equals
		    list.points.Add(list.points[0]); // same reference
		    try
		    {
                StringBuilder output = new StringBuilder();
                list.serialize(output,Format.XML);

			    ListDotEquals deserialized = (ListDotEquals) TranslationScope.Get("ListDotEquals",
					    typeof(ListDotEquals), typeof(PointDotEquals)).deserializeString(output.ToString(),new TranslationContext(),Format.XML);
			    PointDotEquals first = deserialized.points[0];
			    PointDotEquals secondToLast = deserialized.points[deserialized.points.Count - 2];
			    PointDotEquals last = deserialized.points[deserialized.points.Count - 1];
			    if (first != last)
			    {
				    fail("first--last reference was not maintained.");
			    }
			    if (first != secondToLast)
			    {
				    fail("first--secondToLast reference was not maintained.");
			    }
			    if (secondToLast != last)
			    {
				    fail("secondToLast--last reference was not maintained.");
			    }
			    for (int i = 1; i < deserialized.points.Count - 3; ++i)
			    {
				    if (first == deserialized.points[i])
					    fail("extra reference was created between items 0 and " + i);
			    }
		    }
		    catch (Exception e)
		    {
			    fail("exception.");
                Console.WriteLine(e.StackTrace);			    
		    }
	    }
    }
}
