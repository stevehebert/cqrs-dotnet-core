using cqrs.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace cqrs_test.EventSourcing
{
    public class EventSourcedTests
    {
        #region Test Classes

        public class ArticleMetadataUpdated : VersionedEvent
        {
            public IEnumerable<Tuple<string, string>> Metadata;
        }

        public class ArticlePublished :VersionedEvent
        {

            public DateTime Published { get; set; }
        }

        public class Article : EventSourced
        {
            public IEnumerable<Tuple<string, string>> Metadata
            {
                get;
                private set;
            }

            public DateTime? Published { get; private set; }

            protected Article(Guid id) : base(id)
            {
                this.Metadata = new Tuple<string,string>[0];
                base.Handles<ArticleMetadataUpdated>(this.OnMetadataUpdated);
                base.Handles<ArticlePublished>(this.OnArticlePublished);
            }

            public Article(Guid id, IEnumerable<IVersionedEvent> history) : this(id)
            {
                this.LoadFrom(history);
            }

            private void OnMetadataUpdated(ArticleMetadataUpdated e)
            {
                this.Metadata = e.Metadata;
            }

            private void OnArticlePublished(ArticlePublished e)
            {
                this.Published = e.Published;
            }

            #region Domain Events
            public void Publish()
            {
                this.Update(new ArticlePublished() { Published = DateTime.Now });
            }

            #endregion
        }

        #endregion

        [Fact]
        public void update_process_works()
        {
            var metaupdate = new ArticleMetadataUpdated
            {
                Metadata = new[] { new Tuple<string, string>("a", "1"), new Tuple<string, string>("b", "2") }
            };

            var item = new Article(Guid.NewGuid(), new [] { metaupdate } );

            Assert.Equal(item.Metadata.Count(), 2);
            Assert.Equal(item.Metadata.First().Item1, "a");
        }

        [Fact]
        public void update_via_domain_method_works()
        {
            var metaupdate = new ArticleMetadataUpdated
            {
                Metadata = new[] { new Tuple<string, string>("a", "1"), new Tuple<string, string>("b", "2") }
            };

            var item = new Article(Guid.NewGuid(), new[] { metaupdate });
            Assert.Null(item.Published);
            Assert.Equal(item.Version, 0);

            item.Publish();
            Assert.NotNull(item.Published);
            Assert.Equal(item.Version, 1);
        }
    }
}
