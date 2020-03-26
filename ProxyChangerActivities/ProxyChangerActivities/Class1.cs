using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Activities;
using System.ComponentModel;

namespace ProxyChangerActivities
{
    [DisplayName("Enable Proxy")]
    [Description("This activity enable system proxy (proxy for IE and Chrome)")]
    public class EnableProxy : CodeActivity
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption
          (IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        [Category("Input")]
        [RequiredArgument]
        [Description("Type here proxy address like xxx.xxx.xxx.xxx:pppp")]
        public InArgument<String> ProxyAddress { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            bool settingsReturn, refreshReturn;
            RegistryKey registry = Registry.CurrentUser.OpenSubKey
             ("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 1);
            registry.SetValue
            ("ProxyServer", ProxyAddress.Get(context));
            if ((int)registry.GetValue("ProxyEnable", 0) == 0)
                Console.WriteLine("Unable to enable the proxy.");
            else
                Console.WriteLine("The proxy has been turned on.");
            registry.Close();
            settingsReturn = InternetSetOption
            (IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
            refreshReturn = InternetSetOption
            (IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
        }

    }
    [DisplayName("Disable Proxy")]
    [Description("This activity disables system proxy (proxy for IE and Chrome)")]
    public class DisableProxy : CodeActivity
    {
        [DllImport("wininet.dll")]
        public static extern bool InternetSetOption
          (IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;

        protected override void Execute(CodeActivityContext context)
        {
            RegistryKey registry = Registry.CurrentUser.OpenSubKey
              ("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            registry.SetValue("ProxyEnable", 0);
            registry.SetValue("ProxyServer", 0);
            registry.Close();
        }
    }
}
