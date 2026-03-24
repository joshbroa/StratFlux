// This function is called to change the style of the selected elements and make sure they are selected if any part of the element is clicked
function selectOption(buttonId) {
    var button = document.getElementById(buttonId);

    if (buttonId[1] == 't') {
        // If at this point, then the item has been selected from the strategies list
        const options = Array.from(document.getElementsByClassName('strategy-choice-list-item'));

        options.forEach(item => {
            item.classList.remove('selected-list-item');
        });
    }
    else {
        // If at this point, then the item has been selected from the settings list
        const options = Array.from(document.getElementsByClassName('settings-choice-list-item'));

        options.forEach(item => {
            item.classList.remove('selected-list-item');
        });
    }

    // Checks hidden radio button and sets parent element to have selected css style
    button.checked = true;
    button.parentElement.classList.add('selected-list-item');
}