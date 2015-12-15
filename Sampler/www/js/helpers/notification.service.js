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
	
	var _isMutedDelegate;
	
    // Sounds hub defined on server
    var soundsHub = $.connection.soundsHub;
	
    // Function invoked by server
    soundsHub.client.addNewMessageToPage = function (sound, user) {
        $rootScope.$apply(function() {
            alertService.addAlert(user + ' vient de jouer ' + sound + ' !!');
        });
    };
	
	soundsHub.client.syncIsMuted = function (isMuted) {
		if (_isMutedDelegate)
		{
			_isMutedDelegate(isMuted);
		}
	}
	
	service.setMuteChangedHandler = function(handler)	{
		_isMutedDelegate = handler;
	}
	
    // Start conection
    $.connection.hub.start();

  /*
   * Internal
   */
  
  return service;
}
