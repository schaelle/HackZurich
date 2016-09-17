(function () {
    app.controller("AlternativerouteController",
	    function ($scope, $firebaseObject) {

      function parseRoute(input){
        return input.split('|').map(i=>i.split(',').map(a=>parseFloat(a)));
      }

			var ref = firebase.database().ref();
			var logbox = $firebaseObject(ref.child('LogBox'));
			var trips = [];
	    	logbox.$loaded().then(function(data){
	    		data.forEach(function(car) {
	    			if(car)
	    			for(var key in car.Data){
	    				var record = car.Data[key];
              record.Route = (parseRoute(record.Route));
              record.Center = record.Route[0];
	    				trips.push(record);
	    			}
	    		});
			});

      $scope.trackings = [];

	    	$scope.trips = trips;

	    });
})();
