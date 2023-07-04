using System.Runtime.InteropServices;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace gcgcg
{
    public static class Program
    {
        // [DllImport("kernel32.dll")]
        // private static extern bool AllocConsole();

        private static void Main()
        {
            // AllocConsole();

            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 800),
                Title = "CG_N4",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            try
            {
                using (var window = new Mundo(GameWindowSettings.Default, nativeWindowSettings))
                {
                    window.Run();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                System.Console.ReadLine();
            }
        }
    }
}
