/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
  .factory('Sounds', ['$resource','constants', function($resource, constants) {
      return $resource(constants.baseUri+'/api/Sounds/play/:soundId', null,
        {
            'play': { method:'GET' },
            'getList': {
                method: 'GET',
                isArray: true,
                url: constants.baseUri+'/api/Sounds/info'
            },
            muteUnmute: {
                method: 'POST',
                url: constants.baseUri+'/api/Sounds/mute'
            },
            isMuted: {
                method: 'GET',
                url: constants.baseUri+'/api/Sounds/ismuted'
            },
            favorite: {
                method: 'POST',
                url: constants.baseUri+'/api/Sounds/favorite'
            },
            history: {
                method: 'GET',
                params: {
                    count: 10
                },
                url: constants.baseUri+'/api/Sounds/latest/:count',
                isArray: true
            }
        });
  }]);