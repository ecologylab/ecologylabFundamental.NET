using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Diagnostics.Contracts;

namespace Simpl.Fundamental.Generic
{
    /// <summary>
    /// A static utility class containing tools that make reflection tasks easier.
    /// </summary>
    public static class ReflectionTools
    {
        /// <summary>
        /// Get the parameterized type tokens that the generic Field was declared with.        
        /// </summary>
        /// <param name="field">Field infor to obtain parametized types for.</param>
        /// <returns> A type array of parametized types.</returns>
        public static Type[] GetParameterizedTypeTokens(FieldInfo field)
	    {
	        return field.FieldType.GetGenericArguments();
	    }

        /// <summary>
        /// Find a method object if there is one in the context class, or return null if not.
        /// </summary>
        /// <param name="context"> Class to find the method in</param>
        /// <param name="name"> Name of the method to find</param>
        /// <param name="types">Array of Type objects indicating desired parameter types</param>
        /// <returns> the associated method object, or null if none is accessible or matches.</returns>
  	    public static MethodInfo GetMethod(Type context, string name, Type[] types)
  	    {
            // TODO: CONSIDER THROWING EXCEPTIONS HERE.
  		    MethodInfo result	= null;

  		    try
		    {
			    result = context.GetMethod(name, types);
		    }
            catch (AmbiguousMatchException)
		    {
		        // Swallow this exception
            }
            catch (ArgumentNullException)
		    {
                // Swallow this exception
		    }
  		    
            return result;
  	    }

        ///<summary>
        ///Get the Field object with name fieldName, in thatClass.
        ///</summary>
        ///<param name="thatClass"> Context class to find fieldName in. </param>
        ///<returns>The field in that class, unless it is unaccessible, then returns null.</returns>
        public static FieldInfo GetField(Type thatClass, string fieldName)
        {
            FieldInfo result = null;
            
            try
            {
                result = thatClass.GetField(fieldName);
            } 
            catch (ArgumentNullException)
            {
                // Swallow this exception
            } 
            catch (NotSupportedException)
            {
                // Swallow this exception
            }	

            return result;
        }

        /// <summary>
        /// A new object, which represents "BAD ACCESS" 
        /// TODO: Replace with exception
        /// </summary>
        public static readonly Object	BadAccess	= new object();
   
        /// <summary>
        /// Return the value of the Field in the Object, or BAD_ACCESS if it can't be accessed.
        /// </summary>
        /// <param name="contextObject"> Context object to obtain value in</param>
        /// <param name="field">Field to obtain value of</param>
        public static object GetFieldValue(object contextObject, FieldInfo field)
        {
            object result = null;
            
            try
            {
	            result = field.GetValue(contextObject);
            } 
            catch (Exception e)
            {
	            result = BadAccess;
	            Console.Error.WriteLine(e.Message);
            }
            
            return result;
        }
   
        ///<summary>
        /// Set a reference type Field to a value.
        ///</summary.
        ///<param name="that">Context object to set field value within</param>
        ///<param name="field">Field info to set value within</param>
        ///<param name="value">Value to set</param>
        ///<returns>true if the set succeeds.</returns>
        public static bool SetFieldValue(object that, FieldInfo field, object value)
        {
	        bool result	= false;
	        
            try
	        {
		        field.SetValue(that, value);
		        result = true;
	        }
            catch (Exception e)
	        {
		       Console.Error.WriteLine(e.Message);
	        }

	        return result;
        }

        ///<summary>
        /// Wraps the no argument getInstance() method.
         /// Checks to see if the class object passed in is null, or if
         /// any exceptions are thrown by newInstance().
         /// </summary>
         ///<param name="thatClass"> Class to obtain new instance of... ... ... </param>
         ///<typeparam name="T"> Type of the class... </typeparam>
         ///<returns>An instance of an object of the specified class, or null if the Class object was null or
        /// an InstantiationException or IllegalAccessException was thrown in the attempt to instantiate.
        /// </returns>
  	    public static T GetInstance<T>(Type thatClass, params object[] args) where T : class
  	    {
            if(typeof(T).IsAssignableFrom(thatClass))
            {
                try
                {
                    if (args.Length == 0)
                    {
                        return (T)Activator.CreateInstance(thatClass, args);
                    }
                    else
                    {
                        return Activator.CreateInstance<T>();
                    }
                }
                catch (Exception e)
                {
                    throw new ArgumentException(String.Format("Type given ({0}) cannot be created as ({1}) with given args.", thatClass.Name, typeof(T).Name), e);
                }
            }
            else
            {
                throw new ArgumentException(String.Format("The given type ({0}) is not assignable to T ({1})", thatClass.Name, typeof(T).Name));
            }
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
