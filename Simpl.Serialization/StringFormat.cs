namespace Simpl.Serialization
{
    /// <summary>
    /// Specifies string formats for Serialization and Deserialization
    /// </summary>
    public enum StringFormat
    {
        /// <summary>
        /// XML De/serialization
        /// </summary>
        Xml, 
        /// <summary>
        /// Json De/serialization
        /// </summary>
        Json, 
        /// <summary>
        /// Bibtex de/serialization
        /// </summary>
        Bibtex, 
        /// <summary>
        /// YAML de/serialization
        /// </summary>
        Yaml
    }
}