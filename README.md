# ASP.Net Core MVC with Facebook Authentication
Sometimes we want the user to login using their existing credentials of third party applications such as Facebook, Twitter, Google etc. into our application. In this article we are going to look into authentication of ASP.NET Core app using Facebook account.

The initial project was copied from the next github account:
https://github.com/AnkitSharma-007/ASPCore.FacebookAuth

# Read the full article here
http://ankitsharmablogs.com/authentication-using-facebook-in-asp-net-core-2-0/


A few more changes were added:

## Access Token Page:
In order to get the access token from facebook, there is a prvioues step to get the authorization code.
Having the Authorization code we can use a new request to get the access token. 
More details can be found on the AccessTokenController.

In the same time added a few Facebook Services in order to get the user details and to add some text on post.


Here are few urls that helped:
https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-2.1&tabs=aspnetcore2x

https://steemit.com/utopian-io/@babelek/how-to-create-facebook-login-in-asp-net-core-web-api

https://retifrav.github.io/blog/2017/11/25/csharp-dotnet-core-publish-facebook/


 
