/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

function callSound(soundId) {
  $.post('/api/sounds/play/'+soundId, function(data) {
     console.log(data);
  });
}

function getSounds() {
    $.post('/api/sounds/info', function (data) {
        console.log(data);
    });
}

function mute() {
    $.post('/api/mute', function (data) {
        console.log(data);
    });
}