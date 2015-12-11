/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .controller('LoginController',
    function($scope, Login, alertService, $localStorage, $location) {
        $scope.doLogin = function () {
            Login.login({
                userName: $scope.login,
                password: $scope.password
            }, function(data) {
                // Authentication successful
                console.log(data);
				$localStorage.token = data.token;
				$location.url('/sounds');
                alertService.addAlert('Bienvenue '+$scope.login,'danger');
            }, function(error) {
                alertService.addAlert('Veuillez vérifier votre login/mot de passe','danger');
				console.log('erreur login');
            });
        }
    });