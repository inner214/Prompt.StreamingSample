using System.Diagnostics;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Prompt.StreamingSample;

Kernel kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(
        modelId: "gpt-5",
        apiKey: Environment.GetEnvironmentVariable("OpenAI__ApiKey")!,
        endpoint: new Uri(Environment.GetEnvironmentVariable("AzureOpenAI__Endpoint")!))
    .Build();

OpenAIPromptExecutionSettings executionSettings = new OpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(PromptResult),
    ReasoningEffort = "MINIMAL"
};

KernelArguments kernelArguments = new KernelArguments(executionSettings);

int count = 75;
string prompt = $"Generate {count} funny company names.";
IAsyncEnumerable<StreamingKernelContent> results = kernel.InvokePromptStreamingAsync(prompt, kernelArguments);

string pattern = @"(:|\[|\]|:|,|\.)";
IAsyncEnumerable<string> tokens = results.Tokenize(pattern, x => x.TrimEnd('\r', '\n'));

JsonArray array = new JsonArray(tokens);
List<long> times = new List<long>();
Stopwatch stopWatch = Stopwatch.StartNew();

await foreach (string item in array)
{
    Console.WriteLine(item);
    times.Add(stopWatch.ElapsedMilliseconds);
}

Console.WriteLine(new string('-', 10));
Console.WriteLine($"Time to First Token (TTFT): {times[0] / 1_000}s");
