var chat;
$(function () {
    $("#bingo_board").ajaxStart($.blockUI).ajaxStop($.unblockUI);
    //chat = null;
    // User Slack ID
    if ($('#slackID').val() === "") {
        $('#ajaxNameArea').modal('show');
    } else {
        showContainer($('#slackID').val());

    }
    
    // Declare a proxy to reference the hub. 
    // 1. connection
    // 2. client
    // 3. server call
    chat = $.connection.chatHub;
    var bingo = $.connection.bingoHub;
    
    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message,time) {
        // Html encode display name and message. 
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page. 
        $('#discussion').prepend('<li tabindex="1">' + time +  ':<strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };
    chat.client.setSlackID = function (name) {
        // Add the message to the page. 
        $('#user_name').html('<text>' + name + '님 안녕하세요</text>');
        $('#slackID').val(name);
        
    };
    chat.client.setSelectNumberToAll = function (number){
        var link = $('.bingo_no[data-key=' + number + ']');
        link.removeClass("not_selected");
        link.addClass("selected");
    };
    chat.client.setInitData = function (bingo) {
        drawBing(bingo);
        //$('#slackID').val(name);
    };
    chat.client.setBingoGameStart = function () {
        $("#getBingosuffle").fadeOut();
    };
    chat.client.setBingoMyTurn = function () {
        $(".not_selected").addClass("hand");
        //$(".not_selected").html("<div style='background-color:blue'>hora<div/>");
    };
    chat.client.setBingoNotMyTurn = function () {
        $(".not_selected").removeClass("hand");
    };
    // Set initial focus to message input box.  
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {

        // init
        if ($('#slackID').val() !== "") {
            chat.server.getBingoInfo($('#slackID').val());
        }

        $(document).on('click', '#sendmessage', function (e) {
            // Call the Send method on the hub. 
            e.preventDefault();
            
            sendMessageToAll();
            // Clear text box and reset focus for next comment. 
            $('#message').val('').focus();
        });

        $('form').submit(function (e) {
            // Call the Send method on the hub. 
            e.preventDefault();
            sendMessageToAll();
            // Clear text box and reset focus for next comment. 
            $('#message').val('').focus();
            return false;
        });

        
        //getBingosuffle
        $(document).on('click', '#getBingoshuffle', function (e) {
            $("#bingo_board").block();
            getBingoshuffle($('#slackID').val());
        });
        //hand
        $(document).on('click', '.hand', function (e) {
            var self = $(this);
            $(".not_selected").removeClass("hand");
            self.removeClass("not_selected").addClass("selected");
            setSelectNumberToAll(self);
        });
    });

    //$.connection.hub.connectionSlow(function () {
    //    bootbox.alert("Oh my god!!!!!! it's disconnected");
    //});

    //$.connection.hub.reconnecting(function () {
    //    bootbox.alert("Yahoo!!! it's reconnected");
    //});
    
    //btn-name-save
    $(document).on('click', '#btn-name-save', function (e) {
        if ($('#slackHdnID').val() === "") {
            bootbox.alert("please register your slack ID");
        } else {
            confirmUser($('#slackHdnID').val());
        }
    });
});

function drawBing(bingo) {
    $('#bingo_number').fadeOut(1000, function () {
        var text = "";
        for (i = 0; i < bingo.length; i++) {
            if (bingo[i].selected) {
                text += "<div class='bingo_no selected' id='bingo_" + bingo[i].BingoID + "' data-key='" + bingo[i].BingoNo + "' >" + bingo[i].BingoNo + "</div>";
            } else {
                text += "<div class='bingo_no not_selected' id='bingo_" + bingo[i].BingoID + "' data-key='" + bingo[i].BingoNo + "' >" + bingo[i].BingoNo + "</div>";
            }
            if (i % 5 === 4) {
                text += "<div class='new_row'></div>"
            }
        }
        // Add the message to the page. 
        $('#bingo_number').html(text).fadeIn(1000, function () {
            $("#bingo_board").unblock();
        });
    });
}

function confirmUser(slackID) {
    $.ajax("/Account/login/" + slackID).then(
    function (data, textStatus, jqXHR) {
        //bootbox.alert("OK");
        $('#ajaxNameArea').modal('hide');
        showContainer();
        
        // get user name
        chat.server.joinGroup($('#slackHdnID').val());
    }, function (jqXHR, textStatus, errorThrown) {　// error
        bootbox.alert(errorThrown);
    });
}

function showContainer(name){
    $('#container').show();
    
    
    //var chat = $.connection.chatHub;
}


function sendMessageToAll() {
    var message = $('#message').val();
    if(message.length !== 0){
        chat.server.send($('#slackID').val(), message);
    }
    
}

function getBingoshuffle(name) {
    
    chat.server.getBingoshuffle(name);
}

function setSelectNumberToAll(self) {
    var number = self.attr("data-key");
    chat.server.setSelectNumberToAll(number);
}