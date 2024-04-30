namespace ET
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene root, string account, string password)
        {
            root.RemoveComponent<ClientSenderComponent>();
            
            ClientSenderComponent clientSenderComponent = root.AddComponent<ClientSenderComponent>();
            
            long playerId = await clientSenderComponent.LoginAsync(account, password);
            
            Log.Info($"Login success playerId:{playerId}");

            // root.GetComponent<PlayerComponent>().MyId = playerId;
            //
            // await EventSystem.Instance.PublishAsync(root, new LoginFinish());
            await ETTask.CompletedTask;
        }
    }
}