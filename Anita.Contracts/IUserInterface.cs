using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SachsenCoder.Anita.Contracts.Data;

namespace SachsenCoder.Anita.Contracts
{
    public interface IUserInterface
    {
        event Action<string> SearchCelebrityRequest;
        event Action<FetchCelebrityPicturesData> FetchCelebrityPicturesRequest;
        event Action<KeyValuePair<string, string>> StoreSettingRequest;

        void InputCancellationTokenSource(CancelSource<FetchCelebrityPicturesData> data);
        void ShowCelebritySearchResult(IEnumerable<SearchCelebrityAnswerData> data);
        void InputFetchProgressInfo(FetchProgressInfo data);
        void ReceiveErrorData(ErrorData data);
        void InputSetting(KeyValuePair<string, string> data);
    }
}
