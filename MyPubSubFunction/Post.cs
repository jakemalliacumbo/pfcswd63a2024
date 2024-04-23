using Google.Cloud.Firestore;

namespace MyPubSubFunction
{
    [FirestoreData]
    public class Post
    {
        public string Id { get; set; }
        [FirestoreProperty]
        public string Title { get; set; }
        [FirestoreProperty]
        public string Content { get; set; }
        [FirestoreProperty]
        public Google.Cloud.Firestore.Timestamp DateCreated { get; set; }
        [FirestoreProperty]
        public Google.Cloud.Firestore.Timestamp DateUpdated { get; set; }
        public string BlogId { get; set; }
        [FirestoreProperty]
        public string PictureUri { get; set; }

    }
}
