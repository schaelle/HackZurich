(function () {
    app.controller("AlternativerouteController", 
	    function ($scope, $firebaseObject) {
			var ref = firebase.database().ref();
			var logbox = $firebaseObject(ref.child('LogBox'));
			var trips = [];
	    	logbox.$loaded().then(function(data){
	    		data.forEach(function(car) {
	    			if(car )
	    			for(var key in car.Data){
	    				var record = car.Data[key];
	    				trips.push(record);
	    			}
	    		});
			});
	    	$scope.trips = trips;
	    });
})();