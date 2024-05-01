using FishNet.Broadcast;

namespace VENTUS.Networking
{
    public struct ClientInfoBroadcast : IBroadcast
    {
        public UserInfo UserInfo;

        public ClientInfoBroadcast(UserInfo userInfo)
        {
			UserInfo = userInfo;
        }
    }
}