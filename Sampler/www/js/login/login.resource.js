/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .factory('Login', ['$resource', function($resource) {
        return $resource('/api/login', null,
            {
                login: {
                    method: 'POST'
                },
                isLoggedIn: {
                    method: 'GET',
                    url: '/api/login/test'
                }
            });
    }]);