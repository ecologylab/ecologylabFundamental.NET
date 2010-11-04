//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using ecologylab.serialization.json;

//namespace ecologylab.serialization
//{
//    public class ElementStateJSONHandler : FieldTypes, IJSONContentHandler
//    {

//        public ElementStateJSONHandler()
//        {
//        }

//        public void parse(String jsonString)
//        {
//            JSONParser parser = new JSONParser(this);
//            parser.parse(jsonString);
//        }

//        public void StartJSON()
//        {
//            Console.WriteLine("start json");
//        }

//        public void StartObject()
//        {
//            Console.WriteLine("start object");
//        }

//        public void StartObjectEntry(string key)
//        {
//            Console.WriteLine(key);
//        }

//        public void StartArray()
//        {
//            Console.WriteLine("start array");
//        }

//        public void Primitive(object value)
//        {
//            Console.WriteLine(value.ToString());
//        }

//        public void EndJSON()
//        {
//            Console.WriteLine("end json");
//        }

//        public void EndObject()
//        {
//            Console.WriteLine("end object");
//        }

//        public void EndObjectEntry()
//        {
//            Console.WriteLine("end object entry");
//        }

//        public void EndArray()
//        {
//            Console.WriteLine("end array");
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization.sax;
using System.IO;
using System.Collections;
using ecologylab.serialization.json;
using ecologylab.serialization.types.element;

namespace ecologylab.serialization
{
    /// <summary>
    ///     <c>ElementStateSAXHandler</c> is a SAX Parser to parse XML files
    ///     and unmarshall it into C# rumtime objects. It takes <see cref="TranslationScope"/>
    ///     which is an abstract representation of the translation semantics. TranslationScopes 
    ///     defines which classes bind to which tags in the XML representation.
    /// </summary>
    public class ElementStateJSONHandler : FieldTypes, IJSONContentHandler, IJSONErrorHandler
    {
        #region Private Fields

        /// <summary>
        ///     A SAX Parser object for parsing XML files.
        /// </summary>
        private JSONParser parser;

        /// <summary>
        ///     A files stream object to read XML files.
        /// </summary>
        private FileStream inputFileStream;

        /// <summary>
        ///     Used the unmarshalling context. The root which holds the final
        ///     unmarshalled object. 
        /// </summary>
        private ElementState root;

        /// <summary>
        ///     Used in umarshalling context. The current object being umarshalled
        ///     by the ElementStateSAXHandler. 
        /// </summary>
        private ElementState currentElementState;

        /// <summary>
        ///     Translation scope which holds the class descriptors and optimized 
        ///     data structures for un-marshalling. 
        /// </summary>
        private TranslationScope translationScope;

        /// <summary>
        ///     Used in un-marhshalling context. The field descriptor of the current
        ///     <c>ElementState</c> object. 
        /// </summary>
        private FieldDescriptor currentFieldDescriptor;

        /// <summary>
        ///     A simple stack representation of the field descriptors to mantain
        ///     the states in unmarshalling.
        /// </summary>
        private List<FieldDescriptor> fdStack = new List<FieldDescriptor>();

        /// <summary>
        ///     Holds the characters of, it is mostly the leaf node value.
        /// </summary>
        private StringBuilder currentTextValue = new StringBuilder(1024);
        
        /// <summary>
        ///     some way of dealing with json arrays. 
        ///     should use a stack for nested collection?
        /// </summary>
        private  int numOfCollectionElementsOld = 0;

        private List<Int32> numOfCollectionElements = new List<Int32>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Basic constructor for <c>ElementStateSAXHandler</c>. Only initializes a 
        ///     SAX Parser. It should be used only with the following public Parse method 
        ///     <para>
        ///         <c>public ElementState Parse(FileStream inputFileStream, TranslationScope translationScope)</c>
        ///     </para>
        /// </summary>
        public ElementStateJSONHandler()
        {
            parser = new JSONParser();
            parser.setContentHandler(this);
            parser.setErrorHandler(this);
        }

        /// <summary>
        ///     Constructor which initializes the SAX Parser. Opens an input stream 
        ///     on a file to start reading. Needs to map XML data to runtime representation
        /// </summary>
        /// <param name="url">
        ///     location of the file to parse
        /// </param>
        /// <param name="translationScope">
        ///     translation scope which binds the run-time objects.
        /// </param>
        public ElementStateJSONHandler(String url, TranslationScope translationScope)
            : this()
        {
            this.inputFileStream = File.OpenRead(url);
            this.translationScope = translationScope;
        }

        /// <summary>
        ///     Constructor which initializes the SAX Parser. Opens an input stream 
        ///     on a file to start reading. Needs to map XML data to runtime representation
        /// </summary>
        /// <param name="inputFileStream">
        ///     Stream of the input file to read XML file.
        /// </param>
        /// <param name="translationScope">
        ///     translation scope which binds the run-time objects.
        /// </param>
        public ElementStateJSONHandler(FileStream inputFileStream, TranslationScope translationScope)
            : this()
        {
            this.inputFileStream = inputFileStream;
            this.translationScope = translationScope;
        }

        #endregion

        #region Parse Methods

        /// <summary>
        ///     Parses the XML file and returns the unmarshalled ElementState object.
        /// </summary>
        /// <param name="inputFileStream">
        ///     Stream of the input file to read XML file.
        /// </param>
        /// <param name="translationScope">
        ///     Translation scope which binds the run-time objects.
        /// </param>
        /// <returns>
        ///     <c>ElementState</c> unmarshalled object from input stream
        /// </returns>
        public ElementState Parse(FileStream inputFileStream, TranslationScope translationScope)
        {
            this.inputFileStream = inputFileStream;
            this.translationScope = translationScope;
            return Parse();
        }

        /// <summary>
        ///      Parses the XML file and returns the unmarshalled ElementState object.
        /// </summary>
        /// <returns>
        ///     <c>ElementState</c> unmarshalled object from input stream
        /// </returns>
        public ElementState Parse()
        {
            if (inputFileStream != null)
            {
                parser.parse(inputFileStream);
                return root;
            }
            else
            {
                throw new Exception("input file stream is null");
            }
        }

        #endregion         

        #region Private Methods

        /// <summary>
        ///     Pushes a field descriptor into the stach to mantain the translation
        ///     state machine.
        /// </summary>
        /// <param name="fieldDescriptor">
        ///     <c>FieldDescriptor</c> to be pushed into the stack.
        /// </param>
        private void PushFieldDescriptor(FieldDescriptor fieldDescriptor)
        {
            this.fdStack.Add(fieldDescriptor);
        }

        /// <summary>
        ///     Processes the scalar values. They are essentially the value of 
        ///     the leaf nodes. This method only process the cases of leaf and
        ///     collection scalar.
        /// </summary>
        /// <param name="currentType">
        ///     Type id of field type.
        /// </param>
        /// <param name="currentElementState">
        ///     The current <c>ElementState</c> object. 
        /// </param>
        private void ProcessPendingTextScalar(int currentType, ElementState currentElementState)
        {
            int length = currentTextValue.Length;
            String value = null;

            if (length > 0)
            {
                switch (currentType)
                {
                    case SCALAR:
                        value = currentTextValue.ToString().Substring(0, length);
                        currentFieldDescriptor.SetFieldToScalar(currentElementState, value);
                        break;
                    case COLLECTION_SCALAR:
                        value = currentTextValue.ToString().Substring(0, length);
                        currentFieldDescriptor.AddLeafNodeToCollection(currentElementState, value);
                        break;
                    default:
                        break;
                }

                currentTextValue.Length = 0;
            }
        }

        /// <summary>
        ///     Set the root of the translation tree to supplie <c>ElementState</c>
        /// </summary>
        /// <param name="pRoot">
        ///     <c>ElementState</c> which becomes the root.
        /// </param>
        private void SetRoot(ElementState pRoot)
        {
            this.root = pRoot;
            this.currentElementState = pRoot;
        }

        /// <summary>
        ///     Pop a <c>FieldDescriptor</c> which becomes the current ElementState.
        /// </summary>
        private void PopAndPeekFieldDescriptor()
        {
            int last = fdStack.Count - 1;
            if (last >= 0)
            {
                FieldDescriptor result = fdStack[last];
                fdStack.RemoveAt(last--);

                if (last >= 0)
                    result = fdStack[last];
                this.currentFieldDescriptor = result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        public void pushNumber(Int32 num)
        {
            this.numOfCollectionElements.Add(num);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int32 popNumber()
        {
            int num = 0;
            if (numOfCollectionElements.Count > 0)
            {
                num = this.numOfCollectionElements[numOfCollectionElements.Count - 1];
                this.numOfCollectionElements.RemoveAt(numOfCollectionElements.Count - 1);
            }

            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Int32 topNumber()
        {
            int num = 0;
            if (numOfCollectionElements.Count > 0)
            {
                num = this.numOfCollectionElements[numOfCollectionElements.Count - 1];
            }

            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        public void incrementTop()
        {
            int num = popNumber();
            pushNumber(++num);
        }


        #endregion

        #region Properties

        /// <summary>
        ///     Get the class descriptor of the current <c>ElementState</c>.
        /// </summary>
        public ClassDescriptor CurrentClassDescriptor
        {
            get
            {
                return this.currentElementState.ElementClassDescriptor;
            }
        }

        #endregion

        #region Error Handling Events (Raised during Parsing of JSON File)

        public void warning(JSONParseException exception)
        {
            Console.WriteLine(exception.Message);
        }

        public void error(JSONParseException exception)
        {
            Console.WriteLine(exception.Message);
        }

        public void fatalError(JSONParseException exception)
        {
            Console.WriteLine(exception.Message);
        }

        #endregion
        
        #region JSON Parser Events (used for parsing by JSONHandler)

        /// <summary>
        /// 
        /// </summary>
        public void StartJSON()
        {
            // do nothing
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartObject()
        {
           // same hack as in java
            if (currentFieldDescriptor != null)
            {
                if (currentFieldDescriptor.IsCollection && !currentFieldDescriptor.IsPolymorphic)
                {
                    if (topNumber() != 0)
                    {
                        if (currentFieldDescriptor.IsWrapped)
                        {
                            EndObjectEntry();
                            StartObjectEntry(currentFieldDescriptor.CollectionOrMapTagName);
                        }
                        else
                        {
                            FieldDescriptor lastFD = currentFieldDescriptor;
                            EndObjectEntry();
                            StartObjectEntry(lastFD.CollectionOrMapTagName);
                        }
                    }
                    incrementTop();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void StartObjectEntry(string key)
        {
            FieldDescriptor activeFieldDescriptor = null;
            Boolean isRoot = (root == null);

            if (isRoot)
            {
                ClassDescriptor rootClassDescriptor = translationScope.GetClassDescriptorByTag(key);
                if (rootClassDescriptor != null)
                {
                    ElementState tempRoot;
                    tempRoot = rootClassDescriptor.Instance;
                    if (tempRoot != null)
                    {
                        tempRoot.SetupRoot();
                        SetRoot(tempRoot);
                        activeFieldDescriptor = rootClassDescriptor.PseudoFieldDescriptor;
                    }
                    else
                    {
                        throw new Exception("root element is null");
                    }
                }
                else
                {
                    //TODO: give warnings 
                }
            }
            else
            {
                ProcessPendingTextScalar(currentFieldDescriptor.Type, currentElementState);
                ClassDescriptor currentClassDescriptor = CurrentClassDescriptor;
                activeFieldDescriptor = ((currentFieldDescriptor != null) && (currentFieldDescriptor.Type == IGNORED_ELEMENT)) ?
                    FieldDescriptor.IGNORED_ELEMENT_FIELD_DESCRIPTOR : (currentFieldDescriptor.Type == WRAPPER) ?
                    currentFieldDescriptor.WrappedFieldDescriptor : currentClassDescriptor.GetFieldDescriptorByTag(key);
            }

            this.currentFieldDescriptor = activeFieldDescriptor;
            this.PushFieldDescriptor(activeFieldDescriptor);

            if (isRoot)
                return;

            ElementState childES = null;

            switch (activeFieldDescriptor.Type)
            {
                case COMPOSITE_ELEMENT:
                    childES = activeFieldDescriptor.ConstructChildElementState(currentElementState, key);
                    activeFieldDescriptor.SetFieldToNestedObject(currentElementState, childES);
                    break;
                case COLLECTION_ELEMENT:
                    IList collection = (IList)activeFieldDescriptor.AutomaticLazyGetCollectionOrDict(currentElementState);
                    if (collection != null)
                    {
                        childES = activeFieldDescriptor.ConstructChildElementState(currentElementState, key);
                        collection.Add(childES);
                    }
                    break;
                case MAP_ELEMENT:
                    IDictionary dict = (IDictionary)activeFieldDescriptor.AutomaticLazyGetCollectionOrDict(currentElementState);
                    if (dict != null)
                    {
                        childES = activeFieldDescriptor.ConstructChildElementState(
                                currentElementState, key);
                    }
                    break;
            }

            if (childES != null)
            {
                this.currentElementState = childES;
                this.currentFieldDescriptor = activeFieldDescriptor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartArray()
        {
            pushNumber(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Primitive(object value)
        {
            if (currentFieldDescriptor != null)
            {
                switch (currentFieldDescriptor.Type)
                {
                    case SCALAR:
                    case COLLECTION_SCALAR:
                    case NAME_SPACE_LEAF_NODE:
                        currentTextValue.Append(value.ToString());
                        ProcessPendingTextScalar(currentFieldDescriptor.Type, currentElementState);
                        break;
                    case COMPOSITE_ELEMENT:
                    case COLLECTION_ELEMENT:
                        if (currentElementState.HasScalarTextField)
                            currentTextValue.Append(value.ToString());
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndJSON()
        {
            //do nothing or some cleanup 
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndObject()
        {
            //do nothing
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndObjectEntry()
        {
            ProcessPendingTextScalar(currentFieldDescriptor.Type, currentElementState);
            ElementState parentElementState = currentElementState.parent;


            switch (currentFieldDescriptor.Type)
            {
                case MAP_ELEMENT:
                    if (currentElementState is Mappable)
                    {
                        Object key = ((Mappable)currentElementState).key();
                        IDictionary dict = (IDictionary)currentFieldDescriptor.AutomaticLazyGetCollectionOrDict(parentElementState);
                        dict.Add(key, currentElementState);
                    }
                    this.currentElementState = this.currentElementState.parent;
                    break;
                case COMPOSITE_ELEMENT:
                case COLLECTION_ELEMENT:
                    this.currentElementState = this.currentElementState.parent;
                    break;
                default:
                    break;
            }

            PopAndPeekFieldDescriptor();
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndArray()
        {
            popNumber();
        }

        #endregion

    }
}