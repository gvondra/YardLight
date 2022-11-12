namespace API
{
    public interface ISettingsFactory
    {
        BrassLoon.Interface.Log.ISettings CreateLog(Settings settings);
        YardLight.Interface.Authorization.ISettings CreateAuthorization(Settings settings, string token);
    }
}
