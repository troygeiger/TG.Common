# TG.Common

This library provides methods for logging to file, object cloning delaying when
to invoke a method and various other useful tools.
I try to add helpers that I end up using regularly over the years, such as the
InputBox, AppData and AssemblyInfo.

## AssemblyInfo

This class helps to get various Assembly information, such as the Version,
InformationVersion, Title or Company. The default assembly referenced is Assembly.GetEntryAssembly().

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

This class is pretty outdated but can still be useful if you need some simple
encryption. It has options for encrypting/decrypting strings, byte arrays and
Base64 strings.

## DelayedMethodInvoker

This class provides a way to invoke a method after a set amount of time. Once
initialized, call Invoke and the internal timer is started. You can also
call the RestartTimer method and the timer will start/restart. That can be
useful for "debouncing" a button click.

## WinForms

There are three included forms that can be used if targeting WinForms. InputBox
is the most common form I use since that does't seem to be built into C#.

### Forms

- InputBox
  - Prompt for user input.
- ExMessageBox
  - Honestly, I don't remember why I created this form.
- WaitForm
  - A popup form showing a marquee progress bar and message.

## Miscellaneous

This class has one-off helpers. The only notable one would be CloneObject; which
can do a deep clone of an object.
