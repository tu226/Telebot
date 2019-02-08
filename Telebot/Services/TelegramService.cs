﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Telebot.Commands;
using Telebot.Events;
using Telebot.Managers;
using Telebot.Models;
using Telebot.Views;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Telebot.Services
{
    public class TelegramService : ICommunicationService
    {
        private string token;
        private List<long> whitelist;

        private readonly TelegramBotClient client;
        private readonly ISettings settings;
        private readonly IMainFormView mainFormView;

        private Dictionary<string, ICommand> commands;

        public TelegramService()
        {
            mainFormView = Program.container.GetInstance<MainForm>();

            this.commands = new Dictionary<string, ICommand>();

            var commands = Program.container.GetAllInstances<ICommand>();
            foreach (ICommand command in commands)
            {
                this.commands.Add(command.Name, command);
                command.Completed += CommandCompleted;
            }

            settings = Program.container.GetInstance<ISettings>();

            LoadSettings();

            if (!string.IsNullOrEmpty(token))
            {
                client = new TelegramBotClient(token);
                client.OnMessage += BotMessageHandler;
                doStartup();
            }

            EventAggregator.Instance.Subscribe<HighTemperatureMessage>(OnHighTemperature);
        }

        private void LoadSettings()
        {
            token = settings.TelegramToken;
            whitelist = settings.TelegramWhitelist;
        }

        private async void doStartup()
        {
            mainFormView.Text += $" - ({(await client.GetMeAsync()).Username})";

            foreach(long chatid in whitelist)
            {
                await client.SendTextMessageAsync
                (
                    chatid,
                    "*Telebot*: I'm Up.",
                    parseMode: ParseMode.Markdown
                );
            }
        }

        private void CommandCompleted(object sender, CommandResult e)
        {
            switch (e.SendType)
            {
                case SendType.Text:
                    client.SendTextMessageAsync(e.Message.Chat.Id,
                        e.Text.TrimEnd(),
                        parseMode: ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId);
                    break;
                case SendType.Photo:
                    client.SendDocumentAsync(e.Message.Chat.Id, e.Stream,
                        caption: $"{DateTime.Now.ToString()} by *Telebot*",
                        parseMode: ParseMode.Markdown,
                        replyToMessageId: e.Message.MessageId);
                    break;
            }
        }

        private void OnHighTemperature(HighTemperatureMessage obj)
        {
            foreach (long chatid in whitelist)
            {
                client.SendTextMessageAsync(chatid, obj.Message, parseMode: ParseMode.Markdown);
            }
        }

        private void BotMessageHandler(object sender, MessageEventArgs e)
        {
            if (!whitelist.Exists(x => x.Equals(e.Message.From.Id)))
            {
                var cmdResult = new CommandResult
                {
                    Message = e.Message,
                    SendType = SendType.Text,
                    Text = "Unauthorized."
                };
                CommandCompleted(sender, cmdResult);
                return;
            }

            string cmdKey = e.Message.Text;

            if (string.IsNullOrEmpty(cmdKey))
            {
                return;
            }

            string info = $"Received {cmdKey} from {e.Message.From.Username}.";

            var item = new LvItem
            {
                DateTime = DateTime.Now.ToString(),
                Text = info
            };

            mainFormView.ObjectListView.AddObject(item);

            if (mainFormView.WindowState == FormWindowState.Minimized)
            {
                EventAggregator.Instance.Publish(new ShowNotifyIconBalloon(info));
            }

            if (commands.Keys.Any(key => Regex.IsMatch($"^{cmdKey}$", key)))
            {
                var key = commands.Keys.Single(k => Regex.IsMatch($"^{cmdKey}$", k));

                var cmdInfo = new CommandParam
                {
                    Message = e.Message,
                    Commands = commands.Values.ToArray(),
                    Parameters = Regex.Match(cmdKey, key)
                };

                commands.Single(pair => Regex.IsMatch($"^{cmdKey}$", pair.Key)).Value.ExecuteAsync(cmdInfo);
            }
            else
            {
                var cmdResult = new CommandResult
                {
                    Message = e.Message,
                    SendType = SendType.Text,
                    Text = "Undefined command. For commands list, type */help*."
                };
                CommandCompleted(sender, cmdResult);
            }
        }

        public void Start()
        {
            if (client != null && !client.IsReceiving)
            {
                client.StartReceiving();
            }
        }

        public void Stop()
        {
            if (client != null && client.IsReceiving)
            {
                client.StopReceiving();
            }
        }
    }
}
