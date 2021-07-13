using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace AddCopyButtons
{
    /// <summary>
    /// This little utility helps me as a creator of documentation for software developers.
    /// I use the product Typora to create .md files, many of which include Code blocks.
    /// I don't want my end-users to have to install Typora to read them, so I export them
    /// to HTML files. However, in order to copy a code block to the clipboard (to paste
    /// into their applications) they have to select the entire block and press Ctrl-C.
    /// Sometimes they can miss a character. So, this utility takes the HTML file 
    /// exported by Typora and adds a "Copy" button into each code block to make that easier.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // constants
            const string divsearchstring = "<div class=\"CodeMirror-code\"";
            const string buttonstring = "<button onclick=\"CopyToClipboard('{DIVID}')\">Copy</button>";
            const string copyscript = "<script>function trimText(t){if(t.length>2&&13==t.charCodeAt(0)&&10==t.charCodeAt(1)){var e=\"\";for(i=2;i<t.length;i++)e+=t.charAt(i);t=e}return t}function CopyToClipboard(t){if(window.getSelection){var e=trimText(document.getElementById(t).innerText);navigator.clipboard.writeText(e)}}</script>";

            // Get the input file
            Console.WriteLine("Drop .html file here and press ENTER:");
            string inputFile = Console.ReadLine();

            // remove quotes around file name
            if (inputFile.StartsWith("\""))
            {
                inputFile = inputFile.Substring(1, inputFile.Length - 2);
            }

            // replace non-printable ASCII characters
            string filedata = File.ReadAllText(inputFile);
            //filedata = Regex.Replace(filedata, @"[^\u0000-\u007F]+", "\n");
            filedata = StripUnicode(filedata);

            // find all the code fence divs
            int startPos = 0;
            int pos = 0;
            while (pos > -1)
            {
                // look for a new code fence div
                pos = filedata.IndexOf(divsearchstring, startPos);
                if (pos > -1)   // found?
                {
                    // YES! Create a new div id based on current time
                    Task.Delay(100).GetAwaiter().GetResult();
                    string id = DateTime.Now.Ticks.ToString();

                    // add a copy button and give the div an id.
                    filedata = filedata.Substring(0, pos)
                        + buttonstring.Replace("{DIVID}", id)
                        + " "
                        + divsearchstring
                        + " id=\"" + id + "\""
                        + filedata.Substring(pos + divsearchstring.Length);

                    // update the search start position
                    startPos = pos + divsearchstring.Length + buttonstring.Length;
                }
            }

            // find the end of the body
            pos = filedata.IndexOf("</body>");
            if (pos > -1)
            {
                // add the script
                filedata = filedata.Substring(0, pos)
                    + copyscript
                    + filedata.Substring(pos);
            }

            // write over the input file with new data
            File.Delete(inputFile);
            File.WriteAllText(inputFile, filedata);
        }

        static string StripUnicode(string InputString)
        {
            return Encoding.ASCII.GetString(
                    Encoding.Convert(
                        Encoding.UTF8,
                        Encoding.GetEncoding(
                            Encoding.ASCII.EncodingName,
                            new EncoderReplacementFallback(string.Empty),
                            new DecoderExceptionFallback()),
                    Encoding.UTF8.GetBytes(InputString)));
        }
    }
}
