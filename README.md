# Real-time Chat Application with Sentiment Analysis

A chat application built with ASP.NET Core and SignalR, featuring AI-powered sentiment analysis using Azure 
Language Service. The application analyzes the emotional tone of each message and displays sentiment scores in real-time.

## Architecture

### Backend (ASP.NET Core)

- **Framework**: .NET 9.0
- **Real-time Communication**: SignalR with Azure SignalR Service
- **Database**: Entity Framework Core with SQL Server
- **AI Service**: Azure Cognitive Services (Language service)
- **API**: RESTful endpoints for message retrieval

### Frontend

- **HTML5/CSS3/JavaScript**: Vanilla JavaScript for lightweight performance
- **SignalR Client**: Microsoft SignalR JavaScript library
- **Real-time Updates**: Websocket connection for instant message delivery

## API Endpoints

### REST API

- `GET /api/chat/messages` - Retrieve chat history
- `GET /api/chat/messages/{id}` - Get specific message
- `GET /api/chat/health` - Health check endpoint

### SignalR Hub

- **Hub URL**: `/chathub`

## Key Backend Components

### ChatHub.cs
SignalR hub that handles real-time message broadcasting. Receives messages from clients. Prforms sentiment analysis and saves all messages to database.

### SentimentAnalysisService.cs
Integrates with Azure Language Service to analyze message sentiment. Returns scores for positive, negative and neutral sentiment.

### ChatController.cs
REST API controller providing endpoints to retrieve messages.