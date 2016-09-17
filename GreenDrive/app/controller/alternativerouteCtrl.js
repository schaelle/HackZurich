(function () {
    app.controller("AlternativerouteController",
	    function ($scope, $firebaseObject, uiGmapGoogleMapApi) {
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

      $scope.trackings = [];

	    	$scope.trips = trips;
	    	 // Define variables for our Map object
	    	  var areaLat      = 44.2126995,
	    	      areaLng      = -100.2471641,
	    	      areaZoom     = 3;

	    	  uiGmapGoogleMapApi.then(function(maps) {
	    		    $scope.map     = { center: { latitude: areaLat, longitude: areaLng }, zoom: areaZoom };
	    		    $scope.options = { scrollwheel: false };
	    		  });

	    });
})();
