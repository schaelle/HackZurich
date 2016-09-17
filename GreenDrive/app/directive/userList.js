app.directive('userList', function () {
    return {

        templateUrl: "app/directive/userList.html",
        scope: {
            type: "=",
            dialogFunction: "=",
            filterFunction: "="
        },
        controller: function ($scope, $rootScope, $firebaseObject, $firebaseArray, $state, $stateParams) {

            $rootScope.$on('$stateChangeSuccess', function (e, state, params) {
                setUser(params.userId);
            })

            function setUser(userId) {
                if (userId) {
                    $scope.userdata.forEach(function (item) {
                        if (item.$id == userId)
                            $scope.currentUser = item;
                    })
                }
                else {
                    $scope.currentUser = $scope.userdata[0];
                    $state.go('home', { userId: $scope.currentUser.$id }, { reload: true });
                }
            }

            var ref = firebase.database().ref();
            // download physicsmarie's profile data into a local object
            // all server changes are applied in realtime
            $scope.userdata = $firebaseArray(ref.child('Users'));
            $scope.userdata.$loaded().then(function () {
                setUser($stateParams.userId);
            });

            $scope.switchUser = function (item) {

                $scope.currentUser = item.$id;
                $state.go('home', { userId: $scope.currentUser }, { reload: true });

            };


        }
    }


});
