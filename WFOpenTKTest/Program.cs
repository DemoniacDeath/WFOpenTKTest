using System;

namespace WFOpenTKTest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (Main main = new Main())
            {
                main.Run(60.0);
            }
        }
    }
}
