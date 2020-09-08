using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using Goodwitch.CommonUtils;

namespace Goodwitch.Modules
{
    /// <summary>
    /// Highest Hierarchical Module(1) - Detecting internal memory modification and memory related detection.
    /// </summary>
    internal class OscarModule : ModuleBase
    {
        private List<Assembly> loadedSafeAssemblies;
        private List<Assembly> abnormalLoadedAssemblies;

        private List<string> detectedKnownCheatByteSignatures = new List<string>() { "AbgAGANMAbgAGAOwAbgAGAAUBbgAGACABbg" };

        internal OscarModule()
        {
            loadedSafeAssemblies = new List<Assembly>();
            abnormalLoadedAssemblies = new List<Assembly>();
        }

        internal override async void StartModule()
        {
            await Task.Run(() => ForceLoadAndStoreAllAssemblies());

            await Task.Run(() => {

                CommonUtils.Time.Tick.OnTick += DetectAbnormalLoadings;
                CommonUtils.Time.Tick.OnTick += DetectKnownCheatSignature;
            });

            await Task.Run(() => base.StartModule());
        }

        private void ForceLoadAndStoreAllAssemblies()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            loadedSafeAssemblies = loadedAssemblies;
        }

        private void DetectAbnormalLoadings()
        {
            if (loadedSafeAssemblies != null)
            {
                var appDomainASMList = AppDomain.CurrentDomain.GetAssemblies().ToList();

                foreach (var asm in appDomainASMList)
                {
                    if (abnormalLoadedAssemblies.Contains(asm))
                        continue;

                    if (asm == appDomainASMList.Last() && loadedSafeAssemblies.Contains(asm))
                        break;

                    if (!loadedSafeAssemblies.Contains(asm))
                    {
                        Logger.Log($"Abnormal assembly: {asm} has been loaded.", Logger.LogSeverity.Warning);

                        abnormalLoadedAssemblies.Add(asm);

                        FlagRaiserService.RaiseFlag(Enums.DetectionFlags.ABNORMAL_LOADING_DETECTED, ConstructFlagInformationForAssembly(asm));
                    }
                }
            }
        }

        private void DetectKnownCheatSignature()
        {
            if(abnormalLoadedAssemblies.Count != 0)
            {
                foreach(var asm in abnormalLoadedAssemblies)
                {
                    string currASMBytes = Convert.ToBase64String(File.ReadAllBytes(asm.Location));

                    foreach(var detectedSignature in detectedKnownCheatByteSignatures)
                    {
                        if (currASMBytes.Contains(detectedSignature))
                        {
                            BanMessageDisplayer.DisplayBanMessage();
                        }
                    }
                }
            }
        }

        private string ConstructFlagInformationForAssembly(Assembly asm)
        {
            return $"\nAssmebly Full Name: {asm.FullName}" +
                   $"\nAssembly Name: {asm.GetName().Name}" +
                   $"\nAssembly Path: {asm.Location}" +
                   $"\nAssembly ManifestModule Name: {asm.ManifestModule.Name}" +
                   $"\nAssembly HashType: {asm.GetName().HashAlgorithm.ToString()}" +
                   $"\nAssembly Version: {asm.GetName().Version}" +
                   $"\nAssembly Byte Signature: {Convert.ToBase64String(File.ReadAllBytes(asm.Location))}";
        }
    }
}
