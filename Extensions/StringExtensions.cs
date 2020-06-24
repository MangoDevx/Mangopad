namespace Mangopad.Extensions
{
    public static class StringExtensions
    {
        public static int CountWords(this string input)
        {
            var wordCount = 0;
            var index = 0;

            // Skip any blank space before the first word
            index = FindNextWordIndex(input);

            while (index < input.Length)
            {
                if (char.IsWhiteSpace(input[index]) || index == input.Length-1)
                {
                    // If the current character is whitespace, increment wordcount and find the next word index
                    wordCount++;

                    index = FindNextWordIndex(input, index + 1);
                }
                else
                {
                    // Current character is not whitespace, increment index
                    index++;
                }
            }

            return wordCount;
        }

        private static int FindNextWordIndex(string input, int startIndex = 0)
        {
            var index = startIndex;
            while (index < input.Length && char.IsWhiteSpace(input[index])) index++;

            return index;
        }
    }
}
