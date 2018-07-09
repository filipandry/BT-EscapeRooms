// Write your JavaScript code.

$(document).on("ready", function () {
    var lastUsername;
    var game;
    var button;
    var difficultyName;

    var UserID;

    $('#btnUpdate').on('click', function () {
        var usernameOld = $('#txtUsernameOld').val();
        var usernameNew = $('#txtUsernameNew').val();
        $.ajax({
            url: '/api/Username',
            method: 'PUT',
            data: {
                username: usernameOld,
                newUsername: usernameNew,
            },
            success: function (result) {
                RefreshTable($("#tblScores"));
                $('#txtUsernameOld').val('');
                $('#txtUsernameNew').val('');
            }
        });

    });
    $('#btnDelete').on('click', function () {
        var username = $('#txtUsernameDelete').val();
        $.ajax({
            url: '/api/Scores',
            method: 'DELETE',
            data: {
                username: username,
            },
            success: function (result) {
                RefreshTable($("#tblScores"));
                $('#txtUsernameDelete').val('');
            }
        });

    });

    $('#btnSearch').on('click', function () {
        var username = $('#txtUsernameFilter').val();
        var table = $("#tblScores");
        var url = '/api/scores/';
        if (username) {
            url += "?username=" + username;
        }
        table.attr("data-url", url);
        RefreshTable(table);

    });


    $(document).on('keydown', function (e) {
        switch (e.which) {
            case 32: // space
                Attack();
                break;
            case 37: // left
                MoveTo("W");
                break;
            case 38: // up
                MoveTo("N");
                break;
            case 39: // right
                MoveTo("E");
                break;
            case 40: // down
                MoveTo("S");
                break;
            case 72: // h
                Heal();
                break;
            case 84: // t
                AttackWithToxic();
                break;
            default: return; // exit this handler for other keys
        }
        e.preventDefault();
    });

    InitGame();
    InitTables();

    function InitGame() {
        game = $(".game");
        if (!game) {
            return;
        }
        button = $("<span class='game-button'>NEW GAME</span>");
        
        button.on('click', newGame);

        game.append(button)
    }
    function newGame(e) {
        button.remove();

        var txtUsername = $("<input type='text' id='txtUsername' name='txtUsername' class='game-input' placeholder='Username' />");
        if (lastUsername) {
            txtUsername.val(lastUsername);
        }

        var cboDifficulty = $("<select id='cboDifficulty' name='cboDifficulty' class='game-input' />").append(
            $("<option value=1>Easy</option>"),
            $("<option value=2>Normal</option>"),
            $("<option value=3>Hard</option>"),
        )

        var btnStart = $("<span class='game-button'>START GAME</span>");
        var divMessages = $("<div class='message'> </div>");
        btnStart.on('click', startGame);
        txtUsername.on('keydown', function (e) {
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
                lastUsername = username;
            }
        });
    }
    function getKeyboardInfoControllers() {
        var divController = $("<div class='keyboard' />");
        divController.append(
            $("<span class='keyboard-info'>Keyboard controls</span>"),
            $("<span class='keyboard-north'> - NORTH</span>"),
            $("<span class='keyboard-west'> - WEST</span>"),
            $("<span class='keyboard-east'> - EAST</span>"),
            $("<span class='keyboard-south'> - SOUTH</span>"),
            $("<span class='keyboard-attack'> - ATTACK</span>"),
            $("<span class='keyboard-heal'> - USE HEAL POTION</span>"),
            $("<span class='keyboard-toxic'> - ATTACK WITH TOXIC POTION</span>"),
        )

        return divController;
    }
    function getControllers(result) {
        var divController = $("<div class='controls' />");
        divController.append(getKeyboardInfoControllers());
        var btnAttack;
        switch (result.fullMap.currentAction) {
            case 1:
            case 2:
                btnAttack = $("<span class='game-button attack'>ATTACK</span>");
                btnAttack.on('click', Attack);
                break;
            case 3:
                return divController;
            case 4:
                return divController;
        }

        var btnNorth = $("<span class='game-button north'>NORTH</span>");
        var btnWest = $("<span class='game-button west'>WEST</span>");
        var btnEast = $("<span class='game-button east'>EAST</span>");
        var btnSouth = $("<span class='game-button south'>SOUTH</span>");

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
            if (result.fullMap.currentAction == 1 && result.fullMap.currentPlayer.toxicPotions > 0) {
                var btnToxic = $("<span class='game-button attack'>ATTACK WITH TOXIC POTION</span>");
                btnToxic.on('click', AttackWithToxic);
                divController.append($("<div />").append(btnToxic));
            }
        }
        else {
            divController.append(btnNorth);
            divController.append(btnWest);
            divController.append(btnEast);
            divController.append(btnSouth);

        }
        if (result.fullMap.currentPlayer.healingPotions > 0) {
            var btnHeal= $("<span class='game-button heal'>USE HEALING POTION</span>");
            btnHeal.on('click', Heal);
            divController.append($("<div />").append(btnHeal));
        }

        return divController;
    }
    function Attack() {
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
    }
    function AttackWithToxic() {
        $.ajax({
            url: '/api/AttackWithPotion',
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
    }
    function Heal() {
        $.ajax({
            url: '/api/Heal',
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
    }
    function getInfos(result) {
        var divInfo = $("<div class='info' />");

        divInfo.append($("<span class='username'>" + result.fullMap.currentPlayer.username+ "</span>"));
        divInfo.append($("<span class='lives'>" + result.fullMap.currentPlayer.lives + "x </span>"));
        divInfo.append($("<span class='score'>" + result.fullMap.currentPlayer.score + "</span>"));

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
                divInfo.append($("<span class='game-over'>GAME OVER</span>"));
                divInfo.append(GetButtonPlayAgain());
                return divInfo;
            case 4:
                divInfo.append($("<span class='game-win'>YOU WON</span>"));
                divInfo.append(GetButtonPlayAgain());
                return divInfo;
        }
        divInfo.append($("<span class='action'>" + action + "</span>"));

        if (result.fullMap.currentMonster) {
            divInfo.append($("<span class='monster-lives'>" + result.fullMap.currentMonster.lives + "x </span>"));
        }

        divInfo.append($("<span class='healing-potions'>" + result.fullMap.currentPlayer.healingPotions + "x </span>"));
        divInfo.append($("<span class='toxic-potions'>" + result.fullMap.currentPlayer.toxicPotions + "x </span>"));


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
        var playerExtraClass = "";
        switch (result.fullMap.currentAction) {
            case 1:
                playerExtraClass = "monster";
                break;
            case 2:
                playerExtraClass = "boss";
                break;
            case 3:
                playerExtraClass = "gameover";
                break;
            case 4:
                playerExtraClass = "win";
                break;
            case 5:
                playerExtraClass = "visited";
                break;
            case 6:
                playerExtraClass = "healing";
                break;
            case 7:
                playerExtraClass = "toxic";
                break;
        }
        gameMap.forEach(function (row) {
            row.split('').forEach(function (col) {
                switch (col) {
                    case "P":
                        divMap.append($("<div class='player " + playerExtraClass+"' />"));
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


    function GetButtonPlayAgain() {
        var button = $('<span class="game-button button-playagain">PLAY AGAIN</span>');
        button.on('click', function () {
            game.empty();
            InitGame();
        });
        return button;
    }
});

function InitTables() {
    $('table.sortable').each(function (index, table) {
        InitSortableTable($(table));
    });
    $('table.api-source').each(function (index, table) {
        RefreshTable($(table));
    });
}
function RefreshTable(table) {
    var sourceUrl = table.attr('data-url');
    var method = table.data('method');
    if (sourceUrl && method) {
        $.ajax({
            url: sourceUrl,
            method: method,
            success: function (result) {
                FillTable(table, result);
            }
        });
    }
}
function FillTable(table, dataSource) {
    var tbody = $(table.children("tbody"));
    if (tbody.length == 0) {
        tbody = $("<tbody />");
        table.append(tbody);
    }
    tbody.empty();
    $(dataSource).each(function (index, row) {
        var tableRow = $("<tr data-source='" + JSON.stringify(row) + "'/>");
        table.find('th').each(function (index, col) {
            var cellData = row[$(col).data('source')];
            var dataType = $(col).data('type');
            switch (dataType) {
                case "date":
                    cellData = (new Date(cellData)).toLocaleDateString();
                    break;
            }
            tableRow.append('<td data-type='+dataType+'>' + cellData + '</td>');
        });
        tbody.append(tableRow);
    });
}
function InitSortableTable(table) {
    table.find('th').each(function (index, row) {
        if ($(row).hasClass('sortable')) {
            $(row).on('click', function (e) {
                sortTable(table, index);
            });
        }
    });
}

function sortTable(table,n) {
    var rows, switching, i, x, y, shouldSwitch, dir, switchcount = 0;
    //var table = document.getElementById("myTable2");
    switching = true;
    // Set the sorting direction to ascending:
    dir = "asc";
    /* Make a loop that will continue until
    no switching has been done: */
    while (switching) {
        // Start by saying: no switching is done:
        switching = false;
        rows = table.find("tr");
        /* Loop through all table rows (except the
        first, which contains table headers): */
        for (i = 1; i < (rows.length - 1); i++) {
            // Start by saying there should be no switching:
            shouldSwitch = false;
            /* Get the two elements you want to compare,
            one from current row and one from the next: */
            //x = rows[i].getElementsByTagName("TD")[n];
            //y = rows[i + 1].getElementsByTagName("TD")[n];

            x = $(rows[i]).children('td')[n];
            y = $(rows[i + 1]).children('td')[n];
            /* Check if the two rows should switch place,
            based on the direction, asc or desc: */
            var dataType = $(x).data('type');

            xValue = x.innerHTML.toLowerCase();
            yValue = y.innerHTML.toLowerCase();

            if (dataType == "int") {
                xValue = parseInt(xValue);
                yValue = parseInt(yValue);
            }

            if (dir == "asc") {
                if (xValue > yValue) {
                    // If so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            } else if (dir == "desc") {
                if (xValue < yValue) {
                    // If so, mark as a switch and break the loop:
                    shouldSwitch = true;
                    break;
                }
            }
        }
        if (shouldSwitch) {
            /* If a switch has been marked, make the switch
            and mark that a switch has been done: */
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
            // Each time a switch is done, increase this count by 1:
            switchcount++;
        } else {
            /* If no switching has been done AND the direction is "asc",
            set the direction to "desc" and run the while loop again. */
            if (switchcount == 0 && dir == "asc") {
                dir = "desc";
                switching = true;
            }
        }
    }
}