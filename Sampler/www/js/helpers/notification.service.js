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
    soundsHub.client.addNewSoundMessageToPage = function (soundInfo, user, isMuted) {
        $rootScope.$apply(function () {
            if (isMuted)
                alertService.addAlert(user + ' a tenté de jouer ' + soundInfo.Name + '.', 'warning');
            else
                alertService.addAlert(user + ' vient de jouer ' + soundInfo.Name + '.', 'success');
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
    $.connection.hub.start();

  /*
   * Internal
   */
  
  return service;
}
