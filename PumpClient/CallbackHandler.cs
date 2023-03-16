using PumpClient.PumpServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfService;

namespace PumpClient
{
    internal class CallbackHandler : IPumpServiceCallback
    {

        public void UpdateStatistics(StatisticsService statistics)
        {
            Console.Clear();
            Console.WriteLine(statistics.AllTacts);
            Console.WriteLine(statistics.SucessTacts);
            Console.WriteLine(statistics.ErrorTacts);
        }

    }
}
