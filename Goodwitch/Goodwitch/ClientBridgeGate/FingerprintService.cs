using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Management;
using Goodwitch.CommonUtils;

namespace Goodwitch.ClientBridgeGate
{
    internal class FingerprintService
    {
        internal static string GenerateFingerprint()
        {
            string rawFingerPrint = string.Concat(new string[]
            {
                GetNTAccountSecIdentifier(),
                GetBIOSCombinationIdentifier(),
                GetBaseBoardIdentifier(),
                GetMACIdentifier(),
                GetVideoControllerIdentifier(),
                GetCPUIdentifier(),
                GetDiskDriveIdentifier()
            });

            byte[] rawBytes = Encoding.UTF8.GetBytes(rawFingerPrint);
            return Convert.ToBase64String(rawBytes);
        }

        private static string GetNTAccountSecIdentifier()
        {
            NTAccount nTAccount = new NTAccount(WindowsIdentity.GetCurrent().Name);
            SecurityIdentifier securityIdentifier = (SecurityIdentifier)nTAccount.Translate(typeof(SecurityIdentifier));

            return securityIdentifier.ToString();
        }

        private static string GetBIOSCombinationIdentifier()
        {
            return string.Concat(new string[]{
                ManagementExtension.GetManagementObjectFromName("Win32_BIOS", "Manufacturer"),
                ManagementExtension.GetManagementObjectFromName("Win32_BIOS", "SMBIOSBIOSVersion"),
                ManagementExtension.GetManagementObjectFromName("Win32_BIOS", "IdentificationCode"),
                ManagementExtension.GetManagementObjectFromName("Win32_BIOS", "SerialNumber"),
                ManagementExtension.GetManagementObjectFromName("Win32_BIOS", "ReleaseDate"),
                ManagementExtension.GetManagementObjectFromName("Win32_BIOS", "Version")
            });
        }

        private static string GetBaseBoardIdentifier()
        {
            return string.Concat(new string[]
            {
                ManagementExtension.GetManagementObjectFromName("Win32_BaseBoard", "Model"),
                ManagementExtension.GetManagementObjectFromName("Win32_BaseBoard", "Manufacturer"),
                ManagementExtension.GetManagementObjectFromName("Win32_BaseBoard", "Name"),
                ManagementExtension.GetManagementObjectFromName("Win32_BaseBoard", "SerialNumber")
            });
        }

        private static string GetMACIdentifier() => ManagementExtension.GetManagementObjectFromNameWithStatement("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");

        private static string GetVideoControllerIdentifier() => string.Concat(new string[] {
             ManagementExtension.GetManagementObjectFromName("Win32_VideoController", "DriverVersion"),
             ManagementExtension.GetManagementObjectFromName("Win32_VideoController", "Name")
        });

        private static string GetCPUIdentifier()
        {
            string dummyString = ManagementExtension.GetManagementObjectFromName("Win32_Processor", "UniqueId");

            while (dummyString == "")
            {
                dummyString = ManagementExtension.GetManagementObjectFromName("Win32_Processor", "ProcessorId");
                dummyString = ManagementExtension.GetManagementObjectFromName("Win32_Processor", "Name");
                dummyString = ManagementExtension.GetManagementObjectFromName("Win32_Processor", "Manufacturer") + ManagementExtension.GetManagementObjectFromName("Win32_Processor", "MaxClockSpeed");
            }

            return dummyString;
        }

        private static string GetDiskDriveIdentifier() => string.Concat(new string[] {

            ManagementExtension.GetManagementObjectFromName("Win32_DiskDrive", "Model"),
            ManagementExtension.GetManagementObjectFromName("Win32_DiskDrive", "Manufacturer"),
            ManagementExtension.GetManagementObjectFromName("Win32_DiskDrive", "Signature"),
            ManagementExtension.GetManagementObjectFromName("Win32_DiskDrive", "TotalHeads")
        });

        private class ManagementExtension
        {
            internal static string GetManagementObjectFromName(string className, string instanceName)
            {
                string retVal = "";

                try
                {
                    using (ManagementObjectSearcher mosearcher = new ManagementObjectSearcher($"SELECT * FROM {className}"))
                    {
                        using (ManagementObjectCollection mocollection = mosearcher.Get())
                        {
                            StringBuilder sb = new StringBuilder();

                            foreach (ManagementObject mo in mocollection)
                            {
                                retVal = mo[instanceName] != null ? mo[instanceName].ToString() : "";
                                break;
                            }
                        }
                    }

                }
                catch(Exception Ex) { Logger.Log($"Exception occurred while generating fingerprint: {Ex.Message}", Logger.LogSeverity.Danger); }

                return retVal;
            }

            internal static string GetManagementObjectFromNameWithStatement(string className, string instanceName, string statement)
            {
                string retVal = "";

                try
                {
                    ManagementClass managementClass = new ManagementClass(className);
                    ManagementObjectCollection instances = managementClass.GetInstances();

                    foreach (ManagementBaseObject current in instances)
                    {
                        if (!(current[statement].ToString() != "True") && !(retVal != ""))
                        {
                            retVal = current[instanceName].ToString();
                            break;
                        }
                    }
                }
                catch (Exception Ex) { Logger.Log($"Exception occurred while generating fingerprint: {Ex.Message}", Logger.LogSeverity.Danger); }

                return retVal;
            }
        }
    }
}
