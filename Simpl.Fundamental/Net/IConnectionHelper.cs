using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Simpl.Fundamental.PlatformSpecifics;

namespace Simpl.Fundamental.Net
{
    /// <summary>
    /// Provides callbacks during
    /// {@link ecologylab.net.ParsedURL#connect(ConnectionHelper) ParsedURL.connect},
    /// to enable filtering and custom processing as the connect operation unfolds.
    /// </summary>
    public interface IConnectionHelper : IConnectionHelperJustRemote
    {
        /// <summary>
        /// When this method is called, you know the file is a directory.
        /// Process it if you wish.
        /// connect() will return null in this special case.
        /// </summary>
        /// <param name="file"></param>
        void HandleFileDirectory(object file);

        /// <summary>
        /// Tells the connect() method that it should go ahead and
        /// create a PURLConnection for files that have this suffix. 
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns>true if files with this suffix should be parsed;
        ///  false if they should be ignored.</returns>
        bool ParseFilesWithSuffix(string suffix);
    }

}
