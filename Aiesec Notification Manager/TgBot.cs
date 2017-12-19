using Aiesec_Notification_Manager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace Aiesec_Notification_Manager
{
    class TgBot
    {
        DataContext db = new DataContext();
        TelegramBotClient Bot;
        public delegate void TgBotEventHandler(string msg, Color color);
        public event TgBotEventHandler BotNotify;

        async public void StartBot()
        {
            string key = "405524014:AAFI4RaDV67Ji0thaWCQAaYhcWRgmJIqAgc";
            await TgAuth(key);
        }

        private async Task TgAuth(string key)
        {
            try
            {
                Bot = new TelegramBotClient(key);
                await Bot.SetWebhookAsync("");
                ListenStart();
                BotNotify?.Invoke("Авторизация успешно завершена ==> Telegramm", Color.Green);
            }
            catch (Exception e)
            {
                BotNotify?.Invoke(e.Message, Color.Red);
            }
        }
        bool IsRegisterStart = false;
        bool IsEmailInputing = false;
        string Email = "email";
        string departament = "Departament";

        private void ListenStart()
        {
            Bot.OnUpdate += async (object su, UpdateEventArgs evu) =>
            {
                if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return; // в этом блоке нам келлбэки и инлайны не нужны
                var update = evu.Update;

                var message = update.Message;
                if (message == null) return;
                if (message.Type == MessageType.TextMessage)
                {
                    if (message.Text == "Hi" || message.Text == "Привет" || message.Text == "Hello" || message.Text == "hi" || message.Text == "привет" || message.Text == "hello")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "День добрый, " + message.Chat.FirstName + " " + message.From.LastName);
                        LogSend("Номер чата ==> " + message.Chat.Id, Color.LightGreen);
                        LogSend("Пользователь ==> " + message.Chat.FirstName + " " + message.Chat.LastName, Color.LightGreen);
                        LogSend("Username ==> " + message.Chat.Username, Color.LightGreen);
                        return;
                    }
                    
                    if (message.Text == "Пароль1" && IsRegisterStart)
                    {
                        if (IsEmailInputing == false)
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Напиши свою почту");
                            departament = "Talent Management";
                            IsEmailInputing = true;
                        }
                        return;
                    }

                    if (message.Text == "Пароль2" && IsRegisterStart)
                    {
                        if (IsEmailInputing == false)
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Напиши свою почту");
                            departament = "Buisness Developement";
                            IsEmailInputing = true;
                        }
                        return;
                    }
                    if (message.Text == "Пароль3" && IsRegisterStart)
                    {
                        if (IsEmailInputing == false)
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Напиши свою почту");
                            departament = "Outgoing exchanges";
                            IsEmailInputing = true;
                        }
                        return;
                    }
                    if (message.Text == "Пароль4" && IsRegisterStart)
                    {
                        if (IsEmailInputing == false)
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Напиши свою почту");
                            departament = "Incoming";
                            IsEmailInputing = true;
                        }
                        return;
                    }
                    if (message.Text == "Пароль5" && IsRegisterStart)
                    {
                        if (IsEmailInputing==false)
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Напиши свою почту");
                            departament = "Community";
                            IsEmailInputing = true;
                        }
                        return;
                    }
                    if (IsEmailInputing)
                    {
                        Email = message.Text;
                        await RegisterStartAsync(message, departament, Email);
                    }
                    if (message.Text == "/help")
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Короче смотри:");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/help - список доступных тебе команд");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/Register - Попытка войти в систему");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/Hi - Пожелать боту здоровья");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Пароли регистрации:");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/Пароль: Пароль1 (Talent Management)");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/Пароль: Пароль2 (Buisness Developement)");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/Пароль: Пароль3 (Outgoing exchanges)");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/Пароль: Пароль4 (Incoming)");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "/Пароль: Пароль5 (Community)");
                        return;
                    }
                    if (IsRegisterStart)
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Неа, не подходит. Чтобы попробовать еще раз - введи: /Register");
                        IsRegisterStart = false;
                    }
                    if (message.Text == "/Register" && IsRegisterStart == false)
                    {
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Пароль?");
                        await Bot.SendTextMessageAsync(message.Chat.Id, "Например: \"/Пароль: Рыба-меч!\"");
                        LogSend("Пользователь ==> " + message.Chat.FirstName + " " + message.Chat.LastName + " пробует сказать пароль!", Color.LightGreen);
                        IsRegisterStart = true;
                    }

                }
            };
            Bot.StartReceiving();
        }
        private async Task<bool> RegisterStartAsync(Telegram.Bot.Types.Message message ,string departament, string email)
        {
            Models.User user = new Models.User() { FirstName = message.Chat.FirstName, LastName = "", TgChatId = Convert.ToString(message.Chat.Id), VkUserId = "", PhoneNumber = "", Email = email, Depatment = departament };
            if (NewUserAdd(user))
            {
                string userName = message.Chat.FirstName + " " + message.Chat.LastName + " (@" + message.Chat.Username + "null" + ")";
                await Bot.SendTextMessageAsync(message.Chat.Id, "Рад видеть тебя в наших рядах, " + userName);
                await Bot.SendTextMessageAsync(message.Chat.Id, departament);
                IsEmailInputing = false;
                IsRegisterStart = false;
            }
            return true;
        }
        private bool NewUserAdd(Models.User user)
        {
            try
            {
                var RegistredUser = db.Users.Where(x => x.TgChatId == user.TgChatId).FirstOrDefault();
                if (RegistredUser == null)
                {

                    db.Users.Add(user);
                    db.SaveChanges();
                    BotNotify?.Invoke("Пользователь " + user.FirstName + " " + user.LastName + " добавлен успешно!", Color.Green);
                    IsRegisterStart = false;
                    return true;
                }
                else
                {
                    Bot.SendTextMessageAsync(user.TgChatId, RegistredUser.FirstName + ", я тебя уже запонил. Тебе нет смысла проходить регистрацию снова");
                    IsRegisterStart = false;
                    return false;
                }
            }
            catch (Exception e)
            {
                BotNotify?.Invoke(e.Message + "==>" + user.FirstName + " " + user.LastName, Color.Red);
                return false;
            }
        }
        public async Task<bool> SendPhotoAsync(string Path, int Chatid)
        {
            try
            {

                FileStream fstream = System.IO.File.OpenRead(Path);
                FileToSend file = new FileToSend("name", fstream);
                await Bot.SendPhotoAsync(Chatid, file);
                LogSend("Сообщение отправлено ==>" + Chatid, Color.LightGreen);
                return true;
            }
            catch (Exception e)
            {
                BotNotify?.Invoke(e.Message + "==>" + Chatid, Color.Red);
                return false;
            }
        }

        public async Task<bool> SendMessageAsync(string msg, int Chatid)
        {
            try
            {
                await Bot.SendTextMessageAsync(Chatid, msg);
                LogSend("Сообщение отправлено ==>" + Chatid, Color.LightGreen);
                return true;
            }
            catch (Exception e)
            {

                BotNotify?.Invoke(e.Message + "==>" + Chatid, Color.Red);
                return false;
            }
        }

        public void LogSend(string msg, Color color)
        {
            BotNotify?.Invoke(msg, color);
        }
    }
}