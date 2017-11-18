﻿using System.Collections.ObjectModel;
using UWP_XMPP_Client.DataTemplates;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using XMPP_API.Classes;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using UWP_XMPP_Client.Controls;
using Logging;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.ApplicationModel.Activation;
using UWP_XMPP_Client.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Data_Manager2.Classes.DBManager;
using Data_Manager2.Classes;
using Data_Manager2.Classes.DBTables;

namespace UWP_XMPP_Client.Pages
{
    public sealed partial class ChatPage : Page
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private ObservableCollection<Chat> chats { get; set; }
        private string toastActivationString;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        /// <summary>
        /// Basic Constructor
        /// </summary>
        /// <history>
        /// 26/08/2017 Created [Fabian Sauter]
        /// </history>
        public ChatPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += AbstractBackRequestPage_BackRequested;
            ChatManager.INSTANCE.ChatChanged += INSTANCE_ChatChanged;
            this.toastActivationString = null;
            UiUtils.setBackgroundImage(backgroundImage_img);
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public MasterDetailsView getMasterDetailsView()
        {
            return masterDetail_pnl;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void loadChats()
        {
            chats = new ObservableCollection<Chat>();

            Chat selectedChat = null;
            foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
            {
                List<ChatTable> list = ChatManager.INSTANCE.getAllChatsForClient(c.getXMPPAccount().getIdAndDomain());
                //sortChats(list);
                foreach (ChatTable chat in list)
                {
                    Chat chatElement = new Chat { chat = chat, client = c };
                    addToChatsSorted(chatElement);
                    if (toastActivationString != null && toastActivationString.Equals(chat.id))
                    {
                        selectedChat = chatElement;
                        toastActivationString = null;
                    }
                }
            }
            toastActivationString = null;

            // Show the selected chat
            if (masterDetail_pnl.SelectedItem == null && selectedChat != null)
            {
                masterDetail_pnl.SelectedItem = selectedChat;
            }
        }

        private void sortChats(List<ChatTable> list)
        {
            list.Sort((ChatTable a, ChatTable b) =>
            {
                if (a == b && a == null)
                {
                    return 0;
                }
                if (a.lastActive == null)
                {
                    if (b.lastActive == null)
                    {
                        return 0;
                    }
                    return -1;
                }
                if (b.lastActive == null)
                {
                    return 1;
                }
                return b.lastActive.CompareTo(a.lastActive);
            });
        }

        private async Task addChatAsync(XMPPClient client, string jabberId, bool addToRooster, bool requestSubscription)
        {
            if (client == null || jabberId == null)
            {
                string errorMessage = "Unable to add chat! client ?= " + (client == null) + " jabberId ?=" + (jabberId == null);
                Logger.Error(errorMessage);
                MessageDialog messageDialog = new MessageDialog("Error")
                {
                    Content = errorMessage
                };
                await messageDialog.ShowAsync();
            }
            else
            {
                if (addToRooster)
                {
                    await client.addToRosterAsync(jabberId);
                }
                if (requestSubscription)
                {
                    await client.requestPresenceSubscriptionAsync(jabberId);
                }
                ChatManager.INSTANCE.setChat(new ChatTable()
                {
                    id = ChatTable.generateId(jabberId, client.getXMPPAccount().getIdAndDomain()),
                    chatJabberId = jabberId,
                    userAccountId = client.getXMPPAccount().getIdAndDomain(),
                    ask = null,
                    inRoster = false,
                    lastActive = DateTime.Now,
                    muted = false,
                    presence = Presence.Unavailable,
                    status = null,
                    subscription = requestSubscription ? "pending" : null
                }, false, true);
                loadChats();
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private async void INSTANCE_ChatChanged(ChatManager handler, Data_Manager.Classes.Events.ChatChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ChatTable chatEntry = args.CHAT;
                foreach (Chat c in chats)
                {
                    if (c.chat != null && c.chat.id.Equals(chatEntry.id))
                    {
                        if (!args.REMOVED)
                        {
                            c.chat = chatEntry;
                        }
                        else
                        {
                            var selecetItem = masterDetail_pnl.SelectedItem;
                            chats.Remove(c);
                            if(selecetItem != c)
                            {
                                masterDetail_pnl.SelectedItem = selecetItem;
                            }
                        }
                        args.Cancel = true;
                        return;
                    }
                }

                foreach (XMPPClient c in ConnectionHandler.INSTANCE.getClients())
                {
                    if (chatEntry.userAccountId.Equals(c.getXMPPAccount().getIdAndDomain()))
                    {
                        Chat chatElement = new Chat { chat = args.CHAT, client = c };
                        addToChatsSorted(chatElement);
                    }
                }
            });
        }

        private void addToChatsSorted(Chat chat)
        {
            var selecetItem = masterDetail_pnl.SelectedItem;
            for (int i = 0; i < chats.Count; i++)
            {
                if(DateTime.Compare(chats[i].chat.lastActive, chat.chat.lastActive) <= 0)
                {
                    chats.Insert(i, chat);
                    masterDetail_pnl.SelectedItem = selecetItem;
                    return;
                }
            }
            chats.Add(chat);
            masterDetail_pnl.SelectedItem = selecetItem;
        }

        private void AbstractBackRequestPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                return;
            }
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void settings_abb_Click(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(SettingsPage));
        }

        private void add_abb_Click(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loading_grid.Visibility = Visibility.Visible;
            main_grid.Visibility = Visibility.Collapsed;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            if (e.Parameter is ToastNotificationActivatedEventArgs)
            {
                var toasActivationArgs = e.Parameter as ToastNotificationActivatedEventArgs;
                toastActivationString = toasActivationArgs.Argument;
                Logger.Info("ChatPage activated through toast with argument:" + toastActivationString);
            }
            ConnectionHandler.INSTANCE.connectAll();
            loadChats();

            loading_grid.Visibility = Visibility.Collapsed;
            main_grid.Visibility = Visibility.Visible;
        }

        private void ChatMasterControl_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {

        }

        private async void addChat_mfoi_Click(object sender, RoutedEventArgs e)
        {
            AddChatContentDialog addChatContentDialog = new AddChatContentDialog();
            await addChatContentDialog.ShowAsync();
            if (!addChatContentDialog.cancled)
            {
                await addChatAsync(addChatContentDialog.client, addChatContentDialog.jabberId, addChatContentDialog.addToRooster, addChatContentDialog.requestSubscription);
            }
        }

        private void addMUC_mfoi_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void masterDetail_pnl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Settings.getSettingBoolean(SettingsConsts.DONT_SEND_CHAT_STATE))
            {
                foreach (var added in e.AddedItems)
                {
                    if (added is Chat)
                    {
                        Chat c = added as Chat;
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.ACTIVE);
                    }
                }
                foreach (var added in e.RemovedItems)
                {
                    if (added is Chat)
                    {
                        Chat c = added as Chat;
                        await c.client.sendChatStateAsync(c.chat.chatJabberId, XMPP_API.Classes.Network.XML.Messages.XEP_0085.ChatState.INACTIVE);
                    }
                }
            }
        }

        #endregion
    }
}
