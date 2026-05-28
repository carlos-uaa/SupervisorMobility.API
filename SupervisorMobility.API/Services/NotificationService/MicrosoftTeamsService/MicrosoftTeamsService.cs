using SupervisorMobility.API.Services.MicrosoftTeamsService;
using SupervisorMobility.API.DataAccess.Entities;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;


namespace SupervisorMobility.API.Services.MicrosoftTeamsService
{
    public class MicrosoftTeamsService : IMicrosoftTeamsService
    {
        private readonly MicrosoftTeamsConfiguration _mtconfig;

        public MicrosoftTeamsService(
            MicrosoftTeamsConfiguration mtconfig
        )
        {
            this._mtconfig = mtconfig;
        }
        
        public async Task<bool> SendMicrosoftTeamsMessageAsync(string recipientId, string messageText)
        {
            // 1. Autenticación con DefaultAzureCredential
            var credential = new DefaultAzureCredential();

            // 2. Crear cliente de Graph
            var graphClient = new GraphServiceClient(credential);

            // 3. IDs de los usuarios (ObjectId de Entra ID) - ajustar según sea necesario
            string user1Id = "<USER1_OBJECT_ID>";
            string user2Id = "<USER2_OBJECT_ID>";

            // 4. Crear miembros como List<ConversationMember> y usar instancias de AadUserConversationMember
            var members = new List<ConversationMember>
            {
                new AadUserConversationMember
                {
                    Roles = new System.Collections.Generic.List<string> { "owner" },
                    AdditionalData = new System.Collections.Generic.Dictionary<string, object>
                    {
                        {"user@odata.bind", $"https://graph.microsoft.com/v1.0/users/{user1Id}"}
                    }
                },
                new AadUserConversationMember
                {
                    Roles = new System.Collections.Generic.List<string> { "owner" },
                    AdditionalData = new System.Collections.Generic.Dictionary<string, object>
                    {
                        {"user@odata.bind", $"https://graph.microsoft.com/v1.0/users/{user2Id}"}
                    }
                }
            };

            // 5. Crear el chat
            var chat = new Chat
            {
                ChatType = ChatType.OneOnOne,
                Members = members
            };

            // 6. Usar PostAsync para crear el chat (evita el uso incorrecto de RequestInformation.AddAsync)
            var createdChat = await graphClient.Chats.PostAsync(chat);

            Console.WriteLine($"Chat creado con ID: {createdChat?.Id}");

            // 7. Enviar un mensaje al chat usando el texto pasado en messageText
            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    Content = messageText ?? "Hola, este es el primer mensaje en nuestro chat 1:1."
                }
            };

            // 8. Usar PostAsync en Messages para enviar el mensaje
            await graphClient.Chats[createdChat.Id].Messages.PostAsync(chatMessage);

            return true;
        }
    }
}