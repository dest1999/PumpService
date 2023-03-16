using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfService
{
    public interface IStatisticsService
    {
        int SucessTacts { get; set; }
        int ErrorTacts { get; set; }
        int AllTacts { get; set; }

    }
}
