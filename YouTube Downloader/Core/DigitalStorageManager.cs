namespace YouTube.Downloader.Core
{
    using System;

    internal static class DigitalStorageManager
    {
        internal static long GetBytes(double size, string units)
        {
            return (long)(size * GetMultiplier(units));
        }

        internal static long GetMultiplier(string units)
        {
            switch (units)
            {
                case "b":
                case "B":
                    return 1L;

                case "kb":
                case "Kb":
                    return 125L;

                //case "Kib":
                //    return 128L;

                case "kB":
                case "KB":
                    return 1000L;

                case "KiB":
                    return 1024L;

                case "mb":
                case "Mb":
                    return 125L * 1000L;

                //case "Mib":
                //    return 131_072L;

                case "mB":
                case "MB":
                    return 1000L * 1000L;

                case "MiB":
                    return 1024L * 1024L;

                case "gb":
                case "Gb":
                    return 125L * 1000L * 1000L;

                //case "Gib":
                //    return 134_200_000L;

                case "gB":
                case "GB":
                    return 1000L * 1000L;

                case "GiB":
                    return 1024L * 1024L * 1024L;

                case "tb":
                case "Tb":
                    return 125L * 1000L * 1000L * 1000L;

                //case "Tib":
                //    return 137_400_000_000L;

                case "tB":
                case "TB":
                    return 1000L * 1000L * 1000L * 1000L;

                case "TiB":
                    return 1024L * 1024L * 1024L * 1024L;

                default:
                    throw new InvalidOperationException("Invalid units for multiplier.");
            }
        }
    }
}