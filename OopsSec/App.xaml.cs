using OopsSec.Classes;
using System;
using System.Windows;

namespace OopsSec
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Random _random = new Random(); // Generate random numbers
        private readonly int runForSeconds = 5; // Run effect for 5 seconds
        private readonly bool skipEffects = false; // For debugging purposes

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!skipEffects)
            {
                using (GDIEffects effects = new GDIEffects())
                {
                    effects.InvertScreenColors();
                    effects.FlipScreenVertical();
                    DateTime endTime = DateTime.Now.AddSeconds(runForSeconds);

                    while (DateTime.Now < endTime)
                    {
                        effects.ShiftRandomStripDown(_random.Next(160), _random.Next(6));
                    }
                }
            }

            // Show the warning
            Warning warning = new Warning();
            warning.Show();
        }
    }
}
