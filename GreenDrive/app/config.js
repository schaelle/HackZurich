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
	});
});


app.config(function() {
	var config = {
	  apiKey: "AIzaSyA8Eoavf4aFGNYdNO7XqwnwF9hNvzmeCRw",
	  authDomain: "hackzurich-1ced0.firebaseapp.com",
	  databaseURL: "https://hackzurich-1ced0.firebaseio.com",
	  storageBucket: "hackzurich-1ced0.appspot.com",
	  messagingSenderId: "29004659486"
	};
	firebase.initializeApp(config);
});



/*
 * app.config(function (localStorageProvider) {
 *
 * localStorageProvider .setStorageCookie(0, '/'); });
 */
