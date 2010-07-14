using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ecologylabFundamental.ecologylab.atttributes;

namespace ecologylabFundamental.ecologylab.serialization.library.Schmannel
{
    /// <summary>
    /// 
    /// </summary>
    public class Schmannel : ElementState
    {
        /// <summary>
        /// 
        /// </summary>
        [simpl_classes(new Type[] {typeof(Item), typeof(SchmItem)})]
        [simpl_collection]
        List<Item> schmItems;

        /// <summary>
        /// 
        /// </summary>
        [simpl_classes(new Type[] { typeof(BItem) })]
        [simpl_composite]
        public Item polyItem;

        /// <summary>
        /// 
        /// </summary>
        public Schmannel()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void PolyAdd(Item item)
        {
            if (schmItems == null)
                schmItems = new List<Item>();
            schmItems.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public static String WRAP_OUT = "<schmannel><schm_items><item></item></schm_items></schmannel>"; // "<channel><items></items></channel>";

        /// <summary>
        /// 
        /// </summary>
	    public static String ITEMS = "<schmannel><schm_items><item><title>it is called rogue!</title><description>its a game</description><link>http://ecologylab.cs.tamu.edu/rogue/</link><author>zach</author></item><item><title>it is called cf!</title><description>its a creativity support tool</description><author>andruid</author></item></schm_items></schmannel>";


	
    }
}
