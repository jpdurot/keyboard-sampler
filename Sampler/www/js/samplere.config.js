/**
 * Created by Ludo on 04/12/2015.
 */
'use strict';

angular.module('samplereApp').config(
    function($httpProvider, $routeProvider) {

      // Redirects users not logged in to login page on http calls
      $httpProvider.interceptors.push(function($q, $location, $rootScope,
          $localStorage) {
        return {
          responseError : function(response) {
            if (response.status === 401) {
              delete $rootScope.user;
              if (!!$localStorage.token) {
                delete $localStorage.token;
                delete $rootScope.token;
                delete $rootScope.user;
              }
              $location.url('/login');
            }
            return $q.reject(response);
          },
          request : function(config) {
            if (!!config && !!config.url && config.url.indexOf('/api') === 0
                && !!$localStorage.token) {
              // We add authentication headers
              config.headers['ApiToken'] = $rootScope.token;
            }
            return config;
          }
        };
      });

      // Redirects users not logged in to login page on route changes
      var checkLoggedin = function($q, $timeout, Login, $location, $rootScope,$localStorage) {
        // Initialize a new promise
        var deferred = $q.defer();

        var logout = function() {
          delete $rootScope.user;
          delete $localStorage.token;
          // Needs a timeout to force a digest cycle, else the url is not
          // changed
          $timeout(function() {
            $location.url('/login');
          }, 0);
        };

        // Make an AJAX call to check if the user is logged in
        Login.isLoggedIn(function(user) {
          console.log('loggedIn : ', user);
          // Authenticated
          if (user !== '0') {
            deferred.resolve();
          }

          // Not Authenticated
          else {
            $rootScope.message = 'You need to log in.';
            deferred.reject();
            logout();
          }
        }, function(error) {
          logout();
        });

        return deferred.promise;
      };
      
      var checkNotLoggedIn = function($localStorage, $location, $timeout, $q) {
        console.log('checkNotLoggedIn')
        // Initialize a new promise
        var deferred = $q.defer();
        // Needs a timeout to force a digest cycle, else the url is not
        // changed
        if(angular.isDefined($localStorage.token)) {
          console.log('already logged in')
          deferred.reject();
          $timeout(function() {
            $location.url('/');
          }, 0);
        } else {
          deferred.resolve();
        }
        return deferred.promise;
      }

      $routeProvider.when('/login', {
        controller : 'LoginController',
        templateUrl : 'views/login/login.html',
        resolve : {
          notLoggedIn : function($localStorage, $location, $timeout, $q) {
            return checkNotLoggedIn($localStorage, $location, $timeout, $q);
          }
        }
      }).when('/change_password', {
        controller : 'ChangePasswordController',
        templateUrl : 'views/change_password/change_password.html'
      }).when('/sounds', {
        controller : 'SoundsController',
        templateUrl : 'views/sounds/sounds.html',
        resolve : {
          loggedIn : function($q, $timeout, Login, $location, $rootScope,$localStorage) {
            return checkLoggedin($q, $timeout, Login, $location, $rootScope,$localStorage);
          }
        }
      }).when('/favorites', {
        controller : 'SoundsController',
        templateUrl : 'views/sounds/favorites.html',
        resolve : {
          loggedIn : function($q, $timeout, Login, $location, $rootScope,$localStorage) {
            return checkLoggedin($q, $timeout, Login, $location, $rootScope,$localStorage);
          }
        }
      }).when('/profile', {
        controller : 'ProfileController',
        templateUrl : 'views/profile/profile.html'
      }).otherwise({
        controller : 'SoundsController',
        templateUrl : 'views/sounds/sounds.html'
      });
    });