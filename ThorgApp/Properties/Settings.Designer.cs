﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GolemUI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("LazySubnet30")]
        public string Subnet {
            get {
                return ((string)(this["Subnet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool TestNet {
            get {
                return ((bool)(this["TestNet"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://9dbe739909b646a9ab6d5376ab5f8ffb@o921571.ingest.sentry.io/6057947")]
        public string SentryDsn {
            get {
                return ((string)(this["SentryDsn"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("12")]
        public int UserSettingsVersion {
            get {
                return ((int)(this["UserSettingsVersion"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("12")]
        public int BenchmarkResultsVersion {
            get {
                return ((int)(this["BenchmarkResultsVersion"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("n1.mining-proxy.imapp.pl:8069")]
        public string DefaultProxy {
            get {
                return ((string)(this["DefaultProxy"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("www.golem.network")]
        public string GolemWebPage {
            get {
                return ((string)(this["GolemWebPage"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OpenConsoleProvider {
            get {
                return ((bool)(this["OpenConsoleProvider"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DebugLogsYagna {
            get {
                return ((bool)(this["DebugLogsYagna"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool DebugLogsProvider {
            get {
                return ((bool)(this["DebugLogsProvider"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OpenConsoleYagna {
            get {
                return ((bool)(this["OpenConsoleYagna"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("shield")]
        public string dialog_antivir_image {
            get {
                return ((string)(this["dialog_antivir_image"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Your antivirus is blocking Thorg")]
        public string dialog_antivir_title {
            get {
                return ((string)(this["dialog_antivir_title"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("gpu")]
        public string dialog_gpu_image {
            get {
                return ((string)(this["dialog_gpu_image"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Insufficient GPU memory")]
        public string dialog_gpu_title {
            get {
                return ((string)(this["dialog_gpu_title"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("wallet")]
        public string dialog_wallet_image {
            get {
                return ((string)(this["dialog_wallet_image"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Don\'t use an exchange address")]
        public string dialog_wallet_title {
            get {
                return ((string)(this["dialog_wallet_title"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Got it!")]
        public string dialog_wallet_button {
            get {
                return ((string)(this["dialog_wallet_button"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Close")]
        public string dialog_gpu_button {
            get {
                return ((string)(this["dialog_gpu_button"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Close")]
        public string dialog_antivir_button {
            get {
                return ((string)(this["dialog_antivir_button"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("n1.mining-proxy.imapp.pl:8073")]
        public string DefaultProxyLowMem {
            get {
                return ((string)(this["DefaultProxyLowMem"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"Your antivirus is blocking Thorg's mining module called “EthDcrMiner64.exe”. If you do not have antivirus software installed you can unblock this by going to your Windows Security settings, and then clicking on Virus and Threat Protections. 
If it doesn't help you might want to restart Thorg after changing settings in your antivirus.")]
        public string dialog_antivir_message {
            get {
                return ((string)(this["dialog_antivir_message"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("No worries though! We will soon release an update with support for cards that hav" +
            "e less than 6B of RAM. Also, stay tuned for CPU support that is just around the " +
            "corner.\nTill then you can explore the app and get to know Thorg a bit better!")]
        public string dialog_gpu_message {
            get {
                return ((string)(this["dialog_gpu_message"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(@"Thorg is most suited for addresses that the user has custody of, compared to some other individual than the Thorg user (such as a centralized exchange) having custody of the address.
Most exchanges do not support L2 payments like Polygon that Thorg uses. Please change your wallet address to one that you're in control of, such as MetaMask.")]
        public string dialog_wallet_message {
            get {
                return ((string)(this["dialog_wallet_message"]));
            }
        }
    }
}