using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardLight.Client
{
    public static class UserSessionLoader
    {
        private const string USER_SESSION_FILE_NAME = "UserSession.json.gz";
        private static UserSession _userSession;

        private static IsolatedStorageFile GetIsolatedStorageFile() => IsolatedStorageFile.GetUserStoreForAssembly();

        private static JsonSerializerSettings GetJsonSettings() => new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver(),
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static UserSession GetUserSession()
        {
            if (_userSession == null ) 
                _userSession = LoadUserSession();
            return _userSession;
        }

        private static UserSession LoadUserSession()
        {
            UserSession userSession = null;
            IsolatedStorageFile file = GetIsolatedStorageFile();
            if (file.FileExists(USER_SESSION_FILE_NAME))
            {
                using (IsolatedStorageFileStream stream = file.OpenFile(USER_SESSION_FILE_NAME, FileMode.Open, FileAccess.Read))
                {
                    using (System.IO.Compression.GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress))
                    using (StreamReader streamReader = new StreamReader(gzip))
                    using (JsonTextReader jsonReader = new JsonTextReader(streamReader))
                    {
                        JsonSerializer serializer = JsonSerializer.Create(GetJsonSettings());
                        userSession = serializer.Deserialize<UserSession>(jsonReader);
                    }
                }
            }
            if (userSession == null) // create default user session
            {
                userSession = new UserSession();
            }
            userSession.EnableSave();
            return userSession;
        }

        public static void SaveUserSession(UserSession userSession)
        {
            IsolatedStorageFile file = GetIsolatedStorageFile();
            if (userSession == null)
            {
                if (file.FileExists(USER_SESSION_FILE_NAME))
                    file.DeleteFile(USER_SESSION_FILE_NAME);
            }
            else
            {
                using (IsolatedStorageFileStream stream = file.OpenFile(USER_SESSION_FILE_NAME, FileMode.Create, FileAccess.Write))
                {
                    using (GZipStream gzip = new GZipStream(stream, CompressionLevel.Optimal))
                    using (StreamWriter streamWriter = new StreamWriter(gzip))
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        JsonSerializer serializer = JsonSerializer.Create(GetJsonSettings());
                        serializer.Serialize(jsonWriter, userSession);
                    }
                }
            }
            _userSession = userSession;
        }
    }
}
