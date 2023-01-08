using YardLight.CommonAPI;

namespace API
{
    public interface ISettingsFactory
    {
        CoreSettings CreateCore(Settings settings);
        BrassLoon.Interface.Log.ISettings CreateLog(Settings settings);
        AuthorizationSettings CreateAuthorization(Settings settings);
        AuthorizationSettings CreateAuthorization(Settings settings, string token);
    }
}
