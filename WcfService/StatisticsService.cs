using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WcfService
{
    public class StatisticsService : IStatisticsService
    {
        public int SucessTacts { get; set ; }
        public int ErrorTacts { get; set; }
        public int AllTacts { get; set; }
    }
}