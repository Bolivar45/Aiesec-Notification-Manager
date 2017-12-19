using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace Aiesec_Notification_Manager
{
    class VkBot
    {
        private VkApi vkapi = new VkApi();
        public delegate void BotEventHandler(string msg, Color color);
        public event BotEventHandler BotNotify;
        public static ReadOnlyCollection<VkNet.Model.Attachments.Photo> photosForMessage;
        public bool Auth_Start()
        {
            string groupKey = string.Empty;
            groupKey = "648816582225669631b8bf0c9e87f620cda1ea2a431616839ae9c22a8d17876416ab582b43774e65a90a0";
            BotNotify?.Invoke("Попытка авторизации... ==> Vk", Color.Black);

            if (Auth(groupKey))
            {
                BotNotify?.Invoke("Авторизация успешно завершена  ==> Vk", Color.Green);
                BotNotify?.Invoke("Запросов в секунду доступно: " + vkapi.RequestsPerSecond + "  ==> Vk", Color.Green);
                return true;
            }
            else
            {
                BotNotify?.Invoke("Не удалось произвести авторизацию!  ==> Vk", Color.Red);
                return false;
            }
        }
        
        internal bool SendMessageWithPhoto(string Body, string Userdomain)
        {
            try
            {
                
                
                vkapi.Messages.Send(new MessagesSendParams
                {
                    Attachments = photosForMessage,
                    Domain = Userdomain,
                    Message = Body
                });
                BotNotify?.Invoke("Сообщение отправлено ==>" + Userdomain, Color.LightGreen);
                return true;
            }
            catch (Exception e)
            {
                BotNotify?.Invoke(e.Message + "==>" + Userdomain, Color.Red);
                return false;
            }
        }

        public static void SavePhotoToServer(string path)
        {
            VkApi vkApiUser = new VkApi();
            vkApiUser.Authorize(new ApiAuthParams
            {
                ApplicationId = 6286482,
                Settings = Settings.All,
                AccessToken = "483bf679612d22de9a67702b93f6962800252e913c7b5bd388a4c151af81ef1558237151105a8c83170b4"
            });
            var uploadServer = vkApiUser.Photo.GetUploadServer(249668296, 158092312);
            // Загрузить файл.
            var wc = new WebClient();
            var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer.UploadUrl, path));
            // Сохранить загруженный файл
            var photos = vkApiUser.Photo.Save(new PhotoSaveParams
            {
                GroupId = 158092312,
                SaveFileResponse = responseFile,
                AlbumId = uploadServer.AlbumId
            });
            photosForMessage = photos;
        }

        public bool SendMessage(string Body, string Userdomain)
        {
            try
            {
                vkapi.Messages.Send(new MessagesSendParams
                {
                    Domain = Userdomain,
                    Message = Body
                });
                BotNotify?.Invoke("Сообщение отправлено ==>" + Userdomain, Color.LightGreen);
                return true;
            }
            catch (Exception e)
            {
                BotNotify?.Invoke(e.Message + "==>" + Userdomain, Color.Red);
                return false;
            }
        }
        private bool Auth(string GroupID)
        {
            try
            {
                vkapi.Authorize(new ApiAuthParams { AccessToken = GroupID });
                return true;
            }
            catch (Exception e)
            {
                BotNotify?.Invoke(e.Message, Color.Red);
                return false;
            }
        }

        public ReadOnlyCollection<User> GroupUserList()
        {

            ReadOnlyCollection<User> ids = vkapi.Groups.GetMembers(new GroupsGetMembersParams
            {
                GroupId = "158092312",
                Fields = UsersFields.Nickname
            });
            return ids;
        }
    }
}