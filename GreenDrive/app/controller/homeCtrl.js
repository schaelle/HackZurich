(function() {
	app.controller('HomeController', function($scope, $firebaseObject, $firebaseArray,
			$stateParams) {

		$scope.userId = $stateParams.userId;
		var ref = firebase.database().ref();
		var user = $firebaseObject(ref.child('Users/' + $scope.userId));
		user.$loaded().then(function(data) {
			var localCar = data;
			$scope.car = data.Car;
			var logbox = $firebaseArray(ref.child('LogBox/' + data.Car + "/Data"));

			logbox.$loaded().then(function(data) {

				var distance = 0;
				var mileage = 0;
				var emission_kg = 0;

				// formula:
				// https://www.spritmonitor.de/de/berechnung_co2_ausstoss.html
				var co2_petrol_kg_l = 2.33;
				var co2_diesel_kg_l = 2.64;
				var co2_naturalgas_kg_l = 2.79;
				var co2_biogas_kg_l = 1.64;

				data.forEach(function(car) {

					var co2_current_kg_l = 0;

					switch (localCar.Fuel) {
					case 1:
						co2_current_kg_l = co2_petrol_kg_l;
						break;
					case 2:
						co2_current_kg_l = co2_diesel_kg_l;
						break
					case 3:
						co2_current_kg_l = co2_naturalgas_kg_l;
						break;
					case 4:
						co2_current_kg_l = co2_petrol_kg_l;
						break;
					default:
						co2_current_kg_l = co2_petrol_kg_l;
					}

					distance += car.Distance;
					mileage += car.GasConsumption;

					emission_kg = co2_current_kg_l * mileage;

				});

				console.log("Total Distance: " + distance + ' in km');
				console.log('Fuel consumption: ' + mileage + "  in l");
				console.log('CO2 amount: ' + emission_kg / 1000 + 'in t');

				// Test data
				$scope.emission = (emission_kg / 1000) * 100;
				$scope.target = 100;

			});
		});
	});

})();
