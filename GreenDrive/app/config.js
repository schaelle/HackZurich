app.config(function($stateProvider, $urlRouterProvider) {

	$urlRouterProvider.otherwise("/");

	$stateProvider.state('home', {
		url : "/",
		templateUrl : "app/views/home.html",
		controller : "HomeController"
	}).state('ranking', {
		url : "/ranking",
		templateUrl : "app/views/ranking.html",
		controller : "RankingController",
	}).state('alternativeroute', {
		url : "/alternativeroute",
		templateUrl : "app/views/alternativeroute.html",
		controller : "AlternativerouteController",
	}).state('footprint', {
		url : "/footprint",
		templateUrl : "app/views/footprint.html",
		controller : "FootprintController",
	});
});

/*
 * app.config(function (localStorageProvider) {
 * 
 * localStorageProvider .setStorageCookie(0, '/'); });
 */


app.config(function(uiGmapGoogleMapApiProvider) {
    uiGmapGoogleMapApiProvider.configure({
        key: 'AIzaSyA8Eoavf4aFGNYdNO7XqwnwF9hNvzmeCRw',
        v: '3.24', //defaults to latest 3.X anyhow
        libraries: 'weather,geometry,visualization'
    });
})