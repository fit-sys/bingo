var chat;
$.ajaxSetup({ cache: false });
$(function () {
    $("#bingo_board").ajaxStart($.blockUI).ajaxStop($.unblockUI);
    //chat = null;
    // User Slack ID
    if ($('#slackID').val() === "") {
        $('#ajaxNameArea').modal('show');
    } else {
        showContainer($('#slackID').val());

    }

    getUserListData();
    
    // Declare a proxy to reference the hub. 
    // 1. connection
    // 2. client
    // 3. server call
    chat = $.connection.chatHub;
    var bingo = $.connection.bingoHub;
    
    // Create a function that the hub can call to broadcast messages.
    chat.client.broadcastMessage = function (name, message, time) {
        // Html encode display name and message. 
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        // Add the message to the page. 
        $('#discussion').prepend('<li tabindex="1">' + time + ':<strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
    };
    chat.client.setSlackID = function (name) {
        // Add the message to the page. 
        $('#user_name').html('<text>' + name + '님 안녕하세요</text>');
        $('#slackID').val(name);
        
    };
    chat.client.setInitData = function (bingo) {
        drawBing(bingo);
        //$('#slackID').val(name);
    };
    
    // Get the user name and store it to prepend to messages.
    //$('#displayname').val(prompt('Enter your name:', ''));
    // Set initial focus to message input box.  

    
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        //$('#sendmessage').click(function () {
        //    sendMessageToAll();
        //    // Call the Send method on the hub. 
        //    //chat.server.send($('#slackID').val(), $('#message').val());
        //    // Clear text box and reset focus for next comment. 
        //    //$('#message').val('').focus();
        //});
        //sendmessage
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
        
        if ($('#slackID').val() !== "") {
            chat.server.getBingoInfo($('#slackID').val());
        }
        //getBingosuffle
        $(document).on('click', '#getBingoshuffle', function (e) {
            $("#bingo_board").block();
            getBingoshuffle($('#slackID').val());
        });


        //----admin----start
        $(document).on('click', '#reset', function (e) {
            bootbox.confirm("are you sure？", function (result) {
                if (result) {
                    $.ajax("/Admin/bingoGameReset/").then(
                        function (data, textStatus, jqXHR) {
                            bootbox.alert("OK");
                        }, function (jqXHR, textStatus, errorThrown) {　// error
                            bootbox.alert(errorThrown);
                        });
                }

            });
            // Call the reset
            //chat.server.resetBingoGame();
        });
        //chat reset
        $(document).on('click', '#reset_chat', function (e) {
            bootbox.confirm("are you sure？", function (result) {
                if (result) {
                    $.ajax("/Admin/chatReset/").then(
                        function (data, textStatus, jqXHR) {
                            bootbox.alert("OK");
                        }, function (jqXHR, textStatus, errorThrown) {　// error
                            bootbox.alert(errorThrown);
                        });
                }
            });
        });

        

        $(document).on('click', '#start', function (e) {
            chat.server.setBingoGameStart();
        });

        $(document).on('click', '#userList', function (e) {
            getUserListData();
        });

        $(document).on('click', '.user_btn', function (e) {
            var user = $(this).attr("data-key");
            setTurnUser(user);
        });

        //-----admin----- end
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
                text += "<div class='bingo_no selected' id='bingo_" + bingo[i].BingoID + "' >" + bingo[i].BingoNo + "</div>";
            } else {
                text += "<div class='bingo_no not_selected' id='bingo_" + bingo[i].BingoID + "' >" + bingo[i].BingoNo + "</div>";
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
    chat.server.send($('#slackID').val(), $('#message').val());
}

function getBingoshuffle(name) {
    
    chat.server.getBingoshuffle(name);
}

function getUserListData() {
    $.ajax("/Admin/getUserListData/").then(
    function (data, textStatus, jqXHR) {
        $("#user_list").fadeOut(100, function () {
            var text = "<div class='list-group'>";
            for (i = 0; i < data.conn.length; i++) {
                text += "<button type='button' class='list-group-item btn-info user_btn' data-key='" + data.conn[i].SlackID + "' >" + data.conn[i].SlackID + "</button>";
            }
            text += "</div>"
            $("#user_list .panel-body").html(text);
            $("#user_list").fadeIn();
        });
    }, function (jqXHR, textStatus, errorThrown) {　// error
        bootbox.alert(errorThrown);
    });
}

function setTurnUser(user) {
    $.ajax("/Admin/setTurnUser/" + user).then(
    function (data, textStatus, jqXHR) {
        
    }, function (jqXHR, textStatus, errorThrown) {　// error
        bootbox.alert(errorThrown);
    });
}