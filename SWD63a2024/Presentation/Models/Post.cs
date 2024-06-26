﻿using Google.Cloud.Firestore;

namespace Presentation.Models
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
        public Timestamp DateCreated { get; set; }
        [FirestoreProperty]
        public Timestamp DateUpdated { get; set; }

        public string BlogId { get; set; }

        [FirestoreProperty]
        public string PictureUri { get; set; }

    }
}
