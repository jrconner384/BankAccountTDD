### Threading

```
// I didn't manage to catch all the relevant stuff when I started writing these notes.
Task<string> firstTask = new Task<string>(() => return "Hello");
Task<string> secondTask = firstTask.ContinueWith(() =>
{
    // Some executable stuff
});

// Child Tasks
var child = Task.Run( () =>
{
    // Some executable stuff
}, TaskCreationOptions.AttachedToParent);
```

`AggregateException` is meant to collection exceptions so it can be iterated over to handle any arbitrary nested exceptions.

### Async Operations and UI
__Only safe to update UI from the main thread__

* To update UI from background:
    * Get the `Dispatcher` object for the thread that owns the UI element.
    * Call the `BeginInvoke` method.
    * Provide an `Action` delegate as an argument

### Exceptions from Awaitable Methods
* Use `try/catch`
* Subscribe to the `TaskScheduler.UnobservedTaskException event to create an event handler of __last resort__
* Doesn't fire all that often

### Synchronization Primitives
* `ManualResetEventSlim`
    * Limits resource access to one thread at a time.
* `SemaphoreSlim`
    * Can be registered with the OS - locks across multiple processes on the OS
    * Can lock resources across multiple programs/instances
    * Mutex is always registered with the OS
* `CountdownEvent`
    * Block a thread until a fixed number of tasks signal completion
* `ReaderWriterLockSlim`
    * Allow multiple threads to read a resources or a single thread to write to a resource at any one time.
* `Barrier`
    * Block multiple threads until they all satisfy a condition

### Concurrent Collections
__`System.Collections.Concurrent`__ namespace

* `ConcurrentBag<T>`
* `ConcurrentDictionary<TKey, TValue>`
* `ConcurrentQueue<T>`
* `ConcurrentStack<T>`
* `IProducerConsumerCollection<T>`
* `BlockingCollection<T>`
    * When you remove data, it blocks until it has data.
        * One thread is writing, one is reading. The reading thread won't read until the writing thread is done.
    * Some kind of pipe stream uses this collection to compress on one thread and encrypt on another once bytes are compressed

### March 11
* Adding in cross-layer access to SQL Server database (i.e. new IVault implementation).
    * The new vault uses a class which inherits from DbContext. This is not thread-safe.
    * Either need to add locking in the new vault or create new contexts for each atomic DB operation.
* Adding custom exception type.
* Adding helper methods to marshall and umarshall business objects.

### General Topics
* Named parameters
* Default parameters
* Generics
* Parameter arrays
    * Keyword `params`
    * `void method(int number, params string[] s)`
    * Like Java's `args`
    * Can be used on any type.
* Monitoring applications
    * Logging
        * Provides info to users and administrators
        * Windows event log
        * Text files
        * Custom logging destinations
    * Tracing
        * Provides information to developers
        * `Debug` and `Trace` classes
            * Same API in both classes
            * Debug compile targets execute `Debug` calls.
            * Release compile targets execute `Trace` calls.

```
// Go to Administrative Tools > Event Viewer
// Write to application event log
// Can create our own event log types
string eventLog = "Application";
string eventSource = "Logging Demo";
string eventMessage = "Hello from the Logging Demo application";

// They'll probably ask about this.
// Make sure to check if the source exists and create it if it doesn't.
if (!EventLog.SourceExists(eventSource)){
    EventLog.CreateEventSource(eventSource, eventLog);
}
// Write to the log
EventLog.WriteEntry(eventSource, eventMessage);

```

* Performance Counters
    * Administrative Tools > Performance Monitor (in Windows)
    * Can also Run > perfmon
    * Need to know reading and creating performance counters

```
// Example is a shortened version of what's in the class materials.

if (!PerformanceCounterCategory.Exists("FourthCoffeeOrders"))
{
    CounterCreationDataCollection counter = new CounterCreationDataCollection();
    CounterCreationData totalOrders = new CounterCreationData();
    totalOrders.CounterName = "# Orders";
    totalOrders.CounterHelp = "Total number of orders placed";
    totalOrders.CounterType = PerformanceCounterType.NumberOfItems32;
    counters.Add(totalOrders);
    PerformanceCounterCategory.Create("FourthCoffeeOrders", "A custom category");
}

PerformanceCounter counterOrders = new PerformanceCounter("FourthCoffeeOrders", "# Orders", false);

public void OrderCoffee()
{
    counterOrders.Increment();

    // Logic for ordering coffee
}
```

#### Module 4
__Creating Classes, etc.__

* Remember 'Arrange, Act, Assert' testing structure
* Avoid explicit interface implementations
    * When working of types using explicit implementations, you have to cast the variable to get at the interface's members
    * Useful when creating objects via composability.
    * Resolve namespace collisions.
    * Some MS documentation encourages explicit implementation.
* Generics
    * Type safety
    * No casting
    * No boxing and unboxing
    * Constrained generics
    * LIFOs, FIFOs, etc.
    * Collection Interfaces
        * `IEnumerable` and `IEnumerable<T>`
            * `GetEnumerator()` -> `IEnumerator<T>` should expose all members of collection (?)
            * Implemented by arrays
            * `Current` gets the item the enumerator is pointing to
            * `MoveNext` advances the enumerator to the next item
            * `Reset` moves the enumerator back to the start
        * `ICollection<T>` common to all generic collections
            * Can always use `foreach`
            * `Add`, `Clear`, `Contains`, `CopyTo`, `Remove`
        * `IList<T> : ICollection<T>`
            * Looking up items by index number
            * `Insert`
            * `RemoveAt`
            * `IndexOf`
        * `IDictionary<T> : ICollection<T>`
            * Key-value pairs
            * `Item`
            * `Keys`
            * `Values`

#### Module 5
* Inheritance
* `abstract`
* `sealed`
