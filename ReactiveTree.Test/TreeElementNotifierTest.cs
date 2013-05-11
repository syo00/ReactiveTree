using System;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reactive.Linq;
using Kirinji.ReactiveTree.Merging;
using Kirinji.ReactiveTree.TreeElements;
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
            var j = new TreeElementNotifier<string, IDataObject>(TreeElementConverter.ConvertJson("{ }"));
            var nameHistory = j.GetValueAndChanged(rootId, "name").DistinctUntilChanged().SubscribeHistory();
            var userIdHistory = j.GetValueAndChanged(rootId, "user", "id").DistinctUntilChanged().SubscribeHistory();

            j.GetValue(rootId, "name").Any().IsFalse();
            j.GetValue(rootId, "user", "id").Any().IsFalse();

            j.CurrentTree[rootId].MergeWithArraySelector(TreeElementConverter.ConvertJson(json1)[rootId], (t, d) => t.GetSingleValue("id"));
            j.GetValue(rootId, "name").Single().Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            j.GetValue(rootId, "user", "id").Any().IsFalse();

            j.CurrentTree[rootId].MergeWithArraySelector(TreeElementConverter.ConvertJson(json2)[rootId], (t, d) => t.GetSingleValue("id"));
            j.GetValue(rootId, "name").Single().Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            j.GetValue(rootId, "user", "id").Single().Value.LeafValue.CastOrDefault<int>().Is(1);

            j.CurrentTree[rootId].MergeWithArraySelector(TreeElementConverter.ConvertJson(json3)[rootId], (t, d) => t.GetSingleValue("id"));
            j.GetValue(rootId, "name").Single().Value.LeafValue.CastOrDefault<string>().Is("Mark");
            j.GetValue(rootId, "user", "id").Single().Value.LeafValue.CastOrDefault<int>().Is(2);

            nameHistory.ElementAt(0).Value.Any().IsFalse();
            nameHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(2).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.CastOrDefault<string>().Is("Mark");
            nameHistory.Count().Is(3);
            userIdHistory.ElementAt(0).Value.Any().IsFalse();
            userIdHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null, null })).Value.LeafValue.CastOrNull<int>().Is(1);
            userIdHistory.ElementAt(2).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null, null })).Value.LeafValue.CastOrNull<int>().Is(2);
            userIdHistory.Count().Is(3);
        }

        [TestMethod]
        public void MergeJsonTest()
        {
            var json1 =
@"{
    ""id"": 123, 
    ""name"": ""Yamada"",
    ""client"": {
        ""id"": 100
    }
}
";
            var json2 =
@"{
    ""id"": 123, 
    ""client"": {
        ""id"": 200
    },
    ""user"": {
        ""id"": 1
    }
}
";
            var rootId = TreeElementConverter.RootId;
            var j = new TreeElementNotifier<string, IDataObject>(TreeElementConverter.ConvertJson("{ }"));
            var idHistory = j.GetValueAndChanged(rootId, "id").DistinctUntilChanged().SubscribeHistory();
            var nameHistory = j.GetValueAndChanged(rootId, "name").DistinctUntilChanged().SubscribeHistory();
            var userIdHistory = j.GetValueAndChanged(rootId, "user", "id").DistinctUntilChanged().SubscribeHistory();
            var clientIdHistory = j.GetValueAndChanged(rootId, "client", "id").DistinctUntilChanged().SubscribeHistory();

            j.CurrentTree[rootId].MergeWithArraySelector(TreeElementConverter.ConvertJson(json1)[rootId], (x, y) => new object());
            j.CurrentTree[rootId].MergeWithArraySelector(TreeElementConverter.ConvertJson(json2)[rootId], (x, y) => new object());

            idHistory.ElementAt(0).Value.Any().IsFalse();
            idHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.CastOrNull<int>().Is(123);
            idHistory.Count().Is(2);

            nameHistory.ElementAt(0).Value.Any().IsFalse();
            nameHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.Count().Is(2);

            userIdHistory.ElementAt(0).Value.Any().IsFalse();
            userIdHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null, null })).Value.LeafValue.CastOrNull<int>().Is(1);
            userIdHistory.Count().Is(2);

            clientIdHistory.ElementAt(0).Value.Any().IsFalse();
            clientIdHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null, null })).Value.LeafValue.CastOrNull<int>().Is(100);
            clientIdHistory.ElementAt(2).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null, null })).Value.LeafValue.CastOrNull<int>().Is(200);
            clientIdHistory.Count().Is(3);
        }

        [TestMethod]
        public void MergeJsonTest2()
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

            var idHistory = n.GetValueAndChanged(rootId, "id").DistinctUntilChanged().SubscribeHistory();
            var userNameHistory = n.GetValueAndChanged(rootId, "user", "name").DistinctUntilChanged().SubscribeHistory();
            var favoritedHistory = n.GetValueAndChanged(rootId, "favorited").DistinctUntilChanged().SubscribeHistory();
            n.CurrentTree[rootId].MergeWithArraySelector(TreeElementConverter.ConvertJson(json1)[rootId], (x, y) => new object());
            n.CurrentTree[rootId].MergeWithArraySelector(TreeElementConverter.ConvertJson(json2)[rootId], (x, y) => new object());

            idHistory.Values.Select(gc => gc.Single().Value.LeafValue.CastOrNull<long>()).Is(240558470661799936);
            userNameHistory.Values.Select(gc => gc.Single().Value.LeafValue.CastOrDefault<string>()).Is("OAuth Dancer", "OAuth Player");
            userNameHistory.Values.Select(gc => gc.Single().Value.LeafValue.CastOrDefault<string>()).Is("OAuth Dancer", "OAuth Player");
            favoritedHistory.Values.Select(gc => gc.Single().Value.LeafValue.CastOrNull<bool>()).Is(false, true);
        }

        [TestMethod]
        public void ModifyArrayTest()
        {
            var json = TreeElementConverter.ConvertJson(
@"{
    ""a"": 123, 
    ""name"": [""Yamada"", ""Mark""]
}
"
                );

            var rootId = TreeElementConverter.RootId;
            var j = new TreeElementNotifier<string, IDataObject>(json);
            var nameHistory = j.GetValueAndChanged(rootId, "name").DistinctUntilChanged().SubscribeHistory();
            var aHistory = j.GetValueAndChanged(rootId, "a").DistinctUntilChanged().SubscribeHistory();
            var bHistory = j.GetValueAndChanged(rootId, "b").DistinctUntilChanged().SubscribeHistory();

            j.CurrentTree[rootId].ModifyArrayChild("name", l => l.Add(new TreeElement<string, IDataObject>(new DataObject("Yamada"))));
            j.CurrentTree[rootId].ModifyArrayChild("name", l => l.Add(new TreeElement<string, IDataObject>(new DataObject("Tanaka"))));
            j.CurrentTree[rootId].ModifyArrayChild("name", l => l.RemoveLast());
            j.CurrentTree[rootId].ModifyArrayChild("a", _ => new[] { new TreeElement<string, IDataObject>(new DataObject(1)) });
            j.CurrentTree[rootId].ModifyArrayChild("b", _ => new[] { new TreeElement<string, IDataObject>(new DataObject(2)) });

            nameHistory.ElementAt(0).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0 })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(0).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1 })).Value.LeafValue.CastOrDefault<string>().Is("Mark");
            nameHistory.ElementAt(0).Value.Count().Is(2);

            nameHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0 })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1 })).Value.LeafValue.CastOrDefault<string>().Is("Mark");
            nameHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 2 })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(1).Value.Count().Is(3);

            nameHistory.ElementAt(2).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0 })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(2).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1 })).Value.LeafValue.CastOrDefault<string>().Is("Mark");
            nameHistory.ElementAt(2).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 2 })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(2).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 3 })).Value.LeafValue.CastOrDefault<string>().Is("Tanaka");
            nameHistory.ElementAt(2).Value.Count().Is(4);

            nameHistory.ElementAt(3).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0 })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(3).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 1 })).Value.LeafValue.CastOrDefault<string>().Is("Mark");
            nameHistory.ElementAt(3).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 2 })).Value.LeafValue.CastOrDefault<string>().Is("Yamada");
            nameHistory.ElementAt(3).Value.Count().Is(3);

            nameHistory.Count().Is(4);


            aHistory.ElementAt(0).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, null })).Value.LeafValue.CastOrNull<int>().Is(123);
            aHistory.ElementAt(0).Value.Count().Is(1);

            aHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0 })).Value.LeafValue.CastOrNull<int>().Is(1);
            aHistory.ElementAt(1).Value.Count().Is(1);

            aHistory.Count().Is(2);


            bHistory.ElementAt(0).Value.Any().IsFalse();

            bHistory.ElementAt(1).Value.Single(gc => gc.Indexes.SequenceEqual(new int?[] { null, 0 })).Value.LeafValue.CastOrNull<int>().Is(2);
            bHistory.ElementAt(1).Value.Count().Is(1);

            bHistory.Count().Is(2);
        }
    }
}
