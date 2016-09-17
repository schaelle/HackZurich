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
	});
});

/*
 * app.config(function (localStorageProvider) {
 * 
 * localStorageProvider .setStorageCookie(0, '/'); });
 */
