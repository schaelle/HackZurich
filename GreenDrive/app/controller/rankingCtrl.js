(function () {
    app.controller("RankingController", 
	    function ($scope, $firebaseObject) {
	    	 var ref = firebase.database().ref();
	    // download physicsmarie's profile data into a local object
	    // all server changes are applied in realtime
	    	//$scope.profile = $firebaseObject(ref.child('profiles').child('physicsmarie'));
	    	$scope.data = $firebaseObject(ref);
	    	$scope.data.$loaded().then(function(cardata){
	    		console.log(cardata.LogBox);
	    		 
	    	});
	    });


})();