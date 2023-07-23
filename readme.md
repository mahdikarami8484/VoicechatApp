# What is it ?

It is a simple VoiceChat appliction in C# and .NET.  
I used Windows Form for the user interface because this program is only for learning sockets and a simple example of VoiceChat and may have problems and errors in real practice.  
This project is in the form of two client and server programs and the logic of its operation is very simple and non-practical.

# How it works?

This program works with TCP protocol.  
First, the Server program runs a multithreaded TCP server that waits for requests on the incoming port.  
Then the clients connect to the server with the client program.  
Every time a sound is received from the microphone input of the client, the sound is converted into bytes and sent to the server.  
The server also sends it to other clients.  
Clients also have a special thread for receiving messages from the server, which receives those bytes and converts them into sound.

# Pictures 
![Client Program](https://github.com/mahdikarami8484/VoicechatApp/assets/67632452/9ac44dd3-cdda-49c2-9ea0-9f1340fb66ff)
Client Program
![Server Program](https://github.com/mahdikarami8484/VoicechatApp/assets/67632452/8822d57d-1dee-4b79-b738-0c7692f5a6da)
Server Program
