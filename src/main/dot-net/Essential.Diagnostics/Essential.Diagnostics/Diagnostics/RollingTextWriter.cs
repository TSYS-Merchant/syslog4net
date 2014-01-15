//// Class forked from Essential.Diagnostics - https://essentialdiagnostics.codeplex.com/ - RELEASE 1.2.501 (Wed May 1, 2013 at 3:00 AM) - Copyright 2010 Sly Gryphon
//// Updated Class - 1/14/2014 - Copyright © 2014 Merchant Warehouse
//// All Code Released Under the MS-PL License: http://opensource.org/licenses/MS-PL

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Essential.IO;

namespace Essential.Diagnostics
{
    /// <summary>
    /// Wraps and manages an instnace of TextWriter to allow for rolling updates to a text file from multiple threads with minimum blocking. 
    /// </summary>
    class RollingTextWriter : IDisposable
    {
        const int _maxStreamRetries = 5;

        private string _currentPath;
        private TextWriter _currentWriter;
        private object _fileLock = new object();
        private string _filePathTemplate;
        private IFileSystem _fileSystem = new FileSystem();
        TraceFormatter traceFormatter = new TraceFormatter();

        /// <summary>
        /// Creates a new instance of a <see cref="RollingTextWriter"/>
        /// </summary>
        /// <param name="filePathTemplate">file path and naming template for log files</param>
        public RollingTextWriter(string filePathTemplate)
        {
            _filePathTemplate = filePathTemplate;
        }

        /// <summary>
        /// Gets the File path and naming template for generated log files. 
        /// </summary>
        public string FilePathTemplate
        {
            get { return _filePathTemplate; }
        }
        /// <summary>
        /// Gets or sets the <see cref="IFileSystem"/> instance to use when making operations to the file (functions as a repository class for the file)
        /// </summary>
        public IFileSystem FileSystem
        {
            get { return _fileSystem; }
            set
            {
                lock (_fileLock)
                {
                    _fileSystem = value;
                }
            }
        }

        /// <summary>
        /// Flushes any messages waiting in the buffer to the file.
        /// </summary>
        public void Flush()
        {
            lock (_fileLock)
            {
                if (_currentWriter != null)
                {
                    _currentWriter.Flush();
                }
            }
        }

        /// <summary>
        /// Writes a message to the log file
        /// </summary>
        /// <param name="eventCache">logging event context data</param>
        /// <param name="value">value to output to the file</param>
        public void Write(TraceEventCache eventCache, string value)
        {
            string filePath = GetCurrentFilePath(eventCache);
            lock (_fileLock)
            {
                EnsureCurrentWriter(filePath);
                _currentWriter.Write(value);
            }
        }

        /// <summary>
        /// Writes a message to the log file and appends a carriage return to create a new line
        /// </summary>
        /// <param name="eventCache">logging event context data</param>
        /// <param name="value">value to output to the file</param>
        public void WriteLine(TraceEventCache eventCache, string value)
        {
            string filePath = GetCurrentFilePath(eventCache);
            lock (_fileLock)
            {
                EnsureCurrentWriter(filePath);
                _currentWriter.WriteLine(value);
            }
        }

        /// <summary>
        /// Disposes of the current text writer and all internal disposable instances.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        static string getFullPath(string path, int num)
        {
            var extension = Path.GetExtension(path);
            return path.Insert(path.Length - extension.Length, "-" + num.ToString(CultureInfo.InvariantCulture));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_currentWriter != null)
                {
                    _currentWriter.Dispose();
                }
            }
        }

        private void EnsureCurrentWriter(string path)
        {
            // NOTE: This is called inside lock(_fileLock)
            if (_currentPath != path)
            {
                if (_currentWriter != null)
                {
                    _currentWriter.Close();
                }

                var num = 0;
                var stream = default(Stream);

                while (stream == null && num < _maxStreamRetries)
                {
                    var fullPath = num == 0 ? path : getFullPath(path, num);
                    try
                    {
                        stream = FileSystem.Open(fullPath, FileMode.Append, FileAccess.Write, FileShare.Read);

                        this._currentWriter = new StreamWriter(stream);
                        this._currentPath = path;

                        return;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        throw;
                    }
                    catch (IOException)
                    {

                    }
                    num++;
                }

                throw new InvalidOperationException(Resource.RollingTextWriter_ExhaustedLogfileNames);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.DateTimeOffset", Justification = "Deliberate dependency, .NET 2.0 SP1 required.")]
        private string GetCurrentFilePath(TraceEventCache eventCache)
        {
            var result = StringTemplate.Format(CultureInfo.CurrentCulture, FilePathTemplate,
                delegate(string name, out object value, string outputTemplate)
                {
                    switch (name.ToUpperInvariant())
                    {
                        case "APPLICATIONNAME":
                            value = traceFormatter.FormatApplicationName();
                            break;
                        case "DATETIME":
                        case "UTCDATETIME":
                            //value = TraceFormatter.FormatUniversalTime(eventCache);

                            DateTimeOffset utc = TraceFormatter.FormatUniversalTime(eventCache);

                            // TODO: Cultural settings too?
                            value = string.IsNullOrEmpty(outputTemplate) ? utc.ToString() : utc.ToString(outputTemplate);
                            break;
                        case "LOCALDATETIME":
                            //value = TraceFormatter.FormatLocalTime(eventCache);
                            DateTimeOffset localTime = TraceFormatter.FormatLocalTime(eventCache);

                            // TODO: Cultural settings too?
                            value = string.IsNullOrEmpty(outputTemplate) ? localTime.ToString() : localTime.ToString(outputTemplate);
                            break;
                        case "MACHINENAME":
                            value = Environment.MachineName;
                            break;
                        case "PROCESSID":
                            value = traceFormatter.FormatProcessId(eventCache);
                            break;
                        case "PROCESSNAME":
                            value = traceFormatter.FormatProcessName();
                            break;
                        case "APPDATA":
                            value = traceFormatter.HttpTraceContext.AppDataPath;
                            break;
                        default:
                            value = "{" + name + "}";
                            return true;
                    }
                    return true;
                });
            return result;
        }
    }
}
