# LDAP-Auth-using-Novell-LDAP-library-.NET-Core
Perform LDAP Authentication using Novell LDAP library in .NET Core 3.0/3.1

The goal of this repository is to understand how to perform LDAP Authentication in .NET Core 3.0/3.1 with Novell LDAP Library. A service is written
that performs the key LDAP functions like retrieval of user list, groups list, users in groups, creating user from attributes etc. This
service is then injected into the Startup class through DI in the ConfigureServices section and in the AccountController. 

References: https://nicolas.guelpa.me/blog/2017/02/15/dotnet-core-ldap-authentication.html
            https://www.brechtbaekelandt.net/blog/post/authenticating-against-active-directory-with-aspnet-core-2-and-managing-users
