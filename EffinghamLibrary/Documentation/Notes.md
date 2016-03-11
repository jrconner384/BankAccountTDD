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

#### General Topics
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
    * "The worst of all worlds" according to Jeff.
        * Maybe because of single inheritance? I'm not really following his reasoning.
* `sealed` (like Java's `final` as applied to classes)
* `virtual`, `override`, `new` to hide inherited members ("hiding" or "shadowing")
* `base` to call methods and constructors starting one level up in the inheritance hierarchy
* Custom exceptions
    * Inherit from `ApplicationException`
    * Implement `base()`, `base(string message)`, `base(string message, Exception inner)` constructors
    * Optionally add members
    * Allows for more granular error handling by checking exception type
* Inherit from generic type
    * `public class CustomList : List<int>`
    * `public class CustomList<T> : List<T>`
* Extension methods
    * `static` class
    * `static` method
    * `this` keyword on first parameter to method

#### Module 7: Accessing a Database
* Creating and using Entity Data models
    * "Code First"
        * Write C# first then let Entity Framework create DB using code structure
    * "Database First"
        * Create EF data model using existing database schema
        * Automatically divines entity relationships
        * EF versions don't really get along well
    * "Model First"
        * Add "ADO.NET Entity Data Model" > "Empty EF Designer Model"
        * Basically create ER diagram and use it to generate DDL for the database and C#
* Querying data by using LINQ

#### Module 8: Creating and Accessing Data
* Web connectivity
    * `System.Net` namespace
        * `WebRequest` (abstract)
        * `WebResponse` (abstract)
        * `HttpWebRequest`
        * `HttpWebResponse`
        * `FtpWebRequest`
        * `FtpWebResponse`
        * `FileWebRequest`
        * `FileWebResponse`
    * `DataContract` and `DataMember` attributes expose types from a web service
    * Creating a request and processing a response
        * Get a URI
        * Create a request object
        * Get a response object from the request object
        * Read the properties in the response object
    * Authenitcating a web request
        * Create the request object
            * Use the `NetworkCredential` class
            * Use the `CredentialCache` class
                * Info from the OS
                * `CredentialCache.DefaultCredentials;`
            * Use the `X509Certificate2` class
                * If the service requires a certificate
    * Sending and receiving data
        * Send data
            * Create URI
            * Encode data (i.e. JSON object)
            * Create request
            * Set stuff like request method, content type
            * Get the request's data stream
            * Write to the stream
            * Close the stream
        * Process response
            * Get response from request
            * Get stream from response
            * Read stream
            * Close stream
    * Creating WCF data service (distinct from other WCF services)
        * AKA "Project Astoria", "ADO.NET Data Services", "WCF Data Services"
        * Might not be on exam since it kind of flopped
        * This is the "automated magical" way to create services
        * Auto-generates service code (to an extent)
        * Can specify which entities can be accessed and if they are read or read/write
        * Doesn't give granular control (reads all or nothing)
        * Data format "OData" is an extension of RSS ATOM
            * XML-based
            * Pushed by Microsoft
            * Everyone went with JSON instead
            * Built-in means of querying data (looks RESTful)
        * Can expose specific service methods

#### Module 9: Designing the UI for Graphical Applications
* General WPF
    * Default template determines how controls render
        * Obviously can be overridden. Can write custom templates.
    * Information moves from outer elements to inner elements (i.e. tag nesting creates information scope analagous to block nesting in code)
* Using XAML
    * Rendering engine follows top-down hierarchy
    * Properties of controls can be determined by the tags they are nested within
        * Called "attached properties" since the enclosing tag attached propery information to the enclosed tags.
* Binding controls
    * Use `xmlns` property to bring in a data class' namespace.
    * In control to bind data to, do something like this

```
<Slider x:Name="mySlider" Minimum="8" Maximum="50" />
<TextBlock x:Name="txtTarget" FontSize="{Binding ElementName=mySlider Path=Value}" />
<!-- This will take the value of the slider to determine the font size in the TextBlock -->
```
* Stying UI

#### Module 11: Integrating with Unmanaged Code
* Basically, reach outside of .NET to interact with COM objects
* dynamic data type
    * i.e. `ViewBag` in MVC projects
    * They don't do compile-time checking.
        * Delays checking until runtime.
        * No IntelliSense.
        * Can assign anything to any property.
    * Use `dynamic` keywork to create your own.
        * `dynamic thing = "dynamic thing";`
    * Useful for interacting with COM objects
    * Added to help implemented dynamically-typed languages on the CLR

#### Module 12: Reusable Types and Assemblies
* Export library project as DLL
    * Put it in the GAC
        * Go to project properties > Signing Tab
        * Select "Sign the assembly"
        * Create a Strong Name Key file
        * Every time the project is compiled, a digital signature will be applied and it can be pushed to the GAC
        * Need to update assembly version to make sure not to break users of old versions
            * GAC will provide the latest build of the DLL of the specified version number
* GAC was created to maintain multiple versions of the same assemblies so one program using a new version doesn't break another program using an old version
* Two tools:
    * sn.exe generates strong name key file via CLI
    * gacutil.exe to move things into and out of the GAC via CLI
* ildsam.exe to decompile MSIL

#### Miscellaneous
* Overloading operators

```
public static decimal operator +(IBankAccountMultipleCurrency leftAddend, IBankAccountMultipleCurrency rightAddend)
{
    return rightAddend.Balance + leftAddend.Balance;
}

public static IBankAccountMultipleCurrency operator +(IBankAccountMultipleCurrency account, decimal amount)
{
    account.Deposit(amount);
    return account;
}
```
