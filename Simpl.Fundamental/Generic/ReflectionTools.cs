using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;

namespace Simpl.Fundamental.Generic
{
    public static class ReflectionTools
    {
        /**
	     * Get the parameterized type tokens that the generic Field was declared with.
	     * 
	     * @param reflectType
	     * @return
	     */
	    public static Type[] GetParameterizedTypeTokens(FieldInfo field)
	    {
	        return field.FieldType.GetGenericArguments();
	    }

        /**
  	     * Find a Method object if there is one in the context class, or return null if not.
  	     * 
  	     * @param context	Class to find the Method in.
  	     * @param name		Name of the method.
  	     * @param types		Array of Class objects indicating parameter types.
  	     * 
  	     * @return			The associated Method object, or null if non is accessible.	
  	     */
  	    public static MethodInfo GetMethod(Type context, String name, Type[] types)
  	    {
  		    MethodInfo result	= null;
  		    try
		    {
			    result		= context.GetMethod(name, types);
		    } catch (AmbiguousMatchException e)
		    {
		    } catch (ArgumentNullException e)
		    {
		    }
  		    return result;
  	    }

        /**
         * Get the Field object with name fieldName, in thatClass.
         * 
         * @param thatClass
         * @param fieldName
         * 
         * @return	The Field object in thatClass, or null if there is none accessible.
         */
        public static FieldInfo GetField(Type thatClass, String fieldName)
        {
            FieldInfo	result	= null;
            try
            {
                result		= thatClass.GetField(fieldName);
            } catch (ArgumentNullException e)
            {
            } catch (NotSupportedException e)
            {
            }	
            return result;
        }

        /**
         * Get the Field object with name fieldName, in thatClass.
         * 
         * @param thatClass
         * @param fieldName
         * 
         * @return	The Field object in thatClass, or null if there is none accessible.
         */
        public static FieldInfo GetDeclaredField(Type thatClass, String fieldName)
        {
   	       FieldInfo	result	= null;
   	       try
   	       {
   		       result		= thatClass.GetField(fieldName);
   	       } catch (ArgumentNullException e)
   	       {
   	       } catch (NotSupportedException e)
   	       {
   	       }	
   	       return result;
        }

        public static readonly Object	BadAccess	= new Object();
   
        /**
        * Return the value of the Field in the Object, or BAD_ACCESS if it can't be accessed.
        * 
        * @param that
        * @param field
        * @return
        */
        public static Object GetFieldValue(Object that, FieldInfo field)
        {
            Object result	= null;
            try
            {
	            result		= field.GetValue(that);
            } catch (Exception e)
            {
	            result		= BadAccess;
	            Console.Error.WriteLine(e.Message);
            }
            
            return result;
        }
   
       /**
        * Set a reference type Field to a value.
        * 
        * @param that		Object that the field is in.
        * @param field		Reference type field within that object.
        * @param value		Value to set the reference field to.
        * 
        * @return			true if the set succeeds.
        */
        public static bool SetFieldValue(Object that, FieldInfo field, Object value)
        {
	        bool result	= false;
	        try
	        {
		        field.SetValue(that, value);
		        result		= true;
	        } catch (Exception e)
	        {
		       Console.Error.WriteLine(e.Message);
	        }

	        return result;
        }

        /**
         * Wraps the no argument getInstance() method.
         * Checks to see if the class object passed in is null, or if
         * any exceptions are thrown by newInstance().
         * 
         * @param thatClass
         * @return	An instance of an object of the specified class, or null if the Class object was null or
         * an InstantiationException or IllegalAccessException was thrown in the attempt to instantiate.
         */
  	    public static T GetInstance<T>(T thatClass) where T : class
  	    {
  		    T result		= default(T);
  		    if (thatClass != null)
  		    {
  			    try
  			    {
  				    result        	= Activator.CreateInstance<T>();
  			    } catch (MissingMethodException e)
  			    {
  				    Console.Error.WriteLine(e.Message);
  			    }
  		    }
  		    return result;
  	    }

  	    /**
  	     * Wraps the no argument getInstance() method.
  	     * Checks to see if the class object passed in is null, or if
  	     * any exceptions are thrown by newInstance().
  	     * 
  	     * @param thatClass
  	     * @return	An instance of an object of the specified class, or null if the Class object was null or
  	     * an InstantiationException or IllegalAccessException was thrown in the attempt to instantiate.
  	     */
  	    public static T GetInstance<T>(T thatClass, Type[] parameterTypes, Object[] args) where T : class
  	    {
  		    T result				= default(T);
  		    if (thatClass != null)
  		    {
  			    try
  			    {
  			        result		 		= (T) Activator.CreateInstance(thatClass.GetType(), parameterTypes);
  			    }
  			    catch (MissingMethodException e)
  			    {
  				    Console.Error.WriteLine(e.Message);
  			    } 
  		    }
  		    return result;
  	    }

        /**
  	     * Find a PropertyInfo object if there is one in the context class, or return null if not.
  	     * 
  	     * @param context	Class to find the Property in.
  	     * @param name		Name of the method.
  	     * @return			The associated PropertyInfo object, or null if non is accessible.	
  	     */
        public static PropertyInfo GetProperty(Type context, String name)
        {
            PropertyInfo result = null;
            try
            {
                result = context.GetProperty(name);
            }
            catch (AmbiguousMatchException e)
            {
            }
            catch (ArgumentNullException e)
            {
            }
            return result;
        }
    }
}
