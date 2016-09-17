(function () {
    app.controller("RankingController", 
	    function ($scope, $firebaseArray) {
	    	 var ref = firebase.database().ref();
	    	$scope.userdata = $firebaseArray(ref.child('Users'));
	    	$scope.userdata.$loaded().then(function(userdata){
	    		return userdata;	   
	    	});
	    });


})();