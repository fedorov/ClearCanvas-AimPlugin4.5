﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AIMTemplateService.Configuration {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class AtsSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static AtsSettings defaultInstance = ((AtsSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AtsSettings())));
        
        public static AtsSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://imaging.fsm.northwestern.edu")]
        public string AtsUrl {
            get {
                return ((string)(this["AtsUrl"]));
            }
            set {
                this["AtsUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoImportTemplates {
            get {
                return ((bool)(this["AutoImportTemplates"]));
            }
            set {
                this["AutoImportTemplates"] = value;
            }
        }
    }
}
