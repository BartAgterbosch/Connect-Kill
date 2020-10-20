# Connect-Kill
A C# based killswitch with GUI build for Windows x64 that let's you set any running application you want to a list, so whenever your connection drops and/or your public ip address changes, all selected applications will be immediately killed


When connected to an ip address that you do not wish to change from, like a vpn or proxy, simply click "Set" to set the ip, then add all the applications you wish to terminate to the list if the ip address at any point differs from the one you set, like a connection loss, and press "Start" to start the program.
If at any point during operation the ip address changes, or the connection is dropped entirely, so much as 1 second, all applications will be immediately terminated

Requires x64 .NET core 3.1
