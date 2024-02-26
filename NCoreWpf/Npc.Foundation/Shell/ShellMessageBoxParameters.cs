using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Npc.Foundation.Shell
{
    public class ShellMessageBoxParameters
    {
        public ShellMessageBoxTypes Type { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Todo { get; set; }

        public Window OverrideOwner { get; set; }

        public bool IsDialog { get; set; }

        public ShellMessageBoxParameters(ShellMessageBoxTypes type, string message, bool isDialog = true)
        {
            this.Type = type;
            this.Message = message;
            this.IsDialog = isDialog;
        }

        public ShellMessageBoxParameters(ShellMessageBoxTypes type, string title, string message, bool isDialog = true) : this(type, message, isDialog)
        {
            this.Title = title;
        }

        public ShellMessageBoxParameters(ShellMessageBoxTypes type, string title, string message, string todo, bool isDialog = true) : this(type, message, isDialog)
        {
            this.Title = title;
            this.Todo = todo;
        }

        public ShellMessageBoxParameters(ShellMessageBoxTypes type, string title, string message, string todo, Window overrideOwner, bool isDialog = true) : this(type, message, isDialog)
        {
            this.Title = title;
            this.Todo = todo;
            this.OverrideOwner = overrideOwner;
        }

        //public ShellMessageBoxParameters(ShellMessageBoxTypes type, string title, string messageHeader, string message, bool isDialog = true)
        //    : this(type, message, isDialog)
        //{
        //    this.Title = title;
        //    this.MessageHeader = messageHeader;
        //}
    }
}
