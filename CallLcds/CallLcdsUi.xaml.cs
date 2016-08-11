using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using FinalesFunkeln.Lol;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using RtmpSharp.IO;
using RtmpSharp.Messaging;

namespace CallLcds
{
    /// <summary>
    /// Interaction logic for CallLcdsUi.xaml
    /// </summary>
    public partial class CallLcdsUi : UserControl
    {
        public LolClient Client { get; set; }

        public ObservableCollection<StackPanel> ArgsControls = new ObservableCollection<StackPanel>();

        private static readonly List<string> DataTypes = new List<string> { "Number", "String", "Boolean", "Object" };
        private static readonly List<string> DefaultValues = new List<string> { "0", "String", "false", "{\n\t\"TypeName\":\"Type name goes here (if any)\",\n\t\"afield\":\"avalue\"\n}" };
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();
        private static readonly IHighlightingDefinition highlighting;

        private static readonly List<string> KnownServices = new List<string>
        {
            "accountManagementService",
            "accountService",
            "leaguesServiceProxy",
            "lcdsServiceProxy",
            "lcdsChampionTradeService",
            "clientFacadeService",
            "lcdsGameInvitationService",
            "gameMapService",
            "gameService",
            "inventoryService",
            "loginService",
            "masteryBookService",
            "matchmakerService",
            "playerPreferencesService",
            "playerStatsService",
            "lcdsQueueRestrictionService",
            "lcdsRerollService",
            "spellBookService",
            "statisticsService",
            "summonerIconService",
            "summonerRuneService",
            "summonerService",
            "summonerTeamService"
        };

        private static readonly Dictionary<string, List<string>> KnownServiceMethods = new Dictionary<string, List<string>>
        {
            ["accountManagementService"] = new List<string> { "changePassword", "changeAccountInformation", "changeEmail", "getAccountSecurityQuestion", "resetPassword", "changePasswordAfterReset" },
            ["accountService"] = new List<string> { "getAccountStateForCurrentSession" },
            ["lcdsServiceProxy"] = new List<string> { "call" },
            ["leaguesServiceProxy"] = new List<string> { "getAllMyLeagues", "getMyLeaguePositionsAndProgress", "getLeaguesForTeam", "getAllLeaguesForPlayer", "getMyLeaguePositions", "getLeagueForPlayer", "getLeagueForTeam", "getMasterLeague", "getMasterLeagueTopX", "getChallengerLeague", "updateMyDecayMessageLastShown", "updateTeamDecayMessageLastShown" },
            ["lcdsChampionTradeService"] = new List<string> { "attemptTrade", "dismissTrade", "getPotentialTraders" },
            ["clientFacadeService"] = new List<string> { "getLoginDataPacketForUser", "reportPlayer", "callPersistenceMessaging", "ackLeaverBusterWarning", "abandonLeaverBusterLowPriorityQueue", "processMicroFeedbackSurveyResponse", "checkForAndSendSurveyQuestionToClient" },
            ["lcdsGameInvitationService"] = new List<string> { "grantInvitePrivileges", "revokeInvitePrivileges", "transferOwnership", "invite", "inviteBulk", "kick", "accept", "decline", "getLobbyStatus", "checkLobbyStatus", "getPendingInvitations", "createArrangedTeamLobby", "createArrangedBotTeamLobby", "createArrangedRankedTeamLobby", "createGroupFinderLobby", "destroyGroupFinderLobby", "leave" },
            ["gameMapService"] = new List<string> { "getGameMapSet", "getGameMapList"},
            ["gameService"] = new List<string> { "createPracticeGame", "joinOrCreatePracticeGame", "createTutorialGame", "listAllPracticeGames", "listAuditInfo", "joinGame", "observeGame", "switchTeams", "acceptPoppedGame", "startChampionSelection", "selectChampion", "selectSpells", "getGameForUser", "cancelSelectChampion", "quitGame", "banUserFromGame", "setClientReceivedGameMessage", "setClientReceivedMaestroMessage", "selectBotChampion", "removeBotChampion", "retrieveInProgressGameInfo", "retrieveInProgressSpectatorGameInfo", "declineObserverReconnect", "championSelectCompleted", "banChampion", "getChampionsForBan", "selectChampionSkin", "getLatestGameTimerState", "getGame", "getCurrentTimerForGame", "banObserverFromGame", "switchPlayerToObserver", "unlockSkinsForTeam", "getSkinUnlockPrice", "initializeTeamSkinRental", "getFeaturedGameMetadata" },
            ["inventoryService"] = new List<string> { "isStoreEnabled", "getSumonerActiveBoosts"/*The missing 'm' is correct*/, "useGrabBag", "getAllRuneCombiners", "useRuneCombiner", "retrieveInventoryTypes", "getAvailableChampions", "giftFacebookFan", "getOwnedWardSkin", "getAvailableItems", "getEntitledItems", "selectItem", "getSelectedItem" },
            ["loginService"] = new List<string> { "login", "logout", "getLoggedInAccountView", "performLCDSHeartBeat", "getStoreUrl", "isLoggedIn" },
            ["masteryBookService"] = new List<string> { "getMasteryBook", "saveMasteryBook", "selectDefaultMasteryBookPage" },
            ["matchmakerService"] = new List<string> { "getAvailableQueues", "attachToQueue", "purgeFromQueues", "cancelFromQueueIfPossible", "getQueueInfo", "isMatchmakingEnabled", "acceptInviteForMatchmakingGame" },
            ["playerPreferencesService"] = new List<string> { "loadPreferencesByKey", "savePreferences", "setEnabled" },
            ["playerStatsService"] = new List<string> { "retrievePlayerStatsByAccountId", "retrieveTopPlayedChampions", "processEloQuestionaire", "getAggregatedStats", "getRecentGames", "getTeamEndOfGameStats", "getTeamAggregatedStats" },
            ["lcdsQueueRestrictionService"] = new List<string> { "getQueueRestrictions" },
            ["lcdsRerollService"] = new List<string> { "roll", "getPointsBalance" },
            ["spellBookService"] = new List<string> { "getSpellBook", "saveSpellBook", "selectDefaultSpellBookPage" },
            ["statisticsService"] = new List<string> { "getSummonerSummaryByInternalName", "altSetUserRatings" },
            ["summonerIconService"] = new List<string> { "getSummonerIconInventory" },
            ["summonerRuneService"] = new List<string> { "getSummonerRunes", "getSummonerRuneInventory" },
            ["summonerService"] = new List<string> { "checkSummonerName", "getAllSummonerDataByAccount", "getAllPublicSummonerDataByAccount", "getSummonerIcons", "getSummonerByName", "getSummonerInternalNameByName", "getSummonerNames", "getSummonerCatalog", "saveSeenTutorialFlag", "saveSeenHelpFlag", "createDefaultSummoner", "playerChangeSummonerName", "resetTalents", "changeTalentRankings", "updateProfileIconId", "getSocialNetworkUsers", "updateSummonerSocialNetworkUser", "getSocialNetworkFriends", "saveSocialNetworkFriendList" },
            ["summonerTeamService"] = new List<string> { "createTeam", "findTeamById", "findPlayer", "createPlayer", "invitePlayer", "joinTeam", "declineInvite", "kickPlayer", "leaveTeam", "disbandTeam", "changeOwner", "findTeamByName", "findTeamByTag", "isNameValidAndAvailable", "isTagValidAndAvailable" },
        };
        static CallLcdsUi()
        {
            KnownServices.Sort();
            foreach(var kv in KnownServiceMethods)
            {
                kv.Value.Sort();
            }
            //Serializer.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
            
            using (XmlTextReader reader = new XmlTextReader("data/highlighting/Javascript.xshd"))
            {
                highlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }
        public CallLcdsUi()
        {
            InitializeComponent();
            ArgsItems.Children.Insert(0,CreateDefaultArgControl(1));
            ServiceNameTextBox.ItemsSource = KnownServices;
        }

        private async void Call_Click(object sender, RoutedEventArgs e)
        {
            var sname = ServiceNameTextBox.Text;
            var mname = MethodNameTextBox.Text;
            var args = new object[ArgsItems.Children.Count-1];
            for (int i = 0; i < ArgsItems.Children.Count - 1; i++)
            {
                StackPanel p = ArgsItems.Children[i] as StackPanel;
                if (p == null) break;
                string value = (p.Children[2] as TextEditor).Text;
                switch ((p.Children[1] as ComboBox).SelectedIndex)
                {
                    case 0://Number
                        double x;
                        if (!double.TryParse(value, out x))
                        {
                            StatusBlock.Foreground = Brushes.Crimson;
                            StatusBlock.Text = $"Argument {i + 1} <{value}> is not a valid Number!";
                            return;
                        }
                        args[i] = x;
                        break;
                    case 1://String
                        args[i] = value;
                        break;
                    case 2://Boolean
                        bool b;
                        if (!bool.TryParse(value, out b))
                        {
                            StatusBlock.Foreground = Brushes.Crimson;
                            StatusBlock.Text = $"Argument {i + 1} <{value}> is not a valid Boolean!";
                            return;
                        }
                        args[i] = b;
                        break;
                    case 3://Object
                        try
                        {
                            object o= Serializer.Deserialize<object>(value);

                            if (o is Dictionary<string, object>)
                            {
                                args[i] = ConvertToAsObject(o as Dictionary<string,object>);
                            }
                            else if (o is Array)
                            {
                                HandleArray(o as object[]);
                            }
                            else
                                args[i] = o;
                        }
                        catch
                        {
                            StatusBlock.Foreground = Brushes.Crimson;
                            StatusBlock.Text = $"Argument {i + 1} is not a valid Object!";
                        }
                        break;
                }
            }
            try
            {
                if (Client != null)
                {
                    object obj = await Client.Connection.InvokeAsync(sname, mname, args);
                    ObjectTree.SetRoot("Invoke", new AsObject {["Destination"] = sname,["Operation"] = mname,["Arguments"] = args,["Result"] = obj });
                    StatusBlock.Foreground = Brushes.Lime;
                    StatusBlock.Text = $"Call successful!";
                }
                else
                {
                    StatusBlock.Foreground = Brushes.Crimson;
                    StatusBlock.Text = $"You are not connected to a server!";
                }
            }
            catch (InvocationException ex)
            {
                ObjectTree.SetRoot("Invoke", new AsObject {["Destination"] = sname,["Operation"] = mname,["Arguments"] = args,["Error"] = ex.RootCause });
                StatusBlock.Foreground = Brushes.Orange;
                StatusBlock.Text = $"The server returned an error!";
            }
        }

        private void HandleArray(object[] v)
        {
            for (int i=0;i<v.Length;i++)
            {
                if (v[i] is Dictionary<string, object>)
                    v[i] = ConvertToAsObject(v[i] as Dictionary<string, object>);
            }
        }

        AsObject ConvertToAsObject(Dictionary<string, object> dict)
        {
            var asobj = new AsObject();
            foreach (var el in dict as Dictionary<string, object>)
                if (el.Key == "TypeName")
                    asobj.TypeName = el.Value as string;
                else
                    asobj[el.Key] = el.Value is Dictionary<string, object>? ConvertToAsObject(el.Value as Dictionary<string,object>):el.Value;
            return asobj;
        }

        private StackPanel CreateDefaultArgControl(int index)
        {
            var ret = new StackPanel { Orientation = Orientation.Horizontal, MinWidth = 300, HorizontalAlignment = HorizontalAlignment.Center };
            ret.Children.Add(new TextBlock { Text = index + ".", Margin = new Thickness(5, 0, 5, 5), MaxHeight=25});
            var cb = new ComboBox { ItemsSource = DataTypes, Margin = new Thickness(5, 0, 5, 5), Width = 100, SelectedIndex = 0, MaxHeight = 25 }; cb.SelectionChanged += DataTypeCB_SelectionChanged;
            ret.Children.Add(cb);
            ret.Children.Add(new TextEditor { Text = "0", Margin = new Thickness(5, 0, 5, 5), MinWidth = 150, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Hidden ,SyntaxHighlighting=highlighting,Padding= new Thickness(3,3,3,3),FontSize=15, VerticalAlignment=VerticalAlignment.Stretch} );
            var removebutton = new Button { Content = "Remove", Margin = new Thickness(5, 0, 5, 5), Width = 60, MaxHeight = 25 };
            ret.DataContext = ret;
            removebutton.Click += Removebutton_Click;
            ret.Children.Add(removebutton);
            return ret;
        }

        private void DataTypeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var x = ((e.Source as Control)?.DataContext as StackPanel)?.Children[2] as TextEditor;
            var cb = sender as ComboBox;
            if (x == null || cb == null)
                return;
            x.Text = DefaultValues[cb.SelectedIndex];
        }

        private void Removebutton_Click(object sender, RoutedEventArgs e)
        {
            ArgsItems.Children.Remove((e.Source as Control).DataContext as StackPanel);
            for (int i = 0; i < ArgsItems.Children.Count - 1; i++)
            {
                StackPanel p = ArgsItems.Children[i] as StackPanel;
                if (p == null) break;
                (p.Children[0] as TextBlock).Text = i + 1 + ".";
            }
        }

        private void AddArgument_Click(object sender, RoutedEventArgs e)
        {
            ArgsItems.Children.Insert(ArgsItems.Children.Count - 1, CreateDefaultArgControl(ArgsItems.Children.Count));
        }

        private void ServiceNameTextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (KnownServiceMethods.ContainsKey(ServiceNameTextBox.Text))
            {
                MethodNameTextBox.ItemsSource = KnownServiceMethods[ServiceNameTextBox.Text];
            }
            else
            {
                MethodNameTextBox.ItemsSource = null;
                //MethodNameTextBox.Text = "";
            }
        }
    }
}
