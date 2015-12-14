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

    // Sounds hub defined on server
    var soundsHub = $.connection.soundsHub;
    // Function invoked by server
    soundsHub.client.addNewMessageToPage = function (sound, user) {
        $rootScope.$apply(function() {
            alertService.addAlert(user + ' vient de jouer ' + sound + ' !!');
        });
    };
    // Start conection
    $.connection.hub.start();

  /*
   * Internal
   */

  return service;
}