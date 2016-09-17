app.directive('userList', function() {
    return {

        templateUrl: "app/directive/userList.html",
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

            $scope.switchUser = function (item) {

                $scope.currentUser = item;

            };


        }
    }


});