(function () {
    app.controller("AlternativerouteController", 
	    function ($scope, $firebaseObject) {
	    	 var ref = firebase.database().ref();
	    // download physicsmarie's profile data into a local object
	    // all server changes are applied in realtime
	    	$scope.userdata = $firebaseObject(ref.child('Users'));
	    	$scope.userdata.$loaded().then(function(userdata){
	    		return userdata;	   
	    	});
	    });


})();