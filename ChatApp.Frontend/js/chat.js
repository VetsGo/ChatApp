const API_URL = 'https://chatapp-backend-sergo-beabe7hrfxcch6fj.westeurope-01.azurewebsites.net';
const HUB_URL = `${API_URL}/chathub`;

const messagesContainer = document.getElementById('messagesContainer');
const usernameInput = document.getElementById('usernameInput');
const messageInput = document.getElementById('messageInput');
const sendButton = document.getElementById('sendButton');

let connection = null;

async function init() {
    try {
        await loadPreviousMessages();
        
        await connectToSignalR();
        
        setupEventListeners();
    } catch (error) {
        console.error('Помилка ініціалізації:', error);
        showError('Не вдалося підключитися до сервера');
    }
}

// Loading the latest messages from the server via REST API and displaying them
async function loadPreviousMessages() {
    try {
        const response = await fetch(`${API_URL}/api/chat/messages?limit=50`);
        if (!response.ok) throw new Error('Помилка завантаження повідомлень');
        
        const messages = await response.json();
        
        messagesContainer.innerHTML = '';
        
        messages.forEach(msg => displayMessage(msg));
    } catch (error) {
        console.error('Помилка завантаження повідомлень:', error);
        messagesContainer.innerHTML = '<div class="loading-messages">Помилка завантаження історії</div>';
    }
}

// Creating and configuring the SignalR connection
async function connectToSignalR() {
    connection = new signalR.HubConnectionBuilder()
        .withUrl(HUB_URL)
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on('ReceiveMessage', (message) => {
        displayMessage(message);
        scrollToBottom();
    });

    connection.onreconnecting(() => {
        sendButton.disabled = true;
    });

    connection.onreconnected(() => {
        sendButton.disabled = false;
    });

    connection.onclose(() => {
        sendButton.disabled = true;
    });

    try {
        await connection.start();
        sendButton.disabled = false;
        console.log('SignalR підключено');
    } catch (error) {
        console.error('Помилка підключення SignalR:', error);
        sendButton.disabled = true;
        throw error;
    }
}

// Adding event handlers for the send button and saving the entered username
function setupEventListeners() {
    sendButton.addEventListener('click', sendMessage);
    
    messageInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            sendMessage();
        }
    });
    
    usernameInput.addEventListener('change', () => {
        localStorage.setItem('chatUsername', usernameInput.value);
    });
    
    const savedUsername = localStorage.getItem('chatUsername');
    if (savedUsername) {
        usernameInput.value = savedUsername;
    }
}

// Sending a message via SignalR
async function sendMessage() {
    const username = usernameInput.value.trim();
    const message = messageInput.value.trim();
    
    if (!username) {
        alert('Будь ласка, введіть ваш name!');
        usernameInput.focus();
        return;
    }
    
    if (!message) {
        messageInput.focus();
        return;
    }
    
    try {
        sendButton.disabled = true;
        await connection.invoke('SendMessage', username, message);
        messageInput.value = '';
        messageInput.focus();
    } catch (error) {
        console.error('Помилка відправки повідомлення:', error);
        alert('Не вдалося відправити повідомлення');
    } finally {
        sendButton.disabled = false;
    }
}

// Returning an emoji based on sentiment
function getSentimentEmoji(sentiment) {
    if (!sentiment) return '😐';

    switch (sentiment.toLowerCase()) {
        case 'positive':
            return '😊';
        case 'negative':
            return '😞';
        default:
            return '😐';
    }
}

// CSS classes for styling messages depending on sentiment
function getSentimentClass(sentiment) {
    if (!sentiment) return 'sentiment-neutral';

    switch (sentiment.toLowerCase()) {
        case 'positive':
            return 'sentiment-positive';
        case 'negative':
            return 'sentiment-negative';
        default:
            return 'sentiment-neutral';
    }
}

// Function for rendering the message layout
function displayMessage(msg) {
    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${getSentimentClass(msg.sentiment)}`;
    
    const timestamp = new Date(msg.timestamp + 'Z').toLocaleTimeString('uk-UA', {
        hour: '2-digit',
        minute: '2-digit'
    });
    
    const sentimentEmoji = getSentimentEmoji(msg.sentiment);
    const sentimentText = msg.sentiment || 'Neutral';
    
    let sentimentScoresHTML = '';
    if (msg.positiveScore !== undefined) {
        const posPercent = (msg.positiveScore * 100).toFixed(0);
        const negPercent = (msg.negativeScore * 100).toFixed(0);
        const neuPercent = (msg.neutralScore * 100).toFixed(0);
        
        sentimentScoresHTML = `
            <div class="sentiment-scores">
                <span class="score positive">${posPercent}%</span>
                <span class="score neutral">${neuPercent}%</span>
                <span class="score negative">${negPercent}%</span>
                <span class="sentiment-indicator" title="Sentiment: ${sentimentText}">
                    ${sentimentEmoji}
                </span>
            </div>
        `;
    }
    
    messageDiv.innerHTML = `
        <div class="message-header">
            <span class="message-username">${escapeHtml(msg.username)}</span>
            <span class="message-timestamp">${timestamp}</span>
        </div>
        <div class="message-content">${escapeHtml(msg.message)}</div>
        ${sentimentScoresHTML}
    `;
    
    messagesContainer.appendChild(messageDiv);
}

function scrollToBottom() {
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function showError(message) {
    messagesContainer.innerHTML = `<div class="loading-messages" style="color: #ff3425ff;">${message}</div>`;
}

window.addEventListener('load', init);