/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .factory('User', ['$resource','constants', function($resource, constants) {
        return $resource(constants.baseUri+'/api/user', null,
            {
                chatHistory: {
                    method: 'GET',
                    url: constants.baseUri+'/api/user/messages',
                    isArray: true
                },
                trophies: {
                    method: 'GET',
                    url: constants.baseUri+'/api/Trophies/info',
                    isArray: true
                },
                getAll: {
                    method: 'GET',
                    isArray: true
                }
            });
    }]);