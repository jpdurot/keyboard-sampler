/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .factory('Login', ['$resource','constants', function($resource, constants) {
        return $resource(constants.baseUri+'/api/login', null,
            {
                login: {
                    method: 'POST'
                },
                isLoggedIn: {
                    method: 'GET',
                    url: constants.baseUri+'/api/login/test'
                },
                logout: {
                    method: 'GET',
                    url: constants.baseUri+'/api/login/logout'
                },
                changePassword: {
                    method: 'POST',
                    url: constants.baseUri+'/api/login/password'
                }
            });
    }]);