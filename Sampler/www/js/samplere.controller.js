/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp')
  .controller('MainController',
  function ($scope, $rootScope, Sounds, alertService, notificationService, Login, $location,
            $localStorage, waitSpinnerService, $timeout) {

    // Menu is collapsed by default on mobile devices
    $scope.isCollapsed = true;

    // To display alerts to user
    $scope.alerts = alertService.getAlerts();
    $scope.closeAlert = alertService.closeAlert;

    // To search sound with scope inheritance
    $scope.search = {};

    var completeSoundList = [];

    function removeAccents(value) {
      return value
        .replace(/à|â/g, 'a')
        .replace(/ç/g, 'c')
        .replace(/é|è|ê/g, 'e')
        .replace(/ï|î/g, 'i')
        .replace(/ö|ô/g, 'o')
        .replace(/ù|û/g, 'u');
    }
    $scope.ignoreAccents = function(item, searchQuery) {
      if (!searchQuery) {
        return true;
      }
      if(!item) {
        return false;
      }
      var text = removeAccents(item.toLowerCase());
      var search = removeAccents(searchQuery.toLowerCase());
      return text.indexOf(search) > -1;
    };

    $scope.clear = function() {
      $scope.search.query = '';
    };

    $scope.callSound = function (sound){
      if ($rootScope.user.playingProfil == 1) {
        if (!$scope.isMuted) {
          // We register sound, if false then fileload will play sound if true (already loaded) then we play
          if(createjs.Sound.registerSound({id: sound.Id, src: sound.Uri})) {
            createjs.Sound.play(sound.Id);
          }
        }
      } else {
        Sounds.play({soundId: sound.Id}, function(success) {

        }, function(error) {
          if(error.status === 403) {
            // Quota exceeded
            alertService.addAlert('Quota de lecture dépassé !','danger');
          }
        });
      }
    };

    $scope.mute = function (){
      Sounds.muteUnmute(function(data) {
        $scope.isMuted = data.ismuted;
      }, function(error) {
        if(error.status === 403) {
          // Quota exceeded
          alertService.addAlert('Quota de mute dépassé !','danger');
        }
      });
    };

    $scope.callFavorite = function(sound){
      Sounds.favorite({soundId: sound.Id}, function(data) {
        sound.IsFavorite = data.isFavorite;
      });
    };

    notificationService.setMuteChangedHandler(function(isMuted, user){
      $scope.$apply(function(){
        $scope.isMuted = isMuted;
        if (isMuted) {
          createjs.Sound.stop();
          alertService.addAlert(user + ' vient de couper le son.', 'danger');
        }
        else
          alertService.addAlert(user + ' vient de réactiver le son.', 'danger');
      });
    });

    $scope.logout = function () {
      Login.logout(function() {
        delete $localStorage.token;
        delete $rootScope.token;
        delete $rootScope.user;
        $location.url('/login');
      });
    };

    $scope.isCurrentPage = function(page) {
      return $location.url() === page;
    };

    // Spinner animation while waiting response
    $scope.displaySpinner = waitSpinnerService.displaySpinner;
    $scope.$on('spinner', function() {
      // Do it inside $timeout to automatically and safely $apply
      $timeout(function(){
        $scope.displaySpinner = waitSpinnerService.displaySpinner;
      },0);
    });

    // When a sound is loaded, it means we must play it
    createjs.Sound.on("fileload", function(event) {
      createjs.Sound.play(event.id);
    });

    // Updates sound play counter
    $scope.$on('soundPlayed', function($event, sound) {
      for(var i=0; i<$scope.soundList.length; i++) {
        if($scope.soundList[i].Id === sound.Id) {
          $scope.soundList[i].PlayedCount = sound.PlayedCount;
          break;
        }
      }
    });


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