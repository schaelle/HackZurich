(function() {
	app.controller("AlternativerouteController", function($scope, $firebaseArray, $firebaseObject, $stateParams) {

		function parseRoute(input){
			return input.split('|').map(i=>i.split(',').map(a=>parseFloat(a)));
		}

		$scope.userId = $stateParams.userId;
		var ref = firebase.database().ref();
		var user = $firebaseObject(ref.child('Users/' + $scope.userId));
		user.$loaded().then(function(data) {
			var logbox = $firebaseArray(ref.child('LogBox/' + data.Car + "/Data"));
			var trips = [];
			logbox.$loaded().then(function(data) {
				data.forEach(function(car) {
					if (car) {
						var record = car;
						record.Route = (parseRoute(record.Route));
						record.Center = record.Route[0];
						trips.push(record);
					}
				});
			});
			$scope.trackings = [];
			$scope.trips = trips;			
		});

	});
})();
