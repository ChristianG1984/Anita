using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SachsenCoder.Anita.Contracts.Data;
using SachsenCoder.Anita.Core.Compositions;

namespace ImageGrabber
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ui = new Form1();
            var grabber = new WikifeetGrabber();
            var async = new AsyncCancellable<string>();
            var suggestCancel = new AutoCanceller<string>();
            var asyncFetch = new AsyncCancellable<FetchCelebrityPicturesData>();
            var sync = new Synchronizer<IEnumerable<SearchCelebrityAnswerData>>();
            var syncProgressInfo = new Synchronizer<FetchProgressInfo>();
            var syncErrorData = new Synchronizer<ErrorData>();
            var packetHandler = new PacketHandler<CancelTarget<string>, IEnumerable<SearchCelebrityAnswerData>>();
            var settingStore = new Store();

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

            settingStore.DistributeSettings();

            Application.Run(ui);
        }
    }
}
