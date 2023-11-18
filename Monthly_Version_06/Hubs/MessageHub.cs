using Microsoft.AspNetCore.SignalR;

namespace Monthly_Version_06.Hubs
{
    public class MessageHub : Hub
    {
        static Dictionary<string, string> users = new Dictionary<string, string>();
        private readonly IWebHostEnvironment env;
        public MessageHub(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async override Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("connection", "You are connected");
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinAsync(string username)
        {
            users.Add(Context.ConnectionId, username);
            await Clients.All.SendAsync("userList", users.Values.ToList());
        }

        public async Task SendAllAsync(string msg)
        {
            await Clients.All.SendAsync("message", new { sender = users[Context.ConnectionId], message = msg });
        }

        public async Task UploadAsync(FileData data)
        {
            string ext = Path.GetExtension(data.Filename);
            string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
            string savePath = Path.Combine(this.env.WebRootPath, "Uploads", fileName);
            byte[] buffer = Convert.FromBase64String(data.Content.Split(',')[1]);
            await File.WriteAllBytesAsync(savePath, buffer);
            await Clients.All.SendAsync("uploaded", new { sender = users[Context.ConnectionId], File = fileName, Type = ext });
        }
    }

    public class FileData
    {
        public string Filename { get; set; } = default!;
        public string Content { get; set; } = default!;
    }
}
