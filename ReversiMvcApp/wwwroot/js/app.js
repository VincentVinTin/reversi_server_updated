const Game = (function () {
    let configMap = {
        apiUrl: "https://localhost:5001/api",
        bordId: "bord",
        chartId: "chart",
        environment: "production",
    };

    let stateMap = {};

    const _getCurrentGameState = () => {
        Game.Model.updateGameState(configMap.apiUrl, stateMap.currentGameToken);
    };

    const _getPlayer = () => {
        return stateMap.player;
    };

    const privateInit = (gameToken, playerToken) => {
        stateMap.currentGameToken = gameToken;
        stateMap.player = playerToken;

        Game.Data.init(configMap.environment);
        Game.Reversi.init(configMap.bordId, configMap.apiUrl, gameToken);
        Game.Stats.init(configMap.chartId);
        _getCurrentGameState();
        setInterval(() => {
            _getCurrentGameState();
        }, 1000);
    };

    return {
        init: privateInit,
        update: _getCurrentGameState,
        player: _getPlayer,
    };
})();

class FeedbackWidget {
    constructor(elementId) {
        this._elementId = elementId;
        this._element = $("#" + elementId);

        this._element.empty();
        this._element.append(`
            <div class="feedbackWidget">
                <div class="feedbackWidget__check">
                    <i class="glyphicon glyphicon-ok"></i>
                </div>

                <section class="feedbackWidget__message">
                </section>

                <div class="feedbackWidget__close">
                    <i class="glyphicon glyphicon-remove"></i>
                </div>

                <div class="feedbackWidget__buttons">
                    <button class="feedbackWidget__button button__success">Akkoord</button>
                    <button class="feedbackWidget__button">Weigeren</button>
                </div>
            <div>
        `);
        this.hide();
    }

    get elementId() {
        return this._elementId;
    }

    show(message, type) {
        this._element.addClass(type);

        this._element.find(".feedbackWidget__message").html(message);

        this.log({ message: message, type: type });

        this._element.show();
    }

    hide() {
        this._element.hide();
    }

    log(message) {
        let storageWidget = JSON.parse(localStorage.getItem("feedback_widget"));

        if (storageWidget == null) {
            storageWidget = [message];
        } else {
            if (storageWidget.push(message) > 10) {
                storageWidget.shift();
            }
        }

        localStorage.setItem("feedback_widget", JSON.stringify(storageWidget));
    }

    removeLog() {
        localStorage.removeItem("feedback_widget");
    }

    history() {
        let storageWidget = JSON.parse(localStorage.getItem("feedback_widget"));

        let string = "";

        for (let i = 0; i < storageWidget.length; i++) {
            if (i !== 0 && i !== storageWidget - 1) {
                string += "\n";
            }

            if (storageWidget[i]["type"] !== "success") {
                string += "error - ";
            } else {
                string += "success - ";
            }

            string += storageWidget[i]["message"];
        }

        return string;
    }
}

Game.Data = (function () {
    let configMap = {
        mock: {
            id: 1,
            token: "test",
            speler1Token: "Test",
            speler2Token: "Test2",
            omschrijving: "Test game",
            token: "test",
            bord: "[[0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0],[0,0,0,1,2,0,0,0],[0,0,0,2,1,0,0,0],[0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0]]",
            aanDeBeurt: 1,
            winnaar: 0,
        },
    };

    let stateMap = {
        environment: "development",
    };

    const get = async (url) => {
        if (stateMap.environment == "production") {
            return $.ajax({
                url: url,
                type: "GET",
                crossDomain: true,
            });
        } else {
            return getMockData();
        }
    };

    const put = async (url, data) => {
        return $.ajax({
            url: url,
            type: "PUT",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            crossDomain: true,
        });
    };

    const getMockData = async () => {
        const mockData = configMap.mock;

        return new Promise((resolve, reject) => {
            resolve(mockData);
        });
    };

    const privateInit = function (environment) {
        if (environment !== "production" && environment !== "development")
            throw new Error("Environment bestaat niet");
        stateMap.environment = environment;
    };

    return {
        init: privateInit,
        get: get,
        put: put,
    };
})();

Game.Model = (function () {
    const _updateGameState = async function (apiUrl, token) {
        await Game.Data.get(`${apiUrl}/spel/${token}`).then((r) => {
            Game.Reversi.update(r);
        });
    };

    return {
        updateGameState: _updateGameState,
    };
})();

Game.Reversi = (function () {
    let configMap = {
        size: 8,
    };

    let stateMap = {
        token: undefined,
        apiUrl: undefined,
        spel: {
            bord: [],
        },
    };

    const privateInit = (bordId, apiUrl, token) => {
        stateMap.bord = $(`#${bordId}`);
        stateMap.apiUrl = apiUrl;
        stateMap.token = token;
        _updateBoard();
    };

    const _updateBoard = () => {
        stateMap.bord.html(
            Game.Template.parseTemplate("game.spel", {
                spel: stateMap.spel,
                size: configMap.size,
            })
        );

        stateMap.bord.find(".cell").on("click", function () {
            _placeChip($(this).attr("data-row"), $(this).attr("data-col"));
        });

        Game.Stats.update(stateMap.spel.bord);

        stateMap.bord.trigger("bordUpdate");
    };

    const _placeChip = async (rij, kolom) => {
        let result = await Game.Data.put(
            `${stateMap.apiUrl}/spel/${stateMap.token}/zet/${Game.player()}`,
            {
                rij: parseInt(rij),
                kolom: parseInt(kolom),
            }
        )
            .then((res) => res)
            .catch((err) => {
                console.log(err);
                return false;
            });

        if (result) {
            _update(result);
        }
    };

    const _update = (spel) => {
        spel.aandeBeurt = spel.aandeBeurt == 1 ? "Wit" : "Zwart";
        spel.winnaar =
            spel.winnaar == 0 ? null : spel.winnaar == 1 ? "Wit" : "Zwart";
        stateMap.spel = spel;
        stateMap.spel.bord = JSON.parse(spel.bord);

        _updateBoard();
    };

    return {
        init: privateInit,
        update: _update,
    };
})();

Game.Stats = (function () {
    let configMap = {
        type: "bar",
        data: {
            labels: ["Geen", "Wit", "Zwart"],
            datasets: [
                {
                    label: "Aantal fiches",
                    data: [60, 2, 2],
                    backgroundColor: [
                        "rgba(43, 196, 156,0.1)",
                        "rgba(255,255,255,0.5)",
                        "rgba(0,0,0,0.8)",
                    ],
                    borderColor: [
                        "rgba(43, 196, 156, 1)",
                        "rgba(255,255,255, 1)",
                        "rgba(0,0,0, 1)",
                    ],
                    borderWidth: 1,
                },
            ],
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                },
            },
        },
    };

    let stateMap = {
        chart: undefined,
    };

    const privateInit = function (id) {
        let ctx = document.getElementById(id).getContext("2d");
        stateMap.chart = new Chart(ctx, configMap);
    };

    const _update = (board) => {
        let geen = 0;
        let wit = 0;
        let zwart = 0;

        board.forEach((row) => {
            row.forEach((col) => {
                switch (col) {
                    case 0:
                        geen++;
                        break;
                    case 1:
                        wit++;
                        break;
                    case 2:
                        zwart++;
                        break;
                }
            });
        });

        if (stateMap.chart === undefined) return;

        stateMap.chart.data.datasets[0].data = [geen, wit, zwart];
        stateMap.chart.update();
    };

    return {
        init: privateInit,
        update: _update,
    };
})();

Game.Template = (() => {
    const _getTemplate = (templateName) => {
        var template = spa_templates.templates;

        templateName.split(".").forEach((path) => {
            template = template[path];
        });

        return template;
    };

    const _parseTemplate = (templateName, data) => {
        const template = _getTemplate(templateName);
        return template(data);
    };

    return {
        getTemplate: _getTemplate,
        parseTemplate: _parseTemplate,
    };
})();