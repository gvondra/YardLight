namespace API
{
    public interface ISettingsFactory
    {
        BrassLoon.Interface.Log.ISettings CreateLog(Settings settings);
    }
}
