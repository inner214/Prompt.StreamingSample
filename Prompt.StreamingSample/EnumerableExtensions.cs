using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel;

namespace Prompt.StreamingSample
{
    internal static class EnumerableExtensions
    {
        public static async IAsyncEnumerable<string> Tokenize(this IAsyncEnumerable<StreamingKernelContent> fragments, string pattern, Func<string, string> transformer)
        {
            ArgumentNullException.ThrowIfNull(fragments);
            ArgumentNullException.ThrowIfNull(pattern);
            ArgumentNullException.ThrowIfNull(transformer);

            await foreach (StreamingKernelContent fragment in fragments)
            {
                string[] groups = Regex.Split(transformer(fragment.ToString()), pattern);

                foreach (string group in groups)
                {
                    if (string.IsNullOrEmpty(group))
                    {
                        continue;
                    }

                    yield return group;
                }
            }
        }
    }
}
