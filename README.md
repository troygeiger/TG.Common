# TG.Common

This library provides utilities for logging to file, object cloning, delayed method invocation, task helpers, and other small helpers.
It targets .NET Standard 2.0 and 2.1 and has nullable reference types enabled.

Note on WinForms helpers: InputBox/ExMessageBox/WaitForm have moved to a separate repository ([TG.Common.WinForms](https://github.com/troygeiger/TG.Common.WinForms)) and are no longer part of this package.

## AssemblyInfo

This class helps to get various Assembly information, such as the Version,
InformationVersion, Title or Company. The default assembly referenced is Assembly.GetEntryAssembly() (with a safe fallback to the executing assembly).

### Basic AssemblyInfo Usage

```cs
Console.WriteLine(AssemblyInfo.Title);
Console.WriteLine(AssemblyInfo.InformationalVersion);
```

### Changing the referenced assembly

```cs
AssemblyInfo.ReferenceAssembly = Assembly.GetCallingAssembly();
```

Any subsequent calls on the AssemblyInfo properties will pull from the new
ReferenceAssembly.

## AppData

The AppData class is a helper for generating a folder in the user's AppData
folder. If the folder doesn't exist, it will automatically be created.
There are several schemes to choose from for the subfolder structure.
For Windows, that would be at C:\Users\\\<user>\Roaming.
In Linux, that would be at ~/.config.

### Schemes

- CompanyTitle
  - Generates %AppData%\AssemblyInfo.Company\AssemblyInfo.Title
- CompanyProduct
  - Generates %AppData%\AssemblyInfo.Company\AssemblyInfo.Product
- Company
  - Generates %AppData%\AssemblyInfo.Company
- Title
  - Generates %AppData%\AssemblyInfo.Title
- Product
  - Generates %AppData%\AssemblyInfo.Product

You can also call AppData.GetAppDataPath(AppDataSchemes) for better control or
setting the property AppData.DefaultScheme.

## LogManager

The LogManager class is a simple file logger were you only need something small
to log to a file. It is suitable if you only need a small library to write error
to a log. It doesn't currently have any mechanism for writing for only certain
log levels but that could be added if there is interest.

### Log Example

```cs
// Sets up logging folder using AppData.AppDataPath/Logging.
// Setting AppData.DefaultScheme can control the folder structure.
LogManager.InitializeDefaultLog();

try
{
    throw new Exception("Something bad happened!");
}
catch (Exception ex)
{
    LogManager.WriteExceptionToLog(ex);
}
```

## Crypto

Modern symmetric encryption with AES-CBC and a random IV per message.

- New encryptions: payload is IV || CIPHERTEXT (IV is 16 bytes)
- Decryption first attempts the new AES format, then falls back to legacy Rijndael with the historical fixed IV for backward compatibility
- Keys: input key bytes are normalized to 32 bytes (SHA-like padding behavior preserved from prior version)

Examples

```csharp
var crypto = new TG.Common.Crypto("my-secret-key");
string cipher = crypto.EncryptBase64("Hello, world");
string plain = crypto.DecryptBase64(cipher); // "Hello, world"
```

## DelayedMethodInvoker

This class provides a way to invoke a method after a set amount of time. Once
initialized, call Invoke and the internal timer is started. You can also
call the RestartTimer method and the timer will start/restart. That can be
useful for "debouncing" a button click.

## WinForms (moved)

The WinForms helpers (InputBox, ExMessageBox, WaitForm) were split into a separate package to keep TG.Common cross-platform. See the TG.Common.WinForms repository/package.

## Miscellaneous

This class has one-off helpers. The only notable one would be CloneObject; which
can do a deep clone of an object.

## Task helpers

Includes helpers like SafeFireAndForget for safely ignoring tasks while still catching exceptions.

```csharp
// Attach a global exception handler if desired
TG.Common.TaskHelpers.DefaultSafeFireAndForgetExceptionHandler = ex => TG.Common.LogManager.WriteExceptionToLog(ex);

// Fire and forget safely
SomeAsyncCall().SafeFireAndForget(onException: ex => Console.WriteLine(ex));
```

## Target frameworks and nullability

- Target frameworks: netstandard2.0; netstandard2.1
- Nullable reference types: enabled
