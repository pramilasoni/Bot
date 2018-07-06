using System;
using System.Collections.Generic;
using Microsoft.Cognitive.LUIS;

namespace Build.Labs.BotFramework.Models
{
    public class LuisOptions : ILuisOptions
    {
        public bool? Log { get; set; }
        public bool? SpellCheck { get; set; }
        public bool? Staging { get; set; }
        public double? TimezoneOffset { get; set; }
        public bool? Verbose { get; set; }
        public string BingSpellCheckSubscriptionKey { get; set; }
    }
}
