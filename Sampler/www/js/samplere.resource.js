/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .factory('Sounds', ['$resource', function($resource) {
    return $resource('http://samplairre.progx.org:9000/api/sounds/play/', null,
        {
            'play': { method:'GET' },
            'getList': {
                method: 'GET',
                isArray: true,
                url: 'http://samplairre.progx.org:9000/api/sounds/info'
            },
            muteUnmute: {
                method: 'POST',
                url: 'http://samplairre.progx.org:9000/api/sounds/mute'
            },
            isMuted: {
                method: 'GET',
                url: 'http://samplairre.progx.org:9000/api/sounds/ismuted'
            }
        });
}]);