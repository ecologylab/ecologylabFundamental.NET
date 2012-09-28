using System;
using System.Collections;
using System.Collections.Generic;

namespace Simpl.Serialization.Deserializers.Parsers.Xml
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
        public List<object> AttArray= new List<object>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetLength()
        {
            return AttArray.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetQName(int index)
        {
            SaxAttribute saxAtt = (SaxAttribute)AttArray[index];
            return saxAtt.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetValue(int index)
        {
            SaxAttribute saxAtt = (SaxAttribute)AttArray[index];
            return saxAtt.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Attributes TrimArray()
        {
            //AttArray.TrimToSize();
            return this;
        }
    }
}
