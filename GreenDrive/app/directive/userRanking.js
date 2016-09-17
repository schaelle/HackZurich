app.directive('userRanking', function() {
    return {

        templateUrl: "app/directive/userRanking.html",
        scope: {
            type: "=",
            dialogFunction: "=",
            filterFunction: "="
        },
        controller: function($scope, $firebaseObject, $firebaseArray, $state, $stateParams) {

         var ref = firebase.database().ref('Users' + '/' + $stateParams.userId);
        // download physicsmarie's profile data into a local object
        // all server changes are applied in realtime
        $scope.userdata = $firebaseArray(ref);
        $scope.userdata.$loaded().then(function () {
            $scope.currentUser = $scope.userdata;

        });
   
         
	    		   
        }
    }


});