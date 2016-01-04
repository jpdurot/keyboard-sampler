/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .controller('ProfileController',
    function($scope, User) {
        $scope.trophies = User.trophies();
    });