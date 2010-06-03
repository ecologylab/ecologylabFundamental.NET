using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.xml.sax;
using System.IO;
using System.Collections;

namespace ecologylabFundamental.ecologylab.xml
{
    class ElementStateSAXHandler : FieldTypes, ISAXContentHandler, ISAXErrorHandler
    {
        private SaxParser parser;
        private FileStream inputFileStream;
        private ElementState root;
        private ElementState currentElementState;
        private TranslationScope translationScope;
        private FieldDescriptor currentFieldDescriptor;
        private List<FieldDescriptor> fdStack = new List<FieldDescriptor>();
        private StringBuilder currentTextValue = new StringBuilder(1024);

        public ElementStateSAXHandler()
        {
            parser = new SaxParser();
            parser.setContentHandler(this);
            parser.setErrorHandler(this);
        }

        public ElementStateSAXHandler(String Url, TranslationScope translationScope)
            : this()
        {
            this.inputFileStream = File.OpenRead(Url);
            this.translationScope = translationScope;
        }

        public ElementStateSAXHandler(FileStream inputFile, TranslationScope translationScope)
            : this()
        {
            this.inputFileStream = inputFile;
            this.translationScope = translationScope;
        }

        public ElementState Parse(FileStream inputFileStream, TranslationScope translationScope)
        {
            this.inputFileStream = inputFileStream;
            this.translationScope = translationScope;
            return Parse();
        }

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

        public void setDocumentLocator(sax.Locator locator)
        {
            //Console.WriteLine("setDocumentLocator");
        }

        public void startDocument()
        {
            Console.WriteLine("START DOCUMENT");
        }

        public void endDocument()
        {
            Console.WriteLine("END DOCUMENT");
        }

        public void processingInstruction(string target, string data)
        {
            Console.WriteLine("processingInstruction");
        }

        public void startPrefixMapping(string prefix, string uri)
        {
            Console.WriteLine("startPrefixMapping");
        }

        public void endPrefixMapping(string prefix)
        {
            Console.WriteLine("endPrefixMapping");
        }

        public void startElement(string namespaceURI, string localName, string rawName, sax.Attributes atts)
        {
            Console.WriteLine("startElement " + localName + " " + atts.attArray.Count);

            FieldDescriptor activeFieldDescriptor = null;
            Boolean isRoot = (root == null);

            if (isRoot)
            {
                ClassDescriptor rootClassDescriptor = translationScope.GetClassDescriptorByTag(rawName);
                if (rootClassDescriptor != null)
                {
                    ElementState tempRoot;
                    tempRoot = rootClassDescriptor.Instance;
                    if (tempRoot != null)
                    {
                        tempRoot.SetupRoot();
                        SetRoot(tempRoot);
                        tempRoot.TranslateAttributes(translationScope, atts, this, tempRoot);
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
                    currentFieldDescriptor.WrappedFieldDescriptor : currentClassDescriptor.GetFieldDescriptorByTag(rawName, translationScope, currentElementState);
            }

            this.currentFieldDescriptor = activeFieldDescriptor;
            this.PushFieldDescriptor(activeFieldDescriptor);

            if (isRoot)
                return;

            ElementState childES = null;

            switch (activeFieldDescriptor.Type)
            {
                case NESTED_ELEMENT:
                    childES = activeFieldDescriptor.ConstructChildElementState(currentElementState, rawName);
                    activeFieldDescriptor.SetFieldToNestedObject(currentElementState, childES);
                    break;
                case COLLECTION_ELEMENT:
                    IList collection = (IList)activeFieldDescriptor.AutomaticLazyGetCollectionOrDict(currentElementState);
                    if (collection != null)
                    {
                        childES = activeFieldDescriptor.ConstructChildElementState(currentElementState, rawName);
                        collection.Add(childES);
                    }
                    break;
                case MAP_ELEMENT:
                    break;
            }

            if (childES != null)
            {
                childES.TranslateAttributes(translationScope, atts, this, currentElementState);
                this.currentElementState = childES;
                this.currentFieldDescriptor = activeFieldDescriptor;
            }
        }

        private void PushFieldDescriptor(FieldDescriptor fieldDescriptor)
        {
            this.fdStack.Add(fieldDescriptor);
        }

        private void ProcessPendingTextScalar(int currentType, ElementState currentElementState)
        {
            int length = currentTextValue.Length;
            String value = null;

            if (length > 0)
            {
                switch (currentType)
                {
                    case LEAF:
                        value = currentTextValue.ToString().Substring(0, length);
                        currentFieldDescriptor.SetFieldToScalar(currentElementState, value, this);
                        break;
                    case COLLECTION_SCALAR:
                        value = currentTextValue.ToString().Substring(0, length);
                        currentFieldDescriptor.AddLeafNodeToCollection(currentElementState, value, this);
                        break;
                    default:
                        break;
                }

                currentTextValue.Length = 0;
            }
        }

        private void SetRoot(ElementState pRoot)
        {
            this.root = pRoot;
            this.currentElementState = pRoot;
        }

        public void endElement(String namespaceURI, String localName, String rawName)
        {
            Console.WriteLine("endElement " + rawName);

            ProcessPendingTextScalar(currentFieldDescriptor.Type, currentElementState);
            ElementState parentElementState = currentElementState.parent;

            
            switch (currentFieldDescriptor.Type)
            {
                case NESTED_ELEMENT:
                case COLLECTION_ELEMENT:
                    this.currentElementState = this.currentElementState.parent;
                    break;
                default:
                    break;
            }
            
            PopAndPeekFieldDescriptor();
        }

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

        public void characters(Char[] ch, int start, int end)
        {
            if (currentFieldDescriptor != null)
            {
                switch (currentFieldDescriptor.Type)
                {
                    case LEAF:
                    case COLLECTION_SCALAR:
                    case NAME_SPACE_LEAF_NODE:
                        currentTextValue.Append(ch);
                        break;
                    case ROOT:
                    case NESTED_ELEMENT:
                    case COLLECTION_ELEMENT:
                        if (currentElementState.HasScalarTextField)
                            currentTextValue.Append(ch);
                        break;
                    default:
                        break;
                }
            }
        }

        public void ignorableWhitespace(char[] ch, int start, int end)
        {
            Console.WriteLine("ignorableWhitespace");
        }

        public void skippedEntity(string name)
        {
            Console.WriteLine(name);
        }

        public void warning(sax.SAXParseException exception)
        {
            Console.WriteLine("warning");
        }

        public void error(sax.SAXParseException exception)
        {
            Console.WriteLine(exception.getMessage().ToString());
        }

        public void fatalError(sax.SAXParseException exception)
        {
            Console.WriteLine("fatalError");
        }

        public ClassDescriptor CurrentClassDescriptor
        {
            get
            {
                return this.currentElementState.ElementClassDescriptor;
            }
        }

    }
}
