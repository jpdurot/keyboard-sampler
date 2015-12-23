/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .controller('SoundsController',
    function($scope, Sounds, waitSpinnerService) {
      
      console.log('SoundsController');

        var completeSoundList = [];

        var accentMap = {
            "é": "e",
            "è": "e",
            "ê": "e",
            "ë": "e",
            "à": "a",
            "â": "a",
            "ä": "a",
            "ö": "o",
            "ô": "o",
            "ù": "u",
            "ç": "c"
        };
        var normalize = function( term ) {
            var ret = "";
            for ( var i = 0; i < term.length; i++ ) {
                ret += accentMap[ term.charAt(i) ] || term.charAt(i);
            }
            return ret;
        };

        // We check if sound is muted
        Sounds.isMuted(function(data) {
            $scope.$parent.isMuted = data.ismuted;
        });

        Sounds.getList(function(sounds) {
            $scope.$parent.soundList = sounds;
            waitSpinnerService.showSpinner();

            var queue = new createjs.LoadQueue();
            queue.installPlugin(createjs.Sound);
            queue.setMaxConnections(3);
			angular.forEach(sounds, function(sound){
                queue.loadFile({
                    id: sound.Id,
                    src: sound.Uri
                });
			});
            queue.on("complete", function() {
                waitSpinnerService.hideSpinner();
            }, this);
        });
    }
);
/*
 // Autocomplete on search field
 $('#search').autocomplete({
 source: function( request, response ) {
 var matcher = new RegExp( $.ui.autocomplete.escapeRegex( request.term ), "i" );
 response( $.grep( completeSoundList, function( value ) {
 value = value.label || value.value || value;
 return matcher.test( value ) || matcher.test( normalize( value ) );
 }) );
 },
 appendTo: '#navbar',
 delay: 100,
 select: function(event, ui) {
 callSound(ui.item.value);
 $('#search').val(ui.item.label);
 event.preventDefault();
 }
 });

 function search($event) {
 console.log($('#search').val());
 if($event.keyCode === 13) { // Enter
 console.log($('#search').autocomplete('option'));
 } else if($event.keyCode === 27) { // Esc
 clear();
 }
 }

 function clear() {
 $('#search').val('');
 }

 function submit($event) {
 console.log($('#search').val());
 console.log($event);
 }

 $().ready(function() {
 getSounds();
 $('#clearButton span').click(clear);
 });
 */