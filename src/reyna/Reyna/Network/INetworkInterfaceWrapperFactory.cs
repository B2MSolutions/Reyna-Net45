namespace Reyna
{
    public interface INetworkInterfaceWrapperFactory
    {
        INetworkInterfaceWrapper[] GetAllNetworkInterfaces();
    }
}