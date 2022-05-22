// ----------------------------------------------------------------------------
//   ____                    ____                                   __
//  / __ \_  _____ _    __  / __/______ ___ _  ___ _    _____  ____/ /__
// / /_/ / |/ / _ \ |/|/ / / _// __/ _ `/  ' \/ -_) |/|/ / _ \/ __/  '_/
// \____/|___/\___/__,__/ /_/ /_/  \_,_/_/_/_/\__/|__,__/\___/_/ /_/\_\
//
// A 2D gaming framework on MonoGame
//
// Copyright (C) 2019 by daxnet.
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Messaging
{
    internal sealed class MessageDispatcher : IMessageDispatcher
    {
        private readonly ConcurrentDictionary<Type, List<Action<object, IMessage>>> messageHandlers = new ConcurrentDictionary<Type, List<Action<object, IMessage>>>();

        public MessageDispatcher()
        {
        }

        public Task DispatchMessageAsync<TMessage>(object publisher, TMessage message)
            where TMessage : IMessage
        {
            return Task.Factory.StartNew(() =>
            {
                DispatchMessageSync(publisher, message);
            });
        }

        private void DispatchMessageSync<TMessage>(object publisher, TMessage message)
            where TMessage : IMessage
        {
            if (messageHandlers.TryGetValue(typeof(TMessage), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(publisher, message);
                }
            }
        }

        public void RegisterHandler<TMessage>(Action<object, TMessage> handler)
            where TMessage : IMessage
        {
            Action<object, IMessage> convertedHandler = (publisher, message) => handler(publisher, (TMessage)message);

            if (messageHandlers.TryGetValue(typeof(TMessage), out var handlers))
            {
                handlers.Add(convertedHandler);
            }
            else
            {
                messageHandlers.TryAdd(typeof(TMessage), new List<Action<object, IMessage>> { convertedHandler });
            }
        }
    }
}