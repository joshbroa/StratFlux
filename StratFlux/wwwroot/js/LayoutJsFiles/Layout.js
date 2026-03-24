// This function allows for quick access to the vertical nav element
function getVerticalNav() {
    verticalNav = document.getElementsByClassName('vertical-nav')[0];
    return verticalNav;
}

// This returns a boolean indicating whether or not the vertical nav bar is currently extended
function navExtended(verticalNav) {
    if (verticalNav.classList.contains('nav-extended')) {
        return true;
    }
    else {
        return false;
    }
}

// This function will toggle the vertical nav bar to be either extended or retracted
function toggleNav(navButton) {
    verticalNav = getVerticalNav();
    baseUrl = window.location.origin;

    if (navExtended(verticalNav)) {
        navButton.src = baseUrl + '/icons/NavRetractedIcon.svg';
        verticalNav.classList.remove('nav-extended');
    }
    else {
        navButton.src = baseUrl + '/icons/NavExpandedIcon.svg';
        verticalNav.classList.add('nav-extended');
    }
}