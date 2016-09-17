(function () {
    app.controller("RankingController", 
	    function ($scope, $state, $stateParams, $firebaseArray) {
	    	 var ref = firebase.database().ref();
	    	$scope.userdata = $firebaseArray(ref.child('Users'));
	    	$scope.userdata.$loaded().then(function(userdata){
	    		return userdata;	   
	    	});

			var ref = firebase.database().ref('Users' + '/' + $stateParams.userId);
        // download physicsmarie's profile data into a local object
        // all server changes are applied in realtime
        $scope.currentUser = $firebaseArray(ref);
        $scope.currentUser.$loaded().then(function (currentUser) {
            return currentUser;

        });

	    });


})();