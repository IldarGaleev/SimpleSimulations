using CommandLine;

namespace Gravity.Primitives
{
    public class CommandLineOptions
    {
        [Option(
            longName: "celestialfile",
            shortName: 'c',
            Required = false,
            HelpText = "csv file with initial celestial bodies",
            Default = "Data/celestial_init.csv"
            )]
        public string CelestialFileName { get; set; }

        [Option(
            longName: "fullscreen",
            shortName: 'f',
            Required = false,
            HelpText = "Fullscreen mode",
            Default = false
            )]
        public bool FullScreen { get; set; }

        [Option(
            longName: "width",
            shortName: 'W',
            Required = false,
            HelpText = "Window width",
            Default = 800
            )]
        public int Width { get; set; }

        [Option(
            longName: "height",
            shortName: 'H',
            Required = false,
            HelpText = "Window height",
            Default = 600
            )]
        public int Height { get; set; }

        [Option(
            longName: "track",
            shortName: 't',
            Required = false,
            HelpText = "Show objects tracks (also you can press \"T\" key to toggle this option)",
            Default = false
            )]
        public bool ShowTracks { get; set; }

        [Option(
            longName: "interaction",
            shortName: 'i',
            Required = false,
            HelpText = "Show interaction vectors (also you can press \"A\" key to toggle this option)",
            Default = false
            )]
        public bool ShowInteractions { get; set; }

        [Option(
            longName: "worldsize",
            shortName: 's',
            Required = false,
            HelpText = "How many meters",
            Default = 1
            )]
        public double WorldSize { get; set; }

        //[Option(
        //    longName: "timescale",
        //    Required = false,
        //    HelpText = "Time scale",
        //    Default = 1
        //    )]
        //public double TimeScale { get; set; }

        [Option(
            longName: "origin",
            shortName: 'o',
            Required = false,
            HelpText = "Display origin point",
            Default = false
            )]
        public bool DisplayOriginPoint { get; set; }
    }
}
