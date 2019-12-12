using System.Collections.Generic;

namespace Hera.Mobile.Api.Models.Translation
{
    public class TranslationItem
    {
        public string Screen { get; set; }
        public Dictionary<string, string> Translation { get; set; }
    }
}