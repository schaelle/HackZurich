app.config(function($stateProvider, $urlRouterProvider) {

	$urlRouterProvider.otherwise("/");

	$stateProvider
	.state('ranking', {
		url : "/:userId/ranking",
		templateUrl : "app/views/ranking.html",
		controller : "RankingController",
	}).state('alternativeroute', {
		url : "/:userId/alternativeroute",
		templateUrl : "app/views/alternativeroute.html",
		controller : "AlternativerouteController",
	}).state('footprint', {
		url : "/:userId/footprint",
		templateUrl : "app/views/footprint.html",
		controller : "FootprintController",
	})
	.state('home', {
		url : "/:userId",
		templateUrl : "app/views/home.html",
		controller : "HomeController"
	})
	;
});

/*
 * app.config(function (localStorageProvider) {
 *
 * localStorageProvider .setStorageCookie(0, '/'); });
 */
