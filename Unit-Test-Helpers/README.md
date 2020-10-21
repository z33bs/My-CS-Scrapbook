# Unit-Test-Helpers
A collection of Helpers that make unit testing easier / more effective. Note, example code uses Xunit mostly.

## Testing IDisposable
From https://www.inversionofcontrol.co.uk/unit-testing-finalizers-in-csharp/

Write a class that implements `DisposableBase.cs`, such as `DisposableTestClass.cs`. Then unit test explicit disposal (by calling `Dispose()`:
```c#
[Fact]
public void Dispose_CallsExplicitAndImplicitDisposal()
{
    // Arrange
    bool @explicit = false;
    bool @implicit = false;
    var disposable = new Disposable(
        onExplicitDispose: () => @explicit = true,
        onImplicitDispose: () => @implicit = true);
        
    // Act
    disposable.Dispose();
    
    // Assert
    Assert.True(@explicit);
    Assert.True(@implicit);
}
```

To test the implicit GC'ed dispose. How do we make a finalizer deterministic? The secret is a combination of `WeakReference[<T>]`, delegated execution of your test action, and blocked execution until the finalizers have executed:

```c#
[Fact]
public void Dispose_CallsImplicitOnlyOnFinalization()
{
    // Arrange
    bool @explicit = false;
    bool @implicit = false;
    WeakReference<Disposable> weak = null;
    Action dispose = () => 
    {
        // This will go out of scope after dispose() is executed
        var disposable = new Disposable(
            onExplicitDispose: () => @explicit = true,
            onImplicitDispose: () => @implicit = true);
        weak = new WeakReference<Disposable>(disposable, true);
    };
        
    // Act
    dispose();
    GC.Collect(0, GCCollectionMode.Forced);
    GC.WaitForPendingFinalizers();
    
    // Assert
    Assert.False(@explicit); // Not called through finalizer
    Assert.True(@implicit);
}
```

Let's look at what is going on here in detail:

- `WeakReference weak = null;` - we can't initialize the weak reference until we've created our disposable - which we don't want to do in the current scope
- `Action dispose = () => ...` - we create a delegate we can execute later in the test
- `weak = new WeakReference(disposable, true);` - Create our weak reference, rooted in the outer scope, and passing `true` to the second argument will allow it to continue tracking the object *after*finalization
- `dispose();` - Execute our test action
- `GC.Collect(0, GCCollectionMode.Forced);` - Force the GC to collect at this point. Given we havent GC'd at this point before and our references are heap allocated, this means they exist in generation 0
- `GC.WaitForPendingFinalizers();` - Block until the GC has finished processing the finalizer queue for collected objects

With this general pattern of weak references, delegated actions, GC collection and finalization, we can test our finalizer code deterministically.

## DeterministicTaskScheduler
From [Sven Grand's Article](https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/november/async-programming-unit-testing-asynchronous-code-three-solutions-for-better-tests)
To test async code, allowing you to step through the tasks programatically (deterministically) in your test code. Not that this requres the tested method to accept a TaskScheduler so that you can test with this user-specified scheduler. Example of use is:
```c#
[Theory]
[InlineData(500)]
[InlineData(0)]
public void AsyncCommand_ExecuteAsync_IntParameter_Test(int parameter)
{
    //Arrange
    var dts = new DeterministicTaskScheduler();

    ICommand command = new SafeCommand<int>(IntParameterTask,dts,null,null);

    //Act
    command.Execute(parameter);
    dts.RunTasksUntilIdle();

    //Assert

}

```

The method overload looks like:
```c#
/// <summary>
/// For Unit Testing. Command runs using the specified <see cref="TaskScheduler"/>
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public SafeCommand(
    Func<T, Task> executeFunction,
    TaskScheduler scheduler,
    IViewModelBase viewModel = null,
    Action<Exception> onException = null,
    Func<T, bool> canExecute = null,
    //bool mustRunOnCurrentSyncContext is moot
    bool isBlocking = true
    )
```
