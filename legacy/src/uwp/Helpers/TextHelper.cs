using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace SoundByte.App.Uwp.Helpers
{
    /// <summary>
    ///     This class handles text related helper functions
    /// </summary>
    public static class TextHelper
    {
        public static string CalculateMd5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();

            var inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds links and formats a rich text block
        /// </summary>
        /// <param name="inputText">The text to update the text block with</param>
        /// <param name="textBlock"></param>
        public static void ConvertTextToFormattedTextBlock(string inputText, ref RichTextBlock textBlock)
        {
            // Clear the text block
            textBlock.Blocks.Clear();
            var index = 0;
            var par = new Paragraph();
            string text;
            Run run;
            var source = inputText;

            var pattern = @"(?:https?://|www\.|[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]|@([A-Za-z]+[A-Za-z0-9]+))\S+";
            var regex = new Regex(pattern);
            var matches = regex.Matches(source);

            foreach (Match match in matches)
            {
                var matchIndex = match.Index;
                text = source.Substring(index, matchIndex - index);
                run = new Run { Text = text };
                par.Inlines.Add(run);

                // Add match as hyperlink.
                var hyper = match.Value;
                var link = new Hyperlink();
                run = new Run { Text = hyper };
                link.Inlines.Add(run);

                // Complete link if necessary.
                if (!hyper.Contains("@") && !hyper.StartsWith("http"))
                {
                    hyper = @"http://" + hyper;
                }
                else if (!hyper.Contains("@") && !hyper.StartsWith("https"))
                {
                    hyper = @"https://" + hyper;
                }
                else if (!hyper.StartsWith("@") && hyper.Contains("@"))
                {
                    hyper = @"mailto:" + hyper;
                }
                else if (hyper.StartsWith("@"))
                {
                    hyper = @"soundbyte://app/userName=" + hyper.TrimStart('@');
                }

                try { link.NavigateUri = new Uri(hyper); } catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex + "\n\n" + hyper); }
                par.Inlines.Add(link);

                index = matchIndex + match.Length;
            }

            text = source.Substring(index, source.Length - index);

            run = new Run { Text = text };
            par.Inlines.Add(run);
            // Update RichTextBlock content.
            textBlock.Blocks.Clear();
            textBlock.Blocks.Add(par);
        }
    }
}