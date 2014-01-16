using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Essential.Diagnostics.Tests.Utility
{
    static class ConfigUtility
    {
        public static string GetConfigDirFromTestRunDirectory(string testDir)
        {
            return Path.Combine( testDir + "\\Essential.Diagnostics.Tests.dll.config");

        }

    }
}
