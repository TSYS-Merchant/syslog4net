using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

using log4net;

using syslog4net;

// This is required for log4net to automatically load its configuration from a web.config or app.config file. This line may appear anywhere in the application
// the most popular locations are in an app's main class or in the AssemblyInfo.cs file.
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

/// <summary>
/// This very simple program creates a random number of worker threads, runs for a random (but short) period of time and sends messages to each of the threads randomly
/// if a thread recieves too many messages ("pokes") then the thread will throw an exception. All major states and transitions are logged. 
/// Note that this example is meant to show as many possible logging capibilities as possible in a short demo. Most real world logging scenarios would not be so verbose.
/// </summary>
class Program
{
    static ILog _log = LogManager.GetLogger(typeof(Program));

    public static Random Random = new Random();
    public static Collection<Worker> Workers = new Collection<Worker>();

    /// <summary>
    /// Main body hanldes app startup, top level application flow and worker creation
    /// </summary>
    /// <param name="args">no arguments are supported</param>
    public static void Main(string[] args)
    {
        _log.Error("TESTING", new Exception("THIS IS MY ERROR MESSAGE!", new Exception("THIS IS MY INNER EXCEPTION")));

        var zero = 0;

        try
        {
            var test = 99999 / zero;
        }
        catch (Exception ex)
        {
            _log.Error("ERROR", ex);
        }

        using (_log.StartMessage("Program start"))
        {
            _log.Info("Program start");

            using (_log.StartThreadActivity("NDC", "creating workers"))
            {
                // Run program
                int numberOfWorkers = Program.Random.Next(2, 4);

                _log.InfoFormat("Creating {0} workers", numberOfWorkers);
                _log.InfoFormat("WOW! {0} workers!!", numberOfWorkers);

                for (int i = 1; i <= numberOfWorkers; i++)
                {
                    Worker worker = new Worker() { WorkerId = i };
                    Workers.Add(worker);
                }
                StartWorkers();
                foreach (Worker worker in Workers)
                {
                    worker.FinishedEvent.WaitOne();
                }
            }
            _log.Info("Program stop.");
        }

        Console.ReadLine();
    }

    /// <summary>
    /// Fire up all worker threads
    /// </summary>
    static void StartWorkers()
    {
        _log.Info("Starting workers");

        // Do work
        foreach (Worker worker in Workers)
        {
            ThreadPool.QueueUserWorkItem(worker.Work);
        }

        _log.Info("Finished Starting");
    }
}

/// <summary>
/// Controller for an instance of a woker thread, includes methods for messaging. 
/// </summary>
class Worker
{
    int _count;
    static ILog _log = LogManager.GetLogger(typeof(Worker));

    public AutoResetEvent FinishedEvent = new AutoResetEvent(false);
    public int WorkerId;
    public string Id
    {
        get
        {
            return string.Format("Worker {0}", WorkerId);
        }
    }

    /// <summary>
    /// Recieve a poke from another thread and report it in the log
    /// </summary>
    public void Poke()
    {
        // Work
        Thread.Sleep(Program.Random.Next(500));
        _count++;
        if (_count < 4)
            Console.WriteLine("Hello World {1}", Id, _count);
        else if (_count < 6)
        {
            Console.WriteLine("Hi", Id);
            _log.WarnFormat("Worker {0} getting annoyed", Id);
        }
        else
        {
            _log.ErrorFormat("Worker {0} - too many pokes", Id);
        }
    }

    /// <summary>
    /// Primary control method for worker thread, handles primary control loop and checks poke counts.
    /// </summary>
    /// <param name="state">thread state object</param>
    public void Work(object state)
    {
        using (_log.StartThreadActivity("NDC", "Work"))
        {
            log4net.LogicalThreadContext.Properties["WorkerId"] = WorkerId;

            _log.InfoFormat("Worker {0} start.", Id);

            // Do work
            int numberOfPokes = Program.Random.Next(3, 6);

            log4net.LogicalThreadContext.Properties["NumberOfPokes"] = numberOfPokes;

            _log.InfoFormat("Worker {0} will poke {1} times", Id, numberOfPokes);
            for (int i = 1; i <= numberOfPokes; i++)
            {
                Thread.Sleep(Program.Random.Next(500));
                int index = Program.Random.Next(Program.Workers.Count);

                _log.DebugFormat("Worker {0} poking {1}", Id, Program.Workers[index].Id);
                Program.Workers[index].Poke();
            }
            FinishedEvent.Set();

            _log.Info("Worker stop");
        }
    }
}