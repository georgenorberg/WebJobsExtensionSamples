# Azure Functions/WebJobs binding extension sample

This repo has a simple input and output binding for use with Azure Functions or WebJobs. The sample extension supports reading and writing name/value pairs from a text file. It demonstrates input bindings, output bindings, and converters.

To author a custom binding for Azure Functions, you must use the 2.0 runtime. See [Azure Functions 2.0 support](https://aka.ms/func-xplat).

For reference documentation on authoring an extension, see [Creating custom input and output bindings](https://github.com/Azure/azure-webjobs-sdk/wiki/Creating-custom-input-and-output-bindings).

To write a new value, use an `ICollector`:
```
[Sample] ICollector<string> output
```

To read, just bind to an object:
```
 [Sample(Name = "{name}")] string contents, 
```

## Sample projects

This sample has the following projects:

1. **SampleExtension**. This is the actual extension and has input and output bindings. If it is referenced by a Functions project in Visual Studio, the Functions runtime will automatically load the binding.
2. **SampleFunctions**. A sample Azure Functions Visual Studio project that uses **SampleExtension**.
3. **Host**. An executable for running the sample in WebJobs.
4. **Sample2Extension**. An extension that extends **SampleExtension**.

## Consuming the binding

To use the binding in C#, simply add a reference to the project or assembly and use the binding via attributes. When you run locally or in Azure, the extension will be loaded. 

For JavaScript, the process is currently manual. Do the following:
1. Copy the extension to an output folder such as "extensions". This can be done in a post-build step in the .csproj
2. Add the app setting `AzureWebJobs_ExtensionsPath` to local.settings.json (or in Azure, in App Settings). Set the value to the **parent** of the "extension" folder from the previous step.

Here's an example of using the writer functionality:

[WriterFunction](https://github.com/Azure/WebJobsExtensionSamples/blob/master/FunctionApp/WriterFunction.cs#L14)

```csharp
[FunctionName("WriterFunction")]
public static void Run(
    [HttpTrigger] string item,
    [Sample] ICollector<SampleItem> sampleOutput, TraceWriter log)
```

And here's how to read the key/value pairs:

[ReaderFunction](https://github.com/Azure/WebJobsExtensionSamples/blob/master/FunctionApp/ReaderFunction.cs#L15)

```csharp
[FunctionName("ReaderFunction")]
public static void Run(
    [HttpTrigger] SampleItem item,
    [Sample(FileName = "{Name}")] string contents, // Bind to SampleExtension  
    TraceWriter log)
```            

## Creating a custom binding

To author an extension, you must perform the following tasks:

1. Declare an attribute, such as `[Blob]`. Attributes are how customers consume the binding. 
2. Choose one or more binding *rules* to support. Rules are **BindToInput** (input binding) and **BindToCollector** (output binding).
3. Add some converters to make the rules more expressive, such as a conversion from a POCO to a JObject.

### 1. Define binding attribute(s)

This sample extension defines [`SampleAttribute`](blob/master/SampleExtension/SampleAttribute.cs#L15) in the **SampleExtension** project:

```csharp
[Binding]
public class SampleAttribute : Attribute
{
    // Name of file to read. 
    [AutoResolve]
    [RegularExpression("^[A-Za-z][A-Za-z0-9]{2,128}$")]
    public string FileName { get; set; }

    // path where 
    [AppSetting(Default = "SamplePath")]
    public string Root { get; set; }
}
```

Binding attributes should apply the meta-attribute `[Binding]`. Attribute properties can have various attributes applied:

- `[AppSetting]` adds App Settings support, which allows customers to provide key names instead of actual values. The values are then defined in either local.settings.json (when running locally) or App Settings in Azure.
- `[AutoResolve]` adds  App Setting support (defined with % signs) as well as {key} values surrounded by curly brackets. 
- `[RegularExpress]` validates the value of a property with a regex.

### 2. Define binding rules

The extension itself is defined by implementing `IExtensionConfigProvider`. The key method is `Initialize(ExtensionConfigContext)`. Here, you define *binding rules* and *converters*.

In this class, you choose one or more binding rules:

- **BindToInput**. Just what it says, bind to an input object.
- **BindToCollector**. Bind to output via IAsyncCollector. This is used in output bindings for sending discrete messages like Queues and EventHub.

For example, the sample extension adds two binding rules for input and output. See the implementation of the [Initialize method](blob/master/SampleExtension/Config/SampleExtensions.cs#L43): 

```csharp
var rule = context.AddBindingRule<SampleAttribute>();

rule.BindToInput<SampleItem>(BuildItemFromAttr);
rule.BindToCollector<SampleItem>(BuildCollector);
```

For more information on binding rules, see [Add binding rules](https://github.com/Azure/azure-webjobs-sdk/wiki/Creating-custom-input-and-output-bindings#2-define-binding-rules).

### 3. Define converters

The extension should also register one or more *converters*. Converters are used by the Functions runtime to convert one type to another, allowing your binding to be used on more types. For more information, see [Add Converters](https://github.com/Azure/azure-webjobs-sdk/wiki/Creating-custom-input-and-output-bindings#3-add-converters).

For example, the sample extension [registers two converters](https://github.com/Azure/WebJobsExtensionSamples/blob/master/SampleExtension/Config/SampleExtensions.cs#L32), for both input and output bindings:

```
// This allows a user to bind to IAsyncCollector<string>, and the sdk
// will convert that to IAsyncCollector<SampleItem>
context.AddConverter<string, SampleItem>(ConvertToItem);

// This is useful on input, supports a string value as the binding type
context.AddConverter<SampleItem, string>(ConvertToString);
```

