using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Npc.Foundation.Command
{
    public static class StringToCommand
    {
        public static void Execute(object dataContext, string command)
        {
            StringToCommand.Execute<object>(dataContext, command, null);
        }

        public static void Execute<T>(object dataContext, string command, T parameter)
        {
            // [NCS-2226] [Coverity : 127963]
            if (dataContext == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(command) == true)
            {
                return;
            }

            PropertyInfo pi = dataContext.GetType().GetProperty(command);
            if (pi == null)
            {
                return;
            }

            if (!(pi.GetValue(dataContext, null) is ICommand ic))
            {
                return;
            }

            ic.Execute(parameter);
        }
    }

    public class ToParameters<T>
    {
        public T Param { get; set; }

        public ToParameters(T param)
        {
            this.Param = param;
        }
    }
    public class ToParameters<T1, T2>
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }

        public ToParameters(T1 param1, T2 param2)
        {
            this.Param1 = param1;
            this.Param2 = param2;
        }
    }
    public class ToParameters<T1, T2, T3>
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }

        public ToParameters(T1 param1, T2 param2, T3 param3)
        {
            this.Param1 = param1;
            this.Param2 = param2;
            this.Param3 = param3;
        }
    }
    public class ToParameters<T1, T2, T3, T4>
    {
        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }

        public ToParameters(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            this.Param1 = param1;
            this.Param2 = param2;
            this.Param3 = param3;
            this.Param4 = param4;
        }
    }
}
