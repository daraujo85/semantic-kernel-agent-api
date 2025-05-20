using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using DotNetEnv;

// Carrega as variáveis do .env
Env.Load();

var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var weatherKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");

var builder = Kernel.CreateBuilder();

// Configura o modelo
builder.AddOpenAIChatCompletion("gpt-3.5-turbo", openAiKey);

// Configura logging
builder.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Trace));

var kernel = builder.Build();

// Registra o plugin de previsão do tempo
kernel.ImportPluginFromObject(new WeatherPlugin(weatherKey), "Weather");

// Loop interativo com o usuário
Console.WriteLine("🤖 Assistente IA iniciado. Digite sua pergunta (ou 'sair'):");

// 2. Enable automatic function calling
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();

string? userInput;

// Collect user input
userInput = "Me diga se vai chover amanhã no Rio de Janeiro.";

// Add user input
history.AddUserMessage(userInput);

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// 3. Get the response from the AI with automatic function calling
var result = await chatCompletionService.GetChatMessageContentAsync(
    history,
    executionSettings: openAIPromptExecutionSettings,
    kernel: kernel);

// Print the results
Console.WriteLine("Assistente > " + result);

// Add the message from the agent to the chat history
history.AddMessage(result.Role, result.Content ?? string.Empty);