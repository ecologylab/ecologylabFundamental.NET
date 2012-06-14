using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Simpl.Serialization.Attributes;

namespace Simpl.Serialization.PlatformSpecifics
{
    interface IFundamentalPlatformSpecifics
    {
	// in ApplicationEnvironment
	//void InitializePlatformSpecificTranslation();

	// in ecologylab.serialization.ClassDescriptor;
	void DeriveSuperClassGenericTypeVars(ClassDescriptor classDescriptor);

	// in ecologylab.serialization.FieldDescriptor;
	void DeriveFieldGenericTypeVars(FieldDescriptor fieldDescriptor);

	//Type GetTypeArgClass(FieldInfo field, int i, FieldDescriptor fiedlDescriptor);

	// in ecologylab.serialization.GenericTypeVar;
	void CheckBoundParameterizedTypeImpl(GenericTypeVar g, Type bound);

	//void CheckTypeWildcardTypeImpl(GenericTypeVar g, Type type);

	void CheckTypeParameterizedTypeImpl(GenericTypeVar g, Type type);

	// more stuff
	//Object getOrCreatePrefsEditor(MetaPrefSet metaPrefSet, PrefSet prefSet, ParsedURL savePrefsPURL,
	//		final boolean createJFrame, final boolean isStandalone);

	// in ParsedUrl
/*	String[] getReaderFormatNames();

	// in Generic
	void beep();

	void showDialog(String msg, String[] digital_options);

	// in Pref
	Object usePrefColor(String name, Object defaultValue);

    Object lookupColor(String name, Object defaultValue); //throws ClassCastException;

	Type[] addtionalPrefOpTranslations();

	Type[] additionalPrefSetBaseTranslations();

	// platform specific types
	void initializePlatformSpecificTypes();
*/
    }
}
