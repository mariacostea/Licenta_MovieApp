namespace MobyLabWebProgramming.Core.DataTransferObjects
{
    public class RegisterDTO
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ProfilePictureUrl { get; set; } = default!;
    }
}