using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WcfService
{
    public class ScriptService : IScriptService
    {
        private readonly IStatisticsService statisticsService;
        private readonly ISettingsService settingsService;
        private readonly IPumpServiceCallback pumpServiceCallback;
        private CompilerResults compilerResults = null;

        public ScriptService(IStatisticsService StatisticsService, ISettingsService SettingsService, IPumpServiceCallback PumpServiceCallback)
        {
            statisticsService = StatisticsService;
            settingsService = SettingsService;
            pumpServiceCallback = PumpServiceCallback;
        }

        public bool Compile()
        {
            CompilerParameters compilerParameters = new CompilerParameters();
            compilerParameters.GenerateInMemory = true;
            compilerParameters.ReferencedAssemblies.Add("System.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Core.dll");
            compilerParameters.ReferencedAssemblies.Add("System.Data.dll");
            compilerParameters.ReferencedAssemblies.Add("SystemWindows.Forms.dll");
            compilerParameters.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

            FileStream fileStream = new FileStream(settingsService.FileName, FileMode.Open);
            byte[] buffer;
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count, sum = 0;
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
            }
            finally
            {
                fileStream.Close();
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();
            compilerResults = provider.CompileAssemblyFromSource(compilerParameters, Encoding.UTF8.GetString(buffer));
            if (compilerResults.Errors != null && compilerResults.Errors.Count != 0)
            {
                //TODO errors statistics
                return false;
            }
            return true;
        }

        public void Run(int count)
        {
            if (compilerResults == null || (compilerResults != null && compilerResults.Errors != null && compilerResults.Errors.Count != 0))
            {
                if (Compile() == false)
                {
                    return;
                }
            }

            Type type = compilerResults.CompiledAssembly.GetType("Sample.SampleScript");
            if (type == null)
            {
                return;
            }
            MethodInfo entryPoint = type.GetMethod("EntryPoint");
            if (entryPoint == null)
            {
                return;
            }

            Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    if ((bool)entryPoint.Invoke(Activator.CreateInstance(type), null))
                    {
                        statisticsService.SucessTacts++;
                    }
                    else
                    {
                        statisticsService.ErrorTacts++;
                    }
                    statisticsService.AllTacts++;
                    pumpServiceCallback.UpdateStatistics(statisticsService);
                    Thread.Sleep(1000);
                }

            });

        }
    }
}