using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simpl.Fundamental.Net
{
    /// <summary>
    /// Helps for connect when it only handles net-based connections, not file-based ones.
    /// </summary>
    public interface IConnectionHelperJustRemote
    {
        /// <summary>
        /// Used to provid status feedback to the user.
        /// </summary>
        /// <param name="message"></param>
        void		DisplayStatus(String message);
	
        /// <summary>
        /// Shuffle referential models when a redirect is observed, if you like.
        /// </summary>
        /// <param name="connectionURL"></param>
        /// <returns>true if the redirect is o.k., and we should continue processing the connect().
        /// 	false if the redirect is unacceptable, and we should terminate processing.</returns>
        bool ProcessRedirect(Uri connectionURL);
    }
}
