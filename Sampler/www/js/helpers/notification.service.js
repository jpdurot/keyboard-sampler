'use strict';

angular
    .module('samplereApp')
    .factory('notificationService', notificationService);

/**
 * notification service: manage websocket notifications from server.
 */
function notificationService($rootScope, alertService) {

  /*
   * Public interface
   */

    var service = {};
    service.messages = [];
	
	var _isMutedDelegate;
	
    // Sounds hub defined on server
    var soundsHub = $.connection.soundsHub;
	
    // Function invoked by server to notify sounds played
    soundsHub.client.notifyNewSound = function (soundInfo, user, isMuted) {
        $rootScope.$apply(function () {
            if (isMuted)
                alertService.addAlert(user + ' a tenté de jouer "' + soundInfo.Name + '".', 'warning');
            else
                alertService.addAlert(user + ' vient de jouer "' + soundInfo.Name + '".', 'success');
        });
    };

    // Create a function that the hub can call to broadcast messages.
    soundsHub.client.broadcastChatMessage = function (name, message) {
        $rootScope.$apply(function() {
            service.messages.push({
                'username': name,
                'content': message
            });
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
	}
	
	service.setMuteChangedHandler = function(handler)	{
		_isMutedDelegate = handler;
	}
	
    // Start conection
    // Start the connection.
	$.connection.hub.start().done(function () {
	    service.sendMessage = function (message) {
	        if (message && message !== '') {
	            // Call the Send method on the hub. 
	            soundsHub.server.chatSend($rootScope.user.userName, message);
	            service.messages.push({
	                'username': $rootScope.user.userName,
	                'content': message
	            });
	        }
	    };
	});

  /*
   * Internal
   */
  
  return service;
}
