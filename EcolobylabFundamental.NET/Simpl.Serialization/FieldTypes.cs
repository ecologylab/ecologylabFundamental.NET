namespace Simpl.Serialization
{
    /// <summary>
    ///     
    /// </summary>
    public class FieldTypes
    {
        /// <summary>
        /// 
        /// </summary>
        public const int UnsetType = -999;

        /// <summary>
        /// 
        /// </summary>
        public const int BadField = -99;

        /// <summary>
        /// 
        /// </summary>
        public const int IgnoredAttribute = -1;

        /// <summary>
        /// 
        /// </summary>
        public const int Scalar = 0x12;

        /// <summary>
        /// 
        /// </summary>
        public const int CompositeElement = 3;

        /// <summary>
        /// 
        /// </summary>
        public const int IgnoredElement = -3;

        /// <summary>
        /// 
        /// </summary>
        public const int CollectionElement = 4;

        /// <summary>
        /// 
        /// </summary>
        public const int CollectionScalar = 5;

        /// <summary>
        /// 
        /// </summary>
        public const int MapElement = 6;

        /// <summary>
        /// 
        /// </summary>
        public const int MapScalar = 7;
        
        /// <summary>
        /// 
        /// </summary>
        public const int Wrapper = 0x0a;

        /// <summary>
        /// 
        /// </summary>
        public const int PseudoFieldDescriptor = 0x0d;

        /// <summary>
        /// 
        /// </summary>
        public const int XmlnsAttribute = 0x0e;

        /// <summary>
        /// 
        /// </summary>
        public const int XmlnsIgnored = 0x0f;

        /// <summary>
        /// 
        /// </summary>
        public const int NameSpaceMask = 0x10;

        /// <summary>
        /// 
        /// </summary>
        public const int NamespaceTrialElement = NameSpaceMask;

        /// <summary>
        /// 
        /// </summary>        
        public const int NameSpaceAttribute = NameSpaceMask + Scalar;

        /// <summary>
        /// 
        /// </summary>
        public const int NameSpaceNestedElement = NameSpaceMask + CompositeElement;

        /// <summary>
        /// 
        /// </summary>
        public const int NameSpaceLeafNode = NameSpaceMask + Scalar;
    }
}