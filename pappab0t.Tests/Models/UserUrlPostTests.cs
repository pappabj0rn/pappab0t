using System;
using pappab0t.Models;
using Xunit;

namespace pappab0t.Tests.Models
{
    public abstract class UserUrlPostTests
    {
        public class Constructor : UserUrlPostTests
        {
            [Fact]
            public void Should_set_created_to_datetime_now()
            {
                var post = new UserUrlPost();

                Assert.Equal(DateTime.Now.ToLongDateString(), post.Created.ToLongDateString());
                Assert.Equal(DateTime.Now.ToLongTimeString(), post.Created.ToLongTimeString());
            }
        }
    }
}
