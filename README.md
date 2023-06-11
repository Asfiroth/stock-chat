# Welcome to Stock Chat!

**TL;DR**

Hi! this project is based on a .NET coding challenge which main goal is to create a simple browser-based chat application.
 
 ## Requested Features
- Allow registered users to log in and talk with other users in a chatroom.
- Allow users to post messages as commands into the chatroom with the following format _/stock=stock_code_
- Create a decoupled bot that will call an API using the stock_code as a parameter
(https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv, here aapl.us is the stock_code)
- The bot should parse the received CSV file and then it should send a message back into the chatroom using a message broker like RabbitMQ. The message will be a stock quote using the following format: _“APPL.US quote is $93.42 per share”_. The post owner will be the bot.
- Have the chat messages ordered by their timestamps and show only the last 50 messages.
- Unit tests 
- Have more than one chatroom.
- Use .NET identity for users authentication
- Handle messages that are not understood or any exceptions raised within the bot.
- Build an installer.

Sheesh! that's a lot of stuff to cover... 

Let's begin!

Since we need this to run everywhere, we will use docker, handling "installation" with docker compose.
Containers will use .NET 7 and will run with linux, for identity storage SQL Server image is needed, for chat mongo db will be used, and for queueing RabbitMQ.

So... we end up with something like this:

![alt text](https://github.com/asfiroth/stock-chat/blob/main/img/implementation.png?raw=true)

*This app will use https for communication for this reason a self signed certificate is needed.

## Making this rum on local

1.  Generate a self-signed certificate.

    ***NOTE: password used for this demo 'P@ssw0rd'***
    
    `openssl req -x509 -newkey rsa:4096 -keyout localhost.key -out localhost.crt -subj "/CN=localhost" -addext "subjectAltName=DNS:localhost,DNS:web,DNS:identity,DNS:chat"`
    
    `openssl pkcs12 -export -in localhost.crt -inkey localhost.key -out localhost.pfx -name "Stock Chat"`
    
2.  Import the self-signed certificate.

    **For Windows**
	   `certutil -f -user -importpfx Root localhost.pfx`

	**For Mac**
	Import it to the System Keychain and in Info make Root a trusted source.
    
3.  Add the line below to the hosts file.
    
    ```
    127.0.0.1 web
    127.0.0.1 identity
    127.0.0.1 chat
    ```
    
4.  Start the services.

    `docker compose up -d`

## How to test

There are three users registered on identity database:

1. Alice Smith

```
username: AliceSmith@email.com
password: Pass123$
```

2. Bob Smith

```
username: BobSmith@email.com
password: Pass123$
```

3. John Wick

```
username: JohnWick@email.com
password: Pass123$
```
