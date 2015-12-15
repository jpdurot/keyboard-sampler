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
      var checkLoggedin = function($q, $timeout, Login, $location, $rootScope) {
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
            // Success, user is logged in so we save it into rootScope
            $rootScope.user = {
                userName: user.userName
            };
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

      $routeProvider.when('/login', {
        controller : 'LoginController',
        templateUrl : 'views/login/login.html'
      }).when('/sounds', {
        controller : 'SoundsController',
        templateUrl : 'views/sounds/sounds.html',
        resolve : {
          loggedIn : function() {
            return checkLoggedin;
          }
        }
      }).when('/favorites', {
        controller : 'SoundsController',
        templateUrl : 'views/sounds/favorites.html',
        resolve : {
          loggedIn : function() {
            return checkLoggedin;
          }
        }
      }).otherwise({
        controller : 'SoundsController',
        templateUrl : 'views/sounds/sounds.html'
      });
    });