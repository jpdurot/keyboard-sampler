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
    $.get('/api/sounds/info', function (data) {
        console.log('get');
        data.forEach(function(sound) {
            $('.buttons').append('<a class="btn btn-default col-xs-6 col-sm-3 col-md-2" href="#" role="button" onclick="callSound('+sound.Id+')"><span>'+sound.Name+'</span></a>');
        });

    });
}

$().ready(function() {
    getSounds();
});