using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SachsenCoder.Anita.Contracts;
using SachsenCoder.Anita.Core.Leafs;
using SachsenCoder.Anita.Core.Compositions;
using SachsenCoder.Anita.Contracts.Data;

namespace SachsenCoder.Anita.Core
{
    public class CoreComposition
    {
        public CoreComposition(IUserInterface ui)
        {
            var grabber = new WikifeetGrabber();
            var async = new AsyncCancellable<string>();
            var suggestCancel = new AutoCanceller<string>();
            var asyncFetch = new AsyncCancellable<FetchCelebrityPicturesData>();
            var sync = new Synchronizer<IEnumerable<SearchCelebrityAnswerData>>();
            var syncProgressInfo = new Synchronizer<FetchProgressInfo>();
            var syncErrorData = new Synchronizer<ErrorData>();
            var packetHandler = new PacketHandler<CancelTarget<string>, IEnumerable<SearchCelebrityAnswerData>>();
            var settingStore = new Store();

            _loadSettings += settingStore.DistributeSettings;

            ui.SearchCelebrityRequest += async.Input;
            ui.FetchCelebrityPicturesRequest += asyncFetch.Input;
            ui.StoreSettingRequest += settingStore.ReceiveSettingForStorage;

            async.OutputCancelSource += suggestCancel.Input;
            async.OutputCancelTarget += packetHandler.InputPlain;

            packetHandler.OutputUnique += grabber.SearchCelebrity;
            packetHandler.OutputPlain += sync.Input;

            asyncFetch.OutputCancelSource += ui.InputCancellationTokenSource;
            asyncFetch.OutputCancelTarget += grabber.FetchCelebrityPictures;

            grabber.SearchResult += packetHandler.InputUnique;
            grabber.OutputFetchProgressInfo += syncProgressInfo.Input;
            grabber.OutputError += syncErrorData.Input;

            sync.Output += ui.ShowCelebritySearchResult;
            syncProgressInfo.Output += ui.InputFetchProgressInfo;
            syncErrorData.Output += ui.ReceiveErrorData;

            settingStore.OutputSetting += ui.InputSetting;
        }

        public void LoadSettings()
        {
            _loadSettings();
        }

        private event Action _loadSettings;
    }
}
