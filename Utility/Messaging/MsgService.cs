using System;
using System.Collections.Generic;
using System.Text;
using System.Messaging;

namespace Ifeng.Utility.Messaging
{
    public class MsgService : IMsgService ,IDisposable
    {
        private MessageQueue _queue;
        private MessageQueueTransactionType _transactionType;
        private IMessageFormatter _formatter;

        public IMessageFormatter Formatter
        {
            set { _formatter = value; }
            get { return _formatter; }
        }

        public MessageQueueTransactionType TransactionType
        {
            set { _transactionType = value; }
            get { return _transactionType; }
        }

        public MessageQueue Queue
        {
            set { _queue = value; }
            get { return _queue; }
        }

        #region Constructor
        public MsgService(string path)
        {
            if (!MessageQueue.Exists(path))
                _queue = MessageQueue.Create(path);
            else
                _queue = new MessageQueue(path, true, true);

            if (_formatter != null)
            {
                _queue.Formatter = _formatter;
            }
        }
        #endregion

        #region Receive
        public Message Receive()
        {
            try
            {
                var msg = _queue.Receive();
                msg.Formatter = _formatter;
                return msg;
            }
            catch (MessageQueueException err)
            {
                throw err;
            }
            
        }

        public Message Receive(TimeSpan ts)
        {
            try
            {
                var msg = _queue.Receive(ts);
                msg.Formatter = _formatter;
                return msg;
            }
            catch (MessageQueueException err)
            {
                throw err;
            }
        }

        public IAsyncResult BeginReceive()
        {
            return _queue.BeginReceive();
        }

        public IAsyncResult BeginReceive(TimeSpan ts)
        {
            return _queue.BeginReceive(ts);
        }

        public Message ReceiveTransaction(TimeSpan ts)
        {
            try
            {
                var msg = _queue.Receive(ts, _transactionType);
                msg.Formatter = _formatter;
                return msg;
            }
            catch (MessageQueueException err)
            {
                throw err;
            }
            
        }
        #endregion

        #region Peek
        public Message Peek()
        {
            try
            {
                var msg = _queue.Peek();
                msg.Formatter = _formatter;
                return msg;
            }
            catch (MessageQueueException err)
            {
                throw err;
            }
        }

        public IAsyncResult BeginPeek()
        {
            return _queue.BeginPeek();
        }
        #endregion

        #region Send
        public void Send(object obj,string label)
        {
            try
            {
                _queue.Send(obj, label);
            }
            catch (MessageQueueException err)
            {
                throw err;
            }
        }

        public void SendTransaction(object obj,string label)
        {
            try
            {
                _queue.Send(obj, label, _transactionType);
            }
            catch (MessageQueueException err)
            {
                throw err;
            }
        }
        #endregion 

        #region Refresh
        public void Refresh()
        {
            _queue.Refresh();
        }
        #endregion 

        public void Purge()
        {
            _queue.Purge();
        }

        public void Dispose()
        {
            _queue.Dispose();
            _queue = null;
        }
    }
}
