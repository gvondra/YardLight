namespace AuthorizationAPI
{
    public interface ISettingsFactory
    {
        YardLight.CommonCore.ISettings CreateCore(Settings settings);
        BrassLoon.Interface.Log.ISettings CreateLog(Settings settings, string token);
    }
}
