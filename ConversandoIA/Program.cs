using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;


Console.WriteLine("Ingrese el tema a debatir: ");
var talkTopic = Console.ReadLine();

//var aiTalk = new AITalk("El origen del universo");
var aiTalk = new AITalk(talkTopic!);
await aiTalk.Run(3);


public class AITalk
{ 
    private string _topic;
    private string _robotA;
    private string _robotB;

    private string _apiKey = "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";
    private OpenAIService _openAIService;

    private readonly string _initialText = "Como comenzarías con una pregunta una charla sobre: ";
    private readonly string _continueText = "Contesta a lo siguiente de forma que parezca un debate";
    public string InitQuestion
    {
        get
        { 
            return _initialText + _topic;
        }
    }

    public AITalk(string topic)
    {
        _topic = topic;
        _openAIService = new OpenAIService(new OpenAiOptions()
        { 
            ApiKey = _apiKey,
        });
    }



    public async Task Run(int limit = 10)
    {
        Console.WriteLine("Tema a conversar: " + _topic);
        await Send(InitQuestion, (string response) =>
        { 
            _robotA = response;
            Console.WriteLine("* Robot A dice *: " + _robotA);
            Console.WriteLine("_______________________________________________________________");
        });

        for (int i = 0; i < limit; i++)
        {
            await Send(_continueText + _robotA, (string response) =>
            {
                _robotB = response;
                Console.WriteLine("*** Robot B dice ***: " + _robotB);
                Console.WriteLine("_______________________________________________________________");
            });
            await Send(_continueText + _robotB, (string response) =>
            {
                _robotA = response;
                Console.WriteLine("*** Robot A dice ***: " + _robotA);
                Console.WriteLine("_______________________________________________________________");
            });
        }
    }


    private async Task Send(string message, Action<string> fnSetResponse)
    {
        var completationResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            { 
                ChatMessage.FromUser(message) 
            },
            Model = Models.ChatGpt3_5Turbo
        });

        if (completationResult.Successful)
        {
            fnSetResponse(completationResult.Choices.First().Message.Content);
        }
        else 
        {
            if (completationResult.Error == null)
            {
                throw new Exception("Unknow Error");
            }

            Console.WriteLine($"{completationResult.Error.Code}: {completationResult.Error.Message}");
        }
            
    }


}