/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .factory('Sounds', ['$resource', function($resource) {
        return $resource('/api/sounds/play/:soundId', null,
            {
                'play': { method:'GET' },
                'getList': {
                    method: 'GET',
                    isArray: true,
                    url: '/api/sounds/info'
                },
                muteUnmute: {
                    method: 'POST',
                    url: '/api/sounds/mute'
                },
                isMuted: {
                    method: 'GET',
                    url: '/api/sounds/ismuted'
                }
            });
    }]);