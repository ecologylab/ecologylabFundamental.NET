using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Simpl.Serialization.Types
{
    /// <summary>
    /// A class that represents a given composite type.
    /// </summary>
    public class CompositeType : SimplType
    {
        /// <summary>
        /// Determines if a simpl composite Type can be created for the given C# TYpe
        /// </summary>
        /// <param name="aType">The type to consider</param>
        /// <returns>True if a collection type can be made</returns>
        public static bool CanBeCreatedFrom(Type aType)
        {
            return !CollectionType.CanBeCreatedFrom(aType); 
        }

        public CompositeType(Type theType) : base(theType.Name,theType,"java","objc","db")
        {
            this.Type = ClassDescriptor.GetClassDescriptor(theType);
            this.cSharpTypeName = theType.Name;
        }

        public ClassDescriptor Type
        {
            get;
            private set;
        }

        protected override string DeriveJavaTypeName()
        {
            return this.Type.JavaTypeName;
        }

        protected override string DeriveObjectiveCTypeName()
        {
            return this.Type.ObjectiveCTypeName;
        }

        protected override bool IsScalar
        {
            get { return false; }
        }

        

        private bool RecursiveSimplEquals(object left, object right, List<object> leftCompared, List<object> rightCompared)
        {
            leftCompared.Add(left);
            rightCompared.Add(right);

            if (left.GetType().Equals(right.GetType()))
            {
                foreach (var fieldDescriptor in this.Type.AllFieldDescriptors)
                {
                    var leftDescribedValue = fieldDescriptor.GetValue(left);    
                    var rightDescribedValue = fieldDescriptor.GetValue(right);

                    if (fieldDescriptor.IsComposite)
                    {
                     
                        if (object.ReferenceEquals(leftDescribedValue, left) || Object.ReferenceEquals(rightDescribedValue,right))
                        {
                            // Circular refernces are fine, skip them / move on.
                            continue;
                        }

                        if (leftCompared.Contains(leftDescribedValue) || rightCompared.Contains(rightDescribedValue))
                        {
                            // Skip cycles. 
                            continue;
                        }

                        var composite = new CompositeType(leftDescribedValue.GetType());
                        return composite.SimplEquals(leftDescribedValue, rightDescribedValue, leftCompared, rightCompared);
                    }
                    else
                    {
                        if (!fieldDescriptor.ContextSimplEquals(left,right))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool SimplEquals(object left, object right)
        {
            var leftList = new List<object>();
            var rightList = new List<object>();
            return RecursiveSimplEquals(left, right, leftList, rightList);
        }

        internal bool SimplEquals(object left, object right, List<Object> leftCompared, List<Object> rightCompared)
        {
            return this.RecursiveSimplEquals(left, right, leftCompared, rightCompared);
        }
    }
}
