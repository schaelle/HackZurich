(function () {
    app.controller('FootprintController',
    function ($scope, $state, $stateParams) {

	$scope.footprintChart = {
			options : {
				chart : {
					type : 'pie'
				}
			},
			series : [ {
				data : [ 10, 15, 12, 8, 7 ]
			} ],
			title : {
				text : ''
			},
			loading : true
		}

    });


})();