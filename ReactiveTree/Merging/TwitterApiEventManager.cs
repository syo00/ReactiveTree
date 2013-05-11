using Kirinji.ReactiveTree.TreeElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kirinji.ReactiveTree.Merging
{
    internal class TwitterApiEventManager
    {
        private readonly TreeElementDictionary<TreeElement<string, IDataObject>, string, IDataObject> tweetsDictionary = new TreeElementDictionary<TreeElement<string, IDataObject>, string, IDataObject>(tree => tree.GetSingleChildOrDefault("id_str"));
        private readonly TreeElementDictionary<TreeElement<string, IDataObject>, string, IDataObject> usersDictionary = new TreeElementDictionary<TreeElement<string, IDataObject>, string, IDataObject>(tree => tree.GetSingleChildOrDefault("id_str"));

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.tweetsDictionary != null);
            Contract.Invariant(this.usersDictionary != null);
        }


        public IDirectoryValueChanged<string, IDataObject> MergeAsTweet(TreeElement<string, IDataObject> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var user = source.GetSingleChildOrDefault("user");
            if (user != null)
            {
                var mergedUser = this.usersDictionary.Merge(user);
                if (mergedUser != null && mergedUser.TreeElement != null) source["user"] = mergedUser.TreeElement;
            }

            var tweet = this.tweetsDictionary.Merge(source);
            if (tweet != null) return tweet.Notifier;
            return null;
        }

        public IDirectoryValueChanged<string, IDataObject> MergeAsUser(TreeElement<string, IDataObject> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var status = source.GetSingleChildOrDefault("status");
            if (status != null)
            {
                var mergedStatus = this.tweetsDictionary.Merge(status);
                if (mergedStatus != null && mergedStatus.TreeElement != null) source["status"] = mergedStatus.TreeElement;
            }
            var user = this.usersDictionary.Merge(source);
            if (user != null) return user.Notifier;
            return null;
        }

        public IDirectoryValueChanged<string, IDataObject> MergeAsRetweet(TreeElement<string, IDataObject> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var retweetedStatus = source.GetSingleChildOrDefault("retweeted_status");
            if (retweetedStatus != null)
            {
                var mergedRetweetedStatus = this.tweetsDictionary.Merge(retweetedStatus);
                if (mergedRetweetedStatus != null && mergedRetweetedStatus.TreeElement != null) source["retweeted_status"] = mergedRetweetedStatus.TreeElement;
            }

            var retweet = this.tweetsDictionary.Merge(source);
            if (retweet != null) return retweet.Notifier;
            return null;
        }

        public IDirectoryValueChanged<string, IDataObject> MergeAsFavoriteOrUnfavorite(TreeElement<string, IDataObject> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);

            var targetObject = source.GetSingleChildOrDefault("target_object");
            if (targetObject != null)
            {
                var mergedTargetObject = this.tweetsDictionary.Merge(targetObject);
                if (mergedTargetObject != null && mergedTargetObject.TreeElement != null) source["target_object"] = mergedTargetObject.TreeElement;
            }

            var target = source.GetSingleChildOrDefault("target");
            if (target != null)
            {
                var mergedTarget = this.usersDictionary.Merge(target);
                if (mergedTarget != null && mergedTarget.TreeElement != null) source["target"] = mergedTarget.TreeElement;
            }

            var sourceUser = source.GetSingleChildOrDefault("source");
            if (sourceUser != null)
            {
                var mergedSourceUser = this.usersDictionary.Merge(sourceUser);
                if (mergedSourceUser != null && mergedSourceUser.TreeElement != null) source["source"] = mergedSourceUser.TreeElement;
            }

            var inner = new TreeElementNotifier<string, IDataObject>(source);
            return new TreeElementNotifierFilter<string, IDataObject>(inner, directory =>
            {
                var first = directory.ElementAtOrDefault(0);
                if (first == "target_object" || first == "target" || first == "source") return true;
                return false;
            });
        }

        public IEnumerable<WeakReference<TreeElementDictionary<TreeElement<string, IDataObject>, string, IDataObject>.TreeElementAndNotifierPair>> GetTweets()
        {
            return this.tweetsDictionary.GetAllTreeElement().Select(pair => pair.Value).ToArray();
        }

        public IEnumerable<WeakReference<TreeElementDictionary<TreeElement<string, IDataObject>, string, IDataObject>.TreeElementAndNotifierPair>> GetUsers()
        {
            return this.usersDictionary.GetAllTreeElement().Select(pair => pair.Value).ToArray();
        }
    }
}
