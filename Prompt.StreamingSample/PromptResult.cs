using System;
using System.Collections.Generic;
using System.Text;

namespace Prompt.StreamingSample
{
    internal sealed class PromptResult
    {
        public PromptResult() { }

        public List<string> Items { get; set; } = default!;
    }
}
