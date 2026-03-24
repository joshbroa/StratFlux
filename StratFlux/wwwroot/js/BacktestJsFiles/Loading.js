// All elements to be altered upon 
const loadingElement = document.getElementById('loading-spinner');
const spinnerLabelElement = document.getElementById('spinner-label')
const completedElement = document.getElementById('completed-container');
const failedElement = document.getElementById('failed-container');
const errorMessageElement = document.getElementById('error-message');
const viewResultsButton = document.getElementById('view-results-button');

// Connection is made with the site's Backtests Hub
var connection = new signalR.HubConnectionBuilder().withUrl('/backtestsHub').build();
connection.start()

// If signal is given that backtesting is complete, then elements to be shown upon completion are shown and everything else is hidden
connection.on('BacktestComplete', generalResultsId => {
    completedElement.classList.remove('no-display');
    loadingElement.classList.add('no-display');
    spinnerLabelElement.classList.add('no-display');

    viewResultsButton.href = `/Backtests/ViewResult?Id=${generalResultsId}`;

    connection.stop();
});

// If signal is given that backtesting encountered an error, the error message will be shown and everything else will be hidden
connection.on('BacktestFailed', errorMessage => {
    failedElement.classList.remove('no-display');
    loadingElement.classList.add('no-display');
    spinnerLabelElement.classList.add('no-display');

    errorMessageElement.innerText = errorMessage;

    connection.stop();
});