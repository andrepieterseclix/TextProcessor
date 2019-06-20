using System;
using System.Collections.Generic;

namespace TextProcessor.TestStream
{
    class RandomTextHelper
    {
        Random r = new Random();

        string[] strings = null;

        public RandomTextHelper()
        {
            strings = new[] { "algorithm", "beach", "curriculum", "dinner", "expat", "fly", "gullible", "heart", "internet", "junk", "kind", "ladder", "mixture", "net", "open", "plague", "question", "revolve", "stereo", "template", "universal", "valve", "welcome", "xenophobia", "yellow", "zebra" };
        }

        public string GetRandomString()
        {
            int index = r.Next(0, strings.Length);
            return strings[index];
        }
    }
}
