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
