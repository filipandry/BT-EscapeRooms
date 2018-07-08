// Write your JavaScript code.

$(document).on("ready", function () {
    var game;
    var button;
    var difficultyName;

    var UserID;

    InitGame();

    function InitGame() {
        game = $(".game");
        button = $("<span class='game-button'>NEW GAME</span>");
        
        button.on('click', newGame);

        game.append(button)
    }

    function newGame(e) {
        button.remove();

        var txtUsername = $("<input type='text' id='txtUsername' name='txtUsername' class='game-input' placeholder='Username' />");

        var cboDifficulty = $("<select id='cboDifficulty' name='cboDifficulty' class='game-input' />").append(
            $("<option value=1>Easy</option>"),
            $("<option value=2>Normal</option>"),
            $("<option value=3>Hard</option>"),
        )

        var btnStart = $("<span class='game-button'>START GAME</span>");
        var divMessages = $("<div class='message'> </div>");
        btnStart.on('click', startGame);
        txtUsername.on('keydown', function (e) {
            console.log(e);
            if (13 === e.keyCode) {
                startGame(e);
            }
        });
        game.append(txtUsername);
        game.append(cboDifficulty);
        game.append(btnStart);
        game.append(divMessages);
    }

    function startGame(e) {
        var username = $('#txtUsername').val();
        var difficulty = $('#cboDifficulty').val();
        difficultyName = $("#cboDifficulty option:selected").text();
        $('.message').empty();
        if (!username) {
            $('.message').html($('<span>Username required</span>'));
            return;
        }
        game.empty();
        $.ajax({
            url: '/api/NewGame',
            data: {
                username: username,
                difficulty: difficulty
            },
            success: function (result) {
                UserID = result.user;
                DrawMap(result);
            }
        });
    }
    function getControllers(result) {
        var divController = $("<div class='controls' />");
        var btnAttack;
        switch (result.fullMap.currentAction) {
            case 1:
            case 2:
                btnAttack = $("<span class='game-button'>ATTACK</span>");
                btnAttack.on('click', function () {
                    $.ajax({
                        url: '/api/Attack',
                        method: "POST",
                        data: {
                            user: UserID,
                        },
                        success: function (result) {
                            game.empty();
                            UserID = result.user;
                            DrawMap(result);
                        }
                    });
                });
                break;
            case 3:
                return divController;
            case 4:
                return divController;
        }

        var btnNorth = $("<span class='game-button'>NORTH</span>");
        var btnWest = $("<span class='game-button'>WEST</span>");
        var btnEast = $("<span class='game-button'>EAST</span>");
        var btnSouth = $("<span class='game-button'>SOUTH</span>");

        btnNorth.on('click', function () {
            MoveTo("N");
        });
        btnWest.on('click', function () {
            MoveTo("W");
        });
        btnEast.on('click', function () {
            MoveTo("E");
        });
        btnSouth.on('click', function () {
            MoveTo("S");
        });

        if (btnAttack) {
            divController.append($("<div />").append(btnAttack));
        }
        else {
            divController.append(btnNorth);
            divController.append(btnWest);
            divController.append(btnEast);
            divController.append(btnSouth);

        }

        return divController;
    }
    function getInfos(result) {
        var divInfo = $("<div class='info' />");

        divInfo.append($("<span>Username: " + result.fullMap.currentPlayer.username+ "</span>"));
        divInfo.append($("<span>Lives: " + result.fullMap.currentPlayer.lives + "</span>"));
        divInfo.append($("<span>Score: " + result.fullMap.currentPlayer.score + "</span>"));

        var action = "Walk";
        switch (result.fullMap.currentAction) {
            case 0:
                break;
            case 1:
                action = "Attack monster";
                break;
            case 2:
                action = "Attack The boss";
                break;
            case 3:
                divInfo.append($("<span>GAME OVER</span>"));
                return divInfo;
            case 4:
                divInfo.append($("<span>YOU WON</span>"));
                return divInfo;
        }
        divInfo.append($("<span>Action: " + action + "</span>"));

        if (result.fullMap.currentMonster) {
            divInfo.append($("<span>Monster lives: " + result.fullMap.currentMonster.lives + "</span>"));
        }

        return divInfo;
    }
    function MoveTo(direction) {
        $.ajax({
            url: '/api/Move',
            method: "POST",
            data: {
                user: UserID,
                direction: direction,
            },
            success: function (result) {
                game.empty();
                UserID = result.user;
                DrawMap(result);
            }
        });

    }
    function DrawMap(result) {
        var map = result.map;
        var divMap = $("<div class='map " + difficultyName.toLowerCase() + "' />");
        var gameMap = map.split('\r\n');
        gameMap.forEach(function (row) {
            row.split('').forEach(function (col) {
                switch (col) {
                    case "P":
                        divMap.append($("<div class='player' />"));
                        break;
                    case "B":
                        divMap.append($("<div class='boss' />"));
                        break;
                    case "M":
                        divMap.append($("<div class='monster' />"));
                        break;
                    case "H":
                        divMap.append($("<div class='healing' />"));
                        break;
                    case "T":
                        divMap.append($("<div class='toxic' />"));
                        break;
                    case "V":
                        divMap.append($("<div class='visited' />"));
                        break;
                    case " ":
                        divMap.append($("<div class='empty' />"));
                        break;
                }
            });
        });
        game.append(getInfos(result));
        game.append(divMap);
        game.append(getControllers(result));
    }
});

