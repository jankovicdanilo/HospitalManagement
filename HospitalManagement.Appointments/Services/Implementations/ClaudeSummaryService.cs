using Anthropic;
using Anthropic.Models.Messages;
using HospitalManagement.Appointments.Services.Interfaces;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class ClaudeSummaryService : IClaudeSummaryService
    {
        private readonly AnthropicClient anthropicClient;
        private readonly ILogger<ClaudeSummaryService> logger;

        public ClaudeSummaryService(IConfiguration configuration, ILogger<ClaudeSummaryService> logger)
        {
            var apiKey = configuration["Anthropic:ApiKey"];
            anthropicClient = new AnthropicClient { ApiKey = apiKey };
            this.logger = logger;
        }

        public async Task<string> GenerateSummaryAsync(string prompt)
        {
            var parameters = new MessageCreateParams
            {
                MaxTokens = 1024,
                Model = Model.ClaudeSonnet5,
                Messages =
                [
                    new()
                    {
                        Role = Role.User,
                        Content = prompt,
                    },
                ],
            };

            try
            {
                var message = await anthropicClient.Messages.Create(parameters);

                foreach (var block in message.Content)
                {
                    if (block.TryPickText(out var textBlock))
                    {
                        return textBlock.Text;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate patient summary via Claude");
                throw;
            }
        }
    }
}
