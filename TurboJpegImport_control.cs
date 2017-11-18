using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurboJpegWrapper
{
    /// <summary>
    /// seperate class required to disable static constructor function in TurboJpegImport
    /// </summary>
    public static class TurboJpegImport_control
    {
        /// <summary>
        /// whether exceptions are thrown for platforms with incomplete support
        /// </summary>
        public static bool isThrowOnPlatformErrors = true;
        /// <summary>
        /// whether library load is attempted automatically
        /// (if disabled, can still call TurboJpegImport.Load(..))
        /// </summary>
        public static bool isAutoLoadLibrary = true;
    }
}
