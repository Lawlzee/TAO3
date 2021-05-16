using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Toast
{
    internal class ToastService : IToastService
    {
        public void Notify(string title, string text, DateTimeOffset expirationTime)
        {
            //https://den.dev/blog/powershell-windows-notification/
            string psCommmand = $@"
[Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > $null
$Template = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::ToastText02)

$RawXml = [xml] $Template.GetXml()
($RawXml.toast.visual.binding.text|where {{$_.id -eq ""1""}}).AppendChild($RawXml.CreateTextNode(""{EscapeString(title)}"")) > $null
($RawXml.toast.visual.binding.text|where {{$_.id -eq ""2""}}).AppendChild($RawXml.CreateTextNode(""{EscapeString(text)}"")) > $null

$SerializedXml = New-Object Windows.Data.Xml.Dom.XmlDocument
$SerializedXml.LoadXml($RawXml.OuterXml)

$Toast = [Windows.UI.Notifications.ToastNotification]::new($SerializedXml)
$Toast.ExpirationTime = [DateTimeOffset]::Now.AddSeconds({(DateTimeOffset.Now - expirationTime).Seconds})

$Notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier(""PowerShell"")
$Notifier.Show($Toast);
";

            byte[] psCommandBytes = Encoding.Unicode.GetBytes(psCommmand);
            string psCommandBase64 = Convert.ToBase64String(psCommandBytes);

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted -EncodedCommand {psCommandBase64}",
                UseShellExecute = false
            };
            Process.Start(startInfo);
        }

        private string EscapeString(string str)
        {
            return str
                .Replace("\"", "`");
        }
    }
}
