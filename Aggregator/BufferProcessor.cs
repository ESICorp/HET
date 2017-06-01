using Het.Backend;
using Het.Common;
using System;
using System.Collections;
using System.Linq;
using System.Threading;

namespace Aggregator
{
    public partial class Processor 
    {
        private static Hashtable LockTable { get; } = new Hashtable();

        private void ProcessBuffer(object state)
        {
            var command = state as Command;
            var context = command.Context;
            var attribute = context.Attribute as AggregatorAttribute;

            var syncRoot = LockTable[context.Id];
            if (syncRoot == null)
            {
                lock (LockTable.SyncRoot)
                {
                    syncRoot = LockTable[context.Id];
                    if (syncRoot == null)
                    {
                        LockTable.Add(context.Id, syncRoot = new object());
                    }
                }
            }

            lock (syncRoot)
            {
                Command buffer = null;
                if (command.Ordinal > 0)
                {
                    buffer = MessageHelper.Receive(attribute.InputChannel + "buffer", context.Id, TimeSpan.FromSeconds(1)) as Command;

                    if (buffer == null)
                    {
                        buffer = command;
                        buffer.Partial = new Response[command.Length];
                    }

                    buffer.Partial[command.Ordinal - 1] = command.Response ?? new Response();
                }
                else
                {
                    buffer = command; //sin indicador de secuencia
                }

                if (buffer.Partial.Contains(null))
                {
                    buffer.Context = null; //tick previene serializar contexto
                    MessageHelper.Send(attribute.InputChannel + "buffer", context.Id, buffer);
                }
                else
                {
                    buffer.Context = context;

                    ThreadPool.QueueUserWorkItem(this.ProcessRequest, buffer);

                    lock (LockTable.SyncRoot)
                    {
                        LockTable.Remove(context.Id);
                    }
                }
            }
        }
    }
}

