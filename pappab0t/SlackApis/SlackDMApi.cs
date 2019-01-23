using System;
using MargieBot;

namespace pappab0t.SlackApis
{
    public interface ISlackDMApi
    {
        //void Close(); //Close a direct message channel.
        //void History(); //Fetches history of messages and events from direct message channel.
        //void List(); //Tables.Lists direct message channels for the calling user.
        //void Mark(); //Sets the read cursor in a direct message channel.
        /// <summary>
        /// Opens a direct message channel.
        /// </summary>
        /// <param name="token">Authentication token bearing required scopes.</param>
        /// <param name="user">User to open a direct message channel with.</param>
        /// <param name="includeLocale">Set this to true to receive the locale for this dm. </param>
        /// <param name="inclideResponse">Indicates you want the full IM channel definition in the response.</param>
        /// <returns></returns>
        DmOpenResponse Open(string token, string user, bool includeLocale = false, bool inclideResponse = true);

        //void Replies(); //Retrieve a thread of messages posted to a direct message conversation
    }

    public class DmOpenResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public SlackChatHub Hub { get; set; }
    }



    public class SlackDMApi : ISlackDMApi
    {
        public DmOpenResponse Open(string token, string user, bool includeLocale = false, bool inclideResponse = true)
        {
            Console.WriteLine("Implement https://slack.com/api/im.open.");
            return new DmOpenResponse();
        }
    }


    //public async Task<SlackChatHub> CreateSlackbotDmAsync(SlackImOpenDto data)
    //{
    //    var response = await _dmApi.PostAsync("https://slack.com/api/im.open", new StringContent(JsonConvert.SerializeObject(data))).ConfigureAwait(false);

    //    response.EnsureSuccessStatusCode();

    //    string content = await response.Content.ReadAsStringAsync();
    //    return await Task.Run(() => JsonConvert.DeserializeObject<SlackChatHub>(content);
    //}
}