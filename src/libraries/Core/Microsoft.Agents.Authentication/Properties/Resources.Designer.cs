﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Agents.Authentication.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Agents.Authentication.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &apos;{0}&apos; not found in Assembly &apos;{1}&apos; or is the wrong type for &apos;{2}&apos;.
        /// </summary>
        internal static string Error_AuthModuleNotFound {
            get {
                return ResourceManager.GetString("Error_AuthModuleNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connection &apos;{0}&apos; not found in configuration.
        /// </summary>
        internal static string Error_ConnectionNotFoundByName {
            get {
                return ResourceManager.GetString("Error_ConnectionNotFoundByName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to create authentication provider for connection name &apos;{0}&apos;.
        /// </summary>
        internal static string Error_FailedToCreateAuthModuleProvider {
            get {
                return ResourceManager.GetString("Error_FailedToCreateAuthModuleProvider", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Type &apos;{0},{1}&apos; does not have the required constructor..
        /// </summary>
        internal static string Error_InvalidAuthProviderConstructor {
            get {
                return ResourceManager.GetString("Error_InvalidAuthProviderConstructor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No connections found in for this Agent in the Connections Configuration.
        /// </summary>
        internal static string Error_MissingAuthenticationConfig {
            get {
                return ResourceManager.GetString("Error_MissingAuthenticationConfig", resourceCulture);
            }
        }
    }
}
