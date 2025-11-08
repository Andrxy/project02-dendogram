using System;
using System.Windows;

namespace Project02_Dendogram
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}