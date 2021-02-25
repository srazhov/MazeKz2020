$(document).ready(function() {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    // Disable send button until connection is established.
    $("#sendButton").disabled = true;

    connection.on("ReceiveMessage", addMessage);

    connection.start().then(function() {
        $("#sendButton").disabled = false;
        connection.invoke("GetChatHistory", chat.senderLogin, chat.recipient.login).then(function(lastMessages) {
            for (var i = 0; i < lastMessages.length; i++) {
                addMessage(lastMessages[i].senderLogin, lastMessages[i].text, lastMessages[i].date);
            }
        });
    }).catch(function(err) {
        return console.error(err.toString());
    });

    document.getElementById("sendButton").addEventListener("click",
        function(event) {
            var messageText = $("#messageInput").val();
            connection.invoke("SendMessage", chat.recipient.login, messageText).catch(function(err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        });

    function addMessage(sender, message, date) {
        var isMine = sender == chat.senderLogin;
        var messageHtml;
        if (isMine) {
            messageHtml = `<div class="d-flex justify-content-end mb-4">
                            <div class="sent-message">
                                ${message}
                                <span class="sent-message-time text-muted">${date}</span>
                            </div>
                            <img src="${chat.senderAvatarUrl}" class="rounded-circle" height="50" width="50">
                        </div>`;
        } else {
            messageHtml = `<div class="d-flex justify-content-start mb-4">
                            <img src="${chat.recipient.avatarUrl}" class="rounded-circle" height="50" width="50">
                            <div class="message">
                                ${message}
                                <span class="message-time text-muted">${date}</span>
                            </div>
                        </div>`;
        }

        $("#messages").append(messageHtml);
    }
});