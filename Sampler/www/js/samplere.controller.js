/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
    .controller('MainController',
    function ($scope, $rootScope, Sounds, alertService, notificationService, Login, $location, $localStorage) {

        // Menu is collapsed by default on mobile devices
        $scope.isCollapsed = true;
		
		// To display alerts to user
		$scope.alerts = alertService.getAlerts();
		$scope.closeAlert = alertService.closeAlert;

        // To search sound with scope inheritance
        $scope.search = {};

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
            $scope.search.query = '';
        };

        $scope.callSound = function (soundId){
            Sounds.play({soundId: soundId}, function(success) {

            }, function(error) {
                if(error.status === 403) {
                    // Quota exceeded
                    alertService.addAlert('Quota dépassé !','danger');
                }
            });
        };

        $scope.mute = function (){
            Sounds.muteUnmute(function(data) {
                $scope.isMuted = data.ismuted;
            });
        };
		
		notificationService.setMuteChangedHandler(function(isMuted){
			$scope.$apply(function(){
				$scope.isMuted = isMuted;
			});
		});

        $scope.logout = function () {
            Login.logout(function() {
                delete $localStorage.token;
                delete $rootScope.token;
                delete $rootScope.user;
                $location.url('/login');
            });
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