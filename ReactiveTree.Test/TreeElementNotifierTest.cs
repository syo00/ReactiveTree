using System;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reactive.Linq;
using Kirinji.ReactiveTree.TreeStructures;
using Kirinji.ReactiveTree.Merging;
using Kirinji.LightWands;
using Kirinji.LightWands.Tests;

namespace Kirinji.ReactiveTree.Test
{
    [TestClass]
    public class TreeElementNotifierTest : ReactiveTest
    {
        [TestMethod]
        public void UpdateJsonTest()
        {
            var json1 =
@"{
    ""a"": 123, 
    ""name"": ""Yamada""
}
";
            var json2 =
@"{
    ""a"": 123, 
    ""user"": {
        ""id"": 1
    }
}
";
            var json3 =
@"{
    ""name"": ""Mark"",
    ""user"": {
        ""id"": 2
    }
}
";
            var rootId = TreeElementConverter.RootId;
            var j = new TreeElementNotifier<string, IDataObject>(TreeElementConverter.ConvertJson("{ }").NodeChildren[rootId]);
            j.GetValue(new KeyOrIndex<string>("name")).Value.IsNull();
            j.GetValue(new KeyOrIndex<string>("user"), new KeyOrIndex<string>("id")).Value.IsNull();

            var nameHistory = j.ValueChanged(new KeyOrIndex<string>("name")).SubscribeHistory();
            var userIdHistory = j.ValueChanged(new KeyOrIndex<string>("user"), new KeyOrIndex<string>("id")).SubscribeHistory();

            j.ModifyCurrentTreeStraight(tree => tree.Merge(TreeElementConverter.ConvertJson(json1).NodeChildren[rootId], (x, y) => false));
            j.ModifyCurrentTreeStraight(tree => tree.Merge(TreeElementConverter.ConvertJson(json2).NodeChildren[rootId], (x, y) => false));
            j.ModifyCurrentTreeStraight(tree => tree.Merge(TreeElementConverter.ConvertJson(json3).NodeChildren[rootId], (x, y) => false));

            userIdHistory.Values.ElementAt(0).Value.IsNull();
            userIdHistory.Values.ElementAt(1).Value.LeafValue.CastOrDefault<int>().Is(1);
            userIdHistory.Values.ElementAt(2).Value.LeafValue.CastOrDefault<int>().Is(2);
            userIdHistory.Values.ElementAt(0).Directory.Is(userIdHistory.Values.ElementAt(1).Directory);
            userIdHistory.Values.ElementAt(1).Directory.Is(userIdHistory.Values.ElementAt(2).Directory);
            userIdHistory.Values.Count().Is(3);

            nameHistory.Values.ElementAt(0).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.Values.ElementAt(1).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.Values.ElementAt(2).Value.LeafValue.CastOrDefault<string>().Is("Mark");
            nameHistory.Values.ElementAt(0).Directory.Is(nameHistory.Values.ElementAt(1).Directory);
            nameHistory.Values.ElementAt(1).Directory.Is(nameHistory.Values.ElementAt(2).Directory);
            nameHistory.Values.Count().Is(3);
            
            j.GetValue(new KeyOrIndex<string>("name")).Value.LeafValue.CastOrDefault<string>().Is("Mark");
        }

        [TestMethod]
        public void MergeJsonTestForTweet()
        {
            string json1 =
            #region json1 text
 @"
  {
    ""coordinates"": null,
    ""truncated"": false,
    ""created_at"": ""Tue Aug 28 21:16:23 +0000 2012"",
    ""favorited"": false,
    ""id_str"": ""240558470661799936"",
    ""in_reply_to_user_id_str"": null,
    ""entities"": {
      ""urls"": [
      ],
      ""hashtags"": [
      ],
      ""user_mentions"": [
      ]
    },
    ""text"": ""just another test"",
    ""contributors"": null,
    ""id"": 240558470661799936,
    ""retweet_count"": 0,
    ""in_reply_to_status_id_str"": null,
    ""geo"": null,
    ""retweeted"": false,
    ""in_reply_to_user_id"": null,
    ""place"": null,
    ""source"": ""<a href=\""http://realitytechnicians.com\"" rel=\""nofollow\"">OAuth Dancer Reborn</a>"",
    ""user"": {
      ""name"": ""OAuth Dancer"",
      ""profile_sidebar_fill_color"": ""DDEEF6"",
      ""profile_background_tile"": true,
      ""profile_sidebar_border_color"": ""C0DEED"",
      ""profile_image_url"": ""http://a0.twimg.com/profile_images/730275945/oauth-dancer_normal.jpg"",
      ""created_at"": ""Wed Mar 03 19:37:35 +0000 2010"",
      ""location"": ""San Francisco, CA"",
      ""follow_request_sent"": false,
      ""id_str"": ""119476949"",
      ""is_translator"": false,
      ""profile_link_color"": ""0084B4"",
      ""entities"": {
        ""url"": {
          ""urls"": [
            {
              ""expanded_url"": null,
              ""url"": ""http://bit.ly/oauth-dancer"",
              ""indices"": [
                0,
                26
              ],
              ""display_url"": null
            }
          ]
        },
        ""description"": null
      },
      ""default_profile"": false,
      ""url"": ""http://bit.ly/oauth-dancer"",
      ""contributors_enabled"": false,
      ""favourites_count"": 7,
      ""utc_offset"": null,
      ""profile_image_url_https"": ""https://si0.twimg.com/profile_images/730275945/oauth-dancer_normal.jpg"",
      ""id"": 119476949,
      ""listed_count"": 1,
      ""profile_use_background_image"": true,
      ""profile_text_color"": ""333333"",
      ""followers_count"": 28,
      ""lang"": ""en"",
      ""protected"": false,
      ""geo_enabled"": true,
      ""notifications"": false,
      ""description"": """",
      ""profile_background_color"": ""C0DEED"",
      ""verified"": false,
      ""time_zone"": null,
      ""profile_background_image_url_https"": ""https://si0.twimg.com/profile_background_images/80151733/oauth-dance.png"",
      ""statuses_count"": 166,
      ""profile_background_image_url"": ""http://a0.twimg.com/profile_background_images/80151733/oauth-dance.png"",
      ""default_profile_image"": false,
      ""friends_count"": 14,
      ""following"": false,
      ""show_all_inline_media"": false,
      ""screen_name"": ""oauth_dancer""
    },
    ""in_reply_to_screen_name"": null,
    ""in_reply_to_status_id"": null
  }
";
            #endregion
            string json2 = // ["favorited"] false => true, ["user"/"name"] "OAuth Dancer" => "OAuth Player"
            #region json2 text
 @"
  {
    ""coordinates"": null,
    ""truncated"": false,
    ""created_at"": ""Tue Aug 28 21:16:23 +0000 2012"",
    ""favorited"": true,
    ""id_str"": ""240558470661799936"",
    ""in_reply_to_user_id_str"": null,
    ""entities"": {
      ""urls"": [
      ],
      ""hashtags"": [
      ],
      ""user_mentions"": [
      ]
    },
    ""text"": ""just another test"",
    ""contributors"": null,
    ""id"": 240558470661799936,
    ""retweet_count"": 0,
    ""in_reply_to_status_id_str"": null,
    ""geo"": null,
    ""retweeted"": false,
    ""in_reply_to_user_id"": null,
    ""place"": null,
    ""source"": ""<a href=\""http://realitytechnicians.com\"" rel=\""nofollow\"">OAuth Dancer Reborn</a>"",
    ""user"": {
      ""name"": ""OAuth Player"",
      ""profile_sidebar_fill_color"": ""DDEEF6"",
      ""profile_background_tile"": true,
      ""profile_sidebar_border_color"": ""C0DEED"",
      ""profile_image_url"": ""http://a0.twimg.com/profile_images/730275945/oauth-dancer_normal.jpg"",
      ""created_at"": ""Wed Mar 03 19:37:35 +0000 2010"",
      ""location"": ""San Francisco, CA"",
      ""follow_request_sent"": false,
      ""id_str"": ""119476949"",
      ""is_translator"": false,
      ""profile_link_color"": ""0084B4"",
      ""entities"": {
        ""url"": {
          ""urls"": [
            {
              ""expanded_url"": null,
              ""url"": ""http://bit.ly/oauth-dancer"",
              ""indices"": [
                0,
                26
              ],
              ""display_url"": null
            }
          ]
        },
        ""description"": null
      },
      ""default_profile"": false,
      ""url"": ""http://bit.ly/oauth-dancer"",
      ""contributors_enabled"": false,
      ""favourites_count"": 7,
      ""utc_offset"": null,
      ""profile_image_url_https"": ""https://si0.twimg.com/profile_images/730275945/oauth-dancer_normal.jpg"",
      ""id"": 119476949,
      ""listed_count"": 1,
      ""profile_use_background_image"": true,
      ""profile_text_color"": ""333333"",
      ""followers_count"": 28,
      ""lang"": ""en"",
      ""protected"": false,
      ""geo_enabled"": true,
      ""notifications"": false,
      ""description"": """",
      ""profile_background_color"": ""C0DEED"",
      ""verified"": false,
      ""time_zone"": null,
      ""profile_background_image_url_https"": ""https://si0.twimg.com/profile_background_images/80151733/oauth-dance.png"",
      ""statuses_count"": 166,
      ""profile_background_image_url"": ""http://a0.twimg.com/profile_background_images/80151733/oauth-dance.png"",
      ""default_profile_image"": false,
      ""friends_count"": 14,
      ""following"": false,
      ""show_all_inline_media"": false,
      ""screen_name"": ""oauth_dancer""
    },
    ""in_reply_to_screen_name"": null,
    ""in_reply_to_status_id"": null
  }
";
            #endregion

            var rootId = TreeElementConverter.RootId;
            var n = new TreeElementNotifier<string, IDataObject>(TreeElementConverter.ConvertJson(json1));

            var idHistory = n.ValueChanged(new KeyOrIndex<string>(rootId), new KeyOrIndex<string>("id")).SubscribeHistory();
            var userNameHistory = n.ValueChanged(new KeyOrIndex<string>(rootId), new KeyOrIndex<string>("user"), new KeyOrIndex<string>("name")).SubscribeHistory();
            var favoritedHistory = n.ValueChanged(new KeyOrIndex<string>(rootId), new KeyOrIndex<string>("favorited")).SubscribeHistory();

            n.CurrentTree.Merge(TreeElementConverter.ConvertJson(json1), (x, y) => false);
            n.CurrentTree.Merge(TreeElementConverter.ConvertJson(json2), (x, y) => false);

            idHistory.Values.Select(x => x.Value.LeafValue.CastOrDefault<long>()).Is(240558470661799936, 240558470661799936);
            userNameHistory.Values.Select(x => x.Value.LeafValue.CastOrDefault<string>()).Is("OAuth Dancer", "OAuth Player");
            favoritedHistory.Values.Select(x => x.Value.LeafValue.CastOrNull<bool>()).Is(false, true);
        }
    }
}
