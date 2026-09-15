using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using RestaurantSystem.Data;
using RestaurantSystem.Models;

namespace RestaurantSystem.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatClient _chatClient;
        private readonly RestaurantDbContext _context;

        public ChatController(ChatClient chatClient, RestaurantDbContext context)
        {
            _chatClient = chatClient;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ChatViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChatViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserMessage))
            {
                return View(model);
            }

            // 1. حفظ رسالة الـ User في الداتا بيز
            _context.ChatMessages.Add(new ChatMessageEntity
            {
                Role = "user",
                Content = model.UserMessage,
                Timestamp = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // 2. سحب جميع الرسائل السابقة من الداتا بيز لبناء السياق (Context)
            var dbMessages = await _context.ChatMessages
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            // 3. تجهيز قائمة الرسائل المرسلة للـ AI
            var chatMessagesList = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful and polite assistant for our restaurant. Answer the user's questions clearly based on the menu and restaurant services.")
            };

            foreach (var msg in dbMessages)
            {
                if (msg.Role == "user")
                {
                    chatMessagesList.Add(new UserChatMessage(msg.Content));
                }
                else if (msg.Role == "assistant")
                {
                    chatMessagesList.Add(new AssistantChatMessage(msg.Content));
                }
            }

            var options = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 1000
            };

            // 4. إرسال السياق للـ AI
            var response = await _chatClient.CompleteChatAsync(chatMessagesList, options);
            string aiResponseText = response.Value.Content[0].Text;

            // 5. حفظ رد الـ AI في الداتا بيز
            _context.ChatMessages.Add(new ChatMessageEntity
            {
                Role = "assistant",
                Content = aiResponseText,
                Timestamp = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // 6. الاحتفاظ برسالة المستخدم ووضع رد الـ AI لكي يظهر الاثنان معاً في الـ View
            string originalUserMsg = model.UserMessage;

            model.AiResponse = aiResponseText;
            model.UserMessage = originalUserMsg; // بنرجع نبعتها تاني عشان ما تختفيش

            return View(model);
        }
    }
}