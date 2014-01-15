using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// This very simple program creates a random number of worker threads, runs for a random (but short) period of time and sends messages to each of the threads randomly
/// if a thread recieves too many messages ("pokes") then the thread will throw an exception. All major states and transitions are logged. 
/// Note that this example is meant to show as many possible logging capibilities as possible in a short demo. Most real world logging scenarios would not be so verbose.
/// </summary>
class Program
{
    static TraceSource _trace = new TraceSource("HelloProgram");
    public static Random Random = new Random();
    public static Collection<Worker> Workers = new Collection<Worker>();

    /// <summary>
    /// Main body hanldes app startup, top level application flow and worker creation
    /// </summary>
    /// <param name="args">no arguments are supported</param>
    public static void Main(string[] args)
    {
        // Trace start
        Trace.CorrelationManager.ActivityId = Guid.NewGuid();
        Trace.CorrelationManager.StartLogicalOperation("Main");
        _trace.TraceData(TraceEventType.Critical, 1000, "hello", 5, 0, "soet");
        _trace.TraceEvent(TraceEventType.Start, 1000, "Program start. {0} + {1}", 1, "TEST");
        // Run program
        int numberOfWorkers = Program.Random.Next(2, 4);
        _trace.TraceEvent(TraceEventType.Information, 2000, "Creating {0} workers", numberOfWorkers);
        for (int i = 1; i <= numberOfWorkers; i++)
        {
            Worker worker = new Worker() { Id = string.Format("Worker {0}", i) };
            Workers.Add(worker);
        }
        StartWorkers();
        foreach (Worker worker in Workers)
        {
            worker.FinishedEvent.WaitOne();
        }
        // Trace stop
        _trace.TraceEvent(TraceEventType.Stop, 8000, "Program stop.");
        Trace.CorrelationManager.StopLogicalOperation();
        _trace.Flush();
    }

    /// <summary>
    /// Fire up all worker threads
    /// </summary>
    static void StartWorkers()
    {
        // Trace transfer in
        Guid newActivity = Guid.NewGuid();
        _trace.TraceTransfer(6011, "Transferred to Start", newActivity);
        Guid oldActivity = Trace.CorrelationManager.ActivityId;
        Trace.CorrelationManager.ActivityId = newActivity;
        _trace.TraceEvent(TraceEventType.Start, 1010, "Starting workers.");
        // Do work
        foreach (Worker worker in Workers)
        {
            ThreadPool.QueueUserWorkItem(worker.Work);
        }
        // Trace transfer back
        _trace.TraceTransfer(6012, "Transferred back", oldActivity);
        _trace.TraceEvent(TraceEventType.Stop, 8010, "Finished starting.");
        Trace.CorrelationManager.ActivityId = oldActivity;
    }
}

/// <summary>
/// Controller for an instance of a woker thread, includes methods for messaging. 
/// </summary>
class Worker
{
    int _count;
    static TraceSource _trace = new TraceSource("HelloWorker");
    public AutoResetEvent FinishedEvent = new AutoResetEvent(false);
    public string Id;

    /// <summary>
    /// Recieve a poke from another thread and report it in the log
    /// </summary>
    public void Poke()
    {
        // Trace - mark with logical operation
        Trace.CorrelationManager.StartLogicalOperation(string.Format("Poked:{0}", Id));
        _trace.TraceEvent(TraceEventType.Verbose, 0, "Worker {0} was poked", Id);
        // Work
        Thread.Sleep(Program.Random.Next(500));
        _count++;
        if (_count < 4)
            Console.WriteLine("Hello World {1}", Id, _count);
        else if (_count < 6)
        {
            Console.WriteLine("Hi", Id);
            _trace.TraceEvent(TraceEventType.Warning, 4500, "Worker {0} getting annoyed", Id);
        }
        else
        {
            _trace.TraceEvent(TraceEventType.Error, 5500, "Worker {0} - too many pokes", Id);
        }
        // Trace - end logical operation
        Trace.CorrelationManager.StopLogicalOperation();
    }

    /// <summary>
    /// Primary control method for worker thread, handles primary control loop and checks poke counts.
    /// </summary>
    /// <param name="state">thread state object</param>
    public void Work(object state)
    {
        // Trace transfer to thread
        Guid newActivity = Guid.NewGuid();
        _trace.TraceTransfer(6501, "Transfered to worker", newActivity);
        Trace.CorrelationManager.ActivityId = newActivity;
        Trace.CorrelationManager.StartLogicalOperation(string.Format("Worker:{0}", Id));
        _trace.TraceEvent(TraceEventType.Start, 1500, "Worker {0} start.", Id);
        // Do work
        int numberOfPokes = Program.Random.Next(3, 6);
        _trace.TraceEvent(TraceEventType.Information, 2500, "Worker {0} will poke {1} times", Id, numberOfPokes);
        for (int i = 1; i <= numberOfPokes; i++)
        {
            Thread.Sleep(Program.Random.Next(500));
            int index = Program.Random.Next(Program.Workers.Count);
            _trace.TraceEvent(TraceEventType.Verbose, 0, "Worker {0} poking {1}", Id, Program.Workers[index].Id);
            Program.Workers[index].Poke();
        }
        FinishedEvent.Set();
        // Trace stop (no transfer)
        _trace.TraceEvent(TraceEventType.Stop, 8500, "Worker stop.");
        Trace.CorrelationManager.StopLogicalOperation();
    }
}