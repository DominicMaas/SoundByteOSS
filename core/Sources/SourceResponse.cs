using SoundByte.Core.Items.Generic;
using System.Collections.Generic;

namespace SoundByte.Core.Sources
{
    public class SourceResponse
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <param name="token"></param>
        /// <param name="successful">Was the source successful in getting the items. (Does not count exceptions)</param>
        /// <param name="messageTitle">If the source was not successful, this is the title why.</param>
        /// <param name="messageContent"></param>
        public SourceResponse(IEnumerable<BaseSoundByteItem> items, string token, bool successful = true, string messageTitle = null, string messageContent = null)
        {
            Items = items;
            Token = token;
            IsSuccess = successful;

            Messages = new Message
            {
                MessageTitle = messageTitle,
                MessageContent = messageContent
            };
        }

        public SourceResponse(BaseSoundByteItem[] items, string token, bool successful = true, string messageTitle = null, string messageContent = null)
        {
            Items = items;
            Token = token;
            IsSuccess = successful;

            Messages = new Message
            {
                MessageTitle = messageTitle,
                MessageContent = messageContent
            };
        }

        public IEnumerable<BaseSoundByteItem> Items { get; }

        public string Token { get; }

        public bool IsSuccess { get; }

        /// <summary>
        /// This class contains any error or warning messages
        /// that the model may of generated.
        /// </summary>
        public Message Messages { get;}

        public class Message
        {
            public string MessageTitle { get; set; }

            public string MessageContent { get; set; }
        }
    }
}