namespace API
{
    public interface ISettingsFactory
    {
        YardLight.CommonCore.ISettings CreateCore(Settings settings);
        BrassLoon.Interface.Log.ISettings CreateLog(Settings settings);
        YardLight.Interface.Authorization.ISettings CreateAuthorization(Settings settings, string token);
    }
}
