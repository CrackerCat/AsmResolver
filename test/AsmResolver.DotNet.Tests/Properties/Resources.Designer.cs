﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AsmResolver.DotNet.Tests.Properties {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("AsmResolver.DotNet.Tests.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static byte[] HelloWorld {
            get {
                object obj = ResourceManager.GetObject("HelloWorld", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] HelloWorld_NetCore {
            get {
                object obj = ResourceManager.GetObject("HelloWorld_NetCore", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] HelloWorld_MaliciousNestedClassLoop {
            get {
                object obj = ResourceManager.GetObject("HelloWorld_MaliciousNestedClassLoop", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] HelloWorld_MaliciousNestedClassLoop2 {
            get {
                object obj = ResourceManager.GetObject("HelloWorld_MaliciousNestedClassLoop2", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] HelloWorld_IllegalTypeSpecInTypeDefOrRefSig {
            get {
                object obj = ResourceManager.GetObject("HelloWorld_IllegalTypeSpecInTypeDefOrRefSig", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] HelloWorld_MaliciousTypeSpecLoop {
            get {
                object obj = ResourceManager.GetObject("HelloWorld_MaliciousTypeSpecLoop", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] ConstantZeroValueColumn {
            get {
                object obj = ResourceManager.GetObject("ConstantZeroValueColumn", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] ActualLibrary {
            get {
                object obj = ResourceManager.GetObject("ActualLibrary", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] ForwarderLibrary {
            get {
                object obj = ResourceManager.GetObject("ForwarderLibrary", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        public static byte[] ForwarderRefTest {
            get {
                object obj = ResourceManager.GetObject("ForwarderRefTest", resourceCulture);
                return ((byte[])(obj));
            }
        }
    }
}
