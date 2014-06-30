using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;

namespace SteamworksNET_StandaloneTest
{
    class Program
    {
	    private static bool _steamInited;
		private static 	SteamAPIWarningMessageHook_t SteamAPIWarningMessageHook;
 
	    private static AutoResetEvent _waitForCallback;
	    private static CGameID _gameId;

	    static void Main(string[] args)
        {
	        if (!SteamAPI.IsSteamRunning())
	        {
		        Console.WriteLine("Please start steam");
				return;
	        }

		    _steamInited = SteamAPI.Init();
		    _gameId = new CGameID(SteamUtils.GetAppID());
		    Thread.Sleep(100);

            if(!_steamInited)
            {
                Console.WriteLine("SteamAPI.Init() failed!");
                return;
            }
			Console.WriteLine("Packsize.Test() returned: {0}", Packsize.Test());

			SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(SteamAPIWarningMessageHook);

			Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		    
		    //    Callback<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardDataReceived);
			

			Task.Factory.StartNew(() => {
				                            while (true)
				                            {
					                            SteamAPI.RunCallbacks();
												Thread.Sleep(100);
				                            }
				}
			);

		    _waitForCallback = new AutoResetEvent(false);
		    _waitForCallback.WaitOne();

//		    while (!Console.KeyAvailable)
//            {
//                SteamAPI.RunCallbacks();
//				Console.Write("+");
//                Thread.Sleep(100);
//            }

            SteamAPI.Shutdown();
        }
		private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
		{
			Console.WriteLine(pchDebugText);
		}
	

	    private static void OnUserStatsReceived(UserStatsReceived_t userStatsReceived)
	    {
			if (!_steamInited)
				return;
		    Console.WriteLine("gameId {0}",userStatsReceived.m_nGameID);
			Console.WriteLine("userId {0}", userStatsReceived.m_steamIDUser);
			Console.WriteLine("result {0}", userStatsReceived.m_eResult);
			Console.WriteLine("user : {0}", SteamUser.GetHSteamUser());
			Console.WriteLine("steam level : {0}", SteamUser.GetPlayerSteamLevel());
			Console.WriteLine("steam id : {0}", SteamUser.GetSteamID());
			int totalGamesPlayed;
			SteamUserStats.GetStat("NumGames", out totalGamesPlayed);
			Console.WriteLine("Games played {0}", totalGamesPlayed);
		    _waitForCallback.Set();

	    }

	    public static void GetLeaderBoard()
	    {
			SteamAPICall_t leaderboard = SteamUserStats.FindOrCreateLeaderboard("Score", ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
	ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);

		     
	    }

	    private static void OnLeaderboardDataReceived(LeaderboardScoresDownloaded_t leaderboardScoresDownloaded)
	    {
		    Console.WriteLine("Count {0}",leaderboardScoresDownloaded.m_cEntryCount);
		    Console.WriteLine("leaderboard {0}",leaderboardScoresDownloaded.m_hSteamLeaderboard);
		    Console.WriteLine("leaderboard {0}",leaderboardScoresDownloaded.m_hSteamLeaderboardEntries);
		    
	    }
    }

	
}
