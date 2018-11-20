using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FacebookAuth.Services
{
    /// <summary>
    /// Example from here:
    /// https://retifrav.github.io/blog/2017/11/25/csharp-dotnet-core-publish-facebook/
    /// </summary>
    public class Facebook
    {
        readonly string _accessToken;
        readonly string _pageId;
        readonly string _facebookAPI = "https://graph.facebook.com/";
        readonly string _pageFeed = "feed";
        readonly string _pagePhotos = "photos";
        readonly string _postToPageUrl;
        readonly string _postToPagePhotosUrl;

        public Facebook(string accessToken, string pageId)
        {
            _accessToken = accessToken;
            _pageId = pageId;
            _postToPageUrl = $"{_facebookAPI}{pageId}/{_pageFeed}";
            _postToPagePhotosUrl = $"{_facebookAPI}{pageId}/{_pagePhotos}";
        }

        /// <summary>
        /// Publish a simple text post
        /// </summary>
        /// <returns>StatusCode and JSON response</returns>
        /// <param name="postText">Text for posting</param>
        public async Task<Tuple<int, string>> PublishSimplePost(string postText)
        {
            using (var http = new HttpClient())
            {
                var postData = new Dictionary<string, string>
                {
                    {"access_token", _accessToken},
                    {"message", postText} //,
                    // { "formatting", "MARKDOWN" } // doesn't work
                };

                var httpResponse = await http.PostAsync(
                    _postToPageUrl,
                    new FormUrlEncodedContent(postData)
                    );
                var httpContent = await httpResponse.Content.ReadAsStringAsync();

                return new Tuple<int, string>(
                    (int) httpResponse.StatusCode,
                    httpContent
                    );
            }
        }

        /// <summary>
        /// Publish a post to Facebook page
        /// </summary>
        /// <returns>Result</returns>
        /// <param name="postText">Post to publish</param>
        /// <param name="pictureUrl">Post to publish</param>
        public string PublishToFacebook(string postText, string pictureUrl)
        {
            try
            {
                // upload picture first
                var rezImage = Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        return await UploadPhoto(pictureUrl);
                    }
                });
                var rezImageJson = JObject.Parse(rezImage.Result.Item2);

                if (rezImage.Result.Item1 != 200)
                {
                    try // return error from JSON
                    {
                        return $"Error uploading photo to Facebook. {rezImageJson["error"]["message"].Value<string>()}";
                    }
                    catch (Exception ex) // return unknown error
                    {
                        // log exception somewhere
                        return $"Unknown error uploading photo to Facebook. {ex.Message}";
                    }
                }
                // get post ID from the response
                string postID = rezImageJson["post_id"].Value<string>();

                // and update this post (which is actually a photo) with your text
                var rezText = Task.Run(async () =>
                {
                    using (var http = new HttpClient())
                    {
                        return await UpdatePhotoWithPost(postID, postText);
                    }
                });
                var rezTextJson = JObject.Parse(rezText.Result.Item2);

                if (rezText.Result.Item1 != 200)
                {
                    try // return error from JSON
                    {
                        return $"Error posting to Facebook. {rezTextJson["error"]["message"].Value<string>()}";
                    }
                    catch (Exception ex) // return unknown error
                    {
                        // log exception somewhere
                        return $"Unknown error posting to Facebook. {ex.Message}";
                    }
                }

                return "OK";
            }
            catch (Exception ex)
            {
                // log exception somewhere
                return $"Unknown error publishing post to Facebook. {ex.Message}";
            }
        }

        /// <summary>
        /// Upload a picture (photo)
        /// </summary>
        /// <returns>StatusCode and JSON response</returns>
        /// <param name="photoUrl">URL of the picture to upload</param>
        public async Task<Tuple<int, string>> UploadPhoto(string photoUrl)
        {
            using (var http = new HttpClient())
            {
                var postData = new Dictionary<string, string>
                {
                    {"access_token", _accessToken},
                    {"url", photoUrl}
                };

                var httpResponse = await http.PostAsync(
                    _postToPagePhotosUrl,
                    new FormUrlEncodedContent(postData)
                    );
                var httpContent = await httpResponse.Content.ReadAsStringAsync();

                return new Tuple<int, string>(
                    (int) httpResponse.StatusCode,
                    httpContent
                    );
            }
        }

        /// <summary>
        /// Update the uploaded picture (photo) with the given text
        /// </summary>
        /// <returns>StatusCode and JSON response</returns>
        /// <param name="postId">Post ID</param>
        /// <param name="postText">Text to add tp the post</param>
        public async Task<Tuple<int, string>> UpdatePhotoWithPost(string postId, string postText)
        {
            using (var http = new HttpClient())
            {
                var postData = new Dictionary<string, string>
                {
                    {"access_token", _accessToken},
                    {"message", postText} //,
                    // { "formatting", "MARKDOWN" } // doesn't work
                };

                var httpResponse = await http.PostAsync(
                    $"{_facebookAPI}{postId}",
                    new FormUrlEncodedContent(postData)
                    );
                var httpContent = await httpResponse.Content.ReadAsStringAsync();

                return new Tuple<int, string>(
                    (int) httpResponse.StatusCode,
                    httpContent
                    );
            }
        }
    }
}
