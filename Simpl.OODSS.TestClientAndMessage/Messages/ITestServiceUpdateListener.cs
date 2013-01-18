namespace Simpl.OODSS.TestClientAndMessage.Messages
{
    interface ITestServiceUpdateListener
    {
        void OnReceiveUpdate(TestServiceUpdate response);
    }
}
