using Google.Cloud.Firestore;
using Presentation.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Presentation.Repositories
{
    public class PostsRepository
    {
        FirestoreDb db;
        public PostsRepository(string project)
        {
            db = FirestoreDb.Create(project);
        }
        public async void Add(Post post)
        {
            post.Id = Guid.NewGuid().ToString();

            DocumentReference docRef = db.Collection($"blogs/{post.BlogId}/Posts").Document(post.Id);

            await docRef.SetAsync(post);
        }

        public async Task<List<Post>> GetPosts(string blogId)
        {
            ///blogs/6fe1e165-a934-4a8b-a120-d4c1b74c8820/Posts/88352b2a-a1a8-49b4-9752-485f05c4d8b4
            ///blogs/cookingblogId/Posts/article-how-to-cook-pizza-id
            Query allPostsQuery = db.Collection($"blogs/{blogId}/Posts");
            QuerySnapshot allPostsQuerySnapshot = await allPostsQuery.GetSnapshotAsync();

            List<Post> posts = new List<Post>();

            foreach (DocumentSnapshot documentSnapshot in allPostsQuerySnapshot.Documents)
            {
                Post b = documentSnapshot.ConvertTo<Post>();
                b.Id = documentSnapshot.Id;
                b.BlogId = blogId;
                posts.Add(b);
            }
            return posts;
        }

        public async Task<Post> GetPostForBlog(string blogId, string postId)
        {
         DocumentReference docRef= db.Collection($"blogs/{blogId}/Posts").Document(postId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            Post p =snapshot.ConvertTo<Post>();

            return p;
        }
    }
}
