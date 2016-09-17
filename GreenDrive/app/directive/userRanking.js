app.directive('userRanking', function() {
    return {

        templateUrl: "app/directive/userRanking.html",
        scope: {
            type: "=",
            dialogFunction: "=",
            filterFunction: "="
        },
        controller: function($scope, $firebaseObject, $firebaseArray) {

            var ref = firebase.database().ref();
	    // download physicsmarie's profile data into a local object
	    // all server changes are applied in realtime
            
	    	$scope.userdata = $firebaseArray(ref.child('Users'));
	    	$scope.userdata.$loaded().then(function(userdata){
                $scope.currentUser = userdata[0];
	    		return userdata;	   
	    	});


        }
    }


});