﻿'use strict';

angular
    .module('samplereApp')
    .factory('notificationService', notificationService);

/**
 * notification service: manage websocket notifications from server.
 */
function notificationService($rootScope, alertService, User, Sounds) {

  /*
   * Public interface
   */

    var service = {};
    service.messages = [];
    service.soundsHistory = [];
	
	var _isMutedDelegate;
	
    // Sounds hub defined on server
    var soundsHub = $.connection.soundsHub;
	
    // Function invoked by server to notify sounds played
    soundsHub.client.notifyNewSound = function (soundInfo, user, isMuted) {
        $rootScope.$apply(function () {
            if (isMuted)
                alertService.addAlert(user + ' a tenté de jouer "' + soundInfo.Name + '".', 'warning');
            else {
                alertService.addAlert(user + ' vient de jouer "' + soundInfo.Name + '".', 'success');
                if ($rootScope.user.playingProfil !== 2 || 
					$rootScope.user.allowBroadcastSounds ||
					(!$rootScope.user.allowBroadcastSounds && $rootScope.user.userName === user)) {

                    // We register sound, if false then fileload will play sound if true (already loaded) then we play
                    if(createjs.Sound.registerSound({id: soundInfo.Id, src: soundInfo.Uri})) {
                        createjs.Sound.play(soundInfo.Id);
                    }
				}
            }
        });
    };

    // Create a function that the hub can call to broadcast messages.
    soundsHub.client.broadcastChatMessage = function (name, message, time) {
        $rootScope.$apply(function() {
            service.messages.push({
                'username': name,
                'content': message,
                'time': time
            });
        });
    };

    // Create a function that the hub can call to broadcast messages.
    soundsHub.client.broadcastTrophy = function (user, trophy) {
        $rootScope.$apply(function() {
            alertService.addAlert(user + ' a obtenu le trophée "' + trophy + '".', 'info');
        });
    };

    // Function invoked by server to notify sounds played
    soundsHub.client.notifyLogin = function (user) {
        $rootScope.$apply(function () {
            // Notify only if the user is not the current user
            if ($rootScope.$root && $rootScope.$root.user && $rootScope.$root.user.userName !== user)
                alertService.addAlert(user + ' vient de se connecter.', 'warning');
        });
    };
	
	soundsHub.client.syncIsMuted = function (isMuted, user) {
		if (_isMutedDelegate)
		{
			_isMutedDelegate(isMuted, user);
		}
	};
	
	service.setMuteChangedHandler = function(handler)	{
		_isMutedDelegate = handler;
	};

    service.sendMessage = function (message) {
        if (message && message !== '') {
            // Call the Send method on the hub.
            soundsHub.server.chatSend($rootScope.user.userName, message);
            var now = new Date();
            var mm = (now.getMonth()+1).toString(); // getMonth() is zero-based
            var dd  = now.getDate().toString();
            var hh  = now.getHours().toString();
            var min  = now.getMinutes().toString();
            var nowStr = (dd[1]?dd:"0"+dd[0])+ '/'+ (mm[1]?mm:"0"+mm[0]) + ' à '+ (hh[1]?hh:"0"+hh[0])+':'+(min[1]?min:"0"+min[0]); // padding
            service.messages.push({
                'username': $rootScope.user.userName,
                'content': message,
                'time': nowStr
            });
        }
    };

    service.initChat = function() {
        // Let's pull latest 10 messages
        service.messages = User.chatHistory();
        service.soundsHistory = Sounds.history();
    };
	
    // Start conection
    // Start the connection.
	$.connection.hub.start();

    // Auto reconnection
    function reconnect() {
        setTimeout(function() {
            alertService.addAlert('Tentative de reconnexion de signalAir. Dernière Erreur : '+$.connection.hub.lastError.message, 'info');
            console.dir($.connection.hub.lastError);
            $.connection.hub.start();
        }, 1000); // Restart connection after 1 seconds.
    }
    $.connection.hub.disconnected(function() {
        if(navigator.onLine) {
            reconnect();
        }
        // Test pour voir si ça résoud le problème de son qui se coupe après une longue inactivité
        if (!createjs.initializeDefaultPlugins()) { return; }
    });
    document.addEventListener('online', function() {
        reconnect();
    });

  /*
   * Internal
   */
  
  return service;
}
