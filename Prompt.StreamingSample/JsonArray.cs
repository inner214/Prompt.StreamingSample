using System;
using System.Collections.Generic;
using System.Text;

namespace Prompt.StreamingSample
{
    internal sealed class JsonArray : IAsyncEnumerable<string>
    {
        private readonly IAsyncEnumerable<string> _collection;

        public JsonArray(IAsyncEnumerable<string> collection)
        {
            ArgumentNullException.ThrowIfNull(collection);
            _collection = collection;
        }

        public IAsyncEnumerator<string> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new JsonArrayIterator(_collection.GetAsyncEnumerator(cancellationToken));
        }
    }
}
