using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly TUI tui = new TUI();

        static void Main(string[] args)
        {
            tui.Run();
        }
    }
}
