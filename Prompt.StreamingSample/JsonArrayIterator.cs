using System;
using System.Collections.Generic;
using System.Text;

namespace Prompt.StreamingSample
{
    internal sealed class JsonArrayIterator : IAsyncEnumerator<string>
    {
        private const string OpenSquareBracket = "[";
        private const string CloseSquareBracket = "]";
        private const string DoubleQuotationMarks = "\"";

        private readonly IAsyncEnumerator<string> _iterator;
        private readonly StringBuilder _item = new();
        private State _state;

        public JsonArrayIterator(IAsyncEnumerator<string> iterator)
        {
            ArgumentNullException.ThrowIfNull(iterator);
            _iterator = iterator;
        }

        public string Current => _item.ToString();

        public async ValueTask<bool> MoveNextAsync()
        {
            while (await _iterator.MoveNextAsync())
            {
                switch (_state)
                {
                    case State.None:
                        {
                            if (_iterator.Current.Equals(OpenSquareBracket))
                            {
                                _state = State.ArrayStart;
                            }
                        }
                        break;

                    case State.ArrayStart:
                        {
                            if (_iterator.Current.Equals(DoubleQuotationMarks))
                            {
                                _state = State.ItemStart;
                            }
                        }
                        break;

                    case State.ItemStart:
                        {
                            if (_iterator.Current.Equals(DoubleQuotationMarks))
                            {
                                _state = State.ItemEnd;
                                return true;
                            }

                            _item.Append(_iterator.Current);
                        }
                        break;

                    case State.ItemEnd:
                        {
                            _item.Clear();

                            _state = _iterator.Current switch
                            {
                                DoubleQuotationMarks => State.ItemStart,
                                CloseSquareBracket => State.ArrayEnd,
                                _ => _state
                            };
                        }
                        break;

                    case State.ArrayEnd:
                        break;
                }
            }

            return false;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        private enum State
        {
            None,
            ArrayStart,
            ItemStart,
            ItemEnd,
            ArrayEnd,
        }
    }
}
