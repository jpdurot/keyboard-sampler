/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .controller('MainController',
    function($scope, Sounds) {

        // Pour les mobiles, le menu est plié au départ
        $scope.isCollapsed = true;

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

        $scope.clear = function() {
            $scope.searchString = '';
        };

        $scope.callSound = function (soundId){
            Sounds.play({soundId: soundId});
        };

        Sounds.getList(function(data) {
            $scope.soundList = data;
        });

        function getSounds() {
            /*$.get('/api/sounds/info', function (data) {
             data.forEach(function (sound) {
             $('.buttons').append('<button class="btn btn-default col-xs-6 col-sm-3 col-md-2" href="#" role="button" onclick="callSound(' + sound.Id + ')"><span>' + sound.Name + '</span></button>');
             completeSoundList.push({
             label: sound.Name,
             value: sound.Id
             });
             });
             });*/
        }
    });
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