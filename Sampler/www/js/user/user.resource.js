/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .factory('User', ['$resource', function($resource) {
        return $resource('/api/user', null,
            {
                chatHistory: {
                    method: 'GET',
                    url: '/api/user/messages',
                    isArray: true
                },
                trophies: {
                    method: 'GET',
                    url: '/api/Trophies/info',
                    isArray: true
                }
            });
    }]);