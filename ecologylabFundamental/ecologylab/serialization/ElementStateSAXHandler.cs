using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylab.serialization.sax;
using System.IO;
using System.Collections;
using ecologylab.serialization.types.element;
using ecologylab.net;
using ecologylab.generic;
using ecologylabFundamental.ecologylab.serialization;

namespace ecologylab.serialization
{
    /// <summary>
    ///     <c>ElementStateSAXHandler</c> is a SAX Parser to parse XML files
    ///     and unmarshall it into C# rumtime objects. It takes <see cref="TranslationScope"/>
    ///     which is an abstract representation of the translation semantics. TranslationScopes 
    ///     defines which classes bind to which tags in the XML representation.
    /// </summary>
    public class ElementStateSAXHandler : FieldTypes, ISAXContentHandler, ISAXErrorHandler, IScalarUnmarshallingContext
    {
        #region Private Fields

        /// <summary>
        ///     A SAX Parser object for parsing XML files.
        /// </summary>
        private SaxParser parser;

        /// <summary>
        ///     A files stream object to read XML files.
        /// </summary>
        private Stream inputFileStream;

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

        private ParsedUri uriContext;

        private IDeserializationHookStrategy deserializationHookStrategy;

        TranslationContext translationContext = new TranslationContext();

        #endregion

        #region Constructors 

        /// <summary>
        ///     Basic constructor for <c>ElementStateSAXHandler</c>. Only initializes a 
        ///     SAX Parser. It should be used only with the following public Parse method 
        ///     <para>
        ///         <c>public ElementState Parse(FileStream inputFileStream, TranslationScope translationScope)</c>
        ///     </para>
        /// </summary>
        public ElementStateSAXHandler()
        {
            parser = new SaxParser();
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
        public ElementStateSAXHandler(String url, TranslationScope translationScope, ParsedUri uriContext, TranslationContext translationContext)
            : this()
        {
            this.inputFileStream = File.OpenRead(url);
            this.translationScope = translationScope;
            this.uriContext = uriContext;
            this.translationContext = translationContext;
        }

        public ElementStateSAXHandler(Stream stream, TranslationScope translationScope, ParsedUri uriContext, TranslationContext translationContext, IDeserializationHookStrategy deserializationHookStrategy = null)
            : this()
        {
            this.inputFileStream = stream;
            this.translationScope = translationScope;
            this.uriContext = uriContext;
            this.deserializationHookStrategy = deserializationHookStrategy;
            this.translationContext = translationContext;
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
        public ElementStateSAXHandler(FileStream inputFileStream, TranslationScope translationScope, TranslationContext translationContext)
            : this()
        {
            this.inputFileStream = inputFileStream;
            this.translationScope = translationScope;
            this.translationContext = translationContext;
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

        public ElementState Parse(IDeserializationHookStrategy deserializationHookStrategy)
        {
            this.deserializationHookStrategy = deserializationHookStrategy;
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

        #region SAX Events (used for parsing by SAXHandler)

        /// <summary>
        ///     Implementation start element methods from <c>SAXParser</c>
        ///     This method is invoked for any opening tag in XML file.
        /// </summary>
        /// <param name="namespaceURI"></param>
        /// <param name="localName"></param>
        /// <param name="tagName"></param>
        /// <param name="atts"></param>
        public void startElement(String namespaceURI, String localName, String tagName, sax.Attributes atts)
        {
            FieldDescriptor activeFieldDescriptor = null;
            Boolean isRoot = (root == null);

            if (isRoot)
            {
                ClassDescriptor rootClassDescriptor = translationScope.GetClassDescriptorByTag(tagName);
                if (rootClassDescriptor != null)
                {
                    ElementState tempRoot;
                    tempRoot = rootClassDescriptor.Instance;
                    if (tempRoot != null)
                    {
                        tempRoot.SetupRoot();
                        SetRoot(tempRoot);
                        if (deserializationHookStrategy != null)
                            deserializationHookStrategy.deserializationPreHook(tempRoot, null);
                        tempRoot.TranslateAttributes(atts,translationContext);
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
                ProcessPendingTextScalar(currentFieldDescriptor.Type, currentElementState, this);
                ClassDescriptor currentClassDescriptor = CurrentClassDescriptor;
                activeFieldDescriptor = ((currentFieldDescriptor != null) && (currentFieldDescriptor.Type == IGNORED_ELEMENT)) ?
                    FieldDescriptor.IGNORED_ELEMENT_FIELD_DESCRIPTOR : (currentFieldDescriptor.Type == WRAPPER) ?
                    currentFieldDescriptor.WrappedFieldDescriptor : currentClassDescriptor.GetFieldDescriptorByTag(tagName);
            }

            if (activeFieldDescriptor == null)
                activeFieldDescriptor = FieldDescriptor.IGNORED_ELEMENT_FIELD_DESCRIPTOR;

            this.currentFieldDescriptor = activeFieldDescriptor;
            this.PushFieldDescriptor(activeFieldDescriptor);

            if (isRoot)
                return;

            ElementState childES = null;

            switch (activeFieldDescriptor.Type)
            {
                case COMPOSITE_ELEMENT:
                    childES = activeFieldDescriptor.ConstructChildElementState(currentElementState, tagName, atts, translationContext);
                    activeFieldDescriptor.SetFieldToNestedObject(currentElementState, childES);
                    break;
                case COLLECTION_ELEMENT:
                    IList collection = (IList)activeFieldDescriptor.AutomaticLazyGetCollectionOrDict(currentElementState);
                    if (collection != null)
                    {
                        childES = activeFieldDescriptor.ConstructChildElementState(currentElementState, tagName, atts, translationContext);
                        collection.Add(childES);
                    }
                    break;
                case MAP_ELEMENT:
                    IDictionary dict = (IDictionary)activeFieldDescriptor.AutomaticLazyGetCollectionOrDict(currentElementState);
                    if (dict != null)
                    {
                        childES = activeFieldDescriptor.ConstructChildElementState(
                                currentElementState, tagName, atts, translationContext);
                    }
                    break;
            }

            if (childES != null)
            {
                if (deserializationHookStrategy != null)
                    deserializationHookStrategy.deserializationPreHook(childES, activeFieldDescriptor);

                childES.TranslateAttributes(atts,translationContext);
                this.currentElementState = childES;
                this.currentFieldDescriptor = activeFieldDescriptor;
            }
        }

        /// <summary>
        ///     Implementation end element methods from <c></c>
        ///     This method is invoked for any closing tag in XML file.
        /// </summary>
        /// <param name="namespaceURI"></param>
        /// <param name="localName"></param>
        /// <param name="rawName"></param>
        public void endElement(String namespaceURI, String localName, String rawName)
        {
            ProcessPendingTextScalar(currentFieldDescriptor.Type, currentElementState, this);
            ElementState parentElementState = currentElementState.Parent;


            switch (currentFieldDescriptor.Type)
            {
                case MAP_ELEMENT:
                    if (currentElementState is Mappable)
                    {
                        Object key = ((Mappable)currentElementState).key();
                        Object collection = currentFieldDescriptor.AutomaticLazyGetCollectionOrDict(parentElementState);
                        // because c# casting causes the wrong add method call
                        if (collection is IDictionaryList)
                        {
                            IDictionaryList dict = (IDictionaryList)collection;
                            dict.Add(key, currentElementState);
                        }
                        else
                        {
                            IDictionary dict = (IDictionary)collection;
                            dict.Add(key, currentElementState);
                        }
                    }
                    currentElementState.DeserializationPostHook();
                    if (deserializationHookStrategy != null)
                        deserializationHookStrategy.deserializationPostHook(currentElementState, currentFieldDescriptor);
                    this.currentElementState = this.currentElementState.Parent;
                    break;
                case COMPOSITE_ELEMENT:
                case COLLECTION_ELEMENT:
                    currentElementState.DeserializationPostHook();
                    if (deserializationHookStrategy != null)
                        deserializationHookStrategy.deserializationPostHook(currentElementState, currentFieldDescriptor);
                    this.currentElementState = this.currentElementState.Parent;
                    break;
                default:
                    break;
            }

            PopAndPeekFieldDescriptor();
        }

        /// <summary>
        ///     Implementation end characters methods from <c>SAXParser</c>
        ///     This methods is invoked for text between leaf nodes
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void characters(Char[] ch, int start, int end)
        {
            if (currentFieldDescriptor != null)
            {
                switch (currentFieldDescriptor.Type)
                {
                    case SCALAR:
                    case COLLECTION_SCALAR:
                    case NAME_SPACE_LEAF_NODE:
                        currentTextValue.Append(ch);
                        break;
                    case COMPOSITE_ELEMENT:
                    case COLLECTION_ELEMENT:
                        if (currentElementState.HasScalarTextField)
                            currentTextValue.Append(ch);
                        break;
                    default:
                        break;
                }
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
        private void ProcessPendingTextScalar(int currentType, ElementState currentElementState, IScalarUnmarshallingContext scalarUnmarshallingContext)
        {
            int length = currentTextValue.Length;
            String value = null;

            if (length > 0)
            {
                switch (currentType)
                {
                    case SCALAR:
                        value = currentTextValue.ToString().Substring(0, length);
                        currentFieldDescriptor.SetFieldToScalar(currentElementState, value, scalarUnmarshallingContext);
                        break;
                    case COLLECTION_SCALAR:
                        value = currentTextValue.ToString().Substring(0, length);
                        currentFieldDescriptor.AddLeafNodeToCollection(currentElementState, value, scalarUnmarshallingContext);
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

        #endregion

        #region Properties

        /// <summary>
        ///     Get the class descriptor of the current <c>ElementState</c>.
        /// </summary>
        public ClassDescriptor CurrentClassDescriptor
        {
            get
            {
                return this.currentElementState.ClassDescriptor;
            }
        }

        #endregion

        #region SAX Events (not used by the SAXHandler)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locator"></param>
        public void setDocumentLocator(sax.Locator locator)
        {
            //Console.WriteLine("setDocumentLocator");
        }

        /// <summary>
        /// 
        /// </summary>
        public void startDocument()
        {
            //Do Nothing
        }

        /// <summary>
        /// 
        /// </summary>
        public void endDocument()
        {
            if (root != null)
                root.DeserializationPostHook();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="data"></param>
        public void processingInstruction(string target, string data)
        {
            //Do Nothing
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="uri"></param>
        public void startPrefixMapping(string prefix, string uri)
        {
            //Do Nothing
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        public void endPrefixMapping(string prefix)
        {
            //Do Nothing
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void ignorableWhitespace(char[] ch, int start, int end)
        {
            //Do Nothing
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void skippedEntity(string name)
        {
            //Do Nothing
        }

        #endregion

        #region Error Handling Events (Raised during Parsing of XML File)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void warning(sax.SAXParseException exception)
        {
            Console.WriteLine(exception.getMessage().ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void error(sax.SAXParseException exception)
        {
            Console.WriteLine(exception.getMessage().ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public void fatalError(sax.SAXParseException exception)
        {
            Console.WriteLine(exception.getMessage().ToString());
        }

        #endregion
    
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ParsedUri UriContext()
        {
            return uriContext;
        }
    }
}