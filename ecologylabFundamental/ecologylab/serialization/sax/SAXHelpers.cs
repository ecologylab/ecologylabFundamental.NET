using System;
using System.Collections;

namespace ecologylabFundamental.ecologylab.serialization.sax
{
    /// <summary>
    /// 
    /// </summary>
    public class SAXParseException
    {
        string lineNumber = "";
        string systemID = "";
        string message = "";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getLineNumber()
        {
            return lineNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getSystemID()
        {
            return systemID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string getMessage()
        {
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LineNumber
        {
            set
            {
                lineNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SystemID
        {
            set
            {
                systemID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            set
            {
                message = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Locator
    {
        int lineNumber;
        int columnNumber;

        /// <summary>
        /// 
        /// </summary>
        public Locator()
        {
            lineNumber = 0;
            columnNumber = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public int LineNumber
        {
            set
            {
                lineNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnNumber
        {
            set
            {
                columnNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getLineNumber()
        {
            return lineNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getColumnNumber()
        {
            return columnNumber;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public struct SaxAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name;

        /// <summary>
        /// 
        /// </summary>
        public string NamespaceURI;

        /// <summary>
        /// 
        /// </summary>
        public string Value;
    }

    /// <summary>
    /// 
    /// </summary>
    public class Attributes
    {
        /// <summary>
        /// 
        /// </summary>
        public ArrayList attArray = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int getLength()
        {
            return attArray.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getQName(int index)
        {
            SaxAttribute saxAtt = (SaxAttribute)attArray[index];
            return saxAtt.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getValue(int index)
        {
            SaxAttribute saxAtt = (SaxAttribute)attArray[index];
            return saxAtt.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Attributes TrimArray()
        {
            attArray.TrimToSize();
            return this;
        }
    }
}
