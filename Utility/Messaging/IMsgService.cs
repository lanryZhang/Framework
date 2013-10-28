using System;
namespace Ifeng.Utility.Messaging
{
    interface IMsgService
    {
        IAsyncResult BeginPeek();
        IAsyncResult BeginReceive();
        IAsyncResult BeginReceive(TimeSpan ts);
        System.Messaging.Message Peek();
        System.Messaging.MessageQueue Queue { get; set; }
        System.Messaging.Message Receive();
        System.Messaging.Message Receive(TimeSpan ts);
        System.Messaging.Message ReceiveTransaction(TimeSpan ts);
        void Refresh();
        void Send(object obj, string label);
        void SendTransaction(object obj, string label);
    }
}
