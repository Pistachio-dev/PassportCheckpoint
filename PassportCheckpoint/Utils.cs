using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PassportCheckpoint
{
    public static class Utils
    {
        public static string GetWorldName(uint rowId)
        {
            return Plugin.Data.GetExcelSheet<World>().GetRow(rowId).Name.ToString();
        }
    }
}
