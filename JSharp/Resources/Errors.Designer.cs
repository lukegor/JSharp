﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JSharp.Resources {
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
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JSharp.Resources.Errors", typeof(Errors).Assembly);
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
        ///   Looks up a localized string similar to The image is already grayscale.
        /// </summary>
        internal static string AlreadyGrayscale_Error {
            get {
                return ResourceManager.GetString("AlreadyGrayscale_Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Histogram for the image is already open..
        /// </summary>
        internal static string HistogramAlreadyOpen {
            get {
                return ResourceManager.GetString("HistogramAlreadyOpen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The operation requires for the histogram of the selected image to be open..
        /// </summary>
        internal static string HistogramNotOpen {
            get {
                return ResourceManager.GetString("HistogramNotOpen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The selected operation may only be performed on color images..
        /// </summary>
        internal static string ImageNotColor {
            get {
                return ResourceManager.GetString("ImageNotColor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The selected operation may only be performed on grayscale images..
        /// </summary>
        internal static string ImageNotGrayscale {
            get {
                return ResourceManager.GetString("ImageNotGrayscale", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to load image..
        /// </summary>
        internal static string LoadingFailed {
            get {
                return ResourceManager.GetString("LoadingFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No image is focused.
        /// </summary>
        internal static string NoImageFocused {
            get {
                return ResourceManager.GetString("NoImageFocused", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No image is open.
        /// </summary>
        internal static string NoImageOpen {
            get {
                return ResourceManager.GetString("NoImageOpen", resourceCulture);
            }
        }
    }
}
