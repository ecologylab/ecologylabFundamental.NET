using System;
using System.Collections;

namespace ecologylabFundamental.ecologylab.xml.sax
{
    public class SAXParseException
    {
        string lineNumber = "";
        string systemID = "";
        string message = "";
        public string getLineNumber()
        {
            return lineNumber;
        }
        public string getSystemID()
        {
            return systemID;
        }
        public string getMessage()
        {
            return message;
        }

        public string LineNumber
        {
            set
            {
                lineNumber = value;
            }
        }
        public string SystemID
        {
            set
            {
                systemID = value;
            }
        }
        public string Message
        {
            set
            {
                message = value;
            }
        }

    }

    public class Locator
    {
        int lineNumber;
        int columnNumber;

        public Locator()
        {
            lineNumber = 0;
            columnNumber = 0;
        }
        public int LineNumber
        {
            set
            {
                lineNumber = value;
            }
        }
        public int ColumnNumber
        {
            set
            {
                columnNumber = value;
            }
        }
        public int getLineNumber()
        {
            return lineNumber;
        }
        public int getColumnNumber()
        {
            return columnNumber;
        }
    }

    public struct SaxAttribute
    {
        public string Name;
        public string NamespaceURI;
        public string Value;
    }

    public class Attributes
    {
        public ArrayList attArray = new ArrayList();

        public int getLength()
        {
            return attArray.Count;
        }
        public string getQName(int index)
        {
            SaxAttribute saxAtt = (SaxAttribute)attArray[index];
            return saxAtt.Name;
        }
        public string getValue(int index)
        {
            SaxAttribute saxAtt = (SaxAttribute)attArray[index];
            return saxAtt.Value;
        }

        public Attributes TrimArray()
        {
            attArray.TrimToSize();
            return this;
        }
    }
}
