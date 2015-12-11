/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .factory('Sounds', ['$resource', function($resource) {
        return $resource('/api/Sounds/play/:soundId', null,
            {
                'play': { method:'GET' },
                'getList': {
                    method: 'GET',
                    isArray: true,
                    url: '/api/Sounds/info'
                },
                muteUnmute: {
                    method: 'POST',
                    url: '/api/Sounds/mute'
                },
                isMuted: {
                    method: 'GET',
                    url: '/api/Sounds/ismuted'
                }
            });
    }]);