/**
 * Created by Ludo on 13/12/2015.
 */
'use strict';

// App startup
angular.module('samplereApp')
  .run(function(Login, $rootScope, $localStorage, notificationService, constants) {
    // At startup we check if user is logged in
    if(!!$localStorage.token) {
      $rootScope.token = $localStorage.token;
      Login.isLoggedIn(function(user) {
        // Success, user is logged in so we save it into rootScope
        $rootScope.user = {
          userName: user.userName,
          playingProfil: user.playingProfil,
          allowBroadcastSounds: user.allowBroadcastSounds
        };
        // We pull chat history
        notificationService.initChat();
      })
    }

    // We register base uri
    $rootScope.baseUri = constants.baseUri;
    // And test if running inside Electron
    $rootScope.isElectron = window && window.process && window.process.versions['electron'];
  });