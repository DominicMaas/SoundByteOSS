namespace SoundByte.Core.Models.Playback
{
    public class PlaybackInitializeResponse
    {
        public PlaybackInitializeResponse(bool success = true, string messsage = null)
        {
            Success = success;
            Message = messsage;
        }

        public string Message { get; set; }

        public bool Success { get; set; }
    }
}