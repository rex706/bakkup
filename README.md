# bakkup
Automatically backup game saves to a Google Drive folder to be accessed from anywhere. Never be without your most recent save again!

Latest version: 1.4

![alt tag](http://i.imgur.com/NTk0dsE.png)

Make sure to be logged into your Google Drive account

Files will be saved to "\Google Drive\bakkup\". Directory will be created on first use if not found. 

GUI can be skipped and backup/run game automatically by creating a shortcut and using the parameter "-Game Name" (same name of bakkup directory folder)

TODO:
- Find a way to deal with multiple users playing and saving at the same time.
- Find a way to check if files need to be updated or not. Compare last write times?

Future TODO:
- Fully implement Google Drive, OneDrive and DropBox OAuth2 authentication and syncing functionality. Removes need of having any of the online storage providers installed on the client system.
- Implement cryptographic storage of all OAuth2 authentication strings (client id, client secret, access token and refresh token).
- Add an option to actively watch game folder locations to perform automatic syncing on file changes.
- Syncing functionality will be automatic and manual. Automatic syncing intervals can be specified by user.
- Check if push functionality among the online drive services can be done.
- Set folder location on remote drive to store save game data.