window.preventBack = function (defaultRoute) {
    // Push a dummy state so the browser thinks it cannot go back
    history.pushState(null, document.title, location.href);

    window.addEventListener('popstate', function (event) {
        // Always navigate back inside Blazor
        DotNet.invokeMethodAsync('WeightTrackerUI', 'HandleBackButton')
            .catch(err => console.error(err));

        // Re-push dummy state so user can’t leave the app
        history.pushState(null, document.title, location.href);
    });
};
