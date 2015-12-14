/**
 * Created by Matt on 11/12/2015.
 */
'use strict';

$(function () {
	// Sounds hub defined on server
	var soundsHub = $.connection.soundsHub;
	// Function invoked by server
	soundsHub.client.broadcast = function (sound, user) {
		alert('sound=' + sound + ', user=' + user)
	};
	// Start conection
	$.connection.hub.start();
});